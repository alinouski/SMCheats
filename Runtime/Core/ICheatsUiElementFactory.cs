using System;
using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    public interface ICheatsUiElementFactory
    {
        CheatsButtonView CreateTabButton(Transform parent, Sprite icon, string text);
        CheatsButtonView CreateHeaderActionButton(Transform parent, Sprite icon, string text);
        CheatsButtonView CreateSectionButton(Transform parent, string text, bool emphasized);
        CheatsToggleButtonView CreateToggleButton(Transform parent, Sprite activeIcon, Sprite inactiveIcon);
        RectTransform CreateLayoutRoot(string name, Transform parent, bool vertical, float spacing, RectOffset padding);
        Text CreateText(string name, Transform parent, string text, int fontSize, TextAnchor alignment);
        void CreateSectionHeader(Transform parent, string title);
        void CreatePinnableSectionHeader(Transform parent, string title, string buttonLabel, Action onButtonClicked);
        T InstantiatePrefabView<T>(string resourcePath, Transform parent) where T : Component;
    }
}
