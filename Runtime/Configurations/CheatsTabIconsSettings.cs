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

        public Sprite Info => _info;
        public Sprite Console => _console;
        public Sprite Profiler => _profiler;
        public Sprite Options => _options;

        public Sprite GetIcon(CheatsBuiltInTabs tab)
        {
            return tab switch
            {
                CheatsBuiltInTabs.Info => _info,
                CheatsBuiltInTabs.Console => _console,
                CheatsBuiltInTabs.Profiler => _profiler,
                CheatsBuiltInTabs.Options => _options,
                _ => null
            };
        }
    }
}