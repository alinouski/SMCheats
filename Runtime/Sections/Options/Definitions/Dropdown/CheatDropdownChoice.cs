namespace SMCheats
{
    public readonly struct CheatDropdownChoice
    {
        public CheatDropdownChoice(string label, object value)
        {
            Label = string.IsNullOrWhiteSpace(label) ? "Unnamed" : label;
            Value = value;
        }

        public string Label { get; }
        public object Value { get; }
    }
}