using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Vim.VisualStudio.Implementation.CSharpPlugin;

namespace Vim.VisualStudio
{
    public interface IVsVimPlugin : IDisposable
    {
        bool Init(CSharpPluginGlobals pluginGlobals);

        List<MethodInfo> GetPluginMethods();

        object RunMethod(string methodName, params object[] args);
    }
}
