using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsGraphView : MonoBehaviour, ICheatsViewElement, IPointerClickHandler
    {
        [SerializeField] private Graphic _background;
        [SerializeField] private CheatsGraphGraphicView _graph;
        [SerializeField] private Text _titleText;
        [SerializeField] private RectTransform _currentValuesRoot;
        [SerializeField] private CheatsGraphRowView _currentValueRowTemplate;
        [SerializeField] private List<MonoBehaviour> _componentForPinnedScreen = new();

        [SerializeField]
        private float _valuesUpdateFrequency = 0.1f;

        private readonly List<CheatsGraphRowView> _instantiatedCurrentValueRows = new();
        private CheatsGraphDataSource _source;
        private CheatsViewElementPinState _pinState;
        private bool _isGraphClickSubscribed;
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
            RequestPin();
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
            var showPinBack = _pinState is CheatsViewElementPinState.Pinned or CheatsViewElementPinState.ReadyToPin;
            _background.enabled = showPinBack;
            _background.color = color;
        }
    }
}
