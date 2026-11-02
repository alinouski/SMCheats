using System;

namespace SMCheats
{
    public class MockPinCodeService : IPinCodeService, IDisposable
    {
        public void RequestAccess(Action sessionSuccess)
        {
            sessionSuccess?.Invoke();
        }

        public void Dispose()
        {
        }
    }
}
