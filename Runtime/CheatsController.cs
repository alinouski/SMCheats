using System;
using System.Collections.Generic;
using UnityEngine;

namespace SMCheats
{
    public sealed class CheatsController : MonoBehaviour
    {
        private const string RootName = "Cheats";
        
        public static bool HasInstance => Instance != null;
        public static CheatsController Instance { get; private set; }
        
        private readonly List<ICheatsPanelController> _panels = new();
        private readonly Dictionary<string, ICheatsPanelController> _panelsById = new(StringComparer.Ordinal);
        private ICheatsRuntimeUi _runtimeUi;
        private CheatsServiceLocator _services;
        private ICheatsPanelController _selectedPanel;
        private string _activePanelId;

       
        internal static CheatsController GetOrCreateInstance(
            CheatsServiceLocator services,
            List<ICheatsPanelController> cachedPanelControllers)
        {
            if (Instance != null)
            {
                return Instance;
            }

            var rootObject = new GameObject(RootName);
            DontDestroyOnLoad(rootObject);
            Instance = rootObject.AddComponent<CheatsController>();
            Instance.Construct(services, cachedPanelControllers);
            return Instance;
        }

        internal static void ResetInstance()
        {
            if (Instance == null)
            {
                return;
            }

            var instance = Instance;
            Instance = null;
            Destroy(instance.gameObject);
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            UnregisterPanels();
            UnregisterTickableServices();

            if (_runtimeUi != null)
            {
                _runtimeUi.SelectPanelRequest -= SelectPanel;
                _runtimeUi.CloseRequest -= Hide;
            }
            
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Construct(CheatsServiceLocator services, List<ICheatsPanelController> panels)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (_services != null && !ReferenceEquals(_services, services))
            {
                throw new InvalidOperationException("CheatsController is already bound to a different service registry.");
            }


            _runtimeUi = new CheatsRuntimeUi();
            _services = services;
            _services.Register(_runtimeUi);
            
            _runtimeUi.SelectPanelRequest += SelectPanel;
            _runtimeUi.CloseRequest += Hide;
            
            InitializePanels(panels);
            ValidateActivePanel();
            
            RegisterTickableServices();
        }

        public void RequestShow()
        {
            ValidateActivePanel();
            RenderActivePanel();
            _runtimeUi.SetActive(true);
        }

        public void Hide()
        {
            _selectedPanel?.OnDeselected();
            _selectedPanel = null;
            _runtimeUi.SetActive(false);
        }

        private void SelectPanel(string panelId)
        {
            if (!TrySelectPanel(panelId))
            {
                return;
            }

            RenderActivePanel();
        }

        private void RenderActivePanel()
        {
            ValidateActivePanel();
            var nextPanel = ActivePanel;

            if (!ReferenceEquals(_selectedPanel, nextPanel))
            {
                _selectedPanel?.OnDeselected();
                _selectedPanel = nextPanel;
            }
            _selectedPanel?.OnSelected();
        }

        private ICheatsPanelController ActivePanel =>
            string.IsNullOrWhiteSpace(_activePanelId) ? null : _panelsById.GetValueOrDefault(_activePanelId);

        private void InitializePanels(List<ICheatsPanelController> cachedPanelControllers)
        {
            var preferredPanelId = _activePanelId;
            UnregisterPanels();

            foreach (var controller in cachedPanelControllers)
            {
                RegisterPanel(controller);
            }

            _activePanelId = ResolveActivePanelId(preferredPanelId);
        }

        private void RegisterPanel(ICheatsPanelController controller)
        {
            if (controller == null ||
                string.IsNullOrWhiteSpace(controller.Id) ||
                _panelsById.ContainsKey(controller.Id) ||
                !CheatsPanelUtility.IsPanelEnabled(controller.BuiltInFlag))
            {
                return;
            }

            controller.OnRegistered(_services);
            _panels.Add(controller);
            _panels.Sort((left, right) => left.Order.CompareTo(right.Order));
            _panelsById[controller.Id] = controller;
        }

        private void UnregisterPanels()
        {
            _selectedPanel?.OnDeselected();
            _selectedPanel = null;

            foreach (var panel in _panels)
            {
                panel.OnUnregistered();
            }

            _panels.Clear();
            _panelsById.Clear();
            _activePanelId = null;
        }

        private void ValidateActivePanel()
        {
            if (!string.IsNullOrWhiteSpace(_activePanelId) && _panelsById.ContainsKey(_activePanelId))
            {
                return;
            }

            _activePanelId = ResolveActivePanelId(_activePanelId);
        }

        private bool TrySelectPanel(string panelId)
        {
            if (string.IsNullOrWhiteSpace(panelId) || !_panelsById.ContainsKey(panelId))
            {
                return false;
            }

            _activePanelId = panelId;
            return true;
        }

        private string ResolveActivePanelId(string preferredPanelId)
        {
            if (!string.IsNullOrWhiteSpace(preferredPanelId) && _panelsById.ContainsKey(preferredPanelId))
            {
                return preferredPanelId;
            }

            var defaultTabId = CheatsSettingsProxy.Settings?.DefaultTabId;
            if (!string.IsNullOrWhiteSpace(defaultTabId) && _panelsById.ContainsKey(defaultTabId))
            {
                return defaultTabId;
            }

            return _panels.Count > 0 ? _panels[0].Id : null;
        }

        private void RegisterTickableServices()
        {
            CheatsTicker.Add(_runtimeUi);

            if (_services == null)
            {
                return;
            }

            foreach (var service in _services.Services)
            {
                CheatsTicker.Add(service);
            }
        }

        private void UnregisterTickableServices()
        {
            CheatsTicker.Remove(_runtimeUi);

            if (_services == null)
            {
                return;
            }

            foreach (var service in _services.Services)
            {
                CheatsTicker.Remove(service);
            }
        }
    }
}
