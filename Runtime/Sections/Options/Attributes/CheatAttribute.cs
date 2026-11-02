using System;
using System.Runtime.CompilerServices;

namespace SMCheats
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class CheatAttribute : Attribute
    {
        public CheatAttribute(
            string category = "Default",
            int sortOrder = 0,
            [CallerMemberName] string displayName = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            DisplayName = displayName;
            Category = category;
            SortOrder = sortOrder;
            LineNumber = lineNumber;
        }

        public string DisplayName { get; }
        public string Category { get; }
        public int SortOrder { get; }
        public int LineNumber { get; }
    }
}