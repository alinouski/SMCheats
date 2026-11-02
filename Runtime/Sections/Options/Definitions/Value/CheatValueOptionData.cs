using System;
using System.Collections.Generic;

namespace SMCheats
{
    public sealed class CheatValueOptionData : CheatOptionData
    {
        public CheatValueOptionData(
            Type valueType,
            Func<object> getter,
            Action<object> setter,
            Func<IReadOnlyList<CheatDropdownChoice>> dropdownChoicesGetter)
        {
            ValueType = valueType;
            Getter = getter;
            Setter = setter;
            DropdownChoicesGetter = dropdownChoicesGetter;
        }

        public Type ValueType { get; }
        public Func<object> Getter { get; }
        public Action<object> Setter { get; }
        public Func<IReadOnlyList<CheatDropdownChoice>> DropdownChoicesGetter { get; }
        public double? MinimumValue { get; private set; }
        public double? MaximumValue { get; private set; }
        public double? Step { get; private set; }
        public bool IsReadOnly => Setter == null;
        public bool HasRange => MinimumValue.HasValue && MaximumValue.HasValue;

        internal void SetNumericRange(double? minimumValue, double? maximumValue, double? step)
        {
            MinimumValue = minimumValue;
            MaximumValue = maximumValue;
            Step = step;
        }
    }
}
