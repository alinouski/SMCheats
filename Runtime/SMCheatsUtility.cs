using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SMCheats
{
    public static class SMCheatsUtility
    {
        private static readonly List<ICheatsPanelController> CachedPanelControllers = new();
        private static CheatsServiceLocator _services;
        private static ICheatsActivationTrigger _triggerListener;
        private static IPinCodeService _pinCode;
        private static CheatsSettings Settings => CheatsSettingsProxy.Settings;
        private static bool _isInitialized;

        public static bool IsEnabled { get; private set; }

        public static void Init(params ICheatsPanelController[] panelControllers)
        {
            if (_isInitialized)
            {
                return;
            }

            IsEnabled = ResolveEnabled();
            if (!IsEnabled)
            {
                _isInitialized = true;
                return;
            }

            _triggerListener ??= new CheatsActivationTrigger(Settings.ActivationTriggerSettings);
            _pinCode ??= PinCodeUtility.CreateService(Settings.PinCodeSettings);
            SetupServices();
            SetupPanels(panelControllers);
            InitializeRuntimeUiForNonReleaseBuild();

            _isInitialized = true;
        }

        private static bool ResolveEnabled()
        {
#if SMCHEATS_DISABLED
            return false;
#else
            return Debug.isDebugBuild || Settings.EnableInReleaseBuilds;
#endif
        }

        private static void SetupServices()
        {
            _services ??= new CheatsServiceLocator();
            _services.Register<ICheatsUiElementFactory>(new CheatsUiElementFactory());
            var consoleService = new CheatsConsoleService(Settings.MaximumConsoleEntries);
            _services.Register(consoleService);
            consoleService.StartListening();
            _services.Register(new CheatsOptionsService());
            _services.Register(new CheatsFpsProfilerService());
            _services.Register<ICheatsCpuProfilerService>(new CheatsCpuProfilerService());
            _services.Register<ICheatsMemoryProfilerService>(new CheatsMemoryProfilerService());
        }

        private static void SetupPanels(ICheatsPanelController[] panelControllers)
        {
            CachedPanelControllers.Clear();
            CachedPanelControllers.Add(new CheatsInfoPanelController());
            CachedPanelControllers.Add(new CheatsConsolePanelController());
            CachedPanelControllers.Add(new CheatsProfilerPanelController());
            CachedPanelControllers.Add(new CheatsOptionsPanelController());
            
            if (panelControllers != null)
            {
                foreach (var controller in panelControllers)
                {
                    CachedPanelControllers.Add(controller);
                }
            }
        }

        public static void Show()
        {
            AssertInitialization();
            if (!IsEnabled)
            {
                return;
            }

            _pinCode.RequestAccess(ShowDirect);
        }
        public static void Hide()
        {
            AssertInitialization();

            if (!IsEnabled || !CheatsController.HasInstance)
            {
                return;
            }

            CheatsController.Instance.Hide();
        }
        public static void RegisterOptions(object target)
        {
            AssertInitialization();
            OptionsService?.Register(target);
        }
        public static void UnregisterOptions(object target)
        {
            AssertInitialization();
            OptionsService?.Unregister(target);
        }

        public static CheatOptionDefinition AddAction(string name, Action callback, string category = null, int sortOrder = 0)
        {
            AssertInitialization();
            return OptionsService?.AddAction(name, callback, category, sortOrder);
        }

        public static CheatOptionDefinition AddToggle(
            string name,
            Func<bool> getter,
            Action<bool> setter,
            string category = null,
            int sortOrder = 0)
        {
            AssertInitialization();
            return OptionsService?.AddToggle(name, getter, setter, category, sortOrder);
        }

        public static CheatOptionDefinition AddValue<T>(
            string name,
            Func<T> getter,
            Action<T> setter = null,
            string category = null,
            int sortOrder = 0,
            double? minimumValue = null,
            double? maximumValue = null,
            double? step = null)
        {
            AssertInitialization();
            return OptionsService?.AddValue(name, getter, setter, category, sortOrder, minimumValue, maximumValue, step);
        }

        public static bool RemoveOption(CheatOptionDefinition option)
        {
            AssertInitialization();
            return OptionsService?.Remove(option) ?? false;
        }

        private static CheatsOptionsService OptionsService => _services?.Get<CheatsOptionsService>();

        public static void SetActiveTriggerListener(bool active)
        {
            AssertInitialization();
            
            if (_triggerListener == null)
            {
                return;
            }

            _triggerListener.OnActivateTrigger -= Show;
            if (active)
            {
                _triggerListener.OnActivateTrigger += Show;
            }
            
            _triggerListener.SetActiveModule(active);
        }
        private static void ShowDirect()
        {
            CheatsController.GetOrCreateInstance(_services, CachedPanelControllers).RequestShow();
        }

        private static void InitializeRuntimeUiForNonReleaseBuild()
        {
            if (!Debug.isDebugBuild)
            {
                return;
            }

            CheatsController.GetOrCreateInstance(_services, CachedPanelControllers);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetState()
        {
            if (_triggerListener != null)
            {
                _triggerListener.OnActivateTrigger -= Show;
                _triggerListener.SetActiveModule(false);
                if (_triggerListener is IDisposable disposableTrigger)
                {
                    disposableTrigger.Dispose();
                }
            }

            if (_pinCode is IDisposable disposablePinCode)
            {
                disposablePinCode.Dispose();
            }

            _services?.Dispose();
            CheatsController.ResetInstance();

            CachedPanelControllers.Clear();
            _services = null;
            _triggerListener = null;
            _pinCode = null;
            _isInitialized = false;
            IsEnabled = false;
        }

        private static void AssertInitialization([CallerMemberName] string callerMemberName = "")
        {
            if (_isInitialized)
            {
                return;
            }

            throw new Exception($"Initialize cheats before {callerMemberName}.");
        }
    }
}
