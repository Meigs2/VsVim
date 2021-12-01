#nullable enable

using Microsoft;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Vim;
using Vim.UI.Wpf;

namespace Vim.UI.Wpf.Implementation.Misc
{
    [Export(typeof(IVimProtectedOperations))]
    [Export(typeof(IProtectedOperations))]
    internal sealed class VimProtectedOperations : IVimProtectedOperations
    {
        private readonly List<Lazy<IExtensionErrorHandler>> _errorHandlers;

        internal JoinableTaskFactory JoinableTaskFactory { get; }

        [ImportingConstructor]
        public VimProtectedOperations(
            IJoinableTaskFactoryProvider joinableTaskFactoryProvider,
            [ImportMany] IEnumerable<Lazy<IExtensionErrorHandler>> errorHandlers) 
            :this(joinableTaskFactoryProvider.JoinableTaskFactory, errorHandlers)
        {
        }

        public VimProtectedOperations(
            JoinableTaskFactory joinableTaskFactory,
            IEnumerable<Lazy<IExtensionErrorHandler>> errorHandlers)
        {
            JoinableTaskFactory = joinableTaskFactory;
            _errorHandlers = errorHandlers.ToList();
        }

        internal async Task RunInMainThreadAsync(Action action, Dispatcher? dispatcher, DispatcherPriority? dispatcherPriority, CancellationToken cancellationToken)
        {
#if VS_SPECIFIC_2017
            // The `WithPriority` methods were added in much later versions of MS.VS.Threading than we use 
            // here hence we have to fall back to dispatcher for now. Ideally though we should just move to 
            // newer versions of the references so we can use the correct method
            dispatcher ??= Dispatcher.CurrentDispatcher;
            action = GetProtectedAction(action);
            await Task.Yield();
            await dispatcher.BeginInvoke(action, dispatcherPriority ?? DispatcherPriority.Normal);

#else
            var joinableTaskFactory = (dispatcher, dispatcherPriority) switch
            {
                (Dispatcher d, DispatcherPriority p) => JoinableTaskFactory.WithPriority(d, p),
                (null, DispatcherPriority p) => JoinableTaskFactory.WithPriority(Dispatcher.CurrentDispatcher, p),
                (Dispatcher d, null) => JoinableTaskFactory.WithPriority(d, DispatcherPriority.Normal),
                (null, null) => JoinableTaskFactory,
            };

            action = GetProtectedAction(action);
            await Task.Yield();
            await joinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            action();
#endif
        }

        internal Action GetProtectedAction(Action action) => () =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Report(ex);
            }
        };

        internal EventHandler GetProtectedEventHandler(EventHandler eventHandler) => (sender, e) =>
        {
            try
            {
                eventHandler.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                Report(ex);
            }
        };

        internal void Report(Exception ex)
        {
            VimTrace.TraceError(ex);

            foreach (var errorHandler in _errorHandlers)
            {
                try
                {
                    errorHandler.Value.HandleError(this, ex);
                }
                catch (Exception nestedEx)
                {
                    Debug.Fail(nestedEx.Message);
                }
            }
        }

#region IVimProtectedOperations

        JoinableTaskFactory IVimProtectedOperations.JoinableTaskFactory => JoinableTaskFactory;

        Action IProtectedOperations.GetProtectedAction(Action action) => GetProtectedAction(action);

        EventHandler IProtectedOperations.GetProtectedEventHandler(EventHandler eventHandler) => GetProtectedEventHandler(eventHandler);

        void IProtectedOperations.Report(Exception ex) => Report(ex);

        Task IVimProtectedOperations.RunInMainThreadAsync(Action action, Dispatcher? dispatcher, DispatcherPriority? dispatcherPriority, CancellationToken cancellationToken) =>
            RunInMainThreadAsync(action, dispatcher, dispatcherPriority, cancellationToken);

#endregion
    }
}
