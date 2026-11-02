using System;

namespace SMCheats
{
    [Flags]
    public enum CheatsBuiltInTabs
    {
        None = 0,
        Info = 1 << 0,
        Console = 1 << 1,
        Profiler = 1 << 2,
        Options = 1 << 3,
        All = Info | Console | Profiler | Options
    }
}