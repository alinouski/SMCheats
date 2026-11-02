using System;
using System.Collections.Generic;
using UnityEngine;

namespace SMCheats
{
    internal sealed class CheatsOptionsPanelController : CheatsPanelControllerBase
    {
        private readonly HashSet<string> _expandedOptionCategoryKeys = new(StringComparer.Ordinal);
        private readonly HashSet<string> _validOptionKeys = new(StringComparer.Ordinal);
        private readonly HashSet<string> _validCategories = new(StringComparer.Ordinal);
        private readonly List<string> _visibleOptionKeys = new();
        private readonly List<string> _visibleOptionKeyScratch = new();
        private readonly List<string> _categoryOptionKeys = new();
        private readonly CheatsOptionsContentModel _contentModel = new();
        private CheatsOptionsControlFactory _controlFactory;
        private CheatsOptionsControlBinder _controlBinder;
        private CheatsOptionsService _optionsService;
        private CheatsOptionsUiPool _uiPool;
        private CheatsToggleButtonView _pinSelectionModeButton;
        private Transform _mainContentRoot;
        private Transform _pinnedContentRoot;
        private int _lastOptionsRevision = -1;

        public override string Id => CheatsTabUiUtility.Options;
        public override int Order => 3;
        public override CheatsBuiltInTabs BuiltInFlag => CheatsBuiltInTabs.Options;

        protected override void OnBeforeContentBuilt()
        {
            _optionsService = Services.Get<CheatsOptionsService>();
            _lastOptionsRevision = _optionsService.Revision;
            var optionsService = _optionsService;
            optionsService.Changed += HandleOptionsChanged;
            AddRegisterDisposable(new DisposableAction(() => optionsService.Changed -= HandleOptionsChanged));

            AddPinnedKeysChangedHandler(HandleOptionsChanged);
            _controlFactory = new CheatsOptionsControlFactory(UiFactory);
            _controlBinder = new CheatsOptionsControlBinder(_controlFactory);
        }

        public override void OnUnregistered()
        {
            base.OnUnregistered();
            _pinSelectionModeButton = null;
            _optionsService = null;
            _controlBinder = null;
            _controlFactory = null;
            _uiPool = null;
        }

        protected override List<IDisposable> BuildHeaderActions()
        {
            var uiSettings = UiSettings;
            _pinSelectionModeButton = UiFactory.CreateToggleButton(
                null,
                uiSettings.ToggleActiveIcon,
                uiSettings.ToggleInactiveIcon);
            UpdatePinSelectionModeButton();
            AddHeaderButton(_pinSelectionModeButton);
            _pinSelectionModeButton.Button.onClick.AddListener(TogglePinSelectionMode);
            return new List<IDisposable>();
        }

        protected override (GameObject, IDisposable) BuildMainContent()
        {
            var mainRoot = UiFactory.InstantiatePrefabView<RectTransform>(
                CheatsPrefabResourcePaths.Main(Id),
                RuntimeUi.MainContentRoot);

            var pinnedRoot = UiFactory.InstantiatePrefabView<RectTransform>(
                CheatsPrefabResourcePaths.Pinned(Id),
                RuntimeUi.PinnedContentRoot);

            _mainContentRoot = FindOptionsContentRoot(mainRoot);
            _pinnedContentRoot = FindOptionsContentRoot(pinnedRoot);
            _uiPool = new CheatsOptionsUiPool(UiFactory, _controlBinder, CheatsPrefabResourcePaths.OptionsCategory);
            pinnedRoot.gameObject.SetActive(true);

            RefreshOptionsContent();
            SelectionState.AddUpdate(UpdateSelectedOptionsContent);
            AddRegisterDisposable(UpdateUtility.BindAction(UpdatePinnedOptionsContent));

            return (mainRoot.gameObject, new DisposableAction(() =>
            {
                _controlBinder.ClearUpdates();
                _uiPool.Clear();
                _visibleOptionKeys.Clear();
                _visibleOptionKeyScratch.Clear();
                _categoryOptionKeys.Clear();
                _validOptionKeys.Clear();
                _validCategories.Clear();
                _mainContentRoot = null;
                _pinnedContentRoot = null;
                DestroyRoot(mainRoot);
                DestroyRoot(pinnedRoot);
            }));
        }

        private static RectTransform FindOptionsContentRoot(RectTransform root)
        {
            var contentRoot = root.Find("Content") as RectTransform;
            return contentRoot != null ? contentRoot : root;
        }

        private void RefreshOptionsContent()
        {
            _controlBinder.ClearUpdates();
            BuildContentModel();

            _uiPool.BeginRefresh();
            BuildMainContent(_mainContentRoot);
            BuildPinnedContent(_pinnedContentRoot);
            _uiPool.EndRefresh();

            _lastOptionsRevision = _optionsService.Revision;
            CacheVisibleOptionKeys();
            _controlBinder.RunUpdates();
        }

        private void BuildContentModel()
        {
            CleanupPinnedState();
            _contentModel.BeginBuild();

            var options = _optionsService.Options;
            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];
                if (option.IsVisible)
                {
                    _contentModel.AddVisibleOption(option, IsOptionPinned(option));
                }
            }
        }

        private void BuildMainContent(Transform parent)
        {
            var categories = _contentModel.Categories;
            _uiPool.SetEmptyVisible(parent, categories.Count <= 0);
            if (categories.Count <= 0)
            {
                return;
            }

            for (var i = 0; i < categories.Count; i++)
            {
                var category = categories[i];
                var categoryName = category.Category;
                var isCategoryPinned = IsOptionCategoryPinned(categoryName);
                var isExpanded = IsPinModeActive || IsOptionCategoryExpanded(categoryName);
                var categoryView = _uiPool.GetCategory(categoryName, parent);
                categoryView.Initialize(
                    BuildCategoryHeaderTitle(categoryName),
                    IsPinModeActive ? isCategoryPinned : isExpanded,
                    isExpanded);
                ConfigureCategoryHeaderIcon(categoryView.HeaderButton, isExpanded, IsPinModeActive, isCategoryPinned);

                categoryView.HeaderButton.Button.onClick.RemoveAllListeners();
                categoryView.HeaderButton.Button.onClick.AddListener(() => HandleCategorySelected(categoryName));

                if (!isExpanded)
                {
                    continue;
                }

                var options = category.Options;
                for (var optionIndex = 0; optionIndex < options.Count; optionIndex++)
                {
                    BuildOptionRow(options[optionIndex], false, categoryView.ContentRoot);
                }
            }
        }

        private void BuildPinnedContent(Transform parent)
        {
            var pinnedOptions = _contentModel.PinnedOptions;
            for (var i = 0; i < pinnedOptions.Count; i++)
            {
                BuildOptionRow(pinnedOptions[i], true, parent);
            }
        }

        private void HandleOptionsChanged()
        {
            RefreshOptionsContent();
        }

        private void BuildOptionRow(CheatOptionDefinition option, bool pinnedContext, Transform parent)
        {
            var row = _uiPool.GetOptionRow(option, pinnedContext, parent);
            _controlBinder.BindOptionRow(
                row,
                option,
                new CheatsOptionBindingContext(
                    pinnedContext,
                    IsPinModeActive,
                    IsOptionPinned,
                    IsOptionCategoryPinned,
                    ToggleOptionPin));
        }

        private void TogglePinSelectionMode()
        {
            SetPinModeActive(!IsPinModeActive);
        }

        private void HandleCategorySelected(string category)
        {
            if (IsPinModeActive)
            {
                ToggleOptionCategoryPin(category);
                return;
            }

            ToggleOptionCategoryExpanded(category);
        }

        private bool IsOptionPinned(CheatOptionDefinition option)
        {
            return option != null && IsPinnedKey(CheatsTabUiUtility.GetOptionKey(option));
        }

        private bool IsOptionCategoryPinned(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return false;
            }

            var hasOptions = false;
            var options = _optionsService.Options;
            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];
                if (!option.IsVisible || !string.Equals(option.Category, category, StringComparison.Ordinal))
                {
                    continue;
                }

                hasOptions = true;
                if (!IsOptionPinned(option))
                {
                    return false;
                }
            }

            return hasOptions;
        }

        private void ToggleOptionPin(CheatOptionDefinition option)
        {
            if (option == null)
            {
                return;
            }

            var optionKey = CheatsTabUiUtility.GetOptionKey(option);
            TogglePinnedKey(optionKey);
        }

        private void ToggleOptionCategoryPin(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return;
            }

            var shouldUnpin = IsOptionCategoryPinned(category);
            _categoryOptionKeys.Clear();
            var options = _optionsService.Options;
            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];
                if (!option.IsVisible || !string.Equals(option.Category, category, StringComparison.Ordinal))
                {
                    continue;
                }

                _categoryOptionKeys.Add(CheatsTabUiUtility.GetOptionKey(option));
            }

            SetPinnedKeys(_categoryOptionKeys, !shouldUnpin);
        }

        private bool IsOptionCategoryExpanded(string category)
        {
            return !string.IsNullOrWhiteSpace(category) && _expandedOptionCategoryKeys.Contains(category);
        }

        private void ToggleOptionCategoryExpanded(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return;
            }

            if (!_expandedOptionCategoryKeys.Add(category))
            {
                _expandedOptionCategoryKeys.Remove(category);
            }

            RefreshOptionsContent();
        }

        private void CleanupPinnedState()
        {
            _validOptionKeys.Clear();
            _validCategories.Clear();
            var options = _optionsService.Options;
            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];
                _validOptionKeys.Add(CheatsTabUiUtility.GetOptionKey(option));
                if (!string.IsNullOrWhiteSpace(option.Category))
                {
                    _validCategories.Add(option.Category);
                }
            }

            RemovePinnedKeysWhere(optionKey => !_validOptionKeys.Contains(optionKey), false);
            _expandedOptionCategoryKeys.RemoveWhere(category => !_validCategories.Contains(category));
        }

        private void ExpandAllVisibleCategories()
        {
            var options = _optionsService.Options;
            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];
                if (option.IsVisible && !string.IsNullOrWhiteSpace(option.Category))
                {
                    _expandedOptionCategoryKeys.Add(option.Category);
                }
            }
        }

        private static string BuildCategoryHeaderTitle(string category)
        {
            return SanitizeCategoryName(category);
        }

        private static void ConfigureCategoryHeaderIcon(
            CheatsButtonView headerButton,
            bool isExpanded,
            bool isPinModeActive,
            bool isCategoryPinned)
        {
            var uiSettings = CheatsSettingsProxy.UiSettings;
            var icon = isPinModeActive
                ? isCategoryPinned
                    ? uiSettings.ToggleActiveIcon
                    : uiSettings.ToggleInactiveIcon
                : isExpanded
                ? uiSettings.PinnedConsoleExpandedIcon
                : uiSettings.PinnedConsoleCollapsedIcon;

            headerButton.Icon.sprite = icon;
            headerButton.Icon.preserveAspect = true;
            headerButton.Icon.color = uiSettings.IconColor;
            headerButton.Icon.gameObject.SetActive(icon != null);
        }

        private static string SanitizeCategoryName(string category)
        {
            return string.IsNullOrWhiteSpace(category)
                ? "General"
                : category;
        }

        private void UpdatePinSelectionModeButton()
        {
            _pinSelectionModeButton.IsActive = IsPinModeActive;
        }

        protected override void OnPinModeChanged(bool isActive)
        {
            if (isActive)
            {
                ExpandAllVisibleCategories();
            }

            UpdatePinSelectionModeButton();
            RefreshOptionsContent();
        }

        private void UpdateSelectedOptionsContent()
        {
            UpdateOptionsContent(true, false);
        }

        private void UpdatePinnedOptionsContent()
        {
            UpdateOptionsContent(false, true);
        }

        private void UpdateOptionsContent(bool updateMain, bool updatePinned)
        {
            if (_optionsService == null)
            {
                return;
            }

            if (_lastOptionsRevision != _optionsService.Revision || HasVisibleOptionsChanged())
            {
                RefreshOptionsContent();
                return;
            }

            if (updateMain)
            {
                _controlBinder.RunMainUpdates();
            }

            if (updatePinned)
            {
                _controlBinder.RunPinnedUpdates();
            }
        }

        private bool HasVisibleOptionsChanged()
        {
            _visibleOptionKeyScratch.Clear();
            FillVisibleOptionKeys(_visibleOptionKeyScratch);
            if (_visibleOptionKeyScratch.Count != _visibleOptionKeys.Count)
            {
                return true;
            }

            for (var i = 0; i < _visibleOptionKeyScratch.Count; i++)
            {
                if (!string.Equals(_visibleOptionKeyScratch[i], _visibleOptionKeys[i], StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private void CacheVisibleOptionKeys()
        {
            _visibleOptionKeys.Clear();
            FillVisibleOptionKeys(_visibleOptionKeys);
        }

        private void FillVisibleOptionKeys(List<string> target)
        {
            if (_optionsService == null)
            {
                return;
            }

            var options = _optionsService.Options;
            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];
                if (option.IsVisible)
                {
                    target.Add(option.Key);
                }
            }
        }

        private static void DestroyRoot(Transform root)
        {
            if (root != null)
            {
                UnityEngine.Object.Destroy(root.gameObject);
            }
        }
    }
}
