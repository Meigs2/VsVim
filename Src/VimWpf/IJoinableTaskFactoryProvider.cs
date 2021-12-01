using Microsoft.VisualStudio.Threading;
using System.Threading;
using System.Windows.Threading;

namespace Vim.UI.Wpf
{
    public interface IJoinableTaskFactoryProvider
    {
        JoinableTaskFactory JoinableTaskFactory { get; }
    }
}
