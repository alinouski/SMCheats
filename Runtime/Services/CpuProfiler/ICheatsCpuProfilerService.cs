namespace SMCheats
{
    public interface ICheatsCpuProfilerService
    {
        float CurrentFrameTimeMs { get; }
        float CurrentCpuTimeMs { get; }
        float AverageFrameTimeMs { get; }
        float MinFrameTimeMs { get; }
        float MaxFrameTimeMs { get; }
        int SampleCapacity { get; }
        int SampleCount { get; }
        int NextSampleIndex { get; }
        float[] FrameTimeSamples { get; }
        float GetCategoryMsUsage(CheatsCpuProfilerCategory categories);
        bool IsCategoryAvailable(CheatsCpuProfilerCategory categories);
    }
}
