﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

namespace VsVimShared.Implementation.CSharpPlugin
{
    /// <summary>
    /// Use to compile C# code to in memory assemblies using the Roslyn compiler
    /// </summary>
    public class AssemblyGenerator
    {
        private readonly IList<MetadataReference> _references = new List<MetadataReference>();
        private readonly IList<Assembly> _assemblies = new List<Assembly>();

        public static string[] HintPaths { get; set; }

        public AssemblyGenerator()
        {
            ReferenceAssemblyContainingType<object>();
            ReferenceAssembly(typeof(Enumerable).GetTypeInfo().Assembly);
        }

        public string AssemblyName { get; set; }

        /// <summary>
        /// Tells Roslyn to reference the given assembly and any of its dependencies
        /// when compiling code
        /// </summary>
        /// <param name="assembly"></param>
        public void ReferenceAssembly(Assembly assembly)
        {
            if (assembly == null) return;

            if (_assemblies.Contains(assembly)) return;

            _assemblies.Add(assembly);

            try
            {
                var referencePath = createAssemblyReference(assembly);

                if (referencePath == null)
                {
                    Console.WriteLine($"Could not make an assembly reference to {assembly.FullName}");
                    return;
                }

                var alreadyReferenced = _references.Any(x => x.Display == referencePath);
                if (alreadyReferenced)
                    return;

                var reference = MetadataReference.CreateFromFile(referencePath);

                _references.Add(reference);

                foreach (var assemblyName in assembly.GetReferencedAssemblies())
                {
                    var referencedAssembly = Assembly.Load(assemblyName);
                    ReferenceAssembly(referencedAssembly);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not make an assembly reference to {assembly.FullName}\n\n{e}");
            }
        }

        private static string createAssemblyReference(Assembly assembly)
        {
            if (assembly.IsDynamic) return null;

            return string.IsNullOrEmpty(assembly.Location)
                ? getPath(assembly)
                : assembly.Location;
        }

        private static string getPath(Assembly assembly)
        {
            return HintPaths?
                .Select(findFile(assembly))
                .FirstOrDefault(file => !string.IsNullOrWhiteSpace(file));
        }

        private static Func<string, string> findFile(Assembly assembly)
        {
            return hintPath =>
            {
                var name = assembly.GetName().Name;
                Console.WriteLine($"Find {name}.dll in {hintPath}");
                var files = Directory.GetFiles(hintPath, name + ".dll", SearchOption.AllDirectories);
                var firstOrDefault = files.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    Console.WriteLine($"Found {name}.dll in {firstOrDefault}");
                }

                return firstOrDefault;
            };
        }

        /// <summary>
        /// Reference the assembly containing the type "T"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ReferenceAssemblyContainingType<T>()
        {
            ReferenceAssembly(typeof(T).GetTypeInfo().Assembly);
        }

        /// <summary>
        /// Compile the code passed into this method to a new assembly in memory. If rootPath
        /// is specified, the assembly will attempt to resolve any other files in the location.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Assembly GeneratePluginAssembly(string pluginMainPath)
        {
            var encoding = Encoding.UTF8;
            var assemblyName = AssemblyName ?? Path.GetRandomFileName();
            var symbolsName = Path.ChangeExtension(assemblyName, "pdb");
            var nonSystemUsings = new HashSet<UsingDirectiveSyntax>();

            var allCsFiles = Directory.GetFiles(Path.GetDirectoryName(pluginMainPath), "*.cs", SearchOption.AllDirectories).ToHashSet();
            allCsFiles.Remove(pluginMainPath);

            var embeddedTexts = new List<EmbeddedText>();

            var pluginFileCompilations = new List<CSharpCompilation>();
            byte[] buffer;
            SourceText sourceText;
            foreach (var csFile in allCsFiles)
            {
                buffer = File.ReadAllBytes(csFile);
                sourceText = SourceText.From(buffer, buffer.Length, encoding, canBeEmbedded: true);
                EmbeddedText.FromSource(csFile, sourceText);

                var compilation = CreateCompilation(csFile, encoding, ref nonSystemUsings, assemblyName, sourceText, OutputKind.NetModule);

                pluginFileCompilations.Add(compilation);
            }

            pluginFileCompilations.ForEach(e =>
            {
                using var memStream = new MemoryStream();
                var result = e.Emit(memStream);
                if (result.Success)
                {
                    _references.Add(MetadataReference.CreateFromStream(memStream));
                }
            });

            buffer = File.ReadAllBytes(pluginMainPath);
            sourceText = SourceText.From(buffer, buffer.Length, encoding, canBeEmbedded: true);

            var pluginCompilation = CreateCompilation(pluginMainPath, encoding, ref nonSystemUsings, assemblyName, sourceText, OutputKind.DynamicallyLinkedLibrary);

            using var assemblyStream = new MemoryStream();
            using var symbolsStream = new MemoryStream();

            var emitOptions = new EmitOptions(
                debugInformationFormat: DebugInformationFormat.PortablePdb,
                pdbFilePath: symbolsName);
;

            var emission = pluginCompilation.Emit(
                peStream: assemblyStream,
                pdbStream: symbolsStream,
                embeddedTexts: embeddedTexts,
                options: emitOptions);


            assemblyStream.Seek(0, SeekOrigin.Begin);
            symbolsStream.Seek(0, SeekOrigin.Begin);

            return Assembly.Load(assemblyStream.ToArray(), symbolsStream.ToArray());
        }

        private CSharpCompilation CreateCompilation(string csFile, Encoding encoding, ref HashSet<UsingDirectiveSyntax> nonSystemUsings, string assemblyName, SourceText sourceText, OutputKind outputKind)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                sourceText,
                new CSharpParseOptions(),
                csFile);

            var rootNode = syntaxTree.GetRoot() as CompilationUnitSyntax;

            foreach (var usingNode in rootNode.Usings)
            {
                if (usingNode.Name.ToString() != "System" && !usingNode.Name.ToString().StartsWith("System."))
                {
                    nonSystemUsings.Add(usingNode);
                }
            }

            var references = _references.ToArray();
            var compilation = CSharpCompilation.Create(assemblyName, new[] { syntaxTree }, references,
                new CSharpCompilationOptions(outputKind)
                    .WithOptimizationLevel(OptimizationLevel.Debug)
                    .WithPlatform(Platform.AnyCpu));

            return compilation;
        }
    }
}
