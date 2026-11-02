using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SMCheats
{
    public class CheatsActivationTrigger : ICheatsActivationTrigger, IDisposable, ICheatsUpdateTickable
    {
        private const int RequiredTapCount = 3;
        private const float MultiTapWindowSeconds = 0.75f;

        private readonly CheatsActivationTriggerSettings _settings;
        public event Action OnActivateTrigger;
        private bool _isActive;
        private CheatsActivationView _activationView;
        private int _tapCount;
        private float _lastTapRealtime = float.NegativeInfinity;

        public CheatsActivationTrigger(CheatsActivationTriggerSettings settings)
        {
            _settings = settings;
            SetupView();
        }

        public void SetActiveModule(bool state)
        {
            _isActive = state;
            ResetTapSequence();

            if (_activationView != null)
            {
                _activationView.SetVisible(_settings.OpenMethods.HasFlag(CheatsOpenMethod.ScreenTrigger) && _isActive);
            }

            if (!_settings.OpenMethods.HasFlag(CheatsOpenMethod.KeyboardShortcut))
            {
                CheatsTicker.Remove(this);
                return;
            }

            if (state)
            {
                CheatsTicker.Add(this);
            }
            else
            {
                CheatsTicker.Remove(this);
            }
        }

        void ICheatsUpdateTickable.Tick()
        {
            if (_isActive && CheatsKeyboardInputUtility.IsKeyPressedThisFrame(_settings.KeyboardShortcut))
            {
                FireTrigger();
            }
        }

        private void FireTrigger()
        {
            if (!_isActive)
            {
                return;
            }
            OnActivateTrigger?.Invoke();
        }

        private void SetupView()
        {
            var activationView = CheatsPrefabFactory.InstantiateView<CheatsActivationView>(CheatsActivationView.ViewPrefabPath);
            Object.DontDestroyOnLoad(activationView.gameObject);
            _activationView = activationView;
            _activationView.SetVisible(false);
            _activationView.TriggerButton.onClick.AddListener(HandleScreenTriggerTap);
        }

        public void Dispose()
        {
            OnActivateTrigger = null;
            CheatsTicker.Remove(this);

            if (_activationView != null)
            {
                _activationView.TriggerButton.onClick.RemoveListener(HandleScreenTriggerTap);
                Object.Destroy(_activationView.gameObject);
                _activationView = null;
            }
        }

        private void HandleScreenTriggerTap()
        {
            if (!_isActive)
            {
                return;
            }

            var now = Time.unscaledTime;
            _tapCount = now - _lastTapRealtime <= MultiTapWindowSeconds
                ? _tapCount + 1
                : 1;
            _lastTapRealtime = now;

            if (_tapCount < RequiredTapCount)
            {
                return;
            }

            ResetTapSequence();
            FireTrigger();
        }

        private void ResetTapSequence()
        {
            _tapCount = 0;
            _lastTapRealtime = float.NegativeInfinity;
        }
    }
}
