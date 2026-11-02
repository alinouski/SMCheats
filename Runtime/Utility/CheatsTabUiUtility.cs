using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace SMCheats
{
    public static class CheatsTabUiUtility
    {
        public const string Info = "info";
        public const string Console = "console";
        public const string Profiler = "profiler";
        public const string Options = "options";

        public static string GetTitle(string id)
        {
            return id switch
            {
                Info => "Info",
                Console => "Console",
                Profiler => "Profiler",
                Options => "Options",
                _ => string.IsNullOrWhiteSpace(id) ? "Panel" : id
            };
        }

        public static string GetOptionKey(CheatOptionDefinition option)
        {
            return option?.Key ?? string.Empty;
        }

        public static string BuildOptionKey(CheatOptionDefinition option)
        {
            var owner = option.Owner;
            var ownerKey = owner == null
                ? "global"
                : $"{owner.GetType().FullName}@{RuntimeHelpers.GetHashCode(owner)}";
            return $"{ownerKey}|{option.Category}|{option.Name}|{option.Type}";
        }

        public static string FormatValue(object value)
        {
            if (value == null)
            {
                return "<null>";
            }

            return value switch
            {
                float floatValue => floatValue.ToString("0.###", CultureInfo.InvariantCulture),
                double doubleValue => doubleValue.ToString("0.###", CultureInfo.InvariantCulture),
                decimal decimalValue => decimalValue.ToString("0.###", CultureInfo.InvariantCulture),
                bool boolValue => boolValue ? "On" : "Off",
                _ => value.ToString()
            };
        }

        public static object GetNextEnumValue(Type enumType, object currentValue)
        {
            if (enumType == null || !enumType.IsEnum)
            {
                return currentValue;
            }

            var values = Enum.GetValues(enumType);
            if (values.Length == 0)
            {
                return currentValue;
            }

            var currentIndex = currentValue == null ? -1 : Array.IndexOf(values, currentValue);
            var nextIndex = (currentIndex + 1 + values.Length) % values.Length;
            return values.GetValue(nextIndex);
        }
    }
}
