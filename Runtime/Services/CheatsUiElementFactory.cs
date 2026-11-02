using System;
using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    internal sealed class CheatsUiElementFactory : ICheatsUiElementFactory
    {
        private static Font _runtimeFont;

        public CheatsButtonView CreateTabButton(Transform parent, Sprite icon, string text)
        {
            var view = CreateButton(parent, text, CheatsPrefabResourcePaths.TabButton);
            ConfigureIcon(view, icon);
            return view;
        }

        public CheatsButtonView CreateHeaderActionButton(Transform parent, Sprite icon, string text)
        {
            var view = CreateButton(parent, text, CheatsPrefabResourcePaths.HeaderButton);
            ConfigureIcon(view, icon);

            return view;
        }

        private static void ConfigureIcon(CheatsButtonView view, Sprite icon)
        {
            view.Label.gameObject.SetActive(icon == null || !string.IsNullOrWhiteSpace(view.Label.text));

            var showIcon = icon != null;
            view.Icon.gameObject.SetActive(showIcon);
            if (showIcon)
            {
                view.Icon.sprite = icon;
                view.Icon.preserveAspect = true;
            }
        }

        public CheatsButtonView CreateSectionButton(Transform parent, string text, bool emphasized)
        {
            var view = CreateButton(parent, text);
            var uiSettings = CheatsSettingsProxy.UiSettings;
            view.Background.color = emphasized
                ? uiSettings.SectionButtonEmphasizedBackgroundColor
                : uiSettings.SectionButtonBackgroundColor;

            return view;
        }

        public CheatsToggleButtonView CreateToggleButton(Transform parent, Sprite activeIcon, Sprite inactiveIcon)
        {
            var view = InstantiatePrefabView<CheatsToggleButtonView>(CheatsPrefabResourcePaths.ToggleButton, parent);
            view.SetIcons(activeIcon, inactiveIcon);
            view.IsActive = false;
            return view;
        }

        public CheatsButtonView CreateButton(Transform parent, string text)
        {
            return CreateButton(parent, text, CheatsPrefabResourcePaths.Button);
        }

        private static CheatsButtonView CreateButton(Transform parent, string text, string resourcePath)
        {
            var view = CheatsPrefabFactory.InstantiateView<CheatsButtonView>(resourcePath, parent);

            view.Label.text = text ?? string.Empty;
            view.Label.gameObject.SetActive(!string.IsNullOrEmpty(text));

            view.Icon.gameObject.SetActive(false);

            return view;
        }

        public RectTransform CreateLayoutRoot(string name, Transform parent, bool vertical, float spacing, RectOffset padding)
        {
            var root = new GameObject(name, typeof(RectTransform));
            root.transform.SetParent(parent, false);
            var rect = root.GetComponent<RectTransform>();
            if (vertical)
            {
                var layout = root.AddComponent<VerticalLayoutGroup>();
                layout.spacing = spacing;
                layout.padding = padding;
                layout.childAlignment = TextAnchor.UpperLeft;
                layout.childControlHeight = false;
                layout.childControlWidth = true;
                layout.childForceExpandHeight = false;
                layout.childForceExpandWidth = true;
            }
            else
            {
                var layout = root.AddComponent<HorizontalLayoutGroup>();
                layout.spacing = spacing;
                layout.padding = padding;
                layout.childAlignment = TextAnchor.MiddleLeft;
                layout.childControlHeight = false;
                layout.childControlWidth = false;
                layout.childForceExpandHeight = false;
                layout.childForceExpandWidth = false;
            }

            root.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return rect;
        }

        public Text CreateText(string name, Transform parent, string text, int fontSize, TextAnchor alignment)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            var uiText = textObject.GetComponent<Text>();
            uiText.font = GetFont();
            uiText.fontSize = fontSize;
            uiText.alignment = alignment;
            uiText.color = CheatsSettingsProxy.UiSettings.TextColor;
            uiText.horizontalOverflow = HorizontalWrapMode.Wrap;
            uiText.verticalOverflow = VerticalWrapMode.Overflow;
            uiText.text = text;
            return uiText;
        }

        public void CreateSectionHeader(Transform parent, string title)
        {
            var headerRoot = new GameObject(title + "Header", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            headerRoot.transform.SetParent(parent, false);
            headerRoot.GetComponent<Image>().color = CheatsSettingsProxy.UiSettings.SectionButtonBackgroundColor;
            headerRoot.GetComponent<LayoutElement>().preferredHeight = 28f;
            var header = CreateText("Label", headerRoot.transform, title, 16, TextAnchor.MiddleLeft);
            Stretch((RectTransform)header.transform, 10f);
        }

        public void CreatePinnableSectionHeader(Transform parent, string title, string buttonLabel, Action onButtonClicked)
        {
            var headerRoot = new GameObject(title + "Header", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            headerRoot.transform.SetParent(parent, false);
            headerRoot.GetComponent<Image>().color = CheatsSettingsProxy.UiSettings.SectionButtonBackgroundColor;
            headerRoot.GetComponent<LayoutElement>().preferredHeight = 32f;

            var layout = headerRoot.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(10, 8, 0, 0);
            layout.spacing = 8f;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;

            var titleText = CreateText("Label", headerRoot.transform, title, 16, TextAnchor.MiddleLeft);
            titleText.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            var pinButton = CreateButton(headerRoot.transform, buttonLabel);
            pinButton.Button.onClick.AddListener(() => onButtonClicked?.Invoke());
        }

        public T InstantiatePrefabView<T>(string resourcePath, Transform parent)
            where T : Component
        {
            return CheatsPrefabFactory.InstantiateView<T>(resourcePath, parent);
        }

        public static void UpdateStateButton(Button button, Text label, bool isActive, string activeText, string inactiveText)
        {
            if (label != null)
            {
                label.text = (isActive ? activeText : inactiveText) ?? string.Empty;
            }

            var image = button.GetComponent<Image>();
            image.color = isActive
                ? CheatsSettingsProxy.UiSettings.ToggleButtonActiveBackgroundColor
                : CheatsSettingsProxy.UiSettings.ToggleButtonInactiveBackgroundColor;
        }

        private static Font GetFont()
        {
            _runtimeFont ??= Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return _runtimeFont;
        }

        private static void Stretch(RectTransform rectTransform, float inset = 0f)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = new Vector2(inset, inset);
            rectTransform.offsetMax = new Vector2(-inset, -inset);
        }
    }
}
