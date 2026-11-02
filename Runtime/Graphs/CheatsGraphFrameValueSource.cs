using System;

namespace SMCheats
{
    public sealed class CheatsGraphFrameValueSource : ICheatsGraphFrameValueSource
    {
        private readonly Func<CheatsGraphFrameSample> _getter;

        public CheatsGraphFrameValueSource(Func<CheatsGraphFrameSample> getter)
        {
            _getter = getter;
        }

        public CheatsGraphFrameSample GetFrameSample()
        {
            return _getter.Invoke();
        }
    }
}