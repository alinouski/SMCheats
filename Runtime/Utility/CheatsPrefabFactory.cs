using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SMCheats
{
    internal static class CheatsPrefabFactory
    {
        public static T InstantiateView<T>(string resourcePath)
            where T : Component
        {
            return InstantiateView<T>(resourcePath, null, false);
        }

        public static T InstantiateView<T>(string resourcePath, Transform parent, bool worldPositionStays = false)
            where T : Component
        {
            if (!TryLoadPrefab(resourcePath, out var prefab))
            {
                throw new InvalidOperationException($"Cheats prefab not found at Resources path '{resourcePath}'.");
            }

            var instance = parent != null
                ? Object.Instantiate(prefab, parent, worldPositionStays)
                : Object.Instantiate(prefab);
            instance.name = prefab.name;
            if (instance.TryGetComponent<T>(out var view))
            {
                return view;
            }

            Object.Destroy(instance);
            throw new InvalidOperationException($"Cheats prefab '{resourcePath}' is missing component {typeof(T).Name}.");
        }

        private static bool TryLoadPrefab(string resourcePath, out GameObject prefab)
        {
            prefab = Resources.Load<GameObject>(resourcePath);
            return prefab != null;
        }
    }
}
