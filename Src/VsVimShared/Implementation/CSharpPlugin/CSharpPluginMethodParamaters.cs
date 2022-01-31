using Vim.Interpreter;

namespace Vim.VisualStudio.Implementation.CSharpScript
{
    public class CSharpPluginMethodParameters
    {
        public string Name { get; } = string.Empty;
        public string Arguments { get; } = string.Empty;
        public LineRangeSpecifier LineRange { get; }
        public bool IsScriptLocal { get; } = false;
        public IVim Vim { get; } = null;
        public IVimBuffer VimBuffer { get; } = null;

        public CSharpPluginMethodParamater;CSharpPluginMethodParameters(CallInfo callInfo, IVimBuffer vimBuffer)
        {
            Name = callInfo.Name;
            Arguments = callInfo.Arguments;
            LineRange = callInfo.LineRange;
            IsScriptLocal = callInfo.IsScriptLocal;
            Vim = vimBuffer.Vim;
            VimBuffer = vimBuffer;
        }
    }
}
