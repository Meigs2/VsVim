﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Vim.Interpreter;
using VsVimShared.Implementation.CSharpPlugin;

namespace Vim.VisualStudio.Implementation.CSharpPlugin
{
    using Microsoft.CodeAnalysis.Scripting;
    using CSharpScript = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript;

    [Export(typeof(ICSharpPluginExecutor))]
    internal sealed partial class CSharpPluginExecutor : ICSharpPluginExecutor
    {
        private const string pluginsFolderName = "vsvimplugins";
        private const string pluginInitFileName = "plugin.cs";
        private Dictionary<string, IVsVimPlugin> _plugins = new(StringComparer.OrdinalIgnoreCase);
        private ScriptOptions _scriptOptions;

        private string PluginsLocationFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), pluginsFolderName);

        [ImportingConstructor]
        public CSharpPluginExecutor()
        {

        }

        internal void LoadAllPlugins(IVim vsVimInstance)
        {
            if (!Directory.Exists(PluginsLocationFolder)) return; // Plugin directory doesn't exist

            foreach (var pluginFolder in Directory.GetDirectories(PluginsLocationFolder))
            {
                var pluginName = new DirectoryInfo(pluginFolder).Name;
                var pluginInitFile = Path.Combine(pluginFolder, pluginInitFileName);

                if (!File.Exists(pluginInitFile))
                {
                    continue;
                }

                LoadPlugin(vsVimInstance, pluginName);
            }
        }

        internal void LoadPlugin(IVim vsVimInstance, string pluginName)
        {
            if (!TryInstantiatePlugin(pluginName, out var plugin)) return;

            if (!plugin.Init(new CSharpPluginGlobals(vsVimInstance))) return;

            if (_plugins.ContainsKey(pluginName)) _plugins[pluginName]?.Dispose();

            _plugins[pluginName] = plugin;
        }

        internal void Execute(IVimBuffer vimBuffer, CallInfo callInfo, bool recompile)
        {
            try
            {
                var pluginInfo = callInfo.Name.Split('.');
                if (pluginInfo.Length != 2)
                {
                    vimBuffer.VimBufferData.StatusUtil.OnError($"{callInfo.Name} is not a valid plugin method call of format 'PluginName'.'PluginMethod'");
                    return;
                }

                var pluginName = pluginInfo[0];
                var methodName = pluginInfo[1];

                if (recompile)
                {
                    LoadPlugin(vimBuffer.Vim, pluginName);
                }

                _plugins[pluginName]?.RunMethod(methodName, new CSharpPluginMethodParameters(callInfo, vimBuffer));
            }
            catch (Exception ex)
            {
                vimBuffer.VimBufferData.StatusUtil.OnError(ex.Message);
            }
        }

        internal bool TryInstantiatePlugin(string pluginName, out IVsVimPlugin plugin)
        {
            plugin = null;

            var pluginFolder = Path.Combine(PluginsLocationFolder, pluginName);
            var pluginInitFilePath = Path.Combine(pluginFolder, pluginInitFileName);

            if (!File.Exists(pluginInitFilePath))
            {
                VimTrace.TraceError($"{pluginInitFileName} not found for {pluginName}.");
                return false;
            }

            var pluginInitText = File.ReadAllText(pluginInitFilePath);
            var assemblyGenerator = new AssemblyGenerator();
            assemblyGenerator.ReferenceAssemblyContainingType<System.Windows.MessageBox>();
            assemblyGenerator.ReferenceAssemblyContainingType<Vim.IVim>();
            assemblyGenerator.ReferenceAssemblyContainingType<Vim.VisualStudio.ApplicationSettingsEventArgs>();

            var assembly = assemblyGenerator.Generate(pluginInitText);

            var returnValue = Activator.CreateInstance(assembly?.GetExportedTypes().FirstOrDefault(t => t.GetInterfaces().Contains(typeof(IVsVimPlugin)))!);

            if (returnValue is not IVsVimPlugin pluginInstance)
            {
                VimTrace.TraceError($"{pluginInitFileName} for the plugin {pluginName} did not return a valid IVsVimPlugin instance.");
                return false;
            }

            plugin = pluginInstance;
            return true;
        }

        #region ICSharpPluginExecutor

        void ICSharpPluginExecutor.Execute(IVimBuffer vimBuffer, CallInfo callInfo, bool recompile)
        {
            Execute(vimBuffer, callInfo, recompile);
            VimTrace.TraceInfo("CSharpPlugin:Execute {0}", callInfo.Name);
        }

        void ICSharpPluginExecutor.LoadPlugins(IVim vsVimInstance)
        {
            LoadAllPlugins(vsVimInstance);
            VimTrace.TraceInfo("CSharpPlugin:LoadPlugins");
        }

        #endregion
    }
}
