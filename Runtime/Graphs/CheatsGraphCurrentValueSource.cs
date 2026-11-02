using System;
using UnityEngine;

namespace SMCheats
{
    public sealed class CheatsGraphCurrentValueSource : ICheatsGraphCurrentValueSource
    {
        private readonly Func<float> _getter;
        private readonly Func<float, string> _formatter;
        private string _cachedString;
        private float _cachedValue;

        public CheatsGraphCurrentValueSource(string name, Func<float> getter, Func<float, string> formatter, Color? color = null)
        {
            Name = name ?? string.Empty;
            _getter = getter;
            _formatter = formatter;
            Color = color;
        }

        public Color? Color { get; }

        public string Name { get; }
        public string GetValueString()
        {
            var val = GetValue();

            if (Mathf.Approximately(val, _cachedValue) && !string.IsNullOrEmpty(_cachedString))
            {
                return _cachedString;
            }

            _cachedValue = val;
            _cachedString = _formatter.Invoke(_cachedValue);
            
            return _cachedString;
        }

        public float GetValue()
        {
            return _getter.Invoke();
        }
    }
}