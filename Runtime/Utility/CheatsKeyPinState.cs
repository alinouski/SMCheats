using System;
using System.Collections.Generic;

namespace SMCheats
{
    internal sealed class CheatsKeyPinState
    {
        private readonly HashSet<string> _pinnedKeys = new(StringComparer.Ordinal);

        public event Action Changed;

        public void Clear()
        {
            if (_pinnedKeys.Count == 0)
            {
                return;
            }

            _pinnedKeys.Clear();
            Changed?.Invoke();
        }

        public bool IsPinned(string key)
        {
            return !string.IsNullOrEmpty(key) && _pinnedKeys.Contains(key);
        }

        public void Toggle(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (!_pinnedKeys.Add(key))
            {
                _pinnedKeys.Remove(key);
            }

            Changed?.Invoke();
        }

        public void SetPinned(IEnumerable<string> keys, bool isPinned)
        {
            if (keys == null)
            {
                return;
            }

            var changed = false;
            foreach (var key in keys)
            {
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                changed |= isPinned
                    ? _pinnedKeys.Add(key)
                    : _pinnedKeys.Remove(key);
            }

            if (changed)
            {
                Changed?.Invoke();
            }
        }

        public void RemoveWhere(Predicate<string> predicate, bool notify = true)
        {
            if (predicate == null || _pinnedKeys.Count == 0)
            {
                return;
            }

            var removed = _pinnedKeys.RemoveWhere(predicate);
            if (removed > 0 && notify)
            {
                Changed?.Invoke();
            }
        }
    }
}
