using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsGraphView : MonoBehaviour, ICheatsViewElement, IPointerClickHandler, ILayoutController
    {
        private const float StatisticsRowHeight = 40f;
        private const float CompactHeaderHeight = 40f;
        private const float MinimumGraphHeight = 56f;

        [SerializeField] private Graphic _background;
        [SerializeField] private CheatsGraphGraphicView _graph;
        [SerializeField] private Text _titleText;
        [SerializeField] private RectTransform _currentValuesRoot;
        [SerializeField] private CheatsGraphRowView _currentValueRowTemplate;
        [SerializeField]
        private RectTransform _headerRect;
        [SerializeField]
        private GridLayoutGroup _currentValuesLayout;
        [SerializeField]
        private Text _averageText;
        [SerializeField]
        private int _averageValueIndex = 2;
        [SerializeField] private List<MonoBehaviour> _componentForPinnedScreen = new();

        [SerializeField]
        private float _valuesUpdateFrequency = 0.1f;

        private readonly List<CheatsGraphRowView> _instantiatedCurrentValueRows = new();
        private CheatsGraphDataSource _source;
        private CheatsViewElementPinState _pinState;
        private bool _isGraphClickSubscribed;
        private bool _showStatistics = true;
        private float _lastUpdate;

        public RectTransform Root => transform as RectTransform;

        public CheatsViewElementPinState PinState
        {
            get => _pinState;
            set
            {
                _pinState = value;
                UpdateBackground();
            }
        }

        public event Action<ICheatsViewElement> PinRequested;

        public void SetSource(CheatsGraphDataSource source)
        {
            _source = source;
            Graph.SetSource(source);
            UpdateBackground();
            RefreshTitle();
            RebuildCurrentValueRows();
            RefreshCurrentValues();
            SetStatisticsVisible(_showStatistics);
            LayoutRebuilder.MarkLayoutForRebuild(Root);
        }

        public void SetLayoutHorizontal()
        {
            var width = Root.rect.width;
            var valuesWidth = Mathf.Max(0f, width + _currentValuesRoot.sizeDelta.x);
            var headerHeight = GetStatisticsHeight(width, out var columns);
            if (_currentValuesRoot.gameObject.activeSelf)
            {
                _currentValuesLayout.constraintCount = columns;
                _currentValuesLayout.cellSize = new Vector2(valuesWidth / columns, StatisticsRowHeight);
            }
            else
            {
                headerHeight = CompactHeaderHeight;
            }

            _headerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, headerHeight);
            Graph.rectTransform.offsetMin = new Vector2(0f, headerHeight);
        }

        public void SetLayoutVertical()
        {
            SetLayoutHorizontal();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (!CanvasUpdateRegistry.IsRebuildingLayout())
            {
                LayoutRebuilder.MarkLayoutForRebuild(Root);
            }
        }

        private void LateUpdate()
        {
            var rect = Root.rect;
            var headerHeight = GetStatisticsHeight(rect.width, out _);
            var showStatistics =
                _showStatistics &&
                rect.width + _currentValuesRoot.sizeDelta.x >= 100f &&
                rect.height >= headerHeight + MinimumGraphHeight;
            if (_currentValuesRoot.gameObject.activeSelf == showStatistics)
            {
                return;
            }

            SetStatisticsVisible(showStatistics);
            LayoutRebuilder.MarkLayoutForRebuild(Root);
        }

        private float GetStatisticsHeight(float width, out int columns)
        {
            var valuesWidth = Mathf.Max(0f, width + _currentValuesRoot.sizeDelta.x);
            columns = Mathf.Max(1, Mathf.FloorToInt(valuesWidth / 100f));
            var valueCount = _instantiatedCurrentValueRows.Count;
            var rows = Mathf.Max(1, Mathf.CeilToInt(valueCount / (float)columns));
            columns = Mathf.Max(1, Mathf.CeilToInt(valueCount / (float)rows));
            return rows * StatisticsRowHeight + 16f;
        }

        public void Refresh()
        {
            if (_lastUpdate + _valuesUpdateFrequency <= Time.unscaledTime)
            {
                RefreshCurrentValues();
                _lastUpdate = Time.unscaledTime;
            }

            Graph.Refresh();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData is { dragging: true })
            {
                return;
            }

            var clickedStatistics =
                eventData is { } &&
                RectTransformUtility.RectangleContainsScreenPoint(_headerRect, eventData.position, eventData.pressEventCamera);
            if (clickedStatistics)
            {
                ToggleStatistics();
                return;
            }

            RequestPin();
        }

        private void ToggleStatistics()
        {
            var showStatistics = !_currentValuesRoot.gameObject.activeSelf;
            if (showStatistics && _pinState == CheatsViewElementPinState.PinnedScreen)
            {
                ResizeToFitStatistics();
            }

            _showStatistics = showStatistics;
            SetStatisticsVisible(showStatistics);
            LayoutRebuilder.MarkLayoutForRebuild(Root);
        }

        private void SetStatisticsVisible(bool visible)
        {
            _currentValuesRoot.gameObject.SetActive(visible);
            _averageText.gameObject.SetActive(!visible);
        }

        private void ResizeToFitStatistics()
        {
            var root = Root;
            var parent = (RectTransform)root.parent;
            var parentRect = parent.rect;
            var width = Mathf.Min(Mathf.Max(root.rect.width, 280f), parentRect.width);
            root.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            var height = GetStatisticsHeight(width, out _) + MinimumGraphHeight;
            height = Mathf.Min(Mathf.Max(root.rect.height, height), parentRect.height);
            root.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            var position = root.anchoredPosition;
            position.x = Mathf.Clamp(position.x, 0f, parentRect.width - width);
            position.y = Mathf.Clamp(position.y, height - parentRect.height, 0f);
            root.anchoredPosition = position;
        }

        private CheatsGraphGraphicView Graph
        {
            get
            {
                if (_graph == null)
                {
                    _graph = GetComponentInChildren<CheatsGraphGraphicView>(true);
                }

                return _graph;
            }
        }

        private void OnEnable()
        {
            SubscribeGraphClick();
            UpdateBackground();
        }

        private void OnDisable()
        {
            UnsubscribeGraphClick();
        }

        private void OnDestroy()
        {
            ClearCurrentValueRows();
        }

        private void SubscribeGraphClick()
        {
            var graph = Graph;
            if (_isGraphClickSubscribed)
            {
                return;
            }

            graph.Clicked += RequestPin;
            _isGraphClickSubscribed = true;
        }

        private void UnsubscribeGraphClick()
        {
            if (_graph == null || !_isGraphClickSubscribed)
            {
                return;
            }

            _graph.Clicked -= RequestPin;
            _isGraphClickSubscribed = false;
        }

        private void RequestPin()
        {
            if (_pinState != CheatsViewElementPinState.None)
            {
                PinRequested?.Invoke(this);
            }
        }

        private void RefreshTitle()
        {
            _titleText.text = _source?.Title ?? string.Empty;
        }

        private void RebuildCurrentValueRows()
        {
            ClearCurrentValueRows();
            if (_source == null)
            {
                return;
            }

            _currentValueRowTemplate.SetActive(false);
            for (var i = 0; i < _source.ValuesAmount; i++)
            {
                var row = Instantiate(_currentValueRowTemplate, _currentValuesRoot, false);
                row.name = _currentValueRowTemplate.name;
                row.SetActive(true);
                _instantiatedCurrentValueRows.Add(row);
                var valueSource = _source.CurrentValueSource(i);
                row.SetName(valueSource.Name);
                row.SetSampleColor(valueSource.Color);
            }
        }

        private void ClearCurrentValueRows()
        {
            for (var i = _instantiatedCurrentValueRows.Count - 1; i >= 0; i--)
            {
                if (_instantiatedCurrentValueRows[i] != null)
                {
                    Destroy(_instantiatedCurrentValueRows[i]);
                }
            }

            _instantiatedCurrentValueRows.Clear();
        }

        private void RefreshCurrentValues()
        {
            if (_source == null)
            {
                return;
            }

            for (var i = 0; i < _source.ValuesAmount; i++)
            {
                _instantiatedCurrentValueRows[i].SetValue(_source.CurrentValueSource(i).GetValueString());
            }

            _averageText.text = (uint)_averageValueIndex < (uint)_source.ValuesAmount
                ? _source.CurrentValueSource(_averageValueIndex).GetValueString()
                : _source.AverageValue.ToString("0.0");
        }

        private Color GetBackgroundColor()
        {
            var pinColor = _pinState switch
            {
                CheatsViewElementPinState.ReadyToPin => (Color?)CheatsSettingsProxy.Settings.Ui.ReadyToPinColor,
                CheatsViewElementPinState.Pinned => CheatsSettingsProxy.Settings.Ui.PinnedColor,
                CheatsViewElementPinState.PinnedScreen => CheatsSettingsProxy.Settings.Ui.PinnedScreenColor,
                _ => null
            };

            return pinColor ?? _source?.DisplayConfig.BackgroundColor ?? Color.clear;
        }

        private void UpdateBackground()
        {
            var isPinnedScreen = _pinState == CheatsViewElementPinState.PinnedScreen;

            foreach (var component in _componentForPinnedScreen)
            {
                component.enabled = isPinnedScreen;
            }

            var color = GetBackgroundColor();
            _background.enabled = !isPinnedScreen;
            _background.color = color;
        }
    }
}
