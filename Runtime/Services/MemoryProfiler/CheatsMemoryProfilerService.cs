using System;
using Unity.Profiling;
using UnityEngine;

namespace SMCheats
{
    internal sealed class CheatsMemoryProfilerService : ICheatsMemoryProfilerService, ICheatsUpdateTickable, IDisposable
    {
        private const int MemorySampleCapacity = 150;
        private const double BytesPerMegabyte = 1024d * 1024d;

        private static readonly ProfilerRecorderOptions RecorderOptions =
            ProfilerRecorderOptions.StartImmediately;

        private readonly float[] _usedMemorySamples = new float[MemorySampleCapacity];
        private ProfilerRecorder _reservedMemoryRecorder;
        private ProfilerRecorder _systemMemoryRecorder;
        private ProfilerRecorder _gcMemoryRecorder;
        private int _nextSampleIndex;
        private int _sampleCount;
        private float _usedMemorySum;

        public CheatsMemoryProfilerService()
        {
            _reservedMemoryRecorder = TryCreateRecorder("Total Reserved Memory");
            _systemMemoryRecorder = TryCreateRecorder("System Used Memory");
            _gcMemoryRecorder = TryCreateRecorder("GC Reserved Memory");
        }

        public int SampleCapacity => MemorySampleCapacity;
        public int SampleCount => _sampleCount;
        public float CurrentReservedMemoryMb { get; private set; }
        public float CurrentUsedMemoryMb { get; private set; }
        public float CurrentRegularMemoryMb { get; private set; }
        public float CurrentGcReservedMemoryMb { get; private set; }
        public float AverageUsedMemoryMb => _sampleCount <= 0 ? 0f : _usedMemorySum / _sampleCount;
        public float MinUsedMemoryMb => GetUsedMemoryExtremum(float.MaxValue, Mathf.Min);
        public float MaxUsedMemoryMb => GetUsedMemoryExtremum(float.MinValue, Mathf.Max);

        public CheatsMemoryProfilerStats Stats => new(
            CurrentReservedMemoryMb,
            CurrentUsedMemoryMb,
            CurrentRegularMemoryMb,
            CurrentGcReservedMemoryMb,
            AverageUsedMemoryMb,
            MinUsedMemoryMb,
            MaxUsedMemoryMb,
            _sampleCount);

        public void Tick()
        {
            CurrentReservedMemoryMb = ReadMegabytes(_reservedMemoryRecorder);
            CurrentUsedMemoryMb = ReadMegabytes(_systemMemoryRecorder);
            CurrentGcReservedMemoryMb = ReadMegabytes(_gcMemoryRecorder);
            if (CurrentReservedMemoryMb <= 0f)
            {
                CurrentReservedMemoryMb = CurrentUsedMemoryMb;
            }

            CurrentRegularMemoryMb = Mathf.Max(0f, CurrentUsedMemoryMb - CurrentGcReservedMemoryMb);

            AddUsedMemorySample(CurrentUsedMemoryMb);
        }

        public void Dispose()
        {
            if (_reservedMemoryRecorder.Valid)
            {
                _reservedMemoryRecorder.Dispose();
            }

            if (_systemMemoryRecorder.Valid)
            {
                _systemMemoryRecorder.Dispose();
            }

            if (_gcMemoryRecorder.Valid)
            {
                _gcMemoryRecorder.Dispose();
            }
        }

        private static ProfilerRecorder TryCreateRecorder(string markerName)
        {
            try
            {
                return ProfilerRecorder.StartNew(ProfilerCategory.Memory, markerName, 1, RecorderOptions);
            }
            catch
            {
                return default;
            }
        }

        private static float ReadMegabytes(ProfilerRecorder recorder)
        {
            if (!recorder.Valid)
            {
                return 0f;
            }

            return (float)(recorder.LastValueAsDouble / BytesPerMegabyte);
        }

        private void AddUsedMemorySample(float value)
        {
            if (_sampleCount < _usedMemorySamples.Length)
            {
                _usedMemorySamples[_nextSampleIndex] = value;
                _usedMemorySum += value;
                _sampleCount++;
                _nextSampleIndex = (_nextSampleIndex + 1) % _usedMemorySamples.Length;
                return;
            }

            _usedMemorySum -= _usedMemorySamples[_nextSampleIndex];
            _usedMemorySamples[_nextSampleIndex] = value;
            _usedMemorySum += value;
            _nextSampleIndex = (_nextSampleIndex + 1) % _usedMemorySamples.Length;
        }

        private float GetUsedMemoryExtremum(float initialValue, Func<float, float, float> selector)
        {
            if (_sampleCount <= 0)
            {
                return 0f;
            }

            var result = initialValue;
            for (var i = 0; i < _sampleCount; i++)
            {
                result = selector(result, _usedMemorySamples[i]);
            }

            return result;
        }
    }
}
