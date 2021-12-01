#nullable enable
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Vim.UI.Wpf
{
    public interface IVimProtectedOperations : IProtectedOperations
    {
        public JoinableTaskFactory JoinableTaskFactory { get; }

        public Task RunInMainThreadAsync(
            Action action,
            Dispatcher? dispatcher = null,
            DispatcherPriority? dispatcherPriority = null,
            CancellationToken cancellation = default);
    }
}
