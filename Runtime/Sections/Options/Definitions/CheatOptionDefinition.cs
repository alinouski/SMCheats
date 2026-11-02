using System;
using System.Collections.Generic;

namespace SMCheats
{
    public sealed class CheatOptionDefinition
    {
        private CheatOptionDefinition(string name, string category, int sortOrder, CheatOptionType type)
        {
            Name = name;
            Category = string.IsNullOrWhiteSpace(category) ? "Default" : category;
            SortOrder = sortOrder;
            Type = type;
        }

        private string _key;

        public string Name { get; }
        public string Category { get; }
        public int SortOrder { get; }
        public CheatOptionType Type { get; }
        public CheatOptionData Data { get; private set; }
        public CheatActionOptionData ActionData => Data as CheatActionOptionData;
        public CheatToggleOptionData ToggleData => Data as CheatToggleOptionData;
        public CheatValueOptionData ValueData => Data as CheatValueOptionData;
        public object Owner { get; private set; }
        public Func<bool> VisibilityGetter { get; private set; }
        public bool IsVisible => VisibilityGetter == null || VisibilityGetter.Invoke();
        public string Key => _key ??= CheatsTabUiUtility.BuildOptionKey(this);

        public static CheatOptionDefinition Action(string name, Action callback, string category, int sortOrder)
        {
            return new CheatOptionDefinition(name, category, sortOrder, CheatOptionType.Action)
            {
                Data = new CheatActionOptionData(callback)
            };
        }

        public static CheatOptionDefinition Toggle(string name, Func<bool> getter, Action<bool> setter, string category, int sortOrder)
        {
            return new CheatOptionDefinition(name, category, sortOrder, CheatOptionType.Toggle)
            {
                Data = new CheatToggleOptionData(getter, setter)
            };
        }

        public static CheatOptionDefinition Value(
            string name,
            Type valueType,
            Func<object> getter,
            Action<object> setter,
            Func<IReadOnlyList<CheatDropdownChoice>> dropdownChoicesGetter,
            string category,
            int sortOrder)
        {
            return new CheatOptionDefinition(name, category, sortOrder, CheatOptionType.Value)
            {
                Data = new CheatValueOptionData(valueType, getter, setter, dropdownChoicesGetter)
            };
        }

        public CheatOptionDefinition WithVisibility(Func<bool> visibilityGetter)
        {
            VisibilityGetter = visibilityGetter;
            return this;
        }

        public CheatOptionDefinition WithNumericRange(double? minimumValue, double? maximumValue, double? step)
        {
            ValueData?.SetNumericRange(minimumValue, maximumValue, step);
            return this;
        }

        public CheatOptionDefinition WithOwner(object owner)
        {
            Owner = owner;
            _key = null;
            return this;
        }
    }
}
