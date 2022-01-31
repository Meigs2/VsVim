using Vim.Interpreter;

namespace Vim.VisualStudio.Implementation.CSharpPlugin
{
    public class CSharpPluginGlobals
    {
        public IVim Vim { get; }

        public CSharpPluginGlobals(IVim vsVimInstance)
        {
            Vim = vsVimInstance;
        }
    }
}
