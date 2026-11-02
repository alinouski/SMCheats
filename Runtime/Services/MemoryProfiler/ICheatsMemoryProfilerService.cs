namespace SMCheats
{
    public interface ICheatsMemoryProfilerService
    {
        int SampleCapacity { get; }
        int SampleCount { get; }
        float CurrentReservedMemoryMb { get; }
        float CurrentUsedMemoryMb { get; }
        float CurrentRegularMemoryMb { get; }
        float CurrentGcReservedMemoryMb { get; }
        float AverageUsedMemoryMb { get; }
        float MinUsedMemoryMb { get; }
        float MaxUsedMemoryMb { get; }
        CheatsMemoryProfilerStats Stats { get; }
    }
}
