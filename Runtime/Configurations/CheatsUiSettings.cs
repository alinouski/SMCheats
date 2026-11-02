using System;
using UnityEngine;

namespace SMCheats
{
    [Serializable]
    public sealed class CheatsUiSettings
    {
        private static CheatsUiColorSettings _resourceColors;

        [SerializeField]
        private CheatsTabIconsSettings _tabIcons = new();

        [SerializeField]
        private CheatsUiColorSettings _colors;

        [SerializeField]
        private Sprite _toggleActiveIcon;

        [SerializeField]
        private Sprite _toggleInactiveIcon;
        
        [SerializeField]
        private Sprite _updateIcon;

        [SerializeField]
        private Sprite _clearIcon;

        [SerializeField]
        private Sprite _searchIcon;

        [SerializeField]
        private Sprite _openCheatsIcon;

        [SerializeField]
        private Sprite _pinnedConsoleExpandedIcon;

        [SerializeField]
        private Sprite _pinnedConsoleCollapsedIcon;

        private CheatsUiColorSettings Colors => _colors != null
            ? _colors
            : GetResourceColors();

        public CheatsTabIconsSettings TabIcons => _tabIcons ??= new CheatsTabIconsSettings();
        public Sprite ToggleActiveIcon => _toggleActiveIcon;
        public Sprite ToggleInactiveIcon => _toggleInactiveIcon;
        public Sprite UpdateIcon => _updateIcon;
        public Sprite ClearIcon => _clearIcon;
        public Sprite SearchIcon => _searchIcon;
        public Sprite OpenCheatsIcon => _openCheatsIcon;
        public Sprite PinnedConsoleExpandedIcon => _pinnedConsoleExpandedIcon;
        public Sprite PinnedConsoleCollapsedIcon => _pinnedConsoleCollapsedIcon;
        
        public Color TextColor => Colors.TextColor;
        public Color IconColor => Colors.IconColor;
        public Color ButtonSelectedBackgroundColor => Colors.ButtonSelectedBackgroundColor;
        public Color SectionButtonBackgroundColor => Colors.SectionButtonBackgroundColor;
        public Color SectionButtonEmphasizedBackgroundColor => Colors.SectionButtonEmphasizedBackgroundColor;
        public Color ToggleButtonActiveBackgroundColor => Colors.ToggleButtonActiveBackgroundColor;
        public Color ToggleButtonInactiveBackgroundColor => Colors.ToggleButtonInactiveBackgroundColor;
        public Color ToggleButtonActiveIconColor => Colors.ToggleButtonActiveIconColor;
        public Color ToggleButtonInactiveIconColor => Colors.ToggleButtonInactiveIconColor;
        public Color OptionBackgroundColor => Colors.OptionBackgroundColor;
        public Color OptionControlBackgroundColor => Colors.OptionControlBackgroundColor;
        public Color ConsoleExceptionIndicatorIdleColor => Colors.ConsoleExceptionIndicatorIdleColor;
        public Color ConsoleExceptionIndicatorFlashColor => Colors.ConsoleExceptionIndicatorFlashColor;
        public Color ReadyToPinColor => Colors.ReadyToPinColor;
        public Color PinnedColor => Colors.PinnedColor;
        public Color PinnedScreenColor => Colors.PinnedScreenColor;

        public Color GetPinStateColor(CheatsViewElementPinState pinState)
        {
            return Colors.GetPinStateColor(pinState);
        }

        public Color GetConsoleEntryBackgroundColor(LogType type, bool isSelected)
        {
            return Colors.GetConsoleEntryBackgroundColor(type, isSelected);
        }

        public Color GetConsoleLogTypeColor(LogType type)
        {
            return Colors.GetConsoleLogTypeColor(type);
        }

        private static CheatsUiColorSettings GetResourceColors()
        {
            if (_resourceColors != null)
            {
                return _resourceColors;
            }

            var colors = Resources.Load<CheatsUiColorSettings>(CheatsUiColorSettings.ResourcePath);
            _resourceColors = colors != null
                ? colors
                : CheatsUiColorSettings.Default;
            return _resourceColors;
        }
    }
}
