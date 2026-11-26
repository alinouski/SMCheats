using System;
using UnityEngine;

namespace SMCheats
{
    [Serializable]
    public sealed class CheatsTabIconsSettings
    {
        [SerializeField]
        private Sprite _info;

        [SerializeField]
        private Sprite _console;

        [SerializeField]
        private Sprite _profiler;

        [SerializeField]
        private Sprite _options;

        public Sprite Info => _info ? _info : CheatsUiSettings.GetDefaultIcon("alert_circle_icon_128964");
        public Sprite Console => _console ? _console : CheatsUiSettings.GetDefaultIcon("terminal_icon_128733");
        public Sprite Profiler => _profiler ? _profiler : CheatsUiSettings.GetDefaultIcon("activity_icon_128966");
        public Sprite Options => _options ? _options : CheatsUiSettings.GetDefaultIcon("inbox_icon_128837");

        public Sprite GetIcon(CheatsBuiltInTabs tab)
        {
            return tab switch
            {
                CheatsBuiltInTabs.Info => Info,
                CheatsBuiltInTabs.Console => Console,
                CheatsBuiltInTabs.Profiler => Profiler,
                CheatsBuiltInTabs.Options => Options,
                _ => null
            };
        }
    }
}
