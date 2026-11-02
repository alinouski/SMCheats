using UnityEngine;

namespace SMCheats
{
    [CreateAssetMenu(fileName = ResourceName, menuName = "Configurations/Cheats/Settings")]
    public sealed class CheatsSettings : ScriptableObject
    {
        public const string ResourceName = nameof(CheatsSettings);
        public const string ResourcePath = "SMCheats/" + ResourceName;

        [SerializeField]
        [Tooltip("Allow cheats in non-development builds. Define SMCHEATS_DISABLED to turn them off everywhere.")]
        private bool _enableInReleaseBuilds;

        [SerializeField]
        private string _defaultTabId = CheatsTabUiUtility.Console;

        [SerializeField]
        private CheatsBuiltInTabs _enabledTabs = CheatsBuiltInTabs.All;

        [SerializeField]
        private CheatsActivationTriggerSettings _activationTriggerSettings = new();
        
        [SerializeField]
        private CheatsPinCodeSettings _pinCodeSettings = new();
        
        [SerializeField]
        private Vector2Int _fpsThreshold = new(50, 30);
        
        [SerializeField]
        private CheatsGraphVisualConfig _profilerFpsGraphVisualConfig;
        
        [SerializeField]
        private CheatsGraphVisualConfig _profilerCpuGraphVisualConfig;

        [SerializeField]
        private CheatsGraphVisualConfig _profilerMemoryGraphVisualConfig;

        [SerializeField]
        private CheatsUiSettings _ui = new();

        [SerializeField]
        [Min(50)]
        private int _maximumConsoleEntries = 500;
        

        public bool EnableInReleaseBuilds => _enableInReleaseBuilds;
        public string DefaultTabId => string.IsNullOrWhiteSpace(_defaultTabId) ? CheatsTabUiUtility.Console : _defaultTabId;
        public CheatsBuiltInTabs EnabledTabs => _enabledTabs;
        public CheatsUiSettings Ui => _ui ??= new CheatsUiSettings();
        public int MaximumConsoleEntries => _maximumConsoleEntries;
        public CheatsGraphVisualConfig ProfilerFpsGraphVisualConfig => _profilerFpsGraphVisualConfig;
        public CheatsGraphVisualConfig ProfilerCpuGraphVisualConfig => _profilerCpuGraphVisualConfig;
        public CheatsGraphVisualConfig ProfilerMemoryGraphVisualConfig => _profilerMemoryGraphVisualConfig;

        public CheatsActivationTriggerSettings ActivationTriggerSettings => _activationTriggerSettings;
        public CheatsPinCodeSettings PinCodeSettings => _pinCodeSettings;

        public Vector2Int FpsThreshold => _fpsThreshold;
    }
}
