using System;
using System.Windows.Threading;
using Microsoft.VisualStudio.Text;
using Moq;
using Xunit;
using Vim;
using Vim.EditorHost;
using Vim.UI.Wpf;
using System.Threading.Tasks;
using Vim.UI.Wpf.Implementation.Misc;

namespace Vim.UnitTest
{
    public sealed class ProtectedOperationsTest : VimTestBase
    {
        private readonly Mock<IExtensionErrorHandler> _errorHandler;
        private readonly VimProtectedOperations _protectedOperationsRaw;
        private readonly IVimProtectedOperations _protectedOperations;

        public ProtectedOperationsTest()
        {
            _errorHandler = new Mock<IExtensionErrorHandler>(MockBehavior.Strict);
            _protectedOperationsRaw = new VimProtectedOperations(JoinableTaskFactory, new[] { new Lazy<IExtensionErrorHandler>(() => _errorHandler.Object) });
            _protectedOperations = _protectedOperationsRaw;
        }

        /// <summary>
        /// Verify the returned action will execute the original one
        /// </summary>
        [WpfFact]
        public void GetProtectedAction_Standard()
        {
            var didRun = false;
            var protectedAction = _protectedOperations.GetProtectedAction(delegate { didRun = true; });
            protectedAction();
            Assert.True(didRun);
        }

        /// <summary>
        /// Verify that when the original action throws that it is passed on to the
        /// listed IExtensionErrorHandlers
        /// </summary>
        [WpfFact]
        public void GetProtectedAction_Throws()
        {
            var exception = new Exception("hello world");
            _errorHandler.Setup(x => x.HandleError(It.IsAny<object>(), exception)).Verifiable();
            var protectedAction = _protectedOperations.GetProtectedAction(delegate { throw exception; });
            protectedAction();
            _errorHandler.Verify();
        }

        /// <summary>
        /// Verify the returned EventHandler will execute the original one
        /// </summary>
        [WpfFact]
        public void GetProtectedEventHandler_Standard()
        {
            var didRun = false;
            var protectedEventHandler = _protectedOperations.GetProtectedEventHandler(delegate { didRun = true; });
            protectedEventHandler(null, EventArgs.Empty);
            Assert.True(didRun);
        }

        /// <summary>
        /// Verify that when the original EventHandler throws that it is passed on to the
        /// listed IExtensionErrorHandlers
        /// </summary>
        [WpfFact]
        public void GetProtectedEventHandler_Throws()
        {
            var exception = new Exception("hello world");
            _errorHandler.Setup(x => x.HandleError(It.IsAny<object>(), exception)).Verifiable();
            var protectedEventHandler = _protectedOperations.GetProtectedEventHandler(delegate { throw exception; });
            protectedEventHandler(null, EventArgs.Empty);
            _errorHandler.Verify();
        }

        /// <summary>
        /// Verify that the BeginInvoke will actually schedule the original action
        /// </summary>
        [WpfFact]
        public async Task RunInMainThreadAsyncNormal()
        {
            var didRun = false;
            var task = _protectedOperations.RunInMainThreadAsync(delegate { didRun = true; }, dispatcherPriority: DispatcherPriority.Normal);
            Dispatcher.CurrentDispatcher.DoEvents();
            await task;
            Assert.True(didRun);
        }

        /// <summary>
        /// Verify that when an exception is thrown during processing that it makes it's way to the 
        /// IExtensionErrorHandler
        /// </summary>
        [WpfFact]
        public async Task RunInMainThreadAsyncThrows()
        {
            var exception = new Exception("hello world");
            _errorHandler.Setup(x => x.HandleError(It.IsAny<object>(), exception)).Verifiable();
            var task = _protectedOperations.RunInMainThreadAsync(delegate { throw exception; });
            Dispatcher.CurrentDispatcher.DoEvents();
            await task;
            _errorHandler.Verify();
        }

        /// <summary>
        /// Verify that the BeginInvoke will actually schedule the original action
        /// </summary>
        [WpfFact]
        public async Task RunInMainThreadAsyncNoPriority()
        {
            var didRun = false;
            var task = _protectedOperations.RunInMainThreadAsync(delegate { didRun = true; });
            Dispatcher.CurrentDispatcher.DoEvents();
            await task;
            Assert.True(didRun);
        }

        /// <summary>
        /// Verify that when an exception is thrown during processing that it makes it's way to the 
        /// IExtensionErrorHandler
        /// </summary>
        [WpfFact]
        public async Task RunInMainThreadAsyncNoPriorityThrows()
        {
            var exception = new Exception("hello world");
            _errorHandler.Setup(x => x.HandleError(It.IsAny<object>(), exception)).Verifiable();
            var task = _protectedOperations.RunInMainThreadAsync(delegate { throw exception; });
            Dispatcher.CurrentDispatcher.DoEvents();
            await task;
            _errorHandler.Verify();
        }
    }
}

