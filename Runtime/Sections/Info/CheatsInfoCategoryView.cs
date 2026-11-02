using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsInfoCategoryView : MonoBehaviour
    {
        [SerializeField] private Text _titleText;
        [SerializeField] private RectTransform _entriesRoot;
        [SerializeField] private CheatsInfoEntryView _entryTemplate;

        public void SetTitle(string title)
        {
            _titleText.text = title ?? string.Empty;
        }

        public CheatsInfoEntryView AddValue(string title, string value)
        {
            var entry = Instantiate(_entryTemplate, _entriesRoot, false);
            entry.name = _entryTemplate.name;
            entry.gameObject.SetActive(true);
            entry.SetValue(title, value);

            return entry;
        }
    }
}
