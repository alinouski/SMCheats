using System.IO;
using UnityEngine;

namespace SMCheats
{
    public static class CheatsSettingsProxy
    {
        private static CheatsSettings _cachedSettings;

        public static CheatsSettings Settings
        {
            get
            {
                if (_cachedSettings != null)
                {
                    return _cachedSettings;
                }

                _cachedSettings = Resources.Load<CheatsSettings>(CheatsSettings.ResourcePath);
                if (_cachedSettings != null)
                {
                    return _cachedSettings;
                }

                throw new FileNotFoundException($"{nameof(CheatsSettings)} not found");
            }
        }

        public static CheatsUiSettings UiSettings => Settings?.Ui;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetCache()
        {
            _cachedSettings = null;
        }
    }
}
