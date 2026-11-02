using System;

namespace SMCheats
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CheatRangeAttribute : Attribute
    {
        public CheatRangeAttribute(double minimumValue, double maximumValue)
        {
            if (maximumValue < minimumValue)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumValue), "Maximum value must be greater than or equal to minimum value.");
            }

            MinimumValue = minimumValue;
            MaximumValue = maximumValue;
        }

        public double MinimumValue { get; }
        public double MaximumValue { get; }
    }
}