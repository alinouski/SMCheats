using System;

namespace SMCheats
{
    public interface IPinCodeService
    {
        void RequestAccess(Action sessionSuccess);
    }
}
