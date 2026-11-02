using System;

namespace SMCheats
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class CheatVisibleIfAttribute : Attribute
    {
        public CheatVisibleIfAttribute(string sourceMemberName, bool expectedValue = true)
        {
            if (string.IsNullOrWhiteSpace(sourceMemberName))
            {
                throw new ArgumentException("Visibility source member name must not be empty.", nameof(sourceMemberName));
            }

            SourceMemberName = sourceMemberName;
            ExpectedValue = expectedValue;
        }

        public string SourceMemberName { get; }
        public bool ExpectedValue { get; }
    }
}