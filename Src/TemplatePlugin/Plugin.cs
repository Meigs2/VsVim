using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Windows;
using Vim.VisualStudio;
using Vim.VisualStudio.Implementation.CSharpPlugin;

namespace VsVimPlugin
{
    /// <summary>
    /// Here is where you define the IVsVimPlugin class to be passed into VsVim. Boilerplate code has been provided
    /// to give you an idea of what needs to be done.
    /// </summary>
    public class VsVimPlugin : IVsVimPlugin
    {
        private CSharpPluginGlobals globals { get; set; }
        private List<MethodInfo> _exposedMethods = new List<MethodInfo>();

        public string PluginGuid => "1aed8e3a-1792-4d50-9f5a-9f3d10dc2eba";

        /// <summary>
        /// Entry point of the plugin.
        /// </summary>
        /// <param name="pluginGlobals"></param>
        /// <returns></returns>
        public bool Init(CSharpPluginGlobals pluginGlobals)
        {
            try
            {
                globals = pluginGlobals;
                _exposedMethods = typeof(VsVimPlugin).GetMethods().Where(m => m.IsPublic && (m.GetCustomAttributes(typeof(ExportVsVimMethod), false).Length > 0)).ToList();

                // perform any additional initialization for the plugin here.

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [ExportVsVimMethod]
        public void Test(CSharpPluginMethodParameters parameters)
        {
            MessageBox.Show("Success!");
        }

        public List<MethodInfo> GetExposedMethods()
        {
            return _exposedMethods;
        }

        public object RunMethod(string methodName, params object[] args)
        {
            try
            {
               var result = _exposedMethods
                    .FirstOrDefault(m => string.Equals(m.Name, methodName, StringComparison.OrdinalIgnoreCase))
                    ?.Invoke(this, args);
               return result;
            }
            catch (Exception)
            {
                globals.Vim.ActiveStatusUtil.OnError($"Error executing method {methodName} in plugin {nameof(VsVimPlugin)}.");
            }

            return null;
        }

        public void Dispose()
        {
            // dispose logic here
        }

    }

    public class ExportVsVimMethod : Attribute
    {
    }
 }