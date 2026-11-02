using System;

namespace SMCheats
{
    public sealed class CheatToggleOptionData : CheatOptionData
    {
        public CheatToggleOptionData(Func<bool> getter, Action<bool> setter)
        {
            Getter = getter;
            Setter = setter;
        }

        public Func<bool> Getter { get; }
        public Action<bool> Setter { get; }
    }
}
