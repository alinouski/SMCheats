using UnityEngine;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsSystemPanelView : MonoBehaviour
    {
        [SerializeField] private RectTransform _categoriesRoot;
        [SerializeField] private CheatsInfoCategoryView _categoryPrefab;

        public RectTransform CategoriesRoot => _categoriesRoot;
        public CheatsInfoCategoryView CategoryPrefab => _categoryPrefab;

        public CheatsInfoCategoryView AddCategory(string title)
        {
            var category = Instantiate(_categoryPrefab, _categoriesRoot, false);
            category.name = _categoryPrefab.name;
            category.gameObject.SetActive(true);
            category.SetTitle(title);

            return category;
        }
    }
}
