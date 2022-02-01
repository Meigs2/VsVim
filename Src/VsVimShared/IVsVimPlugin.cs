using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Vim.VisualStudio.Implementation.CSharpPlugin;

namespace Vim.VisualStudio
{
    public interface IVsVimPlugin : IDisposable
    {
        string PluginGuid { get; }
        bool Init(CSharpPluginGlobals pluginGlobals);

        List<MethodInfo> GetExposedMethods();

        object RunMethod(string methodName, params object[] args);
    }
}
