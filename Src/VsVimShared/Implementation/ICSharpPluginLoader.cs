using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VsVimShared.Implementation
{
    internal interface ICSharpPluginLoader
    {
        List<MethodInfo> LoadPlugins();
    }
}
