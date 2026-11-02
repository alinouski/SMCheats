using UnityEngine;
using UnityEngine.EventSystems;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsRectTransformSideResizeHandle : MonoBehaviour
    {
        [SerializeField] private RectTransform _target;
        [SerializeField] private RectTransform _handle;
        [SerializeField] private ResizeSide _side = ResizeSide.Right;
        [SerializeField] private float _minimumSize;

        private RectTransform _dragParent;
        private Vector2 _startPointerPosition;
        private Vector2 _startSize;
        private Vector2 _startAnchoredPosition;

        private void OnEnable()
        {
            BindHandle();
        }

        private void OnDisable()
        {
            if (_handle == null)
            {
                return;
            }

            var dragProxy = _handle.GetComponent<HandleDragProxy>();
            if (dragProxy != null && dragProxy.Owner == this)
            {
                dragProxy.Owner = null;
            }
        }

        private void BindHandle()
        {
            if (_handle == null)
            {
                return;
            }

            var dragProxy = _handle.GetComponent<HandleDragProxy>();
            if (dragProxy == null)
            {
                dragProxy = _handle.gameObject.AddComponent<HandleDragProxy>();
            }

            dragProxy.Owner = this;
        }

        private void BeginDrag(PointerEventData eventData)
        {
            if (_target == null)
            {
                return;
            }

            _dragParent = _target.parent as RectTransform;
            if (_dragParent == null)
            {
                return;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _dragParent,
                    eventData.position,
                    eventData.pressEventCamera,
                    out _startPointerPosition))
            {
                _dragParent = null;
                return;
            }

            _startSize = _target.rect.size;
            _startAnchoredPosition = _target.anchoredPosition;
        }

        private void Drag(PointerEventData eventData)
        {
            if (_target == null || _dragParent == null)
            {
                return;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _dragParent,
                    eventData.position,
                    eventData.pressEventCamera,
                    out var pointerPosition))
            {
                return;
            }

            var delta = pointerPosition - _startPointerPosition;
            var size = _startSize;
            var anchoredPosition = _startAnchoredPosition;

            switch (_side)
            {
                case ResizeSide.Left:
                    size.x = ClampSize(_startSize.x - delta.x);
                    anchoredPosition.x = _startAnchoredPosition.x - (size.x - _startSize.x) * (1f - _target.pivot.x);
                    break;
                case ResizeSide.Right:
                    size.x = ClampSize(_startSize.x + delta.x);
                    anchoredPosition.x = _startAnchoredPosition.x + (size.x - _startSize.x) * _target.pivot.x;
                    break;
                case ResizeSide.Bottom:
                    size.y = ClampSize(_startSize.y - delta.y);
                    anchoredPosition.y = _startAnchoredPosition.y - (size.y - _startSize.y) * (1f - _target.pivot.y);
                    break;
                case ResizeSide.Top:
                    size.y = ClampSize(_startSize.y + delta.y);
                    anchoredPosition.y = _startAnchoredPosition.y + (size.y - _startSize.y) * _target.pivot.y;
                    break;
            }

            _target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            _target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            _target.anchoredPosition = anchoredPosition;
        }

        private void EndDrag()
        {
            _dragParent = null;
        }

        private float ClampSize(float size)
        {
            return Mathf.Max(Mathf.Max(0f, _minimumSize), size);
        }

        public enum ResizeSide
        {
            Left = 0,
            Right = 1,
            Bottom = 2,
            Top = 3
        }

        private sealed class HandleDragProxy : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
        {
            public CheatsRectTransformSideResizeHandle Owner { get; set; }

            public void OnBeginDrag(PointerEventData eventData)
            {
                Owner?.BeginDrag(eventData);
            }

            public void OnDrag(PointerEventData eventData)
            {
                Owner?.Drag(eventData);
            }

            public void OnEndDrag(PointerEventData eventData)
            {
                Owner?.EndDrag();
            }
        }
    }
}
