using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class CheatsWrapLayoutGroup : LayoutGroup
    {
        [SerializeField] private float _horizontalSpacing = 8f;
        [SerializeField] private float _verticalSpacing = 8f;

        private readonly List<Row> _rows = new();

        public float HorizontalSpacing
        {
            get => _horizontalSpacing;
            set => SetProperty(ref _horizontalSpacing, Mathf.Max(0f, value));
        }

        public float VerticalSpacing
        {
            get => _verticalSpacing;
            set => SetProperty(ref _verticalSpacing, Mathf.Max(0f, value));
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            var minWidth = (float)padding.horizontal;
            var preferredWidth = (float)padding.horizontal;
            for (var i = 0; i < rectChildren.Count; i++)
            {
                var childWidth = GetChildPreferredSize(rectChildren[i], 0);
                minWidth = Mathf.Max(minWidth, padding.horizontal + childWidth);
                preferredWidth += childWidth;
                if (i > 0)
                {
                    preferredWidth += _horizontalSpacing;
                }
            }

            SetLayoutInputForAxis(minWidth, Mathf.Max(minWidth, preferredWidth), -1f, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            BuildRows(GetAvailableWidth());
            SetLayoutInputForAxis(GetRowsHeight(), GetRowsHeight(), -1f, 1);
        }

        public override void SetLayoutHorizontal()
        {
            SetChildren();
        }

        public override void SetLayoutVertical()
        {
            SetChildren();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetDirty();
        }

        protected override void OnTransformChildrenChanged()
        {
            base.OnTransformChildrenChanged();
            SetDirty();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            _horizontalSpacing = Mathf.Max(0f, _horizontalSpacing);
            _verticalSpacing = Mathf.Max(0f, _verticalSpacing);
            base.OnValidate();
        }
#endif

        private void SetChildren()
        {
            BuildRows(GetAvailableWidth());

            var y = GetStartOffset(1, GetRowsHeight() - padding.vertical);
            for (var rowIndex = 0; rowIndex < _rows.Count; rowIndex++)
            {
                var row = _rows[rowIndex];
                var x = GetStartOffset(0, row.Width);
                for (var childIndex = row.StartIndex; childIndex <= row.EndIndex; childIndex++)
                {
                    var child = rectChildren[childIndex];
                    var childWidth = GetChildPreferredSize(child, 0);
                    var childHeight = GetChildPreferredSize(child, 1);
                    SetChildAlongAxis(child, 0, x, childWidth);
                    SetChildAlongAxis(child, 1, y, childHeight);
                    x += childWidth + _horizontalSpacing;
                }

                y += row.Height + _verticalSpacing;
            }
        }

        private void BuildRows(float availableWidth)
        {
            _rows.Clear();

            var rowStartIndex = 0;
            var rowWidth = 0f;
            var rowHeight = 0f;
            for (var i = 0; i < rectChildren.Count; i++)
            {
                var child = rectChildren[i];
                var childWidth = GetChildPreferredSize(child, 0);
                var childHeight = GetChildPreferredSize(child, 1);
                var additionalWidth = rowWidth <= 0f ? childWidth : _horizontalSpacing + childWidth;

                if (rowWidth > 0f && rowWidth + additionalWidth > availableWidth)
                {
                    AddRow(rowStartIndex, i - 1, rowWidth, rowHeight);
                    rowStartIndex = i;
                    rowWidth = childWidth;
                    rowHeight = childHeight;
                    continue;
                }

                rowWidth += additionalWidth;
                rowHeight = Mathf.Max(rowHeight, childHeight);
            }

            if (rectChildren.Count > 0)
            {
                AddRow(rowStartIndex, rectChildren.Count - 1, rowWidth, rowHeight);
            }
        }

        private void AddRow(int startIndex, int endIndex, float width, float height)
        {
            _rows.Add(new Row(startIndex, endIndex, width, height));
        }

        private float GetRowsHeight()
        {
            var height = (float)padding.vertical;
            for (var i = 0; i < _rows.Count; i++)
            {
                height += _rows[i].Height;
                if (i > 0)
                {
                    height += _verticalSpacing;
                }
            }

            return height;
        }

        private float GetAvailableWidth()
        {
            return Mathf.Max(0f, rectTransform.rect.width - padding.horizontal);
        }

        private static float GetChildPreferredSize(RectTransform child, int axis)
        {
            var preferredSize = LayoutUtility.GetPreferredSize(child, axis);
            if (preferredSize > 0f)
            {
                return preferredSize;
            }

            var minSize = LayoutUtility.GetMinSize(child, axis);
            if (minSize > 0f)
            {
                return minSize;
            }

            return axis == 0 ? child.rect.width : child.rect.height;
        }

        private readonly struct Row
        {
            public Row(int startIndex, int endIndex, float width, float height)
            {
                StartIndex = startIndex;
                EndIndex = endIndex;
                Width = width;
                Height = height;
            }

            public int StartIndex { get; }
            public int EndIndex { get; }
            public float Width { get; }
            public float Height { get; }
        }
    }
}
