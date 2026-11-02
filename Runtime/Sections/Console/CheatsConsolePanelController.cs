using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    internal sealed class CheatsConsolePanelController : CheatsPanelControllerBase
    {
        private const int MaxRenderedEntries = 250;

        private readonly CheatsConsoleViewState _state = new();
        private readonly CheatsConsoleEntriesModel _entriesModel = new();

        private CheatsConsoleService _consoleService;
        private CheatsConsolePanelView _view;
        private CheatsInputOptionView _searchInputView;
        private CheatsToggleButtonView _pinButton;
        private CheatsButtonView _searchToggleButton;
        private CheatsButtonView _autoScrollToggleButton;
        private CheatsConsolePinnedPanelView _pinnedConsoleView;
        private CheatsButtonView _pinnedOpenButton;
        private CheatsToggleButtonView _pinnedToggleEntriesButton;
        private IDisposable _pinnedConsoleDisposable;
        private CheatsConsoleFilterButtonsBinder _headerFilterButtons;
        private CheatsConsoleFilterButtonsBinder _pinnedFilterButtons;
        private CheatsConsoleDetailsWindowView _detailsWindow;
        private int _lastCounterDataRevision = -1;
        private int _lastCounterFilterRevision = -1;
        private int _lastTabCounterRevision = -1;

        public override string Id => CheatsTabUiUtility.Console;
        public override int Order => 1;
        public override CheatsBuiltInTabs BuiltInFlag => CheatsBuiltInTabs.Console;

        protected override IDisposable BuildTabButton()
        {
            var view = UiFactory.InstantiatePrefabView<CheatsConsoleTabButtonView>(CheatsPrefabResourcePaths.ConsoleTabButton, null);
            _lastTabCounterRevision = -1;
            UpdateConsoleTabButton(view);
            AddRegisterDisposable(UpdateUtility.BindAction(() => UpdateConsoleTabButton(view)));
            return RegisterTabButton(view);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            RefreshConsole(false, true);
            UpdateHeaderButtons();
        }

        public override void OnUnregistered()
        {
            _entriesModel.Clear();
            _consoleService = null;
            _view = null;
            _searchInputView = null;
            _pinButton = null;
            _searchToggleButton = null;
            _autoScrollToggleButton = null;
            _headerFilterButtons = null;
            ClearPinnedConsole();
            _detailsWindow = null;
            base.OnUnregistered();
        }

        protected override List<IDisposable> BuildHeaderActions()
        {
            CreateHeaderFilterButtons();

            var clearButton = UiFactory.CreateHeaderActionButton(null, UiSettings.ClearIcon, null);
            clearButton.Button.onClick.AddListener(ClearLogs);
            AddHeaderButton(clearButton);

            _pinButton = UiFactory.CreateToggleButton(null, UiSettings.ToggleActiveIcon, UiSettings.ToggleInactiveIcon);
            _pinButton.Button.onClick.AddListener(TogglePinnedConsole);
            AddHeaderButton(_pinButton);

            _searchToggleButton = UiFactory.CreateHeaderActionButton(null, UiSettings.SearchIcon, null);
            _searchToggleButton.Button.onClick.AddListener(ToggleSearch);
            AddHeaderButton(_searchToggleButton);

            _autoScrollToggleButton = UiFactory.CreateHeaderActionButton(null, null, string.Empty);
            _autoScrollToggleButton.Button.onClick.AddListener(ToggleAutoScroll);
            AddHeaderButton(_autoScrollToggleButton);

            return new List<IDisposable> {UpdateUtility.BindAction(UpdatePinnedConsole)};
        }

        private void CreateHeaderFilterButtons()
        {
            var logFilterButton = CreateFilterButton(CheatsConsoleFilterType.Logs, null);
            var warningFilterButton = CreateFilterButton(CheatsConsoleFilterType.Warnings, null);
            var errorFilterButton = CreateFilterButton(CheatsConsoleFilterType.Errors, null);
            AddHeaderButton(logFilterButton);
            AddHeaderButton(warningFilterButton);
            AddHeaderButton(errorFilterButton);
            
            _headerFilterButtons = new CheatsConsoleFilterButtonsBinder(
                _state,
                logFilterButton,
                warningFilterButton,
                errorFilterButton);
        }

        private CheatsConsoleFilterButtonView CreateFilterButton(CheatsConsoleFilterType filterType, Transform parent)
        {
            var view = UiFactory.InstantiatePrefabView<CheatsConsoleFilterButtonView>(
                CheatsPrefabResourcePaths.ConsoleFilterButton, parent);
            view.Initialize(filterType);
            view.Button.onClick.AddListener(() => ToggleFilter(filterType));
            return view;
        }

        protected override (GameObject, IDisposable) BuildMainContent()
        {
            var resourcePath = CheatsPrefabResourcePaths.Main(Id);
            var view = UiFactory.InstantiatePrefabView<CheatsConsolePanelView>(resourcePath, RuntimeUi.MainContentRoot);
            _view = view;
            
            _consoleService = Services.Get<CheatsConsoleService>();
            CreateExceptionIndicator();
            BindView(view);
            BindSearchInput(view.SearchInput);
            BindDetailsWindow(view.DetailsWindow);
            SelectionState.AddUpdate(() => RefreshConsole(false));

            return view.gameObject.GetDestroyDisposable(new DisposableAction(() =>
            {
                _view?.EntriesView.ClearPool();
                _entriesModel.Clear();
                _view = null;
                _searchInputView = null;
                _pinButton = null;
                _headerFilterButtons = null;
                ClearPinnedConsole();
                _detailsWindow = null;
            }));
        }

        private void CreateExceptionIndicator()
        {
            var exceptionIndicatorPresenter = new CheatsConsoleExceptionIndicatorPresenter(
                UiFactory,
                RuntimeUi.PinnedContentRoot,
                _consoleService);
            AddRegisterDisposable(exceptionIndicatorPresenter);
        }

        private void TogglePinnedConsole()
        {
            if (_pinnedConsoleView != null)
            {
                ClearPinnedConsole();
                UpdateHeaderButtons();
                return;
            }

            CreatePinnedConsole();
            RefreshConsole(false, true);
        }

        private void CreatePinnedConsole()
        {
            const string panelResourcePath = CheatsPrefabResourcePaths.ConsolePinnedPanel;
            var pinnedRoot = RuntimeUi.PinnedContentRoot;
            var view = UiFactory.InstantiatePrefabView<CheatsConsolePinnedPanelView>(panelResourcePath, pinnedRoot);
            _pinnedConsoleView = view;
            _state.ResetPinnedEntriesVisible();
            _pinnedConsoleView.EntriesView.Initialize();
            _pinnedConsoleView.SetEntriesVisible(true);
            BindPinnedHeader(view);
            _pinnedConsoleDisposable = view.gameObject.CreateDestroyDisposable();
        }

        private void BindPinnedHeader(CheatsConsolePinnedPanelView view)
        {
            var uiSettings = UiSettings;
            _pinnedOpenButton = UiFactory.CreateHeaderActionButton(view.LeftHeaderRoot, uiSettings.OpenCheatsIcon, null);
            _pinnedOpenButton.Button.onClick.AddListener(OpenCheats);

            _pinnedToggleEntriesButton = UiFactory.CreateToggleButton(
                view.LeftHeaderRoot,
                uiSettings.PinnedConsoleExpandedIcon,
                uiSettings.PinnedConsoleCollapsedIcon);
            _pinnedToggleEntriesButton.Button.onClick.AddListener(TogglePinnedEntries);

            var pinnedClearButton = UiFactory.CreateHeaderActionButton(view.RightHeaderRoot, uiSettings.ClearIcon, null);
            pinnedClearButton.Button.onClick.AddListener(ClearLogs);

            var pinnedLogFilterButton = CreateFilterButton(CheatsConsoleFilterType.Logs, view.RightHeaderRoot);
            var pinnedWarningFilterButton = CreateFilterButton(CheatsConsoleFilterType.Warnings, view.RightHeaderRoot);
            var pinnedErrorFilterButton = CreateFilterButton(CheatsConsoleFilterType.Errors, view.RightHeaderRoot);
            
            _pinnedFilterButtons = new CheatsConsoleFilterButtonsBinder(
                _state,
                pinnedLogFilterButton,
                pinnedWarningFilterButton,
                pinnedErrorFilterButton);
            UpdatePinnedHeaderButtons();
            UpdateCounterLabels(true);
        }

        private void ClearPinnedConsole()
        {
            _pinnedConsoleDisposable?.Dispose();
            _pinnedConsoleDisposable = null;
            _pinnedConsoleView = null;
            _pinnedOpenButton = null;
            _pinnedToggleEntriesButton = null;
            _pinnedFilterButtons = null;
        }

        private void UpdatePinnedConsole()
        {
            if (_pinnedConsoleView == null || _consoleService == null)
            {
                return;
            }

            if (!RefreshFilteredEntries(false))
            {
                return;
            }

            if (_state.PinnedEntriesVisible)
            {
                RenderPinnedConsole();
            }

            UpdateCounterLabels(false);
        }

        private void TogglePinnedEntries()
        {
            _state.TogglePinnedEntriesVisible();
            _pinnedConsoleView.SetEntriesVisible(_state.PinnedEntriesVisible);
            UpdatePinnedHeaderButtons();

            if (_state.PinnedEntriesVisible)
            {
                RefreshConsole(false, true);
            }
        }

        private void BindView(CheatsConsolePanelView view)
        {
            view.EntriesView.Initialize();
            CheatsUiUtility.RebindButton(view.CopySelectedButton, CopySelectedDetails);

            var copySelectedLabel = view.CopySelectedButton.GetComponentInChildren<Text>(true);
            copySelectedLabel.text = "Copy Details";
            UpdateCopySelectedButton();
        }

        private void BindSearchInput(CheatsInputOptionView searchInput)
        {
            _searchInputView = searchInput;
            _searchInputView.gameObject.SetActive(_state.ShowSearch);

            _searchInputView.TitleText.text = "Search";

            _searchInputView.SelectionButton.gameObject.SetActive(false);

            _searchInputView.InputField.onValueChanged.RemoveAllListeners();
            _searchInputView.InputField.onValueChanged.AddListener(SetSearchText);
        }

        private void BindDetailsWindow(CheatsConsoleDetailsWindowView detailsWindow)
        {
            _detailsWindow = detailsWindow;
            ClearDetailsWindow();
        }

        private void RefreshConsole(bool forceFilter, bool forceRender = false)
        {
            if (_consoleService == null)
            {
                return;
            }

            var dataChanged = RefreshFilteredEntries(forceFilter);
            if (!forceRender && !dataChanged)
            {
                return;
            }

            RenderEntries();
            UpdateDetailsWindow();
            UpdateHeaderButtons();

            RenderPinnedConsole();
            UpdateCounterLabels(false);
        }

        private bool RefreshFilteredEntries(bool force)
        {
            return _entriesModel.Refresh(_consoleService, _state, force);
        }

        private void RenderEntries()
        {
            _view.EntriesView.Render(
                _entriesModel.FilteredEntries,
                MaxRenderedEntries,
                _state.SelectedEntryId,
                _state.AutoScroll,
                SelectEntry);
        }

        private void RenderPinnedConsole()
        {
            if (_pinnedConsoleView == null || !_state.PinnedEntriesVisible)
            {
                return;
            }

            _pinnedConsoleView.EntriesView.Render(
                _entriesModel.FilteredEntries,
                MaxRenderedEntries,
                _state.SelectedEntryId,
                _state.AutoScroll,
                SelectEntry);
        }

        private void ToggleFilter(CheatsConsoleFilterType filterType)
        {
            _state.ToggleFilter(filterType);
            RefreshConsole(true);
        }

        private void ToggleSearch()
        {
            _state.ToggleSearch();
            if (_searchInputView != null)
            {
                _searchInputView.gameObject.SetActive(_state.ShowSearch);
                if (!_state.ShowSearch)
                {
                    _searchInputView.InputField.text = string.Empty;
                }
            }

            RefreshConsole(true);
        }

        private void ToggleAutoScroll()
        {
            _state.ToggleAutoScroll();
            RefreshConsole(false, true);
        }

        private void SetSearchText(string value)
        {
            if (_state.SetSearchText(value))
            {
                RefreshConsole(true);
            }
        }

        private void SelectEntry(int id)
        {
            _state.ToggleSelectedEntry(id);
            RefreshConsole(false, true);
        }

        private void ClearDetailsWindow()
        {
            _state.ClearSelection();
            _detailsWindow.Clear();
        }

        private void ClearLogs()
        {
            _state.ClearSelection();
            _consoleService.Clear();
            RefreshConsole(true);
        }

        private void CopySelectedDetails()
        {
            var entry = FindFilteredEntry(_state.SelectedEntryId);
            if (entry != null)
            {
                GUIUtility.systemCopyBuffer = CheatsConsoleEntryFormatter.FormatDetails(entry);
            }
        }

        private CheatsConsoleEntry FindFilteredEntry(int id)
        {
            return _entriesModel.Find(id);
        }

        private void UpdateCounterLabels(bool force)
        {
            var dataRevision = _consoleService.Revision;
            var filterRevision = _state.FilterRevision;
            if (!force &&
                _lastCounterDataRevision == dataRevision &&
                _lastCounterFilterRevision == filterRevision)
            {
                return;
            }

            _lastCounterDataRevision = dataRevision;
            _lastCounterFilterRevision = filterRevision;

            var logCount = _consoleService.LogCount;
            var warningCount = _consoleService.WarningCount;
            var errorCount = _consoleService.ErrorAndExceptionCount;

            _headerFilterButtons.Update(logCount, warningCount, errorCount);
            _pinnedFilterButtons?.Update(logCount, warningCount, errorCount);
        }

        private void UpdateConsoleTabButton(CheatsConsoleTabButtonView view)
        {
            _consoleService ??= Services.Get<CheatsConsoleService>();
            var revision = _consoleService.Revision;
            if (_lastTabCounterRevision == revision)
            {
                return;
            }

            _lastTabCounterRevision = revision;
            view.SetCounts(
                _consoleService.LogCount,
                _consoleService.WarningCount,
                _consoleService.ErrorAndExceptionCount);
        }

        private void UpdateDetailsWindow()
        {
            var entry = FindFilteredEntry(_state.SelectedEntryId);
            var hasEntry = entry != null;
            if (!hasEntry)
            {
                _detailsWindow.Clear();
                return;
            }

            _detailsWindow.SetDetails(CheatsConsoleEntryFormatter.FormatDetails(entry));
        }

        private void UpdateHeaderButtons()
        {
            _searchToggleButton.IsSelected = _state.ShowSearch;
            _autoScrollToggleButton.IsSelected = _state.AutoScroll;
            _autoScrollToggleButton.Label.text = _state.AutoScroll ? "Scroll On" : "Scroll Off";

            _pinButton.IsActive = _pinnedConsoleView != null;

            UpdateCopySelectedButton();

            UpdatePinnedHeaderButtons();
        }

        private void UpdateCopySelectedButton()
        {
            var hasSelectedEntry = FindFilteredEntry(_state.SelectedEntryId) != null;
            _view.CopySelectedButton.gameObject.SetActive(hasSelectedEntry);
            _view.CopySelectedButton.interactable = hasSelectedEntry;
        }

        private void UpdatePinnedHeaderButtons()
        {
            if (_pinnedToggleEntriesButton == null)
            {
                return;
            }
            _pinnedToggleEntriesButton.IsActive = _state.PinnedEntriesVisible;
        }

        private static void OpenCheats()
        {
            if (CheatsController.HasInstance)
            {
                CheatsController.Instance.RequestShow();
            }
        }
    }
}
