using System;

namespace SMCheats
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CheatStepAttribute : Attribute
    {
        public CheatStepAttribute(double value)
        {
            if (value <= 0d)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Step must be greater than zero.");
            }

            Value = value;
        }

        public double Value { get; }
    }
}