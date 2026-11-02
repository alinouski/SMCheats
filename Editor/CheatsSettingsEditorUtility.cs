using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SMCheats.Editor
{
    public static partial class CheatsSettingsEditorUtility
    {
        // Prefabs and UI configs ship with the package; CheatsSettings belongs to the project.
        internal const string ResourcesFolderPath = "Packages/com.alinouski.smcheats/Runtime/Resources";
        internal const string SettingsResourcePath = SMCheats.CheatsSettings.ResourcePath;
        internal const string DefaultSettingsAssetPath = "Assets/Resources/" + SettingsResourcePath + ".asset";
        internal const string ColorSettingsAssetPath = ResourcesFolderPath + "/" + SMCheats.CheatsUiColorSettings.ResourcePath + ".asset";
        internal const string ProjectColorSettingsAssetPath = "Assets/Resources/" + SMCheats.CheatsUiColorSettings.ResourcePath + ".asset";
        internal const string RuntimeViewResourcesFolderPath = ResourcesFolderPath + "/SMCheats";
        internal const string RuntimeViewPrefabPath = ResourcesFolderPath + "/" + SMCheats.CheatsPrefabResourcePaths.CheatsView + ".prefab";
        internal const string ActivationViewPrefabPath = ResourcesFolderPath + "/" + SMCheats.CheatsPrefabResourcePaths.ActivationView + ".prefab";
        internal const string PinPromptViewPrefabPath = ResourcesFolderPath + "/" + SMCheats.CheatsPrefabResourcePaths.PinPromptView + ".prefab";
        internal const string PanelPrefabsResourcesFolderPath = RuntimeViewResourcesFolderPath + "/Panels";
        internal const string OptionControlPrefabsResourcesFolderPath = ResourcesFolderPath + "/" + SMCheats.CheatsPrefabResourcePaths.OptionControlRoot;
        internal const string UiElementPrefabsResourcesFolderPath = RuntimeViewResourcesFolderPath + "/UiElements";
        internal const string ProfilerGraphPrefabPath = ResourcesFolderPath + "/" + SMCheats.CheatsPrefabResourcePaths.ProfilerGraph + ".prefab";
        internal const string InfoCategoryPrefabPath = ResourcesFolderPath + "/" + SMCheats.CheatsPrefabResourcePaths.InfoCategory + ".prefab";
        internal const string ButtonPrefabAssetPath = ResourcesFolderPath + "/" + SMCheats.CheatsPrefabResourcePaths.Button + ".prefab";
        internal const string HeaderButtonPrefabAssetPath = ResourcesFolderPath + "/" + SMCheats.CheatsPrefabResourcePaths.HeaderButton + ".prefab";
        internal const string TabButtonPrefabAssetPath = ResourcesFolderPath + "/" + SMCheats.CheatsPrefabResourcePaths.TabButton + ".prefab";
        internal const string ToggleButtonPrefabAssetPath = ResourcesFolderPath + "/" + SMCheats.CheatsPrefabResourcePaths.ToggleButton + ".prefab";
        internal const string ConsoleTabButtonPrefabPath = ResourcesFolderPath + "/" + SMCheats.CheatsPrefabResourcePaths.ConsoleTabButton + ".prefab";
        internal const string ConsoleFilterButtonPrefabPath = ResourcesFolderPath + "/" + SMCheats.CheatsPrefabResourcePaths.ConsoleFilterButton + ".prefab";
        internal const string ConsoleExceptionIndicatorPrefabPath = ResourcesFolderPath + "/" + SMCheats.CheatsPrefabResourcePaths.ConsoleExceptionIndicator + ".prefab";
        internal const string ConsolePinnedPanelPrefabPath = ResourcesFolderPath + "/" + SMCheats.CheatsPrefabResourcePaths.ConsolePinnedPanel + ".prefab";
        private static readonly BuiltInPanelPrefabDefinition[] BuiltInPanelPrefabs =
        {
            new(SMCheats.CheatsTabUiUtility.Info, "Info"),
            new(SMCheats.CheatsTabUiUtility.Console, "Console"),
            new(SMCheats.CheatsTabUiUtility.Profiler, "Profiler"),
            new(SMCheats.CheatsTabUiUtility.Options, "Options")
        };
        private static readonly OptionControlPrefabDefinition[] OptionControlPrefabs =
        {
            new("ActionOption"),
            new("ToggleOption"),
            new("DropdownOption"),
            new("SliderOption"),
            new("SelectorOption"),
            new("InputOption"),
            new("ValueOption")
        };

        [MenuItem("Tools/Cheats/Create Settings Asset")]
        public static void CreateSettingsAsset()
        {
            var existingSettings = LoadSettingsAsset();
            if (existingSettings != null)
            {
                var existingColorSettings = CreateColorSettingsAssetIfMissing();
                AssignColorSettings(existingSettings, existingColorSettings);
                AssetDatabase.SaveAssets();
                SelectSettingsAsset(existingSettings);
                return;
            }

            var settingsFolderPath = Path.GetDirectoryName(DefaultSettingsAssetPath);
            if (!string.IsNullOrEmpty(settingsFolderPath) && !Directory.Exists(settingsFolderPath))
            {
                Directory.CreateDirectory(settingsFolderPath);
            }

            var colorSettings = CreateColorSettingsAssetIfMissing();
            var settings = ScriptableObject.CreateInstance<SMCheats.CheatsSettings>();
            AssetDatabase.CreateAsset(settings, DefaultSettingsAssetPath);
            AssignColorSettings(settings, colorSettings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            SelectSettingsAsset(settings);
        }

        internal static global::SMCheats.CheatsSettings LoadSettingsAsset()
        {
            foreach (var guid in FindAllSettingsAssetGuids())
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (IsCanonicalAssetPath(assetPath))
                {
                    return AssetDatabase.LoadAssetAtPath<global::SMCheats.CheatsSettings>(assetPath);
                }
            }

            return null;
        }

        private static global::SMCheats.CheatsUiColorSettings CreateColorSettingsAssetIfMissing()
        {
            var existing = AssetDatabase.LoadAssetAtPath<global::SMCheats.CheatsUiColorSettings>(ColorSettingsAssetPath);
            if (existing != null)
            {
                return existing;
            }

            existing = AssetDatabase.LoadAssetAtPath<global::SMCheats.CheatsUiColorSettings>(ProjectColorSettingsAssetPath);
            if (existing != null)
            {
                return existing;
            }

            // The package folder is read-only when installed from git, so a missing asset is created in the project.
            var folderPath = Path.GetDirectoryName(ProjectColorSettingsAssetPath);
            if (!string.IsNullOrEmpty(folderPath) && !Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }

            var colorSettings = ScriptableObject.CreateInstance<global::SMCheats.CheatsUiColorSettings>();
            AssetDatabase.CreateAsset(colorSettings, ProjectColorSettingsAssetPath);
            return colorSettings;
        }

        private static void AssignColorSettings(
            global::SMCheats.CheatsSettings settings,
            global::SMCheats.CheatsUiColorSettings colorSettings)
        {
            if (settings == null || colorSettings == null)
            {
                return;
            }

            var serializedSettings = new SerializedObject(settings);
            var colorProperty = serializedSettings
                .FindProperty("_ui")
                ?.FindPropertyRelative("_colors");
            if (colorProperty == null)
            {
                return;
            }

            colorProperty.objectReferenceValue = colorSettings;
            serializedSettings.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ValidateRuntimeViewPrefabsExist(bool selectCreatedAsset)
        {
            if (!Directory.Exists(RuntimeViewResourcesFolderPath))
            {
                Directory.CreateDirectory(RuntimeViewResourcesFolderPath);
            }

            if (LoadRuntimeViewPrefab() == null)
            {
                var runtimeViewRoot = BuildRuntimeViewPrefabRoot();
                try
                {
                    PrefabUtility.SaveAsPrefabAsset(runtimeViewRoot, RuntimeViewPrefabPath);
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(runtimeViewRoot);
                }
            }

            if (LoadActivationViewPrefab() == null)
            {
                var activationViewRoot = BuildActivationViewPrefabRoot();
                try
                {
                    PrefabUtility.SaveAsPrefabAsset(activationViewRoot, ActivationViewPrefabPath);
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(activationViewRoot);
                }
            }

            if (LoadPinPromptViewPrefab() == null)
            {
                var pinPromptViewRoot = BuildPinPromptViewPrefabRoot();
                try
                {
                    PrefabUtility.SaveAsPrefabAsset(pinPromptViewRoot, PinPromptViewPrefabPath);
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(pinPromptViewRoot);
                }
            }

            ValidatePanelPrefabsExist();
            ValidateOptionControlPrefabsExist();
            ValidateUiElementPrefabsExist();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var runtimeViewPrefab = LoadRuntimeViewPrefab();
            if (selectCreatedAsset && runtimeViewPrefab != null)
            {
                SelectSettingsAsset(runtimeViewPrefab);
            }
        }

        private static void RebuildAllRuntimeViewPrefabs(bool selectCreatedAsset)
        {
            if (!Directory.Exists(RuntimeViewResourcesFolderPath))
            {
                Directory.CreateDirectory(RuntimeViewResourcesFolderPath);
            }

            var runtimeViewRoot = BuildRuntimeViewPrefabRoot();
            try
            {
                PrefabUtility.SaveAsPrefabAsset(runtimeViewRoot, RuntimeViewPrefabPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(runtimeViewRoot);
            }

            var activationViewRoot = BuildActivationViewPrefabRoot();
            try
            {
                PrefabUtility.SaveAsPrefabAsset(activationViewRoot, ActivationViewPrefabPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(activationViewRoot);
            }

            var pinPromptViewRoot = BuildPinPromptViewPrefabRoot();
            try
            {
                PrefabUtility.SaveAsPrefabAsset(pinPromptViewRoot, PinPromptViewPrefabPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(pinPromptViewRoot);
            }

            RebuildPanelPrefabs();
            RebuildOptionControlPrefabs();
            RebuildUiElementPrefabs();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var runtimeViewPrefab = LoadRuntimeViewPrefab();
            if (selectCreatedAsset && runtimeViewPrefab != null)
            {
                SelectSettingsAsset(runtimeViewPrefab);
            }
        }

        internal static GameObject LoadRuntimeViewPrefab()
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(RuntimeViewPrefabPath);
        }

        internal static GameObject LoadActivationViewPrefab()
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(ActivationViewPrefabPath);
        }

        internal static GameObject LoadPinPromptViewPrefab()
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(PinPromptViewPrefabPath);
        }

        internal static GameObject LoadProfilerGraphPrefab()
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(ProfilerGraphPrefabPath);
        }

        internal static GameObject LoadInfoCategoryPrefab()
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(InfoCategoryPrefabPath);
        }

        private static void ValidatePanelPrefabsExist()
        {
            if (!Directory.Exists(PanelPrefabsResourcesFolderPath))
            {
                Directory.CreateDirectory(PanelPrefabsResourcesFolderPath);
            }

            var runtimeFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            EnsureProfilerGraphPrefabExists(runtimeFont);
            EnsureInfoCategoryPrefabExists(runtimeFont);
            foreach (var definition in BuiltInPanelPrefabs)
            {
                var panelFolderPath = PanelPrefabsResourcesFolderPath + "/" + definition.Id;
                if (!Directory.Exists(panelFolderPath))
                {
                    Directory.CreateDirectory(panelFolderPath);
                }

                var mainAssetPath = GetPanelPrefabAssetPath(definition.Id);
                if (AssetDatabase.LoadAssetAtPath<GameObject>(mainAssetPath) == null)
                {
                    SavePanelPrefab(BuildPanelPrefabRoot(definition, runtimeFont), mainAssetPath);
                }
            }
        }

        private static void SavePanelPrefab(GameObject panelRoot, string assetPath)
        {
            try
            {
                PrefabUtility.SaveAsPrefabAsset(panelRoot, assetPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(panelRoot);
            }
        }

        private static void RebuildPanelPrefabs()
        {
            if (!Directory.Exists(PanelPrefabsResourcesFolderPath))
            {
                Directory.CreateDirectory(PanelPrefabsResourcesFolderPath);
            }

            var runtimeFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            EnsureProfilerGraphPrefabExists(runtimeFont);
            EnsureInfoCategoryPrefabExists(runtimeFont);
            foreach (var definition in BuiltInPanelPrefabs)
            {
                var panelFolderPath = PanelPrefabsResourcesFolderPath + "/" + definition.Id;
                if (!Directory.Exists(panelFolderPath))
                {
                    Directory.CreateDirectory(panelFolderPath);
                }

                SavePanelPrefab(BuildPanelPrefabRoot(definition, runtimeFont), GetPanelPrefabAssetPath(definition.Id));
            }
        }

        private static void EnsureProfilerGraphPrefabExists(Font runtimeFont)
        {
            if (LoadProfilerGraphPrefab() != null)
            {
                return;
            }

            var folderPath = Path.GetDirectoryName(ProfilerGraphPrefabPath);
            if (!string.IsNullOrEmpty(folderPath) && !Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            SaveProfilerGraphPrefab(BuildProfilerGraphPrefabRoot(runtimeFont), ProfilerGraphPrefabPath);
        }

        private static void SaveProfilerGraphPrefab(GameObject prefabRoot, string assetPath)
        {
            try
            {
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(prefabRoot);
            }
        }

        private static void EnsureInfoCategoryPrefabExists(Font runtimeFont)
        {
            if (LoadInfoCategoryPrefab() != null)
            {
                return;
            }

            var folderPath = Path.GetDirectoryName(InfoCategoryPrefabPath);
            if (!string.IsNullOrEmpty(folderPath) && !Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            SavePanelPrefab(BuildInfoCategoryPrefabRoot(runtimeFont), InfoCategoryPrefabPath);
        }

        private static void ValidateOptionControlPrefabsExist()
        {
            if (!Directory.Exists(OptionControlPrefabsResourcesFolderPath))
            {
                Directory.CreateDirectory(OptionControlPrefabsResourcesFolderPath);
            }

            var runtimeFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            foreach (var definition in OptionControlPrefabs)
            {
                var assetPath = GetOptionControlPrefabAssetPath(definition.FileName);
                if (AssetDatabase.LoadAssetAtPath<GameObject>(assetPath) != null)
                {
                    continue;
                }

                SaveOptionControlPrefab(BuildOptionControlPrefabRoot(definition, runtimeFont), assetPath);
            }
        }

        private static void SaveOptionControlPrefab(GameObject prefabRoot, string assetPath)
        {
            try
            {
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(prefabRoot);
            }
        }

        private static void RebuildOptionControlPrefabs()
        {
            if (!Directory.Exists(OptionControlPrefabsResourcesFolderPath))
            {
                Directory.CreateDirectory(OptionControlPrefabsResourcesFolderPath);
            }

            var runtimeFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            foreach (var definition in OptionControlPrefabs)
            {
                SaveOptionControlPrefab(BuildOptionControlPrefabRoot(definition, runtimeFont), GetOptionControlPrefabAssetPath(definition.FileName));
            }
        }

        private static void ValidateUiElementPrefabsExist()
        {
            if (!Directory.Exists(UiElementPrefabsResourcesFolderPath))
            {
                Directory.CreateDirectory(UiElementPrefabsResourcesFolderPath);
            }

            var runtimeFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (AssetDatabase.LoadAssetAtPath<GameObject>(ButtonPrefabAssetPath) == null)
            {
                SaveUiElementPrefab(BuildButtonPrefabRoot("Button", runtimeFont), ButtonPrefabAssetPath);
            }

            if (AssetDatabase.LoadAssetAtPath<GameObject>(HeaderButtonPrefabAssetPath) == null)
            {
                SaveUiElementPrefab(BuildButtonPrefabRoot("HeaderButton", runtimeFont), HeaderButtonPrefabAssetPath);
            }

            if (AssetDatabase.LoadAssetAtPath<GameObject>(TabButtonPrefabAssetPath) == null)
            {
                SaveUiElementPrefab(BuildButtonPrefabRoot("TabButton", runtimeFont), TabButtonPrefabAssetPath);
            }

            if (AssetDatabase.LoadAssetAtPath<GameObject>(ToggleButtonPrefabAssetPath) == null)
            {
                SaveUiElementPrefab(BuildToggleButtonPrefabRoot(), ToggleButtonPrefabAssetPath);
            }
        }

        private static void RebuildUiElementPrefabs()
        {
            if (!Directory.Exists(UiElementPrefabsResourcesFolderPath))
            {
                Directory.CreateDirectory(UiElementPrefabsResourcesFolderPath);
            }

            var runtimeFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            SaveUiElementPrefab(BuildButtonPrefabRoot("Button", runtimeFont), ButtonPrefabAssetPath);
            SaveUiElementPrefab(BuildButtonPrefabRoot("HeaderButton", runtimeFont), HeaderButtonPrefabAssetPath);
            SaveUiElementPrefab(BuildButtonPrefabRoot("TabButton", runtimeFont), TabButtonPrefabAssetPath);
            SaveUiElementPrefab(BuildToggleButtonPrefabRoot(), ToggleButtonPrefabAssetPath);
        }

        private static void SaveUiElementPrefab(GameObject prefabRoot, string assetPath)
        {
            try
            {
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(prefabRoot);
            }
        }

        private static string GetPanelPrefabAssetPath(string panelId)
        {
            var resourcePath = global::SMCheats.CheatsPrefabResourcePaths.Main(panelId);
            return ResourcesFolderPath + "/" + resourcePath + ".prefab";
        }

        private static string GetOptionControlPrefabAssetPath(string fileName)
        {
            return OptionControlPrefabsResourcesFolderPath + "/" + fileName + ".prefab";
        }

        internal static bool IsCanonicalAssetPath(string assetPath)
        {
            return assetPath.StartsWith("Assets/", StringComparison.Ordinal) &&
                   assetPath.EndsWith("/Resources/" + SettingsResourcePath + ".asset", StringComparison.Ordinal);
        }

        internal static string[] FindAllSettingsAssetGuids()
        {
            return AssetDatabase.FindAssets($"t:{nameof(global::SMCheats.CheatsSettings)}");
        }

        internal static void SelectSettingsAsset(UnityEngine.Object asset)
        {
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }

        private readonly struct BuiltInPanelPrefabDefinition
        {
            public BuiltInPanelPrefabDefinition(string id, string title)
            {
                Id = id;
                Title = title;
            }

            public string Id { get; }
            public string Title { get; }
        }

        private readonly struct OptionControlPrefabDefinition
        {
            public OptionControlPrefabDefinition(string fileName)
            {
                FileName = fileName;
            }

            public string FileName { get; }
        }
    }
}
