using System;
using System.Collections.Generic;
using UnityEngine;

namespace SMCheats
{
    public sealed class CheatsGraphDataSource
    {
        private const int DefaultSampleCapacity = 120;
        private const float DisplayMaxValueFallSharpness = 5f;

        private readonly ICheatsGraphCurrentValueSource[] _currentValueSources;
        private readonly ICheatsGraphFrameValueSource _frameValueSource;
        private readonly CheatsGraphFrameSample[] _samples;
        private readonly Func<float> _maxValueSource;
        private int _nextSampleIndex;
        private int _sampleCount;
        private float _sampleSum;
        private float _displayMaxValue;
        private bool _hasDisplayMaxValue;

        public CheatsGraphDataSource(
            string title,
            CheatsGraphVisualConfig visualConfig,
            IReadOnlyList<ICheatsGraphCurrentValueSource> currentValueSources,
            ICheatsGraphFrameValueSource frameValueSource,
            int sampleCapacity = DefaultSampleCapacity,
            bool isActive = true,
            float? fixedMaxValue = null,
            Func<float> maxValueSource = null)
            : this(
                title,
                visualConfig != null ? visualConfig.CreateDisplayConfig() : CheatsGraphVisualConfig.CreateDefaultDisplayConfig(),
                currentValueSources,
                frameValueSource,
                sampleCapacity,
                isActive,
                fixedMaxValue,
                maxValueSource)
        {
        }

        public CheatsGraphDataSource(
            string title,
            CheatsGraphDisplayConfig displayConfig,
            IReadOnlyList<ICheatsGraphCurrentValueSource> currentValueSources,
            ICheatsGraphFrameValueSource frameValueSource,
            int sampleCapacity = DefaultSampleCapacity,
            bool isActive = true,
            float? fixedMaxValue = null,
            Func<float> maxValueSource = null)
        {
            Title = title ?? string.Empty;
            DisplayConfig = displayConfig;
            _frameValueSource = frameValueSource;
            _samples = new CheatsGraphFrameSample[Mathf.Max(1, sampleCapacity)];
            _currentValueSources = CopyCurrentValueSources(currentValueSources);
            _maxValueSource = maxValueSource;
            FixedMaxValue = fixedMaxValue;
            IsActive = isActive;
        }

        public string Title { get; }
        public CheatsGraphDisplayConfig DisplayConfig { get; }
        public float? FixedMaxValue { get; }
        public bool IsActive { get; private set; }
        public int SampleCount => _sampleCount;
        public int SampleCapacity => _samples.Length;
        public float LatestValue => _sampleCount <= 0 ? 0f : GetSample(_sampleCount - 1).Value;
        public float AverageValue => _sampleCount <= 0 ? 0f : _sampleSum / _sampleCount;
        public float MinValue => GetExtremum(float.MaxValue, Mathf.Min);
        public float MaxValue => GetExtremum(float.MinValue, Mathf.Max);
        public float DisplayMaxValue => _maxValueSource?.Invoke() ?? FixedMaxValue ?? (_hasDisplayMaxValue ? _displayMaxValue : MaxValue);
        public int ValuesAmount => _currentValueSources.Length;


        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Tick()
        {
            if (!IsActive)
            {
                return;
            }

            AddSample(_frameValueSource?.GetFrameSample() ?? new CheatsGraphFrameSample(0f));
            UpdateDisplayMaxValue();
        }

        public CheatsGraphFrameSample GetSample(int visualIndex)
        {
            if (_sampleCount <= 0)
            {
                return new CheatsGraphFrameSample(0f);
            }

            var clampedIndex = Mathf.Clamp(visualIndex, 0, _sampleCount - 1);
            var firstIndex = _sampleCount < _samples.Length ? 0 : _nextSampleIndex;
            return _samples[(firstIndex + clampedIndex) % _samples.Length];
        }

        public ICheatsGraphCurrentValueSource CurrentValueSource(int i)
        {
            return _currentValueSources[i];
        }

        private static ICheatsGraphCurrentValueSource[] CopyCurrentValueSources(IReadOnlyList<ICheatsGraphCurrentValueSource> sources)
        {
            if (sources == null || sources.Count == 0)
            {
                return new ICheatsGraphCurrentValueSource[0];
            }

            var copy = new ICheatsGraphCurrentValueSource[sources.Count];
            for (var i = 0; i < sources.Count; i++)
            {
                copy[i] = sources[i];
            }

            return copy;
        }

        private void AddSample(CheatsGraphFrameSample sample)
        {
            if (_sampleCount < _samples.Length)
            {
                _samples[_nextSampleIndex] = sample;
                _sampleSum += sample.Value;
                _sampleCount++;
                _nextSampleIndex = (_nextSampleIndex + 1) % _samples.Length;
                return;
            }

            _sampleSum -= _samples[_nextSampleIndex].Value;
            _samples[_nextSampleIndex] = sample;
            _sampleSum += sample.Value;
            _nextSampleIndex = (_nextSampleIndex + 1) % _samples.Length;
        }

        private void UpdateDisplayMaxValue()
        {
            if (FixedMaxValue.HasValue || _maxValueSource != null)
            {
                return;
            }

            var targetMaxValue = Mathf.Max(0f, MaxValue);
            if (!_hasDisplayMaxValue || targetMaxValue >= _displayMaxValue)
            {
                _displayMaxValue = targetMaxValue;
                _hasDisplayMaxValue = true;
                return;
            }

            var t = 1f - Mathf.Exp(-DisplayMaxValueFallSharpness * Mathf.Max(0f, Time.unscaledDeltaTime));
            _displayMaxValue = Mathf.Lerp(_displayMaxValue, targetMaxValue, t);
            if (Mathf.Approximately(_displayMaxValue, targetMaxValue))
            {
                _displayMaxValue = targetMaxValue;
            }
        }

        private float GetExtremum(float initialValue, System.Func<float, float, float> selector)
        {
            if (_sampleCount <= 0)
            {
                return 0f;
            }

            var result = initialValue;
            for (var i = 0; i < _sampleCount; i++)
            {
                result = selector(result, GetSample(i).Value);
            }

            return result;
        }
    }
}
