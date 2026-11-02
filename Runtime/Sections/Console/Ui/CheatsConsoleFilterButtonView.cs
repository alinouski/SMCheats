using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsConsoleFilterButtonView : CheatsButtonView
    {
        [SerializeField] private Text _countText;
        [SerializeField] private Image _typeIcon;
        [SerializeField] private Graphic _colorTarget;
        [SerializeField] private Sprite _logsIcon;
        [SerializeField] private Sprite _warningsIcon;
        [SerializeField] private Sprite _errorsIcon;

        public void Initialize(CheatsConsoleFilterType filterType)
        {
            var icon = GetIcon(filterType);
            var color = GetColor(filterType);
            var targetIcon = _typeIcon != null ? _typeIcon : Icon;
            targetIcon.sprite = icon;
            targetIcon.color = color;
            targetIcon.preserveAspect = true;
            targetIcon.gameObject.SetActive(icon != null);

            var colorTarget = _colorTarget != null ? _colorTarget : targetIcon;
            colorTarget.color = color;
        }

        public void SetCount(int count)
        {
            var targetText = _countText != null ? _countText : Label;
            targetText.text = count.ToString(CultureInfo.InvariantCulture);
        }

        public void SetFilterActive(bool isActive)
        {
            IsSelected = isActive;
        }

        private Sprite GetIcon(CheatsConsoleFilterType filterType)
        {
            return filterType switch
            {
                CheatsConsoleFilterType.Warnings => _warningsIcon,
                CheatsConsoleFilterType.Errors => _errorsIcon,
                _ => _logsIcon
            };
        }

        private Color GetColor(CheatsConsoleFilterType filterType)
        {
            return CheatsSettingsProxy.UiSettings.GetConsoleLogTypeColor(filterType switch
            {
                CheatsConsoleFilterType.Warnings => LogType.Warning,
                CheatsConsoleFilterType.Errors => LogType.Error,
                _ => LogType.Log
            });
        }
    }
}
