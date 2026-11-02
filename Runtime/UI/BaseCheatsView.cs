using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SMCheats
{
    public class BaseCheatsView : MonoBehaviour, ICheatsViewElement, IPointerClickHandler
    {
        [SerializeField]
        private RectTransform _root;

        [SerializeField]
        private Graphic _pinBackground;

        private CheatsViewElementPinState _pinState;

        public RectTransform Root => _root != null ? _root : transform as RectTransform;

        public virtual CheatsViewElementPinState PinState
        {
            get => _pinState;
            set
            {
                _pinState = value;
                UpdatePinVisualState();
            }
        }

        public event Action<ICheatsViewElement> PinRequested;

        public void OnPointerClick(PointerEventData eventData)
        {
            RequestPin();
        }

        protected void RequestPin()
        {
            if (_pinState != CheatsViewElementPinState.None)
            {
                PinRequested?.Invoke(this);
            }
        }

        private void UpdatePinVisualState()
        {
            if (_pinBackground == null)
            {
                return;
            }

            _pinBackground.gameObject.SetActive(_pinState != CheatsViewElementPinState.None);
            _pinBackground.color = CheatsSettingsProxy.UiSettings.GetPinStateColor(_pinState);
        }
    }
}
