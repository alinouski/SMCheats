using System;
using System.Collections.Generic;
using UnityEngine;

namespace SMCheats
{
    internal abstract class CheatsPanelControllerBase : ICheatsPanelController
    {
        private ICheatsTabButton _tabButton;
        private GameObject _contentRoot;
        private readonly List<GameObject> _headerActionRoots = new();

        protected CheatsServiceLocator Services { get; private set; }
        protected ICheatsRuntimeUi RuntimeUi { get; private set; }
        protected ICheatsUiElementFactory UiFactory => Services.Get<ICheatsUiElementFactory>();
        protected string Title => CheatsTabUiUtility.GetTitle(Id);

        public abstract string Id { get; }
        public abstract int Order { get; }
        public virtual CheatsBuiltInTabs BuiltInFlag => CheatsBuiltInTabs.None;

        private readonly List<IDisposable> _registerDisposables = new();
        private readonly CheatsSubscriptionState _selectionState = new();
        private readonly CheatsPinState _pinState = new();
        private readonly CheatsKeyPinState _keyPinState = new();
        private bool _isSelected;

        protected ICheatsSubscriptionState SelectionState => _selectionState;
        protected bool IsPinModeActive => _pinState.IsActive;

        protected CheatsSettings Settings => CheatsSettingsProxy.Settings;
        protected CheatsUiSettings UiSettings => CheatsSettingsProxy.UiSettings;

        public virtual void OnRegistered(CheatsServiceLocator serviceLocator)
        {
            Services = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));
            RuntimeUi = Services.Get<ICheatsRuntimeUi>() ?? throw new InvalidOperationException("Cheats runtime UI is not registered.");
            _pinState.ActiveChanged += OnPinModeChanged;
            
            OnBeforeContentBuilt();
            
            BuildTabButton().AddTo(_registerDisposables);
            var (mainPanel, contentDisposable) = BuildMainContent();
            contentDisposable.AddTo(_registerDisposables);
            BuildHeaderActions().AddTo(_registerDisposables);
            _contentRoot = mainPanel;
            
            SetSelectedState(false);
        }

        public virtual void OnUnregistered()
        {
            OnDeselected();
            _pinState.ActiveChanged -= OnPinModeChanged;
            _pinState.ClearPinnedElements();
            _pinState.ClearRegisteredViews();
            _keyPinState.Clear();
            _registerDisposables.DisposeAndClear();
            _selectionState.Clear();
            _headerActionRoots.Clear();
            _tabButton = null;
            RuntimeUi = null;
            Services = null;
        }

        public virtual void OnSelected()
        {
            RuntimeUi.SetHeaderTitle(Title);
            SetSelectedState(true);
            _selectionState.Start();
        }

        public virtual void OnDeselected()
        {
            _selectionState.Stop();
            SetPinModeActive(false);
            SetSelectedState(false);
        }

        protected virtual void OnBeforeContentBuilt()
        {
        }

        protected virtual List<IDisposable> BuildHeaderActions()
        {
            return new List<IDisposable>();
        }

        protected abstract (GameObject, IDisposable) BuildMainContent();

        protected void SetPinModeActive(bool value)
        {
            _pinState.SetActive(value);
        }

        protected virtual void OnPinModeChanged(bool isActive)
        {
        }

        protected bool IsPinnedKey(string key)
        {
            return _keyPinState.IsPinned(key);
        }

        protected void TogglePinnedKey(string key)
        {
            _keyPinState.Toggle(key);
        }

        protected void SetPinnedKeys(IEnumerable<string> keys, bool isPinned)
        {
            _keyPinState.SetPinned(keys, isPinned);
        }

        protected void RemovePinnedKeysWhere(Predicate<string> predicate)
        {
            _keyPinState.RemoveWhere(predicate);
        }

        protected void RemovePinnedKeysWhere(Predicate<string> predicate, bool notify)
        {
            _keyPinState.RemoveWhere(predicate, notify);
        }

        protected void AddPinnedKeysChangedHandler(Action action)
        {
            if (action == null)
            {
                return;
            }

            _keyPinState.Changed += action;
            AddRegisterDisposable(new DisposableAction(() => _keyPinState.Changed -= action));
        }

        protected void RegisterPinnableView<TView>(
            TView view,
            Func<TView> createPinnedView,
            Func<TView, IDisposable> bindPinnedView)
            where TView : class, ICheatsViewElement
        {
            _pinState.RegisterView(view, createPinnedView, bindPinnedView);
        }

        protected virtual IDisposable BuildTabButton()
        {
            var buttonView = CreateTabButtonView();
            return RegisterTabButton(buttonView);
        }

        protected virtual CheatsButtonView CreateTabButtonView()
        {
            return UiFactory.CreateTabButton(null, UiSettings.TabIcons.GetIcon(BuiltInFlag), Title);
        }

        protected IDisposable RegisterTabButton(CheatsButtonView buttonView)
        {
            _tabButton = new CheatsPanelTabButton(buttonView, Id, Order);
            return RuntimeUi.AddTabButton(_tabButton);
        }

        protected void AddRegisterDisposable(IDisposable disposable)
        {
            disposable.AddTo(_registerDisposables);
        }

        protected void AddHeaderButton(CheatsButtonView buttonView)
        {
            var rootGameObject = buttonView.Root.gameObject;
            rootGameObject.SetActive(_isSelected);
            _headerActionRoots.Add(rootGameObject);
            RuntimeUi.AddHeaderElement(buttonView).AddTo(_registerDisposables);
        }

        private void SetSelectedState(bool isActive)
        {
            _tabButton.IsSelected = isActive;
            _isSelected = isActive;

            if (_contentRoot != null)
            {
                _contentRoot.SetActive(isActive);
            }

            foreach (var root in _headerActionRoots)
            {
                root?.SetActive(isActive);
            }
        }

    }
}
