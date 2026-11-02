using System;
using System.Collections.Generic;

namespace SMCheats
{
    internal sealed class CheatsOptionsContentModel
    {
        private readonly Dictionary<string, CheatsOptionsCategoryModel> _categoriesByKey = new(StringComparer.Ordinal);
        private readonly List<CheatsOptionsCategoryModel> _categories = new();
        private readonly List<CheatOptionDefinition> _pinnedOptions = new();

        public IReadOnlyList<CheatsOptionsCategoryModel> Categories => _categories;
        public IReadOnlyList<CheatOptionDefinition> PinnedOptions => _pinnedOptions;

        public void BeginBuild()
        {
            for (var i = 0; i < _categories.Count; i++)
            {
                _categories[i].Clear();
            }

            _categories.Clear();
            _pinnedOptions.Clear();
        }

        public void AddVisibleOption(CheatOptionDefinition option, bool isPinned)
        {
            if (option == null)
            {
                return;
            }

            if (!_categoriesByKey.TryGetValue(option.Category, out var category))
            {
                category = new CheatsOptionsCategoryModel(option.Category);
                _categoriesByKey.Add(option.Category, category);
            }

            if (category.Count == 0)
            {
                _categories.Add(category);
            }

            category.Add(option);
            if (isPinned)
            {
                _pinnedOptions.Add(option);
            }
        }

        public bool ContainsCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return false;
            }

            for (var i = 0; i < _categories.Count; i++)
            {
                if (string.Equals(_categories[i].Category, category, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
