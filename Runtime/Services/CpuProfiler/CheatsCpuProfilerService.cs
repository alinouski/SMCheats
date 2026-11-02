using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace SMCheats
{
    internal sealed class CheatsCpuProfilerService : ICheatsCpuProfilerService, ICheatsUpdateTickable, IDisposable
    {
        // Markers that are not registered yet (e.g. created on first use) are looked up again with this period.
        private const int RecorderRetryIntervalFrames = 120;

        private static readonly ProfilerRecorderOptions FrameRecorderOptions =
            ProfilerRecorderOptions.StartImmediately;
        private static readonly ProfilerRecorderOptions CategoryRecorderOptions =
            ProfilerRecorderOptions.StartImmediately |
            ProfilerRecorderOptions.SumAllSamplesInFrame;

        private readonly float[] _frameTimeSamples;
        private readonly CategoryRecorder[] _categoryRecorders;
        private readonly CategoryRecorder _waitRecorder;
        private readonly Dictionary<CheatsCpuProfilerCategory, float> _categoryStats = new();
        private ProfilerRecorder _mainThreadRecorder;
        private int _nextSampleIndex;
        private int _sampleCount;
        private float _frameTimeSum;
        private int _framesUntilRetry;

        public CheatsCpuProfilerService()
        {
            _frameTimeSamples = new float[SampleCapacity];
            _mainThreadRecorder = TryCreateRecorder("Main Thread", FrameRecorderOptions);

            // Main thread idle time: frame pacing and waiting for the render thread / GPU. Recorded in release builds too.
            _waitRecorder = new CategoryRecorder(
                CheatsCpuProfilerCategory.None,
                "WaitForTargetFPS",
                "Gfx.WaitForPresentOnGfxThread",
                "Gfx.WaitForRenderThread");

            // Player loop markers work for every render pipeline; Camera.Render, for example, exists only in Built-in RP.
            // Category markers are recorded only in the editor and development builds, "Main Thread" is available everywhere.
            _categoryRecorders = !Debug.isDebugBuild
                ? Array.Empty<CategoryRecorder>()
                : new[]
                {
                    new CategoryRecorder(
                        CheatsCpuProfilerCategory.Scripts,
                        "Update.ScriptRunBehaviourUpdate",
                        "PreLateUpdate.ScriptRunBehaviourLateUpdate",
                        "FixedUpdate.ScriptRunBehaviourFixedUpdate",
                        "Update.ScriptRunDelayedDynamicFrameRate",
                        "FixedUpdate.ScriptRunDelayedFixedFrameRate"),
                    new CategoryRecorder(CheatsCpuProfilerCategory.Physics, "FixedUpdate.PhysicsFixedUpdate"),
                    new CategoryRecorder(CheatsCpuProfilerCategory.Physics2D, "FixedUpdate.Physics2DFixedUpdate"),
                    new CategoryRecorder(
                        CheatsCpuProfilerCategory.Animation,
                        "PreLateUpdate.DirectorUpdateAnimationBegin",
                        "PreLateUpdate.DirectorUpdateAnimationEnd",
                        "PreLateUpdate.LegacyAnimationUpdate"),
                    new CategoryRecorder(CheatsCpuProfilerCategory.Rendering, "PostLateUpdate.FinishFrameRendering"),
                    new CategoryRecorder(CheatsCpuProfilerCategory.UI, "PostLateUpdate.PlayerUpdateCanvases"),
                    new CategoryRecorder(CheatsCpuProfilerCategory.Audio, "PostLateUpdate.UpdateAudio")
                };
        }

        public float CurrentFrameTimeMs { get; private set; }
        public float CurrentCpuTimeMs { get; private set; }
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

        public bool IsCategoryAvailable(CheatsCpuProfilerCategory categories)
        {
            for (var i = 0; i < _categoryRecorders.Length; i++)
            {
                var categoryRecorder = _categoryRecorders[i];
                if ((categories & categoryRecorder.Category) != 0 && categoryRecorder.IsAvailable)
                {
                    return true;
                }
            }

            return false;
        }

        public void Tick()
        {
            CurrentFrameTimeMs = ReadMilliseconds(_mainThreadRecorder);
            if (CurrentFrameTimeMs <= 0f)
            {
                CurrentFrameTimeMs = Mathf.Max(0.0001f, Time.unscaledDeltaTime) * 1000f;
            }

            AddFrameSample(CurrentFrameTimeMs);
            RetryMissingRecorders();
            UpdateCategoryStats();

            _waitRecorder.Tick();
            CurrentCpuTimeMs = Mathf.Max(0f, CurrentFrameTimeMs - _waitRecorder.CurrentMs);
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

            _waitRecorder.Dispose();
        }

        private static ProfilerRecorder TryCreateRecorder(string markerName, ProfilerRecorderOptions options)
        {
            try
            {
                // The category is ignored when a marker is looked up by name.
                return ProfilerRecorder.StartNew(ProfilerCategory.Internal, markerName, 1, options);
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

        private void RetryMissingRecorders()
        {
            if (--_framesUntilRetry > 0)
            {
                return;
            }

            _framesUntilRetry = RecorderRetryIntervalFrames;
            _waitRecorder.CreateMissingRecorders();
            for (var i = 0; i < _categoryRecorders.Length; i++)
            {
                _categoryRecorders[i].CreateMissingRecorders();
            }
        }

        private void UpdateCategoryStats()
        {
            for (var i = 0; i < _categoryRecorders.Length; i++)
            {
                var categoryRecorder = _categoryRecorders[i];
                categoryRecorder.Tick();
                _categoryStats[categoryRecorder.Category] = categoryRecorder.CurrentMs;
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

        private sealed class CategoryRecorder
        {
            private readonly string[] _markerNames;
            private readonly ProfilerRecorder[] _recorders;

            public CategoryRecorder(CheatsCpuProfilerCategory category, params string[] markerNames)
            {
                Category = category;
                _markerNames = markerNames;
                _recorders = new ProfilerRecorder[markerNames.Length];
            }

            public CheatsCpuProfilerCategory Category { get; }
            public bool IsAvailable { get; private set; }
            public float CurrentMs { get; private set; }

            public void CreateMissingRecorders()
            {
                for (var i = 0; i < _recorders.Length; i++)
                {
                    if (_recorders[i].Valid)
                    {
                        continue;
                    }

                    _recorders[i] = TryCreateRecorder(_markerNames[i], CategoryRecorderOptions);
                    IsAvailable |= _recorders[i].Valid;
                }
            }

            public void Tick()
            {
                var currentMs = 0f;
                for (var i = 0; i < _recorders.Length; i++)
                {
                    currentMs += ReadMilliseconds(_recorders[i]);
                }

                CurrentMs = currentMs;
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
        }
    }
}
