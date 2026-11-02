namespace SMCheats
{
    public readonly struct CheatsCpuProfilerCategoryStats
    {
        public CheatsCpuProfilerCategoryStats(
            CheatsCpuProfilerCategory category,
            float currentMs,
            float averageMs,
            float minMs,
            float maxMs,
            int sampleCount)
        {
            CurrentMs = currentMs;
            AverageMs = averageMs;
            MinMs = minMs;
            MaxMs = maxMs;
            SampleCount = sampleCount;
        }

        public float CurrentMs { get; }
        public float AverageMs { get; }
        public float MinMs { get; }
        public float MaxMs { get; }
        public int SampleCount { get; }
    }
}
