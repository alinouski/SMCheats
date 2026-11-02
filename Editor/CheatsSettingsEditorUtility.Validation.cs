using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cheats.Editor
{
    public static partial class CheatsSettingsEditorUtility
    {
        internal static IReadOnlyList<string> GetMissingRuntimePrefabPaths()
        {
            var missingPaths = new List<string>();

            AddMissingPrefabPath(missingPaths, RuntimeViewPrefabPath);
            AddMissingPrefabPath(missingPaths, ActivationViewPrefabPath);
            AddMissingPrefabPath(missingPaths, PinPromptViewPrefabPath);
            AddMissingPrefabPath(missingPaths, ProfilerGraphPrefabPath);
            AddMissingPrefabPath(missingPaths, InfoCategoryPrefabPath);
            AddMissingPrefabPath(missingPaths, ButtonPrefabAssetPath);
            AddMissingPrefabPath(missingPaths, HeaderButtonPrefabAssetPath);
            AddMissingPrefabPath(missingPaths, TabButtonPrefabAssetPath);
            AddMissingPrefabPath(missingPaths, ToggleButtonPrefabAssetPath);
            AddMissingPrefabPath(missingPaths, ConsoleTabButtonPrefabPath);
            AddMissingPrefabPath(missingPaths, ConsoleFilterButtonPrefabPath);
            AddMissingPrefabPath(missingPaths, ConsoleExceptionIndicatorPrefabPath);
            AddMissingPrefabPath(missingPaths, ConsolePinnedPanelPrefabPath);

            foreach (var definition in BuiltInPanelPrefabs)
            {
                if (definition.Id == global::SMCheats.CheatsTabUiUtility.Options)
                {
                    continue;
                }

                AddMissingPrefabPath(missingPaths, GetPanelPrefabAssetPath(definition.Id));
            }

            foreach (var definition in OptionControlPrefabs)
            {
                AddMissingPrefabPath(missingPaths, GetOptionControlPrefabAssetPath(definition.FileName));
            }

            return missingPaths;
        }

        private static void AddMissingPrefabPath(List<string> missingPaths, string assetPath)
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(assetPath) == null)
            {
                missingPaths.Add(assetPath);
            }
        }
    }
}
