using System;

namespace SMCheats
{
    public class DisposableAction : IDisposable
    {
        private readonly Action _disposeAction;
        private bool _isDisposed;

        public DisposableAction(Action disposeAction)
        {
            _disposeAction = disposeAction;
        }
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
            _disposeAction.Invoke();
        }
    }
}