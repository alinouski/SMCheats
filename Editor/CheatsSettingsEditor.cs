using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SMCheats.Editor
{
    [CustomEditor(typeof(global::SMCheats.CheatsSettings))]
    public sealed class CheatsSettingsEditor : UnityEditor.Editor
    {
        private SerializedProperty _enableInReleaseBuildsProperty;
        private SerializedProperty _defaultTabIdProperty;
        private SerializedProperty _enabledTabsProperty;
        private SerializedProperty _openMethodsProperty;
        private SerializedProperty _keyboardShortcutProperty;
        private SerializedProperty _requirePinCodeProperty;
        private SerializedProperty _pinCodeProperty;
        private SerializedProperty _maximumPinAttemptsProperty;
        private SerializedProperty _pinLockoutDurationSecondsProperty;
        private SerializedProperty _uiProperty;
        private SerializedProperty _maximumConsoleEntriesProperty;
        private SerializedProperty _profilerFpsGraphVisualConfigProperty;
        private SerializedProperty _profilerCpuGraphVisualConfigProperty;
        private SerializedProperty _profilerMemoryGraphVisualConfigProperty;
        private SerializedProperty _fpsThresholdProperty;

        private void OnEnable()
        {
            _enableInReleaseBuildsProperty = serializedObject.FindProperty("_enableInReleaseBuilds");
            _defaultTabIdProperty = serializedObject.FindProperty("_defaultTabId");
            _enabledTabsProperty = serializedObject.FindProperty("_enabledTabs");

            var activationTriggerSettingsProperty = serializedObject.FindProperty("_activationTriggerSettings");
            _openMethodsProperty = activationTriggerSettingsProperty?.FindPropertyRelative("_openMethods");
            _keyboardShortcutProperty = activationTriggerSettingsProperty?.FindPropertyRelative("_keyboardShortcut");

            var pinCodeSettingsProperty = serializedObject.FindProperty("_pinCodeSettings");
            _requirePinCodeProperty = pinCodeSettingsProperty?.FindPropertyRelative("_requirePinCode");
            _pinCodeProperty = pinCodeSettingsProperty?.FindPropertyRelative("_pinCode");
            _maximumPinAttemptsProperty = pinCodeSettingsProperty?.FindPropertyRelative("_maximumPinAttempts");
            _pinLockoutDurationSecondsProperty = pinCodeSettingsProperty?.FindPropertyRelative("_pinLockoutDurationSeconds");

            _uiProperty = serializedObject.FindProperty("_ui");
            _maximumConsoleEntriesProperty = serializedObject.FindProperty("_maximumConsoleEntries");
            _profilerFpsGraphVisualConfigProperty = serializedObject.FindProperty("_profilerFpsGraphVisualConfig");
            _profilerCpuGraphVisualConfigProperty = serializedObject.FindProperty("_profilerCpuGraphVisualConfig");
            _profilerMemoryGraphVisualConfigProperty = serializedObject.FindProperty("_profilerMemoryGraphVisualConfig");
            _fpsThresholdProperty = serializedObject.FindProperty("_fpsThreshold");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawLocationInfo();

            if (!HasRequiredProperties())
            {
                EditorGUILayout.HelpBox(
                    "CheatsSettings inspector is out of sync with the current serialized structure. Reimport scripts or recreate the canonical asset if this persists.",
                    MessageType.Error);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            DrawValidationMessages();
            DrawGeneralSection();
            DrawUiSection();
            DrawActivationSection();
            DrawSecuritySection();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawLocationInfo()
        {
            var assetPath = AssetDatabase.GetAssetPath(target);
            var isCanonicalAsset = CheatsSettingsEditorUtility.IsCanonicalAssetPath(assetPath);
            var label = isCanonicalAsset ? "Canonical Asset Path" : "Current Asset Path";

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField(label, assetPath);
                EditorGUILayout.LabelField("Expected Resources Path", CheatsSettingsEditorUtility.SettingsResourcePath);
            }
        }

        private void DrawValidationMessages()
        {
            foreach (var message in CollectValidationMessages())
            {
                EditorGUILayout.HelpBox(message.Text, message.Type);
            }
        }

        private void DrawGeneralSection()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(
                    _enableInReleaseBuildsProperty,
                    new GUIContent(
                        "Enable In Release Builds",
                        "Allow cheats in non-development builds. Define SMCHEATS_DISABLED to turn them off everywhere."));
                EditorGUILayout.PropertyField(
                    _enabledTabsProperty,
                    new GUIContent("Enabled Tabs", "Select which built-in tabs are available in the panel."));
                DrawDefaultTabField();
                EditorGUILayout.PropertyField(
                    _maximumConsoleEntriesProperty,
                    new GUIContent("Maximum Console Entries", "Caps how many console entries Cheats keeps in memory."));
                EditorGUILayout.PropertyField(
                    _fpsThresholdProperty,
                    new GUIContent("Fps Threshold"));
                EditorGUILayout.PropertyField(
                    _profilerFpsGraphVisualConfigProperty,
                    new GUIContent("Profiler FPS Graph Visual", "Visual configuration asset used by the FPS profiler graph."));
                EditorGUILayout.PropertyField(
                    _profilerCpuGraphVisualConfigProperty,
                    new GUIContent("Profiler Cpu Graph Visual", "Visual configuration asset used by the CPU profiler graph."));
                EditorGUILayout.PropertyField(
                    _profilerMemoryGraphVisualConfigProperty,
                    new GUIContent("Profiler Memory Graph Visual", "Visual configuration asset used by the memory profiler graph."));
            }
        }

        private void DrawUiSection()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(_uiProperty, new GUIContent("UI Settings"), true);
            }
        }

        private void DrawDefaultTabField()
        {
            var tabOptions = GetBuiltInTabOptions();
            var currentIndex = 0;
            for (var i = 0; i < tabOptions.Length; i++)
            {
                if (tabOptions[i].Id == _defaultTabIdProperty.stringValue)
                {
                    currentIndex = i;
                    break;
                }
            }

            var selectedIndex = EditorGUILayout.Popup(
                new GUIContent("Default Tab", "Tab selected when the panel opens or when the current tab becomes unavailable."),
                currentIndex,
                GetBuiltInTabTitles(tabOptions));

            _defaultTabIdProperty.stringValue = tabOptions[selectedIndex].Id;
        }

        private void DrawActivationSection()
        {
            if (_openMethodsProperty == null ||
                _keyboardShortcutProperty == null)
            {
                return;
            }

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Activation Trigger", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(
                    _openMethodsProperty,
                    new GUIContent("Open Methods", "Select how the cheats window can be opened at runtime."));

                var openMethods = (global::SMCheats.CheatsOpenMethod)_openMethodsProperty.intValue;
                if (openMethods.HasFlag(global::SMCheats.CheatsOpenMethod.KeyboardShortcut))
                {
                    EditorGUILayout.PropertyField(
                        _keyboardShortcutProperty,
                        new GUIContent("Keyboard Shortcut", "Keyboard key used to open Cheats."));
                }

                if (openMethods.HasFlag(global::SMCheats.CheatsOpenMethod.ScreenTrigger))
                {
                    EditorGUILayout.HelpBox(
                        "Screen trigger placement now comes entirely from the activation trigger prefab. Opening still requires three taps in a row.",
                        MessageType.Info);
                }
            }
        }

        private void DrawSecuritySection()
        {
            if (_requirePinCodeProperty == null ||
                _pinCodeProperty == null ||
                _maximumPinAttemptsProperty == null ||
                _pinLockoutDurationSecondsProperty == null)
            {
                return;
            }

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Security", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(
                    _requirePinCodeProperty,
                    new GUIContent("Require Pin Code", "When enabled, the cheats window requires a PIN before opening."));

                if (_requirePinCodeProperty.boolValue)
                {
                    EditorGUILayout.PropertyField(
                        _pinCodeProperty,
                        new GUIContent("Pin Code", "Pin code required to unlock Cheats. The prompt always allows up to 12 entered characters."));
                    EditorGUILayout.PropertyField(
                        _maximumPinAttemptsProperty,
                        new GUIContent("Maximum Pin Attempts", "How many wrong PIN attempts are allowed before temporary lockout."));
                    EditorGUILayout.PropertyField(
                        _pinLockoutDurationSecondsProperty,
                        new GUIContent("Pin Lockout Duration", "How long Cheats stays locked after too many failed PIN attempts."));
                }
            }
        }

        private List<ValidationMessage> CollectValidationMessages()
        {
            var messages = new List<ValidationMessage>();
            if (!HasRequiredProperties())
            {
                messages.Add(new ValidationMessage(
                    "Some serialized CheatsSettings properties are missing. The asset or inspector code needs resync.",
                    MessageType.Error));
                return messages;
            }

            var assetPath = AssetDatabase.GetAssetPath(target);
            var canonicalAsset = CheatsSettingsEditorUtility.LoadSettingsAsset();
            var allSettingsGuids = CheatsSettingsEditorUtility.FindAllSettingsAssetGuids();
            var missingPrefabPaths = CheatsSettingsEditorUtility.GetMissingRuntimePrefabPaths();
            var openMethods = (global::SMCheats.CheatsOpenMethod)_openMethodsProperty.intValue;
            var enabledTabs = (global::SMCheats.CheatsBuiltInTabs)_enabledTabsProperty.intValue;
            var pinCode = _pinCodeProperty.stringValue;
            var maximumPinAttempts = _maximumPinAttemptsProperty.intValue;
            var pinLockoutDuration = _pinLockoutDurationSecondsProperty.floatValue;

            if (!CheatsSettingsEditorUtility.IsCanonicalAssetPath(assetPath))
            {
                messages.Add(new ValidationMessage(
                    $"This asset is not in the canonical Resources path. Runtime loading expects '{CheatsSettingsEditorUtility.SettingsResourcePath}'.",
                    MessageType.Warning));
            }

            if (canonicalAsset == null)
            {
                messages.Add(new ValidationMessage(
                    "Canonical CheatsSettings asset is missing. Create one so runtime loading can find it.",
                    MessageType.Error));
            }

            if (allSettingsGuids.Length > 1)
            {
                messages.Add(new ValidationMessage(
                    $"Multiple CheatsSettings assets were found ({allSettingsGuids.Length}). This can be confusing during maintenance even if runtime loads only the canonical one.",
                    MessageType.Warning));
            }

            foreach (var missingPrefabPath in missingPrefabPaths)
            {
                messages.Add(new ValidationMessage(
                    $"Missing SMCheats prefab: {missingPrefabPath}",
                    MessageType.Error));
            }

            if (enabledTabs == global::SMCheats.CheatsBuiltInTabs.None)
            {
                messages.Add(new ValidationMessage(
                    "No built-in tabs are enabled. The panel can open, but it will not show useful content.",
                    MessageType.Error));
            }

            if (!IsBuiltInTabEnabled(_defaultTabIdProperty.stringValue, enabledTabs))
            {
                messages.Add(new ValidationMessage(
                    "Default Tab is currently disabled by Enabled Tabs.",
                    MessageType.Warning));
            }

            if (openMethods == global::SMCheats.CheatsOpenMethod.None)
            {
                messages.Add(new ValidationMessage(
                    "No open method is selected. Cheats will not be reachable at runtime.",
                    MessageType.Error));
            }

            if (openMethods.HasFlag(global::SMCheats.CheatsOpenMethod.KeyboardShortcut) &&
                (KeyCode)_keyboardShortcutProperty.intValue == KeyCode.None)
            {
                messages.Add(new ValidationMessage(
                    "Keyboard shortcut open method is enabled, but no key is assigned.",
                    MessageType.Warning));
            }

            if (_requirePinCodeProperty.boolValue && string.IsNullOrWhiteSpace(pinCode))
            {
                messages.Add(new ValidationMessage(
                    "PIN protection is enabled, but Pin Code is empty.",
                    MessageType.Error));
            }
            else if (_requirePinCodeProperty.boolValue && pinCode.Length < 4)
            {
                messages.Add(new ValidationMessage(
                    "Pin Code is shorter than 4 characters. This is probably too weak for a shared QA build.",
                    MessageType.Warning));
            }
            else if (_requirePinCodeProperty.boolValue && pinCode.Length > 12)
            {
                messages.Add(new ValidationMessage(
                    "Pin Code is longer than 12 characters, but the runtime prompt intentionally limits user input to that length.",
                    MessageType.Error));
            }

            if (_requirePinCodeProperty.boolValue && maximumPinAttempts <= 0)
            {
                messages.Add(new ValidationMessage(
                    "Maximum Pin Attempts must be greater than 0.",
                    MessageType.Error));
            }

            if (_requirePinCodeProperty.boolValue && maximumPinAttempts == 1)
            {
                messages.Add(new ValidationMessage(
                    "Maximum Pin Attempts is set to 1. A single typo will trigger lockout.",
                    MessageType.Warning));
            }

            if (_requirePinCodeProperty.boolValue && maximumPinAttempts > 0 && pinLockoutDuration <= 0f)
            {
                messages.Add(new ValidationMessage(
                    "PIN lockout duration is 0. Failed attempts will reset immediately even after exceeding the attempt limit.",
                    MessageType.Info));
            }

            if (_maximumConsoleEntriesProperty.intValue < 50)
            {
                messages.Add(new ValidationMessage(
                    "Maximum Console Entries is below 50. Useful logs may roll out too quickly.",
                    MessageType.Warning));
            }

            return messages;
        }

        private bool HasRequiredProperties()
        {
            return _enableInReleaseBuildsProperty != null &&
                   _defaultTabIdProperty != null &&
                   _enabledTabsProperty != null &&
                   _openMethodsProperty != null &&
                   _keyboardShortcutProperty != null &&
                   _requirePinCodeProperty != null &&
                   _pinCodeProperty != null &&
                   _maximumPinAttemptsProperty != null &&
                   _pinLockoutDurationSecondsProperty != null &&
                   _uiProperty != null &&
                   _maximumConsoleEntriesProperty != null &&
                   _profilerFpsGraphVisualConfigProperty != null;
        }

        private readonly struct ValidationMessage
        {
            public ValidationMessage(string text, MessageType type)
            {
                Text = text;
                Type = type;
            }

            public string Text { get; }
            public MessageType Type { get; }
        }

        private static BuiltInTabOption[] GetBuiltInTabOptions()
        {
            return new[]
            {
                new BuiltInTabOption(global::SMCheats.CheatsTabUiUtility.Info, "Info", global::SMCheats.CheatsBuiltInTabs.Info),
                new BuiltInTabOption(global::SMCheats.CheatsTabUiUtility.Console, "Console", global::SMCheats.CheatsBuiltInTabs.Console),
                new BuiltInTabOption(global::SMCheats.CheatsTabUiUtility.Profiler, "Profiler", global::SMCheats.CheatsBuiltInTabs.Profiler),
                new BuiltInTabOption(global::SMCheats.CheatsTabUiUtility.Options, "Options", global::SMCheats.CheatsBuiltInTabs.Options)
            };
        }

        private static string[] GetBuiltInTabTitles(BuiltInTabOption[] options)
        {
            var titles = new string[options.Length];
            for (var i = 0; i < options.Length; i++)
            {
                titles[i] = options[i].Title;
            }

            return titles;
        }

        private static bool IsBuiltInTabEnabled(string tabId, global::SMCheats.CheatsBuiltInTabs enabledTabs)
        {
            foreach (var option in GetBuiltInTabOptions())
            {
                if (option.Id != tabId)
                {
                    continue;
                }

                return enabledTabs.HasFlag(option.Flag);
            }

            return false;
        }

        private readonly struct BuiltInTabOption
        {
            public BuiltInTabOption(string id, string title, global::SMCheats.CheatsBuiltInTabs flag)
            {
                Id = id;
                Title = title;
                Flag = flag;
            }

            public string Id { get; }
            public string Title { get; }
            public global::SMCheats.CheatsBuiltInTabs Flag { get; }
        }
    }
}
