using UnityEngine;

namespace SMCheats
{
    internal sealed class CheatsFpsProfilerService : ICheatsUpdateTickable
    {
        private readonly float[] _fpsSamples;
        private readonly float[] _frameTimeSamples;
        private int _nextSampleIndex;
        private int _sampleCount;
        private float _fpsSum;
        private float _frameTimeSum;

        public float CurrentFps { get; private set; }
        public float CurrentFrameTimeMs { get; private set; }
        public float AverageFps => _sampleCount <= 0 ? 0f : _fpsSum / _sampleCount;
        public float MinFps => GetFpsExtremum(float.MaxValue, Mathf.Min);
        public float MaxFps => GetFpsExtremum(float.MinValue, Mathf.Max);
        public float AverageFrameTimeMs => _sampleCount <= 0 ? 0f : _frameTimeSum / _sampleCount;
        public float MinFrameTimeMs => GetFrameTimeExtremum(float.MaxValue, Mathf.Min);
        public float MaxFrameTimeMs => GetFrameTimeExtremum(float.MinValue, Mathf.Max);
        public int SampleCount => _sampleCount;
        public int SampleCapacity => 150;
        public int NextSampleIndex => _nextSampleIndex;
        public float[] FpsSamples => _fpsSamples;
        public float[] FrameTimeSamples => _frameTimeSamples;

        public CheatsFpsProfilerService()
        {
            _fpsSamples = new float[SampleCapacity];
            _frameTimeSamples = new float[SampleCapacity];
        }

        public void Tick()
        {
            CurrentFrameTimeMs = Mathf.Max(0.0001f, Time.unscaledDeltaTime) * 1000f;
            CurrentFps = 1f / Mathf.Max(0.0001f, Time.unscaledDeltaTime);

            if (_sampleCount < _fpsSamples.Length)
            {
                _fpsSamples[_nextSampleIndex] = CurrentFps;
                _frameTimeSamples[_nextSampleIndex] = CurrentFrameTimeMs;
                _fpsSum += CurrentFps;
                _frameTimeSum += CurrentFrameTimeMs;
                _sampleCount++;
                _nextSampleIndex = (_nextSampleIndex + 1) % _fpsSamples.Length;
                return;
            }

            _fpsSum -= _fpsSamples[_nextSampleIndex];
            _frameTimeSum -= _frameTimeSamples[_nextSampleIndex];
            _fpsSamples[_nextSampleIndex] = CurrentFps;
            _frameTimeSamples[_nextSampleIndex] = CurrentFrameTimeMs;
            _fpsSum += CurrentFps;
            _frameTimeSum += CurrentFrameTimeMs;
            _nextSampleIndex = (_nextSampleIndex + 1) % _fpsSamples.Length;
        }

        private float GetFpsExtremum(float initialValue, System.Func<float, float, float> selector)
        {
            if (_sampleCount <= 0)
            {
                return 0f;
            }

            var result = initialValue;
            for (var i = 0; i < _sampleCount; i++)
            {
                result = selector(result, _fpsSamples[i]);
            }

            return result;
        }

        private float GetFrameTimeExtremum(float initialValue, System.Func<float, float, float> selector)
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
    }
}
