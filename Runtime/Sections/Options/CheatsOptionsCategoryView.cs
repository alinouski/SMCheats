using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsOptionsCategoryView : MonoBehaviour, ILayoutElement, ILayoutController
    {
        [SerializeField] private CheatsButtonView _headerButton;
        [SerializeField] private RectTransform _contentRoot;
        [SerializeField] private float _spacing = 8f;

        private RectTransform _rectTransform;
        private float _preferredWidth;
        private float _preferredHeight;

        public CheatsButtonView HeaderButton => _headerButton;
        public RectTransform ContentRoot => _contentRoot;
        public float minWidth => _preferredWidth;
        public float preferredWidth => _preferredWidth;
        public float flexibleWidth => 0f;
        public float minHeight => _preferredHeight;
        public float preferredHeight => _preferredHeight;
        public float flexibleHeight => 0f;
        public int layoutPriority => 1;

        private RectTransform RectTransform => _rectTransform != null
            ? _rectTransform
            : _rectTransform = transform as RectTransform;

        public void Initialize(string title, bool isSelected, bool isContentVisible)
        {
            _headerButton.Label.text = title ?? string.Empty;
            _headerButton.IsSelected = isSelected;

            _contentRoot.gameObject.SetActive(isContentVisible);

            SetDirty();
        }

        public void CalculateLayoutInputHorizontal()
        {
            _preferredWidth = Mathf.Max(
                GetElementPreferredSize(GetHeaderRoot(), 0),
                GetContentRootPreferredWidth());
        }

        public void CalculateLayoutInputVertical()
        {
            var headerHeight = GetElementPreferredSize(GetHeaderRoot(), 1);
            var contentHeight = GetElementPreferredSize(_contentRoot, 1);
            _preferredHeight = headerHeight + contentHeight;
            if (headerHeight > 0f && contentHeight > 0f)
            {
                _preferredHeight += Mathf.Max(0f, _spacing);
            }
        }

        public void SetLayoutHorizontal()
        {
            SetChildWidth(_contentRoot);
        }

        public void SetLayoutVertical()
        {
            var y = GetElementPreferredSize(GetHeaderRoot(), 1);
            var contentHeight = GetElementPreferredSize(_contentRoot, 1);
            if (y > 0f && contentHeight > 0f)
            {
                y += Mathf.Max(0f, _spacing);
            }

            SetChildHeight(_contentRoot, y);
        }

        private RectTransform GetHeaderRoot()
        {
            return _headerButton.Root;
        }

        private float GetContentRootPreferredWidth()
        {
            if (!_contentRoot.gameObject.activeSelf)
            {
                return 0f;
            }

            return _contentRoot.GetComponent<CheatsWrapLayoutGroup>() != null
                ? LayoutUtility.GetMinWidth(_contentRoot)
                : GetElementPreferredSize(_contentRoot, 0);
        }

        private static float GetElementPreferredSize(RectTransform element, int axis)
        {
            if (!element.gameObject.activeSelf)
            {
                return 0f;
            }

            var preferredSize = LayoutUtility.GetPreferredSize(element, axis);
            if (preferredSize > 0f)
            {
                return preferredSize;
            }

            var minSize = LayoutUtility.GetMinSize(element, axis);
            if (minSize > 0f)
            {
                return minSize;
            }

            return axis == 0 ? element.rect.width : element.rect.height;
        }

        private void SetChildWidth(RectTransform child)
        {
            child.anchorMin = new Vector2(0f, 1f);
            child.anchorMax = new Vector2(1f, 1f);
            child.pivot = new Vector2(0.5f, 1f);
            child.anchoredPosition = new Vector2(0f, child.anchoredPosition.y);
            child.sizeDelta = new Vector2(0f, child.sizeDelta.y);
        }

        private float SetChildHeight(RectTransform child, float y)
        {
            if (!child.gameObject.activeSelf)
            {
                return 0f;
            }

            var height = GetElementPreferredSize(child, 1);
            child.anchorMin = new Vector2(0f, 1f);
            child.anchorMax = new Vector2(1f, 1f);
            child.pivot = new Vector2(0.5f, 1f);
            child.anchoredPosition = new Vector2(0f, -y);
            child.sizeDelta = new Vector2(0f, height);
            return height;
        }

        private void SetDirty()
        {
            LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
        }
    }
}
