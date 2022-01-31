using Vim.Interpreter;

namespace Vim.VisualStudio
{
    public interface ICSharpPluginExecutor
    {
        void Execute(IVimBuffer vimBuffer, CallInfo callInfo, bool recompile);

        void LoadPlugins(IVim vsVimInstance);
    }
}
