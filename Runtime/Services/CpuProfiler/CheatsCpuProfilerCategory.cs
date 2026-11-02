using System;

namespace SMCheats
{
    [Flags]
    public enum CheatsCpuProfilerCategory
    {
        None = 0,
        Scripts = 1 << 0,
        Physics = 1 << 1,
        Physics2D = 1 << 2,
        Animation = 1 << 3,
        Rendering = 1 << 4,
        UI = 1 << 5,
        Audio = 1 << 6,
        Other = 1 << 7,
        Basic = Scripts | Physics | Rendering,
        All = Scripts | Physics | Physics2D | Animation | Rendering | UI | Audio,
    }
}
