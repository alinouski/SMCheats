using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace SMCheats
{
    internal sealed class CheatsCpuProfilerService : ICheatsCpuProfilerService, ICheatsUpdateTickable, IDisposable
    {
        private static readonly ProfilerRecorderOptions FrameRecorderOptions =
            ProfilerRecorderOptions.StartImmediately;
        private static readonly ProfilerRecorderOptions CategoryRecorderOptions =
            ProfilerRecorderOptions.StartImmediately |
            ProfilerRecorderOptions.SumAllSamplesInFrame;

        private readonly float[] _frameTimeSamples;
        private readonly CategoryRecorder[] _categoryRecorders;
        private readonly Dictionary<CheatsCpuProfilerCategory, float> _categoryStats = new();
        private ProfilerRecorder _mainThreadRecorder;
        private int _nextSampleIndex;
        private int _sampleCount;
        private float _frameTimeSum;

        public CheatsCpuProfilerService()
        {
            _frameTimeSamples = new float[SampleCapacity];
            _mainThreadRecorder = TryCreateRecorder(ProfilerCategory.Internal, "Main Thread", FrameRecorderOptions);
            _categoryRecorders = new[]
            {
                CreateDerivedCategoryRecorder(CheatsCpuProfilerCategory.Scripts, "Scripts", SampleCapacity),
                CreateCategoryRecorder(CheatsCpuProfilerCategory.Physics, "Physics", ProfilerCategory.Physics, SampleCapacity, "Physics.Simulate"),
                CreateCategoryRecorder(CheatsCpuProfilerCategory.Physics2D, "Physics 2D", ProfilerCategory.Physics2D, SampleCapacity, "Physics2D.Simulate"),
                CreateCategoryRecorder(CheatsCpuProfilerCategory.Animation, "Animation", ProfilerCategory.Animation, SampleCapacity, "Animation.Update"),
                CreateCategoryRecorder(CheatsCpuProfilerCategory.Rendering, "Rendering", ProfilerCategory.Render, SampleCapacity, "Camera.Render"),
                CreateCategoryRecorder(CheatsCpuProfilerCategory.UI, "UI", ProfilerCategory.Gui, SampleCapacity, "UGUI.Rendering.RenderOverlays", "Canvas.BuildBatch"),
                CreateCategoryRecorder(CheatsCpuProfilerCategory.Audio, "Audio", ProfilerCategory.Audio, SampleCapacity, "AudioManager.Update")
            };
        }

        public float CurrentFrameTimeMs { get; private set; }
        public float AverageFrameTimeMs => _sampleCount <= 0 ? 0f : _frameTimeSum / _sampleCount;
        public float MinFrameTimeMs => GetFrameTimeExtremum(float.MaxValue, Mathf.Min);
        public float MaxFrameTimeMs => GetFrameTimeExtremum(float.MinValue, Mathf.Max);

        public int SampleCapacity => 150;
        public int SampleCount => _sampleCount;
        public int NextSampleIndex => _nextSampleIndex;
        public float[] FrameTimeSamples => _frameTimeSamples;



        public float GetCategoryMsUsage(CheatsCpuProfilerCategory categories)
        {
            return _categoryStats.GetValueOrDefault(categories, 0);
        }

        public CheatsCpuProfilerCategoryStats GetCategoryStat(CheatsCpuProfilerCategory categories)
        {
            for (var i = 0; i < _categoryRecorders.Length; i++)
            {
                var categoryRecorder = _categoryRecorders[i];
                if (!categoryRecorder.IsAvailable || (categories & categoryRecorder.Category) == 0)
                {
                    continue;
                }

                return categoryRecorder.CreateStats();
            }

            return default;
        }

        public void Tick()
        {
            CurrentFrameTimeMs = ReadMilliseconds(_mainThreadRecorder);
            if (CurrentFrameTimeMs <= 0f)
            {
                CurrentFrameTimeMs = Mathf.Max(0.0001f, Time.unscaledDeltaTime) * 1000f;
            }

            AddFrameSample(CurrentFrameTimeMs);
            UpdateCategoryStats();
        }

        public void Dispose()
        {
            if (_mainThreadRecorder.Valid)
            {
                _mainThreadRecorder.Dispose();
            }

            for (var i = 0; i < _categoryRecorders.Length; i++)
            {
                _categoryRecorders[i].Dispose();
            }
        }

        private static CategoryRecorder CreateDerivedCategoryRecorder(CheatsCpuProfilerCategory cpuCategory, string name, int sampleCapacity)
        {
            return new CategoryRecorder(cpuCategory, name, Array.Empty<ProfilerRecorder>(), true, sampleCapacity);
        }

        private static CategoryRecorder CreateCategoryRecorder(
            CheatsCpuProfilerCategory cpuCategory,
            string name,
            ProfilerCategory category,
            int sampleCapacity,
            params string[] markerNames)
        {
            var recorders = new List<ProfilerRecorder>();
            for (var i = 0; i < markerNames.Length; i++)
            {
                var recorder = TryCreateRecorder(category, markerNames[i], CategoryRecorderOptions);
                if (recorder.Valid)
                {
                    recorders.Add(recorder);
                }
            }

            return new CategoryRecorder(cpuCategory, name, recorders.ToArray(), false, sampleCapacity);
        }

        private static ProfilerRecorder TryCreateRecorder(
            ProfilerCategory category,
            string markerName,
            ProfilerRecorderOptions options)
        {
            try
            {
                return ProfilerRecorder.StartNew(category, markerName, 1, options);
            }
            catch
            {
                return default;
            }
        }

        private static float ReadMilliseconds(ProfilerRecorder recorder)
        {
            if (!recorder.Valid)
            {
                return 0f;
            }

            return recorder.UnitType == ProfilerMarkerDataUnit.TimeNanoseconds
                ? (float)(recorder.LastValueAsDouble / 1000000d)
                : (float)recorder.LastValueAsDouble;
        }

        private void AddFrameSample(float value)
        {
            if (_sampleCount < _frameTimeSamples.Length)
            {
                _frameTimeSamples[_nextSampleIndex] = value;
                _frameTimeSum += value;
                _sampleCount++;
                _nextSampleIndex = (_nextSampleIndex + 1) % _frameTimeSamples.Length;
                return;
            }

            _frameTimeSum -= _frameTimeSamples[_nextSampleIndex];
            _frameTimeSamples[_nextSampleIndex] = value;
            _frameTimeSum += value;
            _nextSampleIndex = (_nextSampleIndex + 1) % _frameTimeSamples.Length;
        }

        private void UpdateCategoryStats()
        {
            for (var i = 0; i < _categoryRecorders.Length; i++)
            {
                var categoryRecorder = _categoryRecorders[i];
                if (!categoryRecorder.IsAvailable || categoryRecorder.IsDerived)
                {
                    continue;
                }

                categoryRecorder.Tick();
                _categoryRecorders[i] = categoryRecorder;
            }

            UpdateDerivedScriptsStats();

            for (var i = 0; i < _categoryRecorders.Length; i++)
            {
                var categoryRecorder = _categoryRecorders[i];
                if (!categoryRecorder.IsAvailable)
                {
                    continue;
                }

                _categoryStats[categoryRecorder.Category] = categoryRecorder.CurrentMs;
            }
        }

        private void UpdateDerivedScriptsStats()
        {
            var scriptsMs = Mathf.Max(
                0f,
                CurrentFrameTimeMs - GetCategoryCurrentSum(CheatsCpuProfilerCategory.All & ~CheatsCpuProfilerCategory.Scripts));

            for (var i = 0; i < _categoryRecorders.Length; i++)
            {
                var categoryRecorder = _categoryRecorders[i];
                if (categoryRecorder.Category != CheatsCpuProfilerCategory.Scripts)
                {
                    continue;
                }

                categoryRecorder.Record(scriptsMs);
                _categoryRecorders[i] = categoryRecorder;
                return;
            }
        }

        private float GetFrameTimeExtremum(float initialValue, Func<float, float, float> selector)
        {
            if (_sampleCount <= 0)
            {
                return 0f;
            }

            var result = initialValue;
            for (var i = 0; i < _sampleCount; i++)
            {
                result = selector(result, _frameTimeSamples[i]);
            }

            return result;
        }

        private CheatsCpuProfilerCategoryStats CreateOtherCategoryStats(CheatsCpuProfilerCategory selectedCategories)
        {
            if (_sampleCount <= 0)
            {
                return new CheatsCpuProfilerCategoryStats(
                    CheatsCpuProfilerCategory.Other,
                    0f,
                    0f,
                    0f,
                    0f,
                    0);
            }

            var sum = 0f;
            var min = float.MaxValue;
            var max = float.MinValue;

            for (var i = 0; i < _sampleCount; i++)
            {
                var value = Mathf.Max(0f, _frameTimeSamples[i] - GetCategorySampleSum(selectedCategories, i));
                sum += value;
                min = Mathf.Min(min, value);
                max = Mathf.Max(max, value);
            }

            return new CheatsCpuProfilerCategoryStats(
                CheatsCpuProfilerCategory.Other,
                Mathf.Max(0f, CurrentFrameTimeMs - GetCategoryCurrentSum(selectedCategories)),
                sum / _sampleCount,
                min,
                max,
                _sampleCount);
        }

        private float GetCategorySampleSum(CheatsCpuProfilerCategory selectedCategories, int sampleIndex)
        {
            var sum = 0f;
            for (var i = 0; i < _categoryRecorders.Length; i++)
            {
                var categoryRecorder = _categoryRecorders[i];
                if (!categoryRecorder.IsAvailable || (selectedCategories & categoryRecorder.Category) == 0)
                {
                    continue;
                }

                sum += categoryRecorder.GetSample(sampleIndex);
            }

            return sum;
        }

        private float GetCategoryCurrentSum(CheatsCpuProfilerCategory selectedCategories)
        {
            var sum = 0f;
            for (var i = 0; i < _categoryRecorders.Length; i++)
            {
                var categoryRecorder = _categoryRecorders[i];
                if (!categoryRecorder.IsAvailable || (selectedCategories & categoryRecorder.Category) == 0)
                {
                    continue;
                }

                sum += categoryRecorder.CurrentMs;
            }

            return sum;
        }

        private struct CategoryRecorder
        {
            private readonly CheatsCpuProfilerCategory _category;
            private readonly string _name;
            private readonly ProfilerRecorder[] _recorders;
            private readonly bool _isDerived;
            private readonly float[] _samples;
            private int _nextSampleIndex;
            private int _sampleCount;
            private float _sum;
            private float _currentMs;

            public CategoryRecorder(CheatsCpuProfilerCategory category,
                string name,
                ProfilerRecorder[] recorders,
                bool isDerived,
                int sampleCapacity)
            {
                _category = category;
                _name = name ?? string.Empty;
                _recorders = recorders ?? Array.Empty<ProfilerRecorder>();
                _isDerived = isDerived;
                _samples = new float[sampleCapacity];
                _nextSampleIndex = 0;
                _sampleCount = 0;
                _sum = 0f;
                _currentMs = 0f;
            }

            public CheatsCpuProfilerCategory Category => _category;
            public bool IsAvailable => _isDerived || _recorders.Length > 0;
            public bool IsDerived => _isDerived;
            public float CurrentMs => _currentMs;

            public void Tick()
            {
                _currentMs = 0f;
                for (var i = 0; i < _recorders.Length; i++)
                {
                    _currentMs += ReadMilliseconds(_recorders[i]);
                }

                AddSample(_currentMs);
            }

            public void Record(float value)
            {
                _currentMs = value;
                AddSample(_currentMs);
            }

            public CheatsCpuProfilerCategoryStats CreateStats()
            {
                return new CheatsCpuProfilerCategoryStats(
                    _category,
                    _currentMs,
                    _sampleCount <= 0 ? 0f : _sum / _sampleCount,
                    GetExtremum(float.MaxValue, Mathf.Min),
                    GetExtremum(float.MinValue, Mathf.Max),
                    _sampleCount);
            }

            public float GetSample(int index)
            {
                return index >= 0 && index < _sampleCount ? _samples[index] : 0f;
            }

            public void Dispose()
            {
                for (var i = 0; i < _recorders.Length; i++)
                {
                    if (_recorders[i].Valid)
                    {
                        _recorders[i].Dispose();
                    }
                }
            }

            private void AddSample(float value)
            {
                if (_sampleCount < _samples.Length)
                {
                    _samples[_nextSampleIndex] = value;
                    _sum += value;
                    _sampleCount++;
                    _nextSampleIndex = (_nextSampleIndex + 1) % _samples.Length;
                    return;
                }

                _sum -= _samples[_nextSampleIndex];
                _samples[_nextSampleIndex] = value;
                _sum += value;
                _nextSampleIndex = (_nextSampleIndex + 1) % _samples.Length;
            }

            private float GetExtremum(float initialValue, Func<float, float, float> selector)
            {
                if (_sampleCount <= 0)
                {
                    return 0f;
                }

                var result = initialValue;
                for (var i = 0; i < _sampleCount; i++)
                {
                    result = selector(result, _samples[i]);
                }

                return result;
            }
        }
    }
}
