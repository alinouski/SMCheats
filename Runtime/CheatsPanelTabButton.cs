using System;
using UnityEngine;

namespace SMCheats
{
    internal sealed class CheatsPanelTabButton : ICheatsTabButton
    {
        private readonly string _panelId;
        private readonly CheatsButtonView _buttonView;

        public CheatsPanelTabButton(CheatsButtonView buttonView, string panelId, int order)
        {
            _buttonView = buttonView ?? throw new ArgumentNullException(nameof(buttonView));
            _panelId = panelId;
            Order = order;

            _buttonView.Button.onClick.RemoveAllListeners();
            _buttonView.Button.onClick.AddListener(RaiseClick);
        }

        public event Action<string> OnClick;
        public event Action<ICheatsViewElement> PinRequested
        {
            add => _buttonView.PinRequested += value;
            remove => _buttonView.PinRequested -= value;
        }

        public int Order { get; }
        public RectTransform Root => _buttonView.Root;
        public CheatsViewElementPinState PinState
        {
            get => _buttonView.PinState;
            set => _buttonView.PinState = value;
        }

        public bool IsSelected
        {
            get => _buttonView.IsSelected;
            set => _buttonView.IsSelected = value;
        }

        private void RaiseClick()
        {
            OnClick?.Invoke(_panelId);
        }
    }
}
