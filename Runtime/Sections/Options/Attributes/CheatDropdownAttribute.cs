using System;

namespace SMCheats
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CheatDropdownAttribute : Attribute
    {
        public CheatDropdownAttribute(string sourceMemberName)
        {
            if (string.IsNullOrWhiteSpace(sourceMemberName))
            {
                throw new ArgumentException("Dropdown source member name must not be empty.", nameof(sourceMemberName));
            }

            SourceMemberName = sourceMemberName;
        }

        public string SourceMemberName { get; }
    }
}
