using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public class CheatsButtonView : BaseCheatsView
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _background;
        [SerializeField] private Text _label;
        [SerializeField] private Image _icon;
        private Color _normalBackgroundColor;
        private bool _hasNormalBackgroundColor;
        private bool _isSelected;

        public Button Button => _button;
        public Image Background => _background;
        public Text Label => _label;
        public Image Icon => _icon;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                UpdateSelectionState();
            }
        }

        internal void Initialize(Button button, Image background, Text label, Image icon)
        {
            _button = button;
            _background = background;
            _label = label;
            _icon = icon;
            _hasNormalBackgroundColor = false;
            CaptureNormalBackgroundColor();
            UpdateSelectionState();
        }

        protected virtual void OnEnable()
        {
            CaptureNormalBackgroundColor();
            UpdateSelectionState();
        }

        protected virtual void Awake()
        {
            CaptureNormalBackgroundColor();
        }

        private void UpdateSelectionState()
        {
            CaptureNormalBackgroundColor();
            _background.color = _isSelected
                ? CheatsSettingsProxy.UiSettings.ButtonSelectedBackgroundColor
                : _normalBackgroundColor;
        }

        private void CaptureNormalBackgroundColor()
        {
            if (_hasNormalBackgroundColor)
            {
                return;
            }

            _normalBackgroundColor = _background.color;
            _hasNormalBackgroundColor = true;
        }
    }
}
