using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SMCheats.Editor
{
    public static partial class CheatsSettingsEditorUtility
    {
        private static GameObject BuildRuntimeViewPrefabRoot()
        {
            var runtimeFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var root = new GameObject(
                "CheatsView",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster),
                typeof(global::SMCheats.CheatsView));

            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = short.MaxValue - 32;

            var canvasScaler = root.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
            canvasScaler.matchWidthOrHeight = 0.5f;

            var rootRect = root.GetComponent<RectTransform>();
            rootRect.localScale = Vector3.one;
            Stretch(rootRect);

            var cheatsScreen = CreatePanel("CheatsScreen", rootRect, new Color(0.08f, 0.09f, 0.11f, 0.96f));
            Stretch(cheatsScreen, 16f);

            var headerPanel = CreatePanel("HeaderPanel", cheatsScreen, new Color(0.12f, 0.13f, 0.16f, 0.96f));
            headerPanel.anchorMin = new Vector2(0f, 0.9225038f);
            headerPanel.anchorMax = new Vector2(1f, 1f);
            headerPanel.pivot = new Vector2(0.5f, 1f);
            headerPanel.anchoredPosition = Vector2.zero;
            headerPanel.sizeDelta = new Vector2(0f, -1f);
            BuildMainHeader(headerPanel, runtimeFont, out var headerTitle, out var headerActionsRoot, out var closeButton);

            var tabs = CreateRectRoot("Tabs", cheatsScreen);
            tabs.anchorMin = new Vector2(0f, 0f);
            tabs.anchorMax = new Vector2(0.13347459f, 0.9225038f);
            tabs.pivot = new Vector2(0f, 0.5f);
            tabs.anchoredPosition = new Vector2(0f, 0.5f);
            tabs.sizeDelta = new Vector2(0f, 1f);

            var tabsPanel = CreatePanel("TabsPanel", tabs, new Color(0.12f, 0.13f, 0.16f, 0.92f));
            tabsPanel.anchorMin = new Vector2(0f, 1f);
            tabsPanel.anchorMax = new Vector2(1f, 1f);
            tabsPanel.pivot = new Vector2(0.5f, 1f);
            tabsPanel.anchoredPosition = new Vector2(0f, -27f);
            tabsPanel.sizeDelta = Vector2.zero;
            AddVerticalLayout(tabsPanel.gameObject, 8f, new RectOffset(12, 12, 12, 12));

            var contentPanel = CreatePanel("ContentPanel", cheatsScreen, new Color(0.12f, 0.13f, 0.16f, 0.92f));
            contentPanel.anchorMin = new Vector2(0.13347459f, 0f);
            contentPanel.anchorMax = new Vector2(1f, 0.9225038f);
            contentPanel.anchoredPosition = new Vector2(0f, 0.39196777f);
            contentPanel.sizeDelta = new Vector2(0f, 0.7839966f);

            var pinnedScreen = CreateCanvasElementRoot("PinnedScreen", rootRect);
            Stretch(pinnedScreen, 16f);

            var runtimeView = root.GetComponent<global::SMCheats.CheatsView>();
            AssignRuntimeViewReferences(
                runtimeView,
                canvas,
                canvasScaler,
                root.GetComponent<GraphicRaycaster>(),
                rootRect,
                cheatsScreen,
                tabsPanel,
                contentPanel,
                pinnedScreen,
                headerActionsRoot,
                headerTitle,
                closeButton);

            return root;
        }

        private static GameObject BuildPinPromptViewPrefabRoot()
        {
            var runtimeFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var root = new GameObject(
                "CheatsPinPromptView",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster),
                typeof(global::SMCheats.CheatsPinPromptView));

            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = short.MaxValue - 31;

            var canvasScaler = root.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
            canvasScaler.matchWidthOrHeight = 0.5f;

            var rootRect = root.GetComponent<RectTransform>();
            rootRect.localScale = Vector3.one;
            Stretch(rootRect);

            var pinScreen = CreatePanel("PinScreen", rootRect, new Color(0.08f, 0.09f, 0.11f, 0.98f));
            pinScreen.anchorMin = new Vector2(0.5f, 0.5f);
            pinScreen.anchorMax = new Vector2(0.5f, 0.5f);
            pinScreen.pivot = new Vector2(0.5f, 0.5f);
            pinScreen.sizeDelta = new Vector2(360f, 420f);

            BuildPinPrompt(
                pinScreen,
                runtimeFont,
                out var pinPromptMaskedText,
                out var pinPromptStatusLabel,
                out var pinPromptSubmitButton,
                out var pinPromptCancelButton,
                out var pinDigitButtons);

            AssignPinPromptViewReferences(
                root.GetComponent<global::SMCheats.CheatsPinPromptView>(),
                canvas,
                canvasScaler,
                root.GetComponent<GraphicRaycaster>(),
                pinScreen,
                pinPromptStatusLabel,
                pinPromptMaskedText,
                pinPromptSubmitButton,
                pinPromptCancelButton,
                pinDigitButtons);

            return root;
        }

        private static GameObject BuildActivationViewPrefabRoot()
        {
            var runtimeFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var root = new GameObject(
                "CheatsActivationView",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster),
                typeof(global::SMCheats.CheatsActivationView));

            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = short.MaxValue - 33;

            var canvasScaler = root.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
            canvasScaler.matchWidthOrHeight = 0.5f;

            var rootRect = root.GetComponent<RectTransform>();
            rootRect.localScale = Vector3.one;
            Stretch(rootRect);

            var triggerButton = CreateButton("Tap x3", rootRect, runtimeFont, out _);
            var triggerButtonRect = (RectTransform)triggerButton.transform;
            triggerButtonRect.anchorMin = new Vector2(0.5f, 1f);
            triggerButtonRect.anchorMax = new Vector2(0.5f, 1f);
            triggerButtonRect.pivot = new Vector2(0.5f, 0.5f);
            triggerButtonRect.anchoredPosition = new Vector2(0f, -50f);
            triggerButtonRect.sizeDelta = new Vector2(92f, 92f);
            triggerButton.GetComponent<Image>().color = new Color(0.18f, 0.2f, 0.24f, 0f);

            var activationView = root.GetComponent<global::SMCheats.CheatsActivationView>();
            AssignActivationViewReferences(
                activationView,
                canvas,
                canvasScaler,
                root.GetComponent<GraphicRaycaster>(),
                triggerButton);

            return root;
        }

        private static GameObject BuildPanelPrefabRoot(BuiltInPanelPrefabDefinition definition, Font runtimeFont)
        {
            var root = new GameObject(
                definition.Id + "MainPanel",
                typeof(RectTransform),
                typeof(Image),
                typeof(LayoutElement),
                typeof(global::SMCheats.BaseCheatsView));

            root.transform.localScale = Vector3.one;
            var rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0f, 1f);
            rootRect.anchorMax = new Vector2(1f, 1f);
            rootRect.pivot = new Vector2(0.5f, 1f);
            rootRect.sizeDelta = Vector2.zero;

            root.GetComponent<Image>().color = new Color(0.11f, 0.12f, 0.15f, 0.7f);

            var layoutElement = root.GetComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1f;
            layoutElement.minHeight = 0f;

            if (definition.Id == global::SMCheats.CheatsTabUiUtility.Profiler)
            {
                AssignBasePanelViewReferences(root.GetComponent<global::SMCheats.BaseCheatsView>(), rootRect);
                BuildProfilerPanelContent(root, rootRect, runtimeFont);
                return root;
            }

            var contentRoot = CreateLayoutRoot("Content", rootRect, true, 8f, new RectOffset(0, 0, 0, 0));
            contentRoot.anchorMin = Vector2.zero;
            contentRoot.anchorMax = Vector2.one;
            contentRoot.offsetMin = new Vector2(12f, 12f);
            contentRoot.offsetMax = new Vector2(-12f, -12f);

            var panelView = root.GetComponent<global::SMCheats.BaseCheatsView>();
            AssignBasePanelViewReferences(panelView, rootRect);

            switch (definition.Id)
            {
                case global::SMCheats.CheatsTabUiUtility.Console:
                    BuildConsolePanelContent(root, contentRoot, runtimeFont);
                    break;
                case global::SMCheats.CheatsTabUiUtility.Info:
                    BuildInfoPanelContent(root, contentRoot, runtimeFont);
                    break;
            }

            return root;
        }

        private static void AssignBasePanelViewReferences(global::SMCheats.BaseCheatsView panelView, RectTransform root)
        {
            var serializedObject = new SerializedObject(panelView);
            serializedObject.FindProperty("_root").objectReferenceValue = root;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildConsolePanelContent(GameObject root, RectTransform contentRoot, Font runtimeFont)
        {
            var view = root.AddComponent<global::SMCheats.CheatsConsolePanelView>();
            var filters = CreateLayoutRoot("ConsoleFilters", contentRoot, false, 8f, new RectOffset(0, 0, 0, 0));
            var copySelectedButton = CreateButton("Copy Selected", filters, runtimeFont, out _);

            var entriesRoot = CreateLayoutRoot("Entries", contentRoot, true, 8f, new RectOffset(0, 0, 0, 0));
            entriesRoot.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            var entriesView = entriesRoot.gameObject.AddComponent<global::SMCheats.CheatsConsoleEntriesView>();
            var entryTemplate = CreateConsoleEntryTemplate(entriesRoot, runtimeFont);
            entryTemplate.gameObject.SetActive(false);

            var entriesSerializedObject = new SerializedObject(entriesView);
            entriesSerializedObject.FindProperty("_entriesRoot").objectReferenceValue = entriesRoot;
            entriesSerializedObject.FindProperty("_entryTemplate").objectReferenceValue = entryTemplate;
            entriesSerializedObject.ApplyModifiedPropertiesWithoutUndo();

            var serializedObject = new SerializedObject(view);
            serializedObject.FindProperty("_copySelectedButton").objectReferenceValue = copySelectedButton;
            serializedObject.FindProperty("_entriesView").objectReferenceValue = entriesView;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildInfoPanelContent(GameObject root, RectTransform contentRoot, Font runtimeFont)
        {
            var view = root.AddComponent<global::SMCheats.CheatsSystemPanelView>();
            contentRoot.name = "InfoCategories";
            var categoryPrefab = LoadInfoCategoryPrefab()?.GetComponent<global::SMCheats.CheatsInfoCategoryView>();

            var serializedObject = new SerializedObject(view);
            serializedObject.FindProperty("_categoriesRoot").objectReferenceValue = contentRoot;
            serializedObject.FindProperty("_categoryPrefab").objectReferenceValue = categoryPrefab;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static GameObject BuildInfoCategoryPrefabRoot(Font runtimeFont)
        {
            var root = new GameObject(
                "InfoCategory",
                typeof(RectTransform),
                typeof(Image),
                typeof(VerticalLayoutGroup),
                typeof(LayoutElement),
                typeof(global::SMCheats.CheatsInfoCategoryView));

            root.transform.localScale = Vector3.one;
            root.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.12f);

            var layout = root.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 6f;
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.childControlHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            var layoutElement = root.GetComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1f;

            var titleText = CreateSectionHeader(root.transform, "Category", runtimeFont);
            var entriesRoot = CreateLayoutRoot("Entries", root.transform, true, 4f, new RectOffset(0, 0, 0, 0));
            var entryTemplate = CreateInfoEntryTemplate(entriesRoot, runtimeFont);
            entryTemplate.gameObject.SetActive(false);

            var serializedObject = new SerializedObject(root.GetComponent<global::SMCheats.CheatsInfoCategoryView>());
            serializedObject.FindProperty("_titleText").objectReferenceValue = titleText;
            serializedObject.FindProperty("_entriesRoot").objectReferenceValue = entriesRoot;
            serializedObject.FindProperty("_entryTemplate").objectReferenceValue = entryTemplate;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            return root;
        }

        private static void BuildProfilerPanelContent(GameObject root, RectTransform contentRoot, Font runtimeFont)
        {
            var view = root.AddComponent<global::SMCheats.CheatsProfilerPanelView>();
            var fpsGraphView = InstantiateProfilerGraphView(contentRoot, runtimeFont);
            var cpuGraphView = InstantiateProfilerGraphView(contentRoot, runtimeFont);

            var serializedObject = new SerializedObject(view);
            serializedObject.FindProperty("_fpsGraph").objectReferenceValue = fpsGraphView;
            serializedObject.FindProperty("_cpuGraph").objectReferenceValue = cpuGraphView;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static global::SMCheats.CheatsGraphView InstantiateProfilerGraphView(RectTransform parent, Font runtimeFont)
        {
            var prefab = LoadProfilerGraphPrefab();
            var graphRoot = prefab != null
                ? PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject
                : BuildProfilerGraphPrefabRoot(runtimeFont);

            if (graphRoot == null)
            {
                throw new InvalidOperationException("Profiler graph prefab could not be instantiated.");
            }

            if (graphRoot.transform.parent == null)
            {
                graphRoot.transform.SetParent(parent, false);
            }

            graphRoot.name = "ProfilerGraph";
            graphRoot.transform.localScale = Vector3.one;
            if (!graphRoot.TryGetComponent<global::SMCheats.CheatsGraphView>(out var graphView))
            {
                throw new InvalidOperationException("Profiler graph prefab must have CheatsGraphView on its root.");
            }

            return graphView;
        }

        private static GameObject BuildProfilerGraphPrefabRoot(Font runtimeFont)
        {
            var root = new GameObject(
                "ProfilerGraph",
                typeof(RectTransform),
                typeof(global::SMCheats.CheatsGraphView));
            root.transform.localScale = Vector3.one;
            Stretch(root.GetComponent<RectTransform>());

            var background = CreateProfilerGraphBackground(root.transform);
            var graphGraphic = CreateProfilerGraphGraphic(root.transform);

            var headerRoot = new GameObject("FPSHeader", typeof(RectTransform), typeof(Image));
            headerRoot.transform.SetParent(root.transform, false);
            headerRoot.transform.localScale = Vector3.one;
            headerRoot.GetComponent<Image>().color = new Color(0.16f, 0.18f, 0.22f, 0.92f);
            var headerRect = headerRoot.GetComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0f, 1f);
            headerRect.anchorMax = new Vector2(1f, 1f);
            headerRect.pivot = new Vector2(0.5f, 1f);
            headerRect.anchoredPosition = Vector2.zero;
            headerRect.sizeDelta = new Vector2(0f, 76f);

            var titleText = CreateText("Label", headerRoot.transform, "FPS", runtimeFont, 16, TextAnchor.MiddleCenter);
            var titleRect = (RectTransform)titleText.transform;
            titleRect.anchorMin = new Vector2(0f, 0f);
            titleRect.anchorMax = new Vector2(0.08f, 1f);
            titleRect.offsetMin = new Vector2(10f, 10f);
            titleRect.offsetMax = new Vector2(-2f, -10f);

            var valuesRoot = CreateLayoutRoot("GraphValues", headerRoot.transform, false, 0f, new RectOffset(0, 0, 0, 0));
            valuesRoot.anchorMin = new Vector2(0.08f, 0f);
            valuesRoot.anchorMax = new Vector2(1f, 1f);
            valuesRoot.offsetMin = new Vector2(14f, 0f);
            valuesRoot.offsetMax = Vector2.zero;

            var valueRowTemplate = CreateProfilerGraphValueRow(valuesRoot, runtimeFont);
            valueRowTemplate.SetActive(false);

            var graphView = root.GetComponent<global::SMCheats.CheatsGraphView>();
            var serializedObject = new SerializedObject(graphView);
            serializedObject.FindProperty("_background").objectReferenceValue = background;
            serializedObject.FindProperty("_graph").objectReferenceValue = graphGraphic;
            serializedObject.FindProperty("_titleText").objectReferenceValue = titleText;
            serializedObject.FindProperty("_currentValuesRoot").objectReferenceValue = valuesRoot;
            serializedObject.FindProperty("_currentValueRowTemplate").objectReferenceValue = valueRowTemplate;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            return root;
        }

        private static Image CreateProfilerGraphBackground(Transform parent)
        {
            var backgroundRoot = new GameObject(
                "Background",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));
            backgroundRoot.transform.SetParent(parent, false);
            backgroundRoot.transform.localScale = Vector3.one;
            Stretch(backgroundRoot.GetComponent<RectTransform>());

            var background = backgroundRoot.GetComponent<Image>();
            background.color = Color.clear;
            background.raycastTarget = false;
            background.gameObject.SetActive(false);
            return background;
        }

        private static global::SMCheats.CheatsGraphGraphicView CreateProfilerGraphGraphic(Transform parent)
        {
            var graphRoot = new GameObject(
                "Graph",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(global::SMCheats.CheatsGraphGraphicView));
            graphRoot.transform.SetParent(parent, false);
            graphRoot.transform.localScale = Vector3.one;
            Stretch(graphRoot.GetComponent<RectTransform>());

            var graphGraphic = graphRoot.GetComponent<global::SMCheats.CheatsGraphGraphicView>();
            graphGraphic.raycastTarget = true;
            return graphGraphic;
        }

        private static GameObject CreateProfilerGraphValueRow(Transform parent, Font runtimeFont)
        {
            var row = new GameObject("ValueRow", typeof(RectTransform), typeof(LayoutElement));
            row.transform.SetParent(parent, false);
            row.transform.localScale = Vector3.one;
            var layout = row.GetComponent<LayoutElement>();
            layout.minWidth = 50f;
            layout.preferredWidth = 100f;

            var label = CreateText("Label", row.transform, "Value", runtimeFont, 15, TextAnchor.MiddleCenter);
            var labelRect = (RectTransform)label.transform;
            labelRect.anchorMin = new Vector2(0f, 1f);
            labelRect.anchorMax = new Vector2(1f, 1f);
            labelRect.pivot = new Vector2(0.5f, 1f);
            labelRect.anchoredPosition = Vector2.zero;
            labelRect.sizeDelta = new Vector2(0f, 27f);

            var value = CreateText("Value", row.transform, string.Empty, runtimeFont, 15, TextAnchor.MiddleCenter);
            var valueRect = (RectTransform)value.transform;
            valueRect.anchorMin = new Vector2(0f, 0f);
            valueRect.anchorMax = new Vector2(1f, 0f);
            valueRect.pivot = new Vector2(0.5f, 0f);
            valueRect.anchoredPosition = Vector2.zero;
            valueRect.sizeDelta = new Vector2(0f, 50f);
            value.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            return row;
        }

        private static GameObject BuildOptionControlPrefabRoot(OptionControlPrefabDefinition definition, Font runtimeFont)
        {
            switch (definition.FileName)
            {
                case "ActionOption":
                    return BuildActionOptionControlPrefab(runtimeFont);
                case "ToggleOption":
                    return BuildToggleOptionControlPrefab(runtimeFont);
                case "DropdownOption":
                    return BuildDropdownOptionControlPrefab(runtimeFont);
                case "SliderOption":
                    return BuildSliderOptionControlPrefab(runtimeFont);
                case "SelectorOption":
                    return BuildSelectorOptionControlPrefab(runtimeFont);
                case "InputOption":
                    return BuildInputOptionControlPrefab(runtimeFont);
                case "ValueOption":
                    return BuildValueOptionControlPrefab(runtimeFont);
                default:
                    throw new ArgumentOutOfRangeException(nameof(definition), definition.FileName, "Unknown option control prefab.");
            }
        }

        private static GameObject BuildActionOptionControlPrefab(Font runtimeFont)
        {
            var root = CreateOptionControlRoot<global::SMCheats.CheatsActionOptionView>("ActionOption", runtimeFont, out var view, out _, out var controlRoot, out _, out _);
            var actionButton = CreateButton("Run", controlRoot, runtimeFont, out var actionButtonLabel);

            var serializedObject = new SerializedObject(view);
            serializedObject.FindProperty("_actionButton").objectReferenceValue = actionButton;
            serializedObject.FindProperty("_actionButtonLabel").objectReferenceValue = actionButtonLabel;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return root;
        }

        private static GameObject BuildToggleOptionControlPrefab(Font runtimeFont)
        {
            var root = CreateOptionControlRoot<global::SMCheats.CheatsToggleOptionView>("ToggleOption", runtimeFont, out var view, out _, out var controlRoot, out _, out _);
            var toggleButton = CreateButton("Off", controlRoot, runtimeFont, out var toggleButtonLabel);

            var serializedObject = new SerializedObject(view);
            serializedObject.FindProperty("_toggleButton").objectReferenceValue = toggleButton;
            serializedObject.FindProperty("_toggleButtonLabel").objectReferenceValue = toggleButtonLabel;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return root;
        }

        private static GameObject BuildDropdownOptionControlPrefab(Font runtimeFont)
        {
            var root = CreateOptionControlRoot<global::SMCheats.CheatsDropdownOptionView>("DropdownOption", runtimeFont, out var view, out _, out var controlRoot, out _, out _);
            var dropdown = CreateDropdown(controlRoot, runtimeFont);

            var serializedObject = new SerializedObject(view);
            serializedObject.FindProperty("_dropdown").objectReferenceValue = dropdown;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return root;
        }

        private static GameObject BuildSliderOptionControlPrefab(Font runtimeFont)
        {
            var root = CreateOptionControlRoot<global::SMCheats.CheatsSliderOptionView>("SliderOption", runtimeFont, out var view, out _, out var controlRoot, out _, out _);
            var slider = CreateSlider(controlRoot, runtimeFont, out var valueLabel);

            var serializedObject = new SerializedObject(view);
            serializedObject.FindProperty("_slider").objectReferenceValue = slider;
            serializedObject.FindProperty("_valueLabel").objectReferenceValue = valueLabel;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return root;
        }

        private static GameObject BuildSelectorOptionControlPrefab(Font runtimeFont)
        {
            var root = CreateOptionControlRoot<global::SMCheats.CheatsSelectorOptionView>("SelectorOption", runtimeFont, out var view, out _, out var controlRoot, out _, out _);
            var previousButton = CreateButton("-", controlRoot, runtimeFont, out _);
            previousButton.GetComponent<LayoutElement>().preferredWidth = 44f;
            var valueLabel = CreateText("Value", controlRoot, string.Empty, runtimeFont, 15, TextAnchor.MiddleCenter);
            var valueLayout = valueLabel.gameObject.AddComponent<LayoutElement>();
            valueLayout.preferredWidth = 96f;
            valueLayout.flexibleWidth = 1f;
            var nextButton = CreateButton("+", controlRoot, runtimeFont, out _);
            nextButton.GetComponent<LayoutElement>().preferredWidth = 44f;

            var serializedObject = new SerializedObject(view);
            serializedObject.FindProperty("_previousButton").objectReferenceValue = previousButton;
            serializedObject.FindProperty("_nextButton").objectReferenceValue = nextButton;
            serializedObject.FindProperty("_valueLabel").objectReferenceValue = valueLabel;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return root;
        }

        private static GameObject BuildInputOptionControlPrefab(Font runtimeFont)
        {
            var root = CreateOptionControlRoot<global::SMCheats.CheatsInputOptionView>("InputOption", runtimeFont, out var view, out _, out var controlRoot, out _, out _);
            var inputField = CreateInputField(controlRoot, string.Empty, runtimeFont);
            var layoutElement = inputField.gameObject.GetComponent<LayoutElement>();
            layoutElement.preferredWidth = 160f;
            layoutElement.flexibleWidth = 1f;

            var serializedObject = new SerializedObject(view);
            serializedObject.FindProperty("_inputField").objectReferenceValue = inputField;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return root;
        }

        private static GameObject BuildValueOptionControlPrefab(Font runtimeFont)
        {
            var root = CreateOptionControlRoot<global::SMCheats.CheatsValueOptionView>("ValueOption", runtimeFont, out var view, out _, out var controlRoot, out _, out _);
            var valueText = CreateText("Value", controlRoot, string.Empty, runtimeFont, 15, TextAnchor.MiddleLeft);
            valueText.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            var serializedObject = new SerializedObject(view);
            serializedObject.FindProperty("_valueText").objectReferenceValue = valueText;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return root;
        }

        private static GameObject CreateOptionControlRoot<TView>(
            string name,
            Font runtimeFont,
            out TView view,
            out Text titleText,
            out RectTransform controlRoot,
            out Image backgroundImage,
            out Button selectionButton)
            where TView : global::SMCheats.CheatsOptionControlView
        {
            var root = new GameObject(
                name,
                typeof(RectTransform),
                typeof(Image),
                typeof(Button),
                typeof(HorizontalLayoutGroup),
                typeof(LayoutElement),
                typeof(TView));

            root.transform.localScale = Vector3.one;
            backgroundImage = root.GetComponent<Image>();
            backgroundImage.color = new Color(0f, 0f, 0f, 0.16f);
            selectionButton = root.GetComponent<Button>();
            selectionButton.targetGraphic = backgroundImage;

            var layoutGroup = root.GetComponent<HorizontalLayoutGroup>();
            layoutGroup.spacing = 8f;
            layoutGroup.padding = new RectOffset(8, 8, 8, 8);
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = false;

            var layoutElement = root.GetComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1f;
            layoutElement.minHeight = 56f;

            view = root.GetComponent<TView>();
            titleText = CreateText("Label", root.transform, "Option", runtimeFont, 15, TextAnchor.MiddleLeft);
            titleText.gameObject.AddComponent<LayoutElement>().preferredWidth = 220f;

            controlRoot = CreateLayoutRoot("ControlRoot", root.transform, false, 8f, new RectOffset(0, 0, 0, 0));
            var controlLayout = controlRoot.gameObject.AddComponent<LayoutElement>();
            controlLayout.flexibleWidth = 1f;
            controlLayout.minWidth = 120f;

            AssignOptionControlBaseReferences(view, titleText, backgroundImage, selectionButton);
            return root;
        }

        private static void AssignOptionControlBaseReferences(global::SMCheats.CheatsOptionControlView view, Text titleText, Image backgroundImage, Button selectionButton)
        {
            var serializedObject = new SerializedObject(view);
            serializedObject.FindProperty("_titleText").objectReferenceValue = titleText;
            serializedObject.FindProperty("_backgroundImage").objectReferenceValue = backgroundImage;
            serializedObject.FindProperty("_selectionButton").objectReferenceValue = selectionButton;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static global::SMCheats.CheatsConsoleEntryView CreateConsoleEntryTemplate(Transform parent, Font runtimeFont)
        {
            var root = new GameObject(
                "ConsoleEntryTemplate",
                typeof(RectTransform),
                typeof(Image),
                typeof(VerticalLayoutGroup),
                typeof(LayoutElement),
                typeof(global::SMCheats.CheatsConsoleEntryView));

            root.transform.SetParent(parent, false);
            root.transform.localScale = Vector3.one;
            root.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.18f);
            var backgroundButton = root.AddComponent<Button>();
            backgroundButton.transition = Selectable.Transition.ColorTint;
            var layout = root.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.padding = new RectOffset(8, 8, 8, 8);
            layout.childControlHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            var typeText = CreateText("Type", root.transform, string.Empty, runtimeFont, 15, TextAnchor.MiddleLeft);
            var messageText = CreateText("Message", root.transform, string.Empty, runtimeFont, 15, TextAnchor.MiddleLeft);
            var stackText = CreateText("Stack", root.transform, string.Empty, runtimeFont, 13, TextAnchor.UpperLeft);
            var selectButton = CreateButton("Select", root.transform, runtimeFont, out var selectButtonLabel);

            var view = root.GetComponent<global::SMCheats.CheatsConsoleEntryView>();
            var serializedObject = new SerializedObject(view);
            serializedObject.FindProperty("_background").objectReferenceValue = root.GetComponent<Image>();
            serializedObject.FindProperty("_typeText").objectReferenceValue = typeText;
            serializedObject.FindProperty("_messageText").objectReferenceValue = messageText;
            serializedObject.FindProperty("_stackText").objectReferenceValue = stackText;
            serializedObject.FindProperty("_backgroundButton").objectReferenceValue = backgroundButton;
            serializedObject.FindProperty("_selectButton").objectReferenceValue = selectButton;
            serializedObject.FindProperty("_selectButtonLabel").objectReferenceValue = selectButtonLabel;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return view;
        }

        private static global::SMCheats.CheatsInfoEntryView CreateInfoEntryTemplate(Transform parent, Font runtimeFont)
        {
            var root = new GameObject(
                "InfoEntryTemplate",
                typeof(RectTransform),
                typeof(Image),
                typeof(VerticalLayoutGroup),
                typeof(LayoutElement),
                typeof(global::SMCheats.CheatsInfoEntryView));

            root.transform.SetParent(parent, false);
            root.transform.localScale = Vector3.one;
            root.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.16f);
            var layout = root.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.padding = new RectOffset(8, 8, 8, 8);
            layout.childControlHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            var titleText = CreateText("Title", root.transform, string.Empty, runtimeFont, 15, TextAnchor.MiddleLeft);
            var bodyText = CreateText("Body", root.transform, string.Empty, runtimeFont, 14, TextAnchor.MiddleLeft);

            var view = root.GetComponent<global::SMCheats.CheatsInfoEntryView>();
            var serializedObject = new SerializedObject(view);
            serializedObject.FindProperty("_titleText").objectReferenceValue = titleText;
            serializedObject.FindProperty("_bodyText").objectReferenceValue = bodyText;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return view;
        }

        private static Text CreateSectionHeader(Transform parent, string title, Font runtimeFont)
        {
            var headerRoot = new GameObject(title + "Header", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            headerRoot.transform.SetParent(parent, false);
            headerRoot.transform.localScale = Vector3.one;
            headerRoot.GetComponent<Image>().color = new Color(0.16f, 0.18f, 0.22f, 0.92f);
            headerRoot.GetComponent<LayoutElement>().preferredHeight = 28f;
            var header = CreateText("Label", headerRoot.transform, title, runtimeFont, 16, TextAnchor.MiddleLeft);
            Stretch((RectTransform)header.transform, 10f);
            return header;
        }

        private static Text CreateValueDisplayRow(Transform parent, string labelText, Font runtimeFont)
        {
            var row = CreateLayoutRoot(labelText + "Row", parent, false, 8f, new RectOffset(8, 8, 8, 8));
            row.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.16f);
            var label = CreateText("Label", row, labelText, runtimeFont, 15, TextAnchor.MiddleLeft);
            label.gameObject.AddComponent<LayoutElement>().preferredWidth = 220f;
            var value = CreateText("Value", row, string.Empty, runtimeFont, 15, TextAnchor.MiddleLeft);
            value.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            return value;
        }

        private static InputField CreateNumericInputRow(Transform parent, string labelText, Font runtimeFont, out Button decreaseButton, out Button increaseButton)
        {
            var row = CreateLayoutRoot(labelText + "Row", parent, false, 8f, new RectOffset(8, 8, 8, 8));
            row.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.16f);
            var label = CreateText("Label", row, labelText, runtimeFont, 15, TextAnchor.MiddleLeft);
            label.gameObject.AddComponent<LayoutElement>().preferredWidth = 220f;
            decreaseButton = CreateButton("-", row, runtimeFont, out _);
            var input = CreateInputField(row, string.Empty, runtimeFont);
            input.gameObject.GetComponent<LayoutElement>().preferredWidth = 90f;
            increaseButton = CreateButton("+", row, runtimeFont, out _);
            return input;
        }

        private static Button CreateToggleRow(Transform parent, string labelText, Font runtimeFont, out Text buttonLabel)
        {
            var row = CreateLayoutRoot(labelText + "Row", parent, false, 8f, new RectOffset(8, 8, 8, 8));
            row.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.16f);
            var label = CreateText("Label", row, labelText, runtimeFont, 15, TextAnchor.MiddleLeft);
            label.gameObject.AddComponent<LayoutElement>().preferredWidth = 220f;
            return CreateButton("Off", row, runtimeFont, out buttonLabel);
        }

        private static InputField CreateInputField(Transform parent, string initialText, Font runtimeFont)
        {
            var root = new GameObject("InputField", typeof(RectTransform), typeof(Image), typeof(InputField), typeof(LayoutElement));
            root.transform.SetParent(parent, false);
            root.transform.localScale = Vector3.one;
            root.GetComponent<Image>().color = new Color(0.14f, 0.15f, 0.18f, 1f);
            root.GetComponent<LayoutElement>().preferredHeight = 40f;
            root.GetComponent<LayoutElement>().minWidth = 72f;

            var text = CreateText("Text", root.transform, initialText, runtimeFont, 16, TextAnchor.MiddleLeft);
            var placeholder = CreateText("Placeholder", root.transform, string.Empty, runtimeFont, 16, TextAnchor.MiddleLeft);
            placeholder.color = new Color(1f, 1f, 1f, 0.45f);
            Stretch((RectTransform)text.transform, 8f);
            Stretch((RectTransform)placeholder.transform, 8f);

            var inputField = root.GetComponent<InputField>();
            inputField.textComponent = text;
            inputField.placeholder = placeholder;
            inputField.text = initialText;
            return inputField;
        }

        private static Dropdown CreateDropdown(Transform parent, Font runtimeFont)
        {
            var root = new GameObject("Dropdown", typeof(RectTransform), typeof(Image), typeof(Dropdown), typeof(LayoutElement));
            root.transform.SetParent(parent, false);
            root.transform.localScale = Vector3.one;
            root.GetComponent<Image>().color = new Color(0.14f, 0.15f, 0.18f, 1f);
            var layoutElement = root.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = 40f;
            layoutElement.preferredWidth = 180f;
            layoutElement.flexibleWidth = 1f;

            var captionText = CreateText("Label", root.transform, string.Empty, runtimeFont, 15, TextAnchor.MiddleLeft);
            Stretch((RectTransform)captionText.transform, 10f);
            ((RectTransform)captionText.transform).offsetMax = new Vector2(-28f, -10f);

            var arrow = CreateText("Arrow", root.transform, "▼", runtimeFont, 12, TextAnchor.MiddleCenter);
            var arrowRect = (RectTransform)arrow.transform;
            arrowRect.anchorMin = new Vector2(1f, 0f);
            arrowRect.anchorMax = new Vector2(1f, 1f);
            arrowRect.pivot = new Vector2(1f, 0.5f);
            arrowRect.sizeDelta = new Vector2(24f, 0f);
            arrowRect.anchoredPosition = new Vector2(-8f, 0f);

            var template = new GameObject("Template", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
            template.transform.SetParent(root.transform, false);
            template.transform.localScale = Vector3.one;
            template.SetActive(false);
            var templateRect = template.GetComponent<RectTransform>();
            templateRect.anchorMin = new Vector2(0f, 0f);
            templateRect.anchorMax = new Vector2(1f, 0f);
            templateRect.pivot = new Vector2(0.5f, 1f);
            templateRect.anchoredPosition = new Vector2(0f, -2f);
            templateRect.sizeDelta = new Vector2(0f, 180f);
            template.GetComponent<Image>().color = new Color(0.14f, 0.15f, 0.18f, 1f);

            var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
            viewport.transform.SetParent(template.transform, false);
            viewport.transform.localScale = Vector3.one;
            Stretch(viewport.GetComponent<RectTransform>());
            viewport.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.05f);
            viewport.GetComponent<Mask>().showMaskGraphic = false;

            var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            content.transform.SetParent(viewport.transform, false);
            content.transform.localScale = Vector3.one;
            var contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = Vector2.zero;
            var contentLayout = content.GetComponent<VerticalLayoutGroup>();
            contentLayout.childControlHeight = false;
            contentLayout.childControlWidth = true;
            contentLayout.childForceExpandHeight = false;
            contentLayout.childForceExpandWidth = true;
            contentLayout.spacing = 2f;
            content.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var item = new GameObject("Item", typeof(RectTransform), typeof(Image), typeof(Toggle), typeof(LayoutElement));
            item.transform.SetParent(content.transform, false);
            item.transform.localScale = Vector3.one;
            item.GetComponent<Image>().color = new Color(0.18f, 0.2f, 0.24f, 1f);
            item.GetComponent<LayoutElement>().preferredHeight = 32f;
            var itemLabel = CreateText("Item Label", item.transform, "Option", runtimeFont, 15, TextAnchor.MiddleLeft);
            Stretch((RectTransform)itemLabel.transform, 10f);

            var toggle = item.GetComponent<Toggle>();
            toggle.targetGraphic = item.GetComponent<Image>();
            toggle.graphic = null;

            var dropdown = root.GetComponent<Dropdown>();
            dropdown.targetGraphic = root.GetComponent<Image>();
            dropdown.template = templateRect;
            dropdown.captionText = captionText;
            dropdown.itemText = itemLabel;
            dropdown.options = new System.Collections.Generic.List<Dropdown.OptionData>();

            var scrollRect = template.GetComponent<ScrollRect>();
            scrollRect.content = contentRect;
            scrollRect.viewport = viewport.GetComponent<RectTransform>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            return dropdown;
        }

        private static Slider CreateSlider(Transform parent, Font runtimeFont, out Text valueLabel)
        {
            var root = CreateLayoutRoot("SliderRoot", parent, false, 8f, new RectOffset(0, 0, 0, 0));
            root.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            var sliderObject = new GameObject("Slider", typeof(RectTransform), typeof(Slider), typeof(LayoutElement));
            sliderObject.transform.SetParent(root, false);
            sliderObject.transform.localScale = Vector3.one;
            var sliderLayout = sliderObject.GetComponent<LayoutElement>();
            sliderLayout.flexibleWidth = 1f;
            sliderLayout.preferredWidth = 160f;

            var background = new GameObject("Background", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(sliderObject.transform, false);
            background.transform.localScale = Vector3.one;
            Stretch(background.GetComponent<RectTransform>(), 0f);
            background.GetComponent<Image>().color = new Color(0.16f, 0.17f, 0.2f, 1f);

            var fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderObject.transform, false);
            fillArea.transform.localScale = Vector3.one;
            var fillAreaRect = fillArea.GetComponent<RectTransform>();
            Stretch(fillAreaRect, 10f);

            var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(fillArea.transform, false);
            fill.transform.localScale = Vector3.one;
            fill.GetComponent<Image>().color = new Color(0.26f, 0.48f, 0.72f, 1f);

            var handleSlideArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleSlideArea.transform.SetParent(sliderObject.transform, false);
            handleSlideArea.transform.localScale = Vector3.one;
            Stretch(handleSlideArea.GetComponent<RectTransform>(), 0f);

            var handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handle.transform.SetParent(handleSlideArea.transform, false);
            handle.transform.localScale = Vector3.one;
            var handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(18f, 18f);
            handle.GetComponent<Image>().color = Color.white;

            var slider = sliderObject.GetComponent<Slider>();
            slider.targetGraphic = handle.GetComponent<Image>();
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.handleRect = handleRect;
            slider.direction = Slider.Direction.LeftToRight;

            valueLabel = CreateText("Value", root, string.Empty, runtimeFont, 15, TextAnchor.MiddleRight);
            valueLabel.gameObject.AddComponent<LayoutElement>().preferredWidth = 72f;
            return slider;
        }

        private static void BuildMainHeader(
            RectTransform parent,
            Font runtimeFont,
            out Text headerTitle,
            out RectTransform headerActionsRoot,
            out Button closeButton)
        {
            headerTitle = CreateText("Title", parent, "Simple mobile cheats", runtimeFont, 20, TextAnchor.MiddleLeft);
            var titleRect = (RectTransform)headerTitle.transform;
            titleRect.anchorMin = new Vector2(0f, 0f);
            titleRect.anchorMax = new Vector2(0.72f, 1f);
            titleRect.pivot = new Vector2(0f, 0.5f);
            titleRect.anchoredPosition = new Vector2(12f, 0f);
            titleRect.sizeDelta = new Vector2(-24f, 0f);

            headerActionsRoot = CreateLayoutRoot("HeaderActions", parent, false, 8f, new RectOffset(0, 0, 0, 0));
            headerActionsRoot.anchorMin = new Vector2(1f, 0f);
            headerActionsRoot.anchorMax = new Vector2(1f, 1f);
            headerActionsRoot.pivot = new Vector2(1f, 0.5f);
            headerActionsRoot.anchoredPosition = new Vector2(-8f, 0f);
            headerActionsRoot.sizeDelta = Vector2.zero;
            closeButton = CreateButton("Close", headerActionsRoot, runtimeFont, out _);
        }

        private static void BuildPinPrompt(
            RectTransform parent,
            Font runtimeFont,
            out Text pinPromptMaskedText,
            out Text pinPromptStatusLabel,
            out Button pinPromptSubmitButton,
            out Button pinPromptCancelButton,
            out Button[] pinDigitButtons)
        {
            var promptTitle = CreateText("PromptTitle", parent, "Enter pin code", runtimeFont, 22, TextAnchor.MiddleCenter);
            var promptTitleRect = (RectTransform)promptTitle.transform;
            promptTitleRect.anchorMin = new Vector2(0f, 1f);
            promptTitleRect.anchorMax = new Vector2(1f, 1f);
            promptTitleRect.pivot = new Vector2(0.5f, 1f);
            promptTitleRect.anchoredPosition = new Vector2(0f, -20f);
            promptTitleRect.sizeDelta = new Vector2(-32f, 28f);

            pinPromptMaskedText = CreateText("MaskedPin", parent, "____________", runtimeFont, 20, TextAnchor.MiddleCenter);
            var maskedRect = (RectTransform)pinPromptMaskedText.transform;
            maskedRect.anchorMin = new Vector2(0f, 1f);
            maskedRect.anchorMax = new Vector2(1f, 1f);
            maskedRect.pivot = new Vector2(0.5f, 1f);
            maskedRect.anchoredPosition = new Vector2(0f, -60f);
            maskedRect.sizeDelta = new Vector2(-32f, 24f);

            pinPromptStatusLabel = CreateText("Status", parent, string.Empty, runtimeFont, 16, TextAnchor.MiddleCenter);
            var statusRect = (RectTransform)pinPromptStatusLabel.transform;
            statusRect.anchorMin = new Vector2(0f, 1f);
            statusRect.anchorMax = new Vector2(1f, 1f);
            statusRect.pivot = new Vector2(0.5f, 1f);
            statusRect.anchoredPosition = new Vector2(0f, -90f);
            statusRect.sizeDelta = new Vector2(-32f, 22f);

            var pinButtons = new System.Collections.Generic.List<Button>();
            var keypadRoot = CreateLayoutRoot("Keypad", parent, true, 8f, new RectOffset(0, 0, 0, 0));
            keypadRoot.anchorMin = new Vector2(0f, 1f);
            keypadRoot.anchorMax = new Vector2(1f, 1f);
            keypadRoot.pivot = new Vector2(0.5f, 1f);
            keypadRoot.anchoredPosition = new Vector2(0f, -124f);
            keypadRoot.sizeDelta = new Vector2(-32f, 232f);
            var keypadRows = new[]
            {
                new[] { "1", "2", "3" },
                new[] { "4", "5", "6" },
                new[] { "7", "8", "9" },
                new[] { "C", "0", "<" }
            };

            foreach (var row in keypadRows)
            {
                var rowRoot = CreateLayoutRoot("Row", keypadRoot, false, 8f, new RectOffset(0, 0, 0, 0));
                foreach (var key in row)
                {
                    var keyButton = CreateButton(key, rowRoot, runtimeFont, out _);
                    keyButton.gameObject.GetComponent<LayoutElement>().preferredHeight = 52f;
                    pinButtons.Add(keyButton);
                }
            }

            var footer = CreateLayoutRoot("Footer", parent, false, 8f, new RectOffset(0, 0, 0, 0));
            footer.anchorMin = new Vector2(0f, 0f);
            footer.anchorMax = new Vector2(1f, 0f);
            footer.pivot = new Vector2(0.5f, 0f);
            footer.anchoredPosition = new Vector2(0f, 16f);
            footer.sizeDelta = new Vector2(-32f, 40f);
            pinPromptSubmitButton = CreateButton("Open", footer, runtimeFont, out _);
            pinPromptCancelButton = CreateButton("Cancel", footer, runtimeFont, out _);
            pinDigitButtons = pinButtons.ToArray();
        }

        private static void AssignRuntimeViewReferences(
            global::SMCheats.CheatsView runtimeView,
            Canvas canvas,
            CanvasScaler canvasScaler,
            GraphicRaycaster graphicRaycaster,
            RectTransform root,
            RectTransform cheatsScreen,
            RectTransform tabsRoot,
            RectTransform mainContentRoot,
            RectTransform pinnedScreen,
            RectTransform headerActionsRoot,
            Text headerTitle,
            Button closeButton)
        {
            var serializedObject = new SerializedObject(runtimeView);
            serializedObject.FindProperty("_canvas").objectReferenceValue = canvas;
            serializedObject.FindProperty("_canvasScaler").objectReferenceValue = canvasScaler;
            serializedObject.FindProperty("_graphicRaycaster").objectReferenceValue = graphicRaycaster;
            serializedObject.FindProperty("_root").objectReferenceValue = root;
            serializedObject.FindProperty("_cheatsScreen").objectReferenceValue = cheatsScreen;
            serializedObject.FindProperty("_tabsRoot").objectReferenceValue = tabsRoot;
            serializedObject.FindProperty("_mainContentRoot").objectReferenceValue = mainContentRoot;
            serializedObject.FindProperty("_pinnedScreen").objectReferenceValue = pinnedScreen;
            serializedObject.FindProperty("_headerActionsRoot").objectReferenceValue = headerActionsRoot;
            serializedObject.FindProperty("_headerTitle").objectReferenceValue = headerTitle;
            serializedObject.FindProperty("_closeButton").objectReferenceValue = closeButton;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void AssignPinPromptViewReferences(
            global::SMCheats.CheatsPinPromptView pinPromptView,
            Canvas canvas,
            CanvasScaler canvasScaler,
            GraphicRaycaster graphicRaycaster,
            RectTransform root,
            Text statusLabel,
            Text maskedText,
            Button submitButton,
            Button cancelButton,
            Button[] digitButtons)
        {
            var serializedObject = new SerializedObject(pinPromptView);
            serializedObject.FindProperty("_canvas").objectReferenceValue = canvas;
            serializedObject.FindProperty("_canvasScaler").objectReferenceValue = canvasScaler;
            serializedObject.FindProperty("_graphicRaycaster").objectReferenceValue = graphicRaycaster;
            serializedObject.FindProperty("_root").objectReferenceValue = root;
            serializedObject.FindProperty("_statusLabel").objectReferenceValue = statusLabel;
            serializedObject.FindProperty("_maskedText").objectReferenceValue = maskedText;
            serializedObject.FindProperty("_submitButton").objectReferenceValue = submitButton;
            serializedObject.FindProperty("_cancelButton").objectReferenceValue = cancelButton;

            var digitButtonsProperty = serializedObject.FindProperty("_digitButtons");
            digitButtonsProperty.arraySize = digitButtons.Length;
            for (var i = 0; i < digitButtons.Length; i++)
            {
                digitButtonsProperty.GetArrayElementAtIndex(i).objectReferenceValue = digitButtons[i];
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void AssignActivationViewReferences(
            global::SMCheats.CheatsActivationView activationView,
            Canvas canvas,
            CanvasScaler canvasScaler,
            GraphicRaycaster graphicRaycaster,
            Button triggerButton)
        {
            var serializedObject = new SerializedObject(activationView);
            serializedObject.FindProperty("_canvas").objectReferenceValue = canvas;
            serializedObject.FindProperty("_canvasScaler").objectReferenceValue = canvasScaler;
            serializedObject.FindProperty("_graphicRaycaster").objectReferenceValue = graphicRaycaster;
            serializedObject.FindProperty("_triggerButton").objectReferenceValue = triggerButton;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static RectTransform CreateCanvasElementRoot(string name, Transform parent)
        {
            var root = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
            root.transform.SetParent(parent, false);
            root.transform.localScale = Vector3.one;
            return root.GetComponent<RectTransform>();
        }

        private static RectTransform CreateLayoutRoot(string name, Transform parent, bool vertical, float spacing, RectOffset padding)
        {
            var root = new GameObject(name, typeof(RectTransform));
            root.transform.SetParent(parent, false);
            root.transform.localScale = Vector3.one;
            var rect = root.GetComponent<RectTransform>();

            if (vertical)
            {
                AddVerticalLayout(root, spacing, padding);
            }
            else
            {
                AddHorizontalLayout(root, spacing, padding);
            }

            return rect;
        }

        private static RectTransform CreateRectRoot(string name, Transform parent)
        {
            var root = new GameObject(name, typeof(RectTransform));
            root.transform.SetParent(parent, false);
            root.transform.localScale = Vector3.one;
            return root.GetComponent<RectTransform>();
        }

        private static void AddVerticalLayout(GameObject target, float spacing, RectOffset padding)
        {
            var layout = target.AddComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = padding;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            target.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        private static void AddHorizontalLayout(GameObject target, float spacing, RectOffset padding)
        {
            var layout = target.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = padding;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;
            target.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        private static RectTransform CreatePanel(string name, Transform parent, Color color)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            panel.transform.localScale = Vector3.one;
            var image = panel.GetComponent<Image>();
            image.color = color;
            return panel.GetComponent<RectTransform>();
        }

        private static Button CreateButton(string text, Transform parent, Font runtimeFont, out Text label)
        {
            var buttonRoot = new GameObject(text + "Button", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            buttonRoot.transform.SetParent(parent, false);
            buttonRoot.transform.localScale = Vector3.one;
            buttonRoot.GetComponent<Image>().color = new Color(0.18f, 0.2f, 0.24f, 1f);
            var layoutElement = buttonRoot.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = 40f;
            layoutElement.minWidth = 64f;

            label = CreateText("Label", buttonRoot.transform, text, runtimeFont, 16, TextAnchor.MiddleCenter);
            Stretch((RectTransform)label.transform, 8f);
            return buttonRoot.GetComponent<Button>();
        }

        private static GameObject BuildButtonPrefabRoot(string name, Font runtimeFont)
        {
            var root = new GameObject(
                name,
                typeof(RectTransform),
                typeof(Image),
                typeof(Button),
                typeof(LayoutElement),
                typeof(global::SMCheats.CheatsButtonView));

            root.transform.localScale = Vector3.one;
            var rectTransform = root.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(160f, 40f);

            var background = root.GetComponent<Image>();
            background.color = new Color(0.18f, 0.2f, 0.24f, 1f);

            var layout = root.GetComponent<LayoutElement>();
            layout.preferredHeight = 40f;
            layout.minWidth = 64f;

            var iconRoot = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            iconRoot.transform.SetParent(root.transform, false);
            iconRoot.transform.localScale = Vector3.one;
            var iconRect = iconRoot.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0f, 0.5f);
            iconRect.anchorMax = new Vector2(0f, 0.5f);
            iconRect.pivot = new Vector2(0f, 0.5f);
            iconRect.anchoredPosition = new Vector2(8f, 0f);
            iconRect.sizeDelta = new Vector2(20f, 20f);
            var iconImage = iconRoot.GetComponent<Image>();
            iconImage.color = Color.white;
            iconRoot.SetActive(false);

            var label = CreateText("Label", root.transform, name, runtimeFont, 16, TextAnchor.MiddleCenter);
            Stretch((RectTransform)label.transform, 8f);

            var serializedObject = new SerializedObject(root.GetComponent<global::SMCheats.CheatsButtonView>());
            serializedObject.FindProperty("_button").objectReferenceValue = root.GetComponent<Button>();
            serializedObject.FindProperty("_background").objectReferenceValue = background;
            serializedObject.FindProperty("_label").objectReferenceValue = label;
            serializedObject.FindProperty("_icon").objectReferenceValue = iconImage;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            return root;
        }

        private static GameObject BuildToggleButtonPrefabRoot()
        {
            var root = new GameObject(
                "ToggleButton",
                typeof(RectTransform),
                typeof(Image),
                typeof(Button),
                typeof(LayoutElement),
                typeof(global::SMCheats.CheatsToggleButtonView));

            root.transform.localScale = Vector3.one;
            var rectTransform = root.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(40f, 40f);

            var background = root.GetComponent<Image>();
            background.color = new Color(0.18f, 0.2f, 0.24f, 1f);

            var layout = root.GetComponent<LayoutElement>();
            layout.preferredHeight = 40f;
            layout.minWidth = 40f;
            layout.preferredWidth = 40f;

            var iconRoot = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            iconRoot.transform.SetParent(root.transform, false);
            iconRoot.transform.localScale = Vector3.one;
            var iconRect = iconRoot.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.5f, 0.5f);
            iconRect.anchorMax = new Vector2(0.5f, 0.5f);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.sizeDelta = new Vector2(20f, 20f);

            var iconImage = iconRoot.GetComponent<Image>();
            iconImage.color = Color.white;
            iconRoot.SetActive(false);

            var view = root.GetComponent<global::SMCheats.CheatsToggleButtonView>();
            var serializedObject = new SerializedObject(view);
            serializedObject.FindProperty("_button").objectReferenceValue = root.GetComponent<Button>();
            serializedObject.FindProperty("_background").objectReferenceValue = background;
            serializedObject.FindProperty("_label").objectReferenceValue = null;
            serializedObject.FindProperty("_icon").objectReferenceValue = iconImage;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            return root;
        }

        private static Text CreateText(string name, Transform parent, string text, Font runtimeFont, int fontSize, TextAnchor alignment)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            textObject.transform.localScale = Vector3.one;
            var uiText = textObject.GetComponent<Text>();
            uiText.font = runtimeFont;
            uiText.fontSize = fontSize;
            uiText.alignment = alignment;
            uiText.color = Color.white;
            uiText.horizontalOverflow = HorizontalWrapMode.Wrap;
            uiText.verticalOverflow = VerticalWrapMode.Overflow;
            uiText.text = text;
            return uiText;
        }

        private static void CreateRuntimeScrollArea(Transform parent, out ScrollRect scrollRect, out RectTransform content, bool vertical)
        {
            var root = new GameObject("ScrollView", typeof(RectTransform), typeof(LayoutElement));
            root.transform.SetParent(parent, false);
            root.transform.localScale = Vector3.one;
            var rootRect = root.GetComponent<RectTransform>();
            root.GetComponent<LayoutElement>().flexibleHeight = 1f;

            var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
            viewport.transform.SetParent(root.transform, false);
            viewport.transform.localScale = Vector3.one;
            var viewportRect = viewport.GetComponent<RectTransform>();
            Stretch(viewportRect);
            viewport.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.12f);
            viewport.GetComponent<Mask>().showMaskGraphic = false;

            var contentObject = new GameObject("Content", typeof(RectTransform));
            contentObject.transform.SetParent(viewport.transform, false);
            contentObject.transform.localScale = Vector3.one;
            content = contentObject.GetComponent<RectTransform>();
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = Vector2.zero;

            if (vertical)
            {
                var layout = contentObject.AddComponent<VerticalLayoutGroup>();
                layout.spacing = 8f;
                layout.padding = new RectOffset(0, 0, 0, 0);
                layout.childAlignment = TextAnchor.UpperLeft;
                layout.childControlHeight = false;
                layout.childControlWidth = true;
                layout.childForceExpandHeight = false;
                layout.childForceExpandWidth = true;
                contentObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }

            scrollRect = root.AddComponent<ScrollRect>();
            scrollRect.viewport = viewportRect;
            scrollRect.content = content;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
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
