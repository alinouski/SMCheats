using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class CheatsLayoutGroupSplitter : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private LayoutGroupDirection _direction = LayoutGroupDirection.Vertical;
        [SerializeField] private LayoutElement _previousPanel;
        [SerializeField] private LayoutElement _nextPanel;
        [SerializeField] private float _minimumPreviousSize = 80f;
        [SerializeField] private float _minimumNextSize = 80f;

        private RectTransform _layoutRoot;
        private Vector2 _startPointerPosition;
        private float _startPreviousSize;
        private float _startNextSize;
        private bool _isDragging;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!TryBeginDrag(eventData))
            {
                return;
            }

            _isDragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging || _layoutRoot == null)
            {
                return;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _layoutRoot,
                    eventData.position,
                    eventData.pressEventCamera,
                    out var pointerPosition))
            {
                return;
            }

            var delta = GetAxisDelta(pointerPosition - _startPointerPosition);
            var previousSize = GetPreviousSize(delta);
            var nextSize = GetNextSize(delta);
            var clampedPreviousSize = Mathf.Clamp(
                previousSize,
                Mathf.Max(0f, _minimumPreviousSize),
                _startPreviousSize + _startNextSize - Mathf.Max(0f, _minimumNextSize));
            var clampedNextSize = _startPreviousSize + _startNextSize - clampedPreviousSize;

            SetPreferredSize(_previousPanel, clampedPreviousSize);
            SetPreferredSize(_nextPanel, clampedNextSize);
            LayoutRebuilder.MarkLayoutForRebuild(_layoutRoot);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isDragging = false;
        }

        private bool TryBeginDrag(PointerEventData eventData)
        {
            _layoutRoot = transform.parent as RectTransform;
            if (_layoutRoot == null || _previousPanel == null || _nextPanel == null)
            {
                return false;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _layoutRoot,
                    eventData.position,
                    eventData.pressEventCamera,
                    out _startPointerPosition))
            {
                return false;
            }

            _startPreviousSize = GetCurrentSize(_previousPanel);
            _startNextSize = GetCurrentSize(_nextPanel);
            SetPreferredSize(_previousPanel, _startPreviousSize);
            SetPreferredSize(_nextPanel, _startNextSize);
            return _startPreviousSize + _startNextSize > 0f;
        }

        private float GetPreviousSize(float delta)
        {
            return _direction == LayoutGroupDirection.Vertical
                ? _startPreviousSize - delta
                : _startPreviousSize + delta;
        }

        private float GetNextSize(float delta)
        {
            return _direction == LayoutGroupDirection.Vertical
                ? _startNextSize + delta
                : _startNextSize - delta;
        }

        private float GetAxisDelta(Vector2 delta)
        {
            return _direction == LayoutGroupDirection.Vertical ? delta.y : delta.x;
        }

        private float GetCurrentSize(LayoutElement panel)
        {
            var rectTransform = panel.transform as RectTransform;
            if (rectTransform == null)
            {
                return 0f;
            }

            return _direction == LayoutGroupDirection.Vertical
                ? rectTransform.rect.height
                : rectTransform.rect.width;
        }

        private void SetPreferredSize(LayoutElement panel, float size)
        {
            if (_direction == LayoutGroupDirection.Vertical)
            {
                panel.preferredHeight = size;
            }
            else
            {
                panel.preferredWidth = size;
            }
        }

        public enum LayoutGroupDirection
        {
            Vertical = 0,
            Horizontal = 1
        }
    }
}
