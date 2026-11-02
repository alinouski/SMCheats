using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsConsoleEntryView : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private Image _typeIcon;
        [SerializeField] private Text _messageText;
        [SerializeField] private Text _stackText;
        [SerializeField] private Button _selectButton;

        [SerializeField]
        private List<TypeEntryConfig> _typeConfigs = new();

        public Image Background => _background;
        public Text MessageText => _messageText;
        public Text StackText => _stackText;
        public Button SelectButton => _selectButton;

        private Action<int> _selectEntry;
        private int _entryId = -1;
        private int _entryCount = -1;
        private bool _isInitialized;

        public void Initialize(Action<int> selectEntry)
        {
            _selectEntry = selectEntry;
            if (_isInitialized)
            {
                return;
            }

            _selectButton.onClick.RemoveListener(HandleSelected);
            _selectButton.onClick.AddListener(HandleSelected);

            _isInitialized = true;
        }

        public void Bind(CheatsConsoleEntry entry, Color backgroundColor)
        {
            var shouldUpdateText = _entryId != entry.Id || _entryCount != entry.Count;
            _entryId = entry.Id;
            _entryCount = entry.Count;

            _background.color = backgroundColor;

            SetType(entry.LogType);

            if (shouldUpdateText)
            {
                _messageText.text = CheatsConsoleEntryFormatter.FormatRowMessage(entry);
            }

            if (shouldUpdateText)
            {
                _stackText.text = CheatsConsoleEntryFormatter.FormatRowStackTrace(entry);
            }
        }

        public void SetType(LogType entryLogType)
        {
            TypeEntryConfig config = null;
            foreach (var e in _typeConfigs)
            {
                if (e.LogType != entryLogType)
                {
                    continue;
                }

                config = e;
                break;
            }

            if (config == null)
            {
                return;
            }

            _typeIcon.sprite = config.Icon;
            _typeIcon.color = CheatsSettingsProxy.UiSettings.GetConsoleLogTypeColor(entryLogType);
        }

        private void HandleSelected()
        {
            _selectEntry?.Invoke(_entryId);
        }
        
        [Serializable]
        private class TypeEntryConfig
        {
            public LogType LogType;
            public Sprite Icon;
        }
    }
}
