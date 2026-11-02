namespace SMCheats
{
    public readonly struct CheatsMemoryProfilerStats
    {
        public CheatsMemoryProfilerStats(
            float currentReservedMemoryMb,
            float currentUsedMemoryMb,
            float currentRegularMemoryMb,
            float currentGcReservedMemoryMb,
            float averageUsedMemoryMb,
            float minUsedMemoryMb,
            float maxUsedMemoryMb,
            int sampleCount)
        {
            CurrentReservedMemoryMb = currentReservedMemoryMb;
            CurrentUsedMemoryMb = currentUsedMemoryMb;
            CurrentRegularMemoryMb = currentRegularMemoryMb;
            CurrentGcReservedMemoryMb = currentGcReservedMemoryMb;
            AverageUsedMemoryMb = averageUsedMemoryMb;
            MinUsedMemoryMb = minUsedMemoryMb;
            MaxUsedMemoryMb = maxUsedMemoryMb;
            SampleCount = sampleCount;
        }

        public float CurrentReservedMemoryMb { get; }
        public float CurrentUsedMemoryMb { get; }
        public float CurrentRegularMemoryMb { get; }
        public float CurrentGcReservedMemoryMb { get; }
        public float AverageUsedMemoryMb { get; }
        public float MinUsedMemoryMb { get; }
        public float MaxUsedMemoryMb { get; }
        public int SampleCount { get; }
    }
}
