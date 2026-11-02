using System;

namespace SMCheats
{
    internal static class CheatsOptionControlResolver
    {
        public static CheatsOptionControlKind Resolve(CheatOptionDefinition option)
        {
            if (option == null)
            {
                return CheatsOptionControlKind.FallbackValue;
            }

            switch (option.Type)
            {
                case CheatOptionType.Action:
                    return CheatsOptionControlKind.Action;
                case CheatOptionType.Toggle:
                    return CheatsOptionControlKind.Toggle;
                case CheatOptionType.Value:
                    return Resolve(option.ValueData);
                default:
                    return CheatsOptionControlKind.FallbackValue;
            }
        }

        public static CheatsOptionControlKind Resolve(CheatValueOptionData valueData)
        {
            if (valueData == null || valueData.IsReadOnly)
            {
                return CheatsOptionControlKind.ReadOnlyValue;
            }

            if (valueData.DropdownChoicesGetter != null || valueData.ValueType.IsEnum)
            {
                return CheatsOptionControlKind.Dropdown;
            }

            if ((valueData.ValueType == typeof(int) || valueData.ValueType == typeof(float)) && valueData.HasRange)
            {
                return CheatsOptionControlKind.Slider;
            }

            if (valueData.ValueType == typeof(int) || valueData.ValueType == typeof(float))
            {
                return valueData.Step.HasValue
                    ? CheatsOptionControlKind.Selector
                    : CheatsOptionControlKind.NumericInput;
            }

            return valueData.ValueType == typeof(string)
                ? CheatsOptionControlKind.TextInput
                : CheatsOptionControlKind.FallbackValue;
        }
    }
}
