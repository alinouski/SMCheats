using UnityEngine;

namespace SMCheats
{
    [CreateAssetMenu(fileName = ResourceName, menuName = "Configurations/Cheats/UI Colors")]
    public sealed class CheatsUiColorSettings : ScriptableObject
    {
        public const string ResourceName = nameof(CheatsUiColorSettings);
        public const string ResourcePath = "SMCheats/Configs/" + ResourceName;

        private static CheatsUiColorSettings _defaultInstance;

        [SerializeField]
        private Color _textColor = Color.white;

        [SerializeField]
        private Color _iconColor = Color.white;

        [SerializeField]
        private Color _buttonSelectedBackgroundColor = new(0.24f, 0.34f, 0.2f, 0.96f);

        [SerializeField]
        private Color _sectionButtonBackgroundColor = new(0.16f, 0.18f, 0.22f, 0.92f);

        [SerializeField]
        private Color _sectionButtonEmphasizedBackgroundColor = new(0.24f, 0.34f, 0.2f, 0.96f);

        [SerializeField]
        private Color _toggleButtonActiveBackgroundColor = new(0.24f, 0.46f, 0.32f, 1f);

        [SerializeField]
        private Color _toggleButtonInactiveBackgroundColor = new(0.18f, 0.2f, 0.24f, 1f);

        [SerializeField]
        private Color _toggleButtonActiveIconColor = Color.white;

        [SerializeField]
        private Color _toggleButtonInactiveIconColor = new(1f, 1f, 1f, 0.48f);

        [SerializeField]
        private Color _optionBackgroundColor = new(0f, 0f, 0f, 0.16f);

        [SerializeField]
        private Color _optionControlBackgroundColor = new(0.18f, 0.2f, 0.24f, 1f);

        [SerializeField]
        private Color _consoleLogEntryBackgroundColor = new(0.13f, 0.17f, 0.21f, 0.92f);

        [SerializeField]
        private Color _consoleWarningEntryBackgroundColor = new(0.36f, 0.28f, 0.08f, 0.94f);

        [SerializeField]
        private Color _consoleErrorEntryBackgroundColor = new(0.42f, 0.12f, 0.12f, 0.96f);

        [SerializeField]
        private Color _consoleSelectedEntryBackgroundColor = new(0.14f, 0.34f, 0.42f, 0.98f);

        [SerializeField]
        private Color _consoleLogTypeColor = new(0.5698112f, 0.5698112f, 0.5698112f, 1f);

        [SerializeField]
        private Color _consoleWarningTypeColor = new(1f, 0.7724564f, 0.33018857f, 1f);

        [SerializeField]
        private Color _consoleErrorTypeColor = new(1f, 0.3406788f, 0.3075471f, 1f);

        [SerializeField]
        private Color _consoleExceptionIndicatorIdleColor = new(1f, 1f, 1f, 0f);

        [SerializeField]
        private Color _consoleExceptionIndicatorFlashColor = new(1f, 0.33257863f, 0.28490567f, 1f);

        [SerializeField]
        private Color _readyToPinColor = new(1f, 0.97023064f, 0.8660377f, 0.34f);

        [SerializeField]
        private Color _pinnedColor = new(0.33018857f, 1f, 0.3899931f, 0.42f);

        [SerializeField]
        private Color _pinnedScreenColor = new(0.14f, 0.28f, 0.36f, 0.42f);

        public static CheatsUiColorSettings Default
        {
            get
            {
                if (_defaultInstance == null)
                {
                    _defaultInstance = CreateInstance<CheatsUiColorSettings>();
                    _defaultInstance.hideFlags = HideFlags.HideAndDontSave;
                }

                return _defaultInstance;
            }
        }

        public Color TextColor => _textColor;
        public Color IconColor => _iconColor;
        public Color ButtonSelectedBackgroundColor => _buttonSelectedBackgroundColor;
        public Color SectionButtonBackgroundColor => _sectionButtonBackgroundColor;
        public Color SectionButtonEmphasizedBackgroundColor => _sectionButtonEmphasizedBackgroundColor;
        public Color ToggleButtonActiveBackgroundColor => _toggleButtonActiveBackgroundColor;
        public Color ToggleButtonInactiveBackgroundColor => _toggleButtonInactiveBackgroundColor;
        public Color ToggleButtonActiveIconColor => _toggleButtonActiveIconColor;
        public Color ToggleButtonInactiveIconColor => _toggleButtonInactiveIconColor;
        public Color OptionBackgroundColor => _optionBackgroundColor;
        public Color OptionControlBackgroundColor => _optionControlBackgroundColor;
        public Color ConsoleExceptionIndicatorIdleColor => _consoleExceptionIndicatorIdleColor;
        public Color ConsoleExceptionIndicatorFlashColor => _consoleExceptionIndicatorFlashColor;
        public Color ReadyToPinColor => _readyToPinColor;
        public Color PinnedColor => _pinnedColor;
        public Color PinnedScreenColor => _pinnedScreenColor;

        public Color GetPinStateColor(CheatsViewElementPinState pinState)
        {
            return pinState switch
            {
                CheatsViewElementPinState.ReadyToPin => _readyToPinColor,
                CheatsViewElementPinState.Pinned => _pinnedColor,
                CheatsViewElementPinState.PinnedScreen => _pinnedScreenColor,
                _ => Color.clear
            };
        }

        public Color GetConsoleEntryBackgroundColor(LogType type, bool isSelected)
        {
            if (isSelected)
            {
                return _consoleSelectedEntryBackgroundColor;
            }

            return type switch
            {
                LogType.Warning => _consoleWarningEntryBackgroundColor,
                LogType.Assert => _consoleErrorEntryBackgroundColor,
                LogType.Error => _consoleErrorEntryBackgroundColor,
                LogType.Exception => _consoleErrorEntryBackgroundColor,
                _ => _consoleLogEntryBackgroundColor
            };
        }

        public Color GetConsoleLogTypeColor(LogType type)
        {
            return type switch
            {
                LogType.Warning => _consoleWarningTypeColor,
                LogType.Assert => _consoleErrorTypeColor,
                LogType.Error => _consoleErrorTypeColor,
                LogType.Exception => _consoleErrorTypeColor,
                _ => _consoleLogTypeColor
            };
        }
    }
}
