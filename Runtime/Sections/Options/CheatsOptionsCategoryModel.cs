using System.Collections.Generic;

namespace SMCheats
{
    internal sealed class CheatsOptionsCategoryModel
    {
        private readonly List<CheatOptionDefinition> _options = new();

        public CheatsOptionsCategoryModel(string category)
        {
            Category = category;
        }

        public string Category { get; }
        public IReadOnlyList<CheatOptionDefinition> Options => _options;
        public int Count => _options.Count;

        public void Clear()
        {
            _options.Clear();
        }

        public void Add(CheatOptionDefinition option)
        {
            _options.Add(option);
        }
    }
}
