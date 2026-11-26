using System;
using System.Collections.Generic;
using UnityEngine;

namespace SMCheats
{
    [Serializable]
    public sealed class CheatsUiSettings
    {
        private static CheatsUiColorSettings _resourceColors;
        private static readonly Dictionary<string, Sprite> DefaultIcons = new();

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
        public Sprite ToggleActiveIcon => _toggleActiveIcon ? _toggleActiveIcon : GetDefaultIcon("toggle_right_icon_128728");
        public Sprite ToggleInactiveIcon => _toggleInactiveIcon ? _toggleInactiveIcon : GetDefaultIcon("toggle_left_icon_128729");
        public Sprite UpdateIcon => _updateIcon ? _updateIcon : GetDefaultIcon("rotate_cw_icon_128766");
        public Sprite ClearIcon => _clearIcon ? _clearIcon : GetDefaultIcon("trash_icon_128726");
        public Sprite SearchIcon => _searchIcon ? _searchIcon : GetDefaultIcon("search_magnifier_icon_145939");
        public Sprite OpenCheatsIcon => _openCheatsIcon ? _openCheatsIcon : GetDefaultIcon("terminal_icon_128733");
        public Sprite PinnedConsoleExpandedIcon => _pinnedConsoleExpandedIcon
            ? _pinnedConsoleExpandedIcon
            : GetDefaultIcon("chevron_up_icon_128917");
        public Sprite PinnedConsoleCollapsedIcon => _pinnedConsoleCollapsedIcon
            ? _pinnedConsoleCollapsedIcon
            : GetDefaultIcon("chevron_down_icon_128920");
        
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

        internal static Sprite GetDefaultIcon(string name)
        {
            if (!DefaultIcons.TryGetValue(name, out var icon) || !icon)
            {
                icon = Resources.Load<Sprite>("SMCheats/Icons/" + name);
                DefaultIcons[name] = icon;
            }

            return icon;
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
