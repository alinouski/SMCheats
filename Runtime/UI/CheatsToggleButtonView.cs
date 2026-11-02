using UnityEngine;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsToggleButtonView : CheatsButtonView
    {
        [SerializeField] private Sprite _activeIcon;
        [SerializeField] private Sprite _inactiveIcon;
        [SerializeField] private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                UpdateToggleState();
            }
        }

        public void SetIcons(Sprite activeIcon, Sprite inactiveIcon)
        {
            _activeIcon = activeIcon;
            _inactiveIcon = inactiveIcon;
            UpdateToggleState();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateToggleState();
        }

        private void UpdateToggleState()
        {
            var uiSettings = CheatsSettingsProxy.UiSettings;
            Background.color = _isActive
                ? uiSettings.ToggleButtonActiveBackgroundColor
                : uiSettings.ToggleButtonInactiveBackgroundColor;

            var icon = _isActive ? _activeIcon : _inactiveIcon;
            Icon.sprite = icon;
            Icon.gameObject.SetActive(icon != null);
            Icon.preserveAspect = true;
            Icon.color = _isActive
                ? uiSettings.ToggleButtonActiveIconColor
                : uiSettings.ToggleButtonInactiveIconColor;
        }
    }
}
