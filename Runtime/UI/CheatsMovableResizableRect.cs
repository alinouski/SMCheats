using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SMCheats
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class CheatsMovableResizableRect : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private const float EdgeHitSize = 24f;

        [SerializeField]
        private Vector2 _minimumSize = new(120f, 80f);

        private readonly Vector3[] _worldCorners = new Vector3[4];
        private RectTransform _rectTransform;
        private DragMode _dragMode;
        private ResizeEdges _resizeEdges;
        private RectBounds _startBounds;
        private Vector2 _pointerOffset;

        private RectTransform RectTransform => _rectTransform != null
            ? _rectTransform
            : _rectTransform = transform as RectTransform;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!TryGetParentLocalPoint(eventData, out var pointerPosition, out var parent))
            {
                return;
            }

            _startBounds = GetBoundsInParent(parent);
            _resizeEdges = GetResizeEdges(pointerPosition, _startBounds);
            _dragMode = _resizeEdges == ResizeEdges.None ? DragMode.Move : DragMode.Resize;
            _pointerOffset = pointerPosition - _startBounds.Center;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_dragMode == DragMode.None || !TryGetParentLocalPoint(eventData, out var pointerPosition, out var parent))
            {
                return;
            }

            var parentRect = parent.rect;
            var bounds = _dragMode == DragMode.Resize
                ? ResizeBounds(_startBounds, pointerPosition, parentRect)
                : MoveBounds(_startBounds, pointerPosition - _pointerOffset, parentRect);

            ApplyBounds(parent, bounds);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _dragMode = DragMode.None;
            _resizeEdges = ResizeEdges.None;
        }

        private bool TryGetParentLocalPoint(PointerEventData eventData, out Vector2 localPoint, out RectTransform parent)
        {
            localPoint = default;
            parent = RectTransform?.parent as RectTransform;
            return parent != null
                   && RectTransformUtility.ScreenPointToLocalPointInRectangle(
                       parent,
                       eventData.position,
                       eventData.pressEventCamera,
                       out localPoint);
        }

        private RectBounds GetBoundsInParent(RectTransform parent)
        {
            RectTransform.GetWorldCorners(_worldCorners);

            var min = (Vector2)parent.InverseTransformPoint(_worldCorners[0]);
            var max = min;
            for (var i = 1; i < _worldCorners.Length; i++)
            {
                var point = (Vector2)parent.InverseTransformPoint(_worldCorners[i]);
                min = Vector2.Min(min, point);
                max = Vector2.Max(max, point);
            }

            return new RectBounds(min, max);
        }

        private ResizeEdges GetResizeEdges(Vector2 pointerPosition, RectBounds bounds)
        {
            var edges = ResizeEdges.None;

            var leftDistance = Mathf.Abs(pointerPosition.x - bounds.Min.x);
            var rightDistance = Mathf.Abs(pointerPosition.x - bounds.Max.x);
            if (leftDistance <= EdgeHitSize || rightDistance <= EdgeHitSize)
            {
                edges |= leftDistance <= rightDistance ? ResizeEdges.Left : ResizeEdges.Right;
            }

            var bottomDistance = Mathf.Abs(pointerPosition.y - bounds.Min.y);
            var topDistance = Mathf.Abs(pointerPosition.y - bounds.Max.y);
            if (bottomDistance <= EdgeHitSize || topDistance <= EdgeHitSize)
            {
                edges |= bottomDistance <= topDistance ? ResizeEdges.Bottom : ResizeEdges.Top;
            }

            return edges;
        }

        private RectBounds MoveBounds(RectBounds startBounds, Vector2 center, Rect parentRect)
        {
            var size = Vector2.Min(startBounds.Size, parentRect.size);
            var halfSize = size * 0.5f;

            center.x = parentRect.width <= size.x
                ? parentRect.center.x
                : Mathf.Clamp(center.x, parentRect.xMin + halfSize.x, parentRect.xMax - halfSize.x);
            center.y = parentRect.height <= size.y
                ? parentRect.center.y
                : Mathf.Clamp(center.y, parentRect.yMin + halfSize.y, parentRect.yMax - halfSize.y);

            return RectBounds.FromCenter(center, size);
        }

        private RectBounds ResizeBounds(RectBounds startBounds, Vector2 pointerPosition, Rect parentRect)
        {
            var min = startBounds.Min;
            var max = startBounds.Max;
            var minimumSize = GetMinimumSize(parentRect);

            if (_resizeEdges.HasFlag(ResizeEdges.Left))
            {
                min.x = Mathf.Clamp(pointerPosition.x, parentRect.xMin, max.x - minimumSize.x);
            }
            else if (_resizeEdges.HasFlag(ResizeEdges.Right))
            {
                max.x = Mathf.Clamp(pointerPosition.x, min.x + minimumSize.x, parentRect.xMax);
            }

            if (_resizeEdges.HasFlag(ResizeEdges.Bottom))
            {
                min.y = Mathf.Clamp(pointerPosition.y, parentRect.yMin, max.y - minimumSize.y);
            }
            else if (_resizeEdges.HasFlag(ResizeEdges.Top))
            {
                max.y = Mathf.Clamp(pointerPosition.y, min.y + minimumSize.y, parentRect.yMax);
            }

            return new RectBounds(min, max);
        }

        private Vector2 GetMinimumSize(Rect parentRect)
        {
            return new Vector2(
                Mathf.Min(Mathf.Max(0f, _minimumSize.x), parentRect.width),
                Mathf.Min(Mathf.Max(0f, _minimumSize.y), parentRect.height));
        }

        private void ApplyBounds(RectTransform parent, RectBounds bounds)
        {
            var rectTransform = RectTransform;
            var size = bounds.Size;
            var pivotPosition = bounds.Min + Vector2.Scale(size, rectTransform.pivot);

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            rectTransform.position = parent.TransformPoint(pivotPosition);
        }

        private enum DragMode
        {
            None,
            Move,
            Resize
        }

        [Flags]
        private enum ResizeEdges
        {
            None = 0,
            Left = 1,
            Right = 2,
            Bottom = 4,
            Top = 8
        }

        private readonly struct RectBounds
        {
            public RectBounds(Vector2 min, Vector2 max)
            {
                Min = min;
                Max = max;
            }

            public Vector2 Min { get; }
            public Vector2 Max { get; }
            public Vector2 Size => Max - Min;
            public Vector2 Center => (Min + Max) * 0.5f;

            public static RectBounds FromCenter(Vector2 center, Vector2 size)
            {
                var halfSize = size * 0.5f;
                return new RectBounds(center - halfSize, center + halfSize);
            }
        }
    }
}
