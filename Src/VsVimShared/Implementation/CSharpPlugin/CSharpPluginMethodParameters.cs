using Vim.Interpreter;

namespace Vim.VisualStudio.Implementation.CSharpPlugin
{
    public class CSharpPluginMethodParameters
    {
        public string Name { get; }
        public string Arguments { get; }
        public LineRangeSpecifier LineRange { get; }
        public IVimBuffer VimBuffer { get; }

        public CSharpPluginMethodParameters(CallInfo callInfo, IVimBuffer vimBuffer)
        {
            Name = callInfo.Name;
            Arguments = callInfo.Arguments;
            LineRange = callInfo.LineRange;
            VimBuffer = vimBuffer;
        }
    }
}
