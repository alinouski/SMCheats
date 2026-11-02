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

        private CheatOptionDefinition Add(CheatOptionDefinition option, bool notify = true)
        {
            _options.RemoveAll(x => x.Key == option.Key);

            if (option.SortOrder == 0)
            {
                _options.Add(option);
                if (notify)
                {
                    NotifyChanged();
                }

                return option;
            }

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
