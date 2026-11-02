using System;
using System.Collections.Generic;

namespace SMCheats
{
    public sealed class CheatsOptionsService
    {
        private readonly List<CheatOptionDefinition> _options = new();
        private readonly Dictionary<object, List<CheatOptionDefinition>> _registeredTargets = new();

        public IReadOnlyList<CheatOptionDefinition> Options => _options;
        public int Revision { get; private set; }
        public event Action Changed;

        public CheatOptionDefinition AddAction(string name, Action callback, string category, int sortOrder)
        {
            return Add(CheatOptionDefinition.Action(name, callback, category, sortOrder));
        }

        public CheatOptionDefinition AddToggle(string name, Func<bool> getter, Action<bool> setter, string category, int sortOrder)
        {
            return Add(CheatOptionDefinition.Toggle(name, getter, setter, category, sortOrder));
        }

        public CheatOptionDefinition AddValue<T>(
            string name,
            Func<T> getter,
            Action<T> setter,
            string category,
            int sortOrder,
            double? minimumValue = null,
            double? maximumValue = null,
            double? step = null)
        {
            if (getter == null)
            {
                throw new ArgumentNullException(nameof(getter));
            }

            var option = CheatOptionDefinition
                .Value(
                    name,
                    typeof(T),
                    () => getter(),
                    setter == null ? null : value => setter((T)value),
                    null,
                    category,
                    sortOrder)
                .WithNumericRange(minimumValue, maximumValue, step);
            return Add(option);
        }

        public CheatOptionDefinition Add(CheatOptionDefinition option)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            return Add(option, true);
        }

        public void Clear()
        {
            if (_options.Count == 0 && _registeredTargets.Count == 0)
            {
                return;
            }

            _options.Clear();
            _registeredTargets.Clear();
            NotifyChanged();
        }

        public void Register(object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Unregister(target, false);

            var options = CheatsReflectionOptionScanner.Scan(target);
            _registeredTargets[target] = options;

            foreach (var option in options)
            {
                Add(option, false);
            }

            NotifyChanged();
        }

        public void Unregister(object target)
        {
            Unregister(target, true);
        }

        private void Unregister(object target, bool notify)
        {
            if (target == null)
            {
                return;
            }

            if (!_registeredTargets.Remove(target, out var options))
            {
                return;
            }

            foreach (var option in options)
            {
                _options.Remove(option);
            }

            if (notify)
            {
                NotifyChanged();
            }
        }

        public bool Remove(CheatOptionDefinition option)
        {
            if (option == null || !_options.Remove(option))
            {
                return false;
            }

            NotifyChanged();
            return true;
        }

        private CheatOptionDefinition Add(CheatOptionDefinition option, bool notify)
        {
            _options.RemoveAll(x => x.Key == option.Key);

            // Within a category options are ordered by SortOrder; equal values keep insertion order.
            var inserted = false;
            for (var i = 0; i < _options.Count; i++)
            {
                var current = _options[i];
                if (!string.Equals(current.Category, option.Category, StringComparison.Ordinal))
                {
                    continue;
                }

                if (current.SortOrder > option.SortOrder)
                {
                    _options.Insert(i, option);
                    inserted = true;
                    break;
                }
            }

            if (!inserted)
            {
                _options.Add(option);
            }

            if (notify)
            {
                NotifyChanged();
            }

            return option;
        }

        private void NotifyChanged()
        {
            Revision++;
            Changed?.Invoke();
        }
    }
}
