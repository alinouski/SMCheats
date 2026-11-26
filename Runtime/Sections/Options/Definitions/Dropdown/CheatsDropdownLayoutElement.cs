using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Dropdown))]
    public sealed class CheatsDropdownLayoutElement : MonoBehaviour, ILayoutElement
    {
        [SerializeField]
        private Dropdown _dropdown;

        public float minWidth => preferredWidth;
        public float preferredWidth { get; private set; }
        public float flexibleWidth => -1f;
        public float minHeight => -1f;
        public float preferredHeight => -1f;
        public float flexibleHeight => -1f;
        public int layoutPriority => 2;

        public void CalculateLayoutInputHorizontal()
        {
            var caption = _dropdown.captionText;
            var settings = caption.GetGenerationSettings(Vector2.zero);
            var pixelsPerUnit = caption.pixelsPerUnit;
            var width = caption.preferredWidth;
            var options = _dropdown.options;
            for (var i = 0; i < options.Count; i++)
            {
                var optionWidth = caption.cachedTextGeneratorForLayout.GetPreferredWidth(options[i].text, settings) / pixelsPerUnit;
                width = Mathf.Max(width, optionWidth);
            }

            preferredWidth = Mathf.Max(100f, Mathf.Ceil(width) - caption.rectTransform.sizeDelta.x);
        }

        public void CalculateLayoutInputVertical()
        {
        }
    }
}
