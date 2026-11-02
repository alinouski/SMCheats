using System;

namespace SMCheats
{
    public sealed class CheatActionOptionData : CheatOptionData
    {
        public CheatActionOptionData(Action callback)
        {
            Callback = callback;
        }

        public Action Callback { get; }
    }
}
