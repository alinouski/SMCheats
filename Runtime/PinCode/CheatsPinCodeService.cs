using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SMCheats
{
    internal sealed class CheatsPinCodeService : IPinCodeService, IDisposable, ICheatsUpdateTickable
    {
        private readonly CheatsPinCodeSettings _settings;
        private readonly CheatsPinCodeState _state;

        private CheatsPinPromptView _view;
        private Action _pendingSuccess;

        public CheatsPinCodeService(CheatsPinCodeSettings settings)
        {
            _settings = settings;
            _state = new CheatsPinCodeState(settings);
        }

        public void RequestAccess(Action sessionSuccess)
        {
            if (!_settings.RequirePinCode || _state.IsSessionStarted)
            {
                sessionSuccess?.Invoke();
                return;
            }

            if (_pendingSuccess != null)
            {
                return;
            }

            CreateView();

            _pendingSuccess = sessionSuccess;
            _state.BeginPrompt();
            CheatsTicker.Add(this);
            SyncView();
        }

        private void CreateView()
        {
            var view = CheatsPrefabFactory.InstantiateView<CheatsPinPromptView>(CheatsPinPromptView.ResourcePath);
            Object.DontDestroyOnLoad(view.gameObject);
            _view = view;
            BindViewEvents();
        }

        void ICheatsUpdateTickable.Tick()
        {
            if (_view == null)
            {
                CheatsTicker.Remove(this);
                return;
            }

            SyncView();
            
            if (!_state.IsLockedOut())
            {
                ProcessKeyboardInput();
            }
        }

        private void BindViewEvents()
        {
            BindButton(_view.SubmitButton, Submit);
            BindButton(_view.CancelButton, CancelPrompt);

            foreach (var button in _view.DigitButtons)
            {
                var key = button.GetComponentInChildren<Text>().text;
                BindButton(button, () => HandlePinKey(key));
            }
        }

        private void SyncView()
        {
            var isLockedOut = _state.IsLockedOut();
            
            _view.MaskedText.text = BuildMaskedInput();
            _view.StatusLabel.text = BuildStatusText();
            _view.SubmitButton.interactable = !isLockedOut && _state.HasInput;

            foreach (var button in _view.DigitButtons)
            {
                button.interactable = !isLockedOut;
            }
        }

        private string BuildMaskedInput()
        {
            var masked = new string('*', _state.InputLength);
            var placeholder = new string('_', Mathf.Max(0, CheatsPinCodeState.MaxInputLength - _state.InputLength));
            return masked + placeholder;
        }

        private string BuildStatusText()
        {
            if (_state.IsLockedOut())
            {
                return $"Locked. Try again in {_state.GetRemainingLockoutSeconds():0}s";
            }

            return _state.LastSubmitResult switch
            {
                CheatsPinCodeSubmitResult.InvalidPin => $"Invalid pin. {_state.GetRemainingAttempts()} attempts left.",
                CheatsPinCodeSubmitResult.LockedOutStarted => _settings != null && _settings.PinLockoutDurationSeconds > 0f
                    ? $"Too many attempts. Locked for {_state.GetRemainingLockoutSeconds():0}s"
                    : "Too many attempts.",
                _ => $"Attempts left: {_state.GetRemainingAttempts()}"
            };
        }

        private void Submit()
        {
            if (_state.Submit() != CheatsPinCodeSubmitResult.Granted)
            {
                SyncView();
                return;
            }

            var callback = _pendingSuccess;
            _pendingSuccess = null;
            DestroyView();
            callback?.Invoke();
        }

        private void CancelPrompt()
        {
            _pendingSuccess = null;
            DestroyView();
        }

        private void DestroyView()
        {
            CheatsTicker.Remove(this);

            if (_view == null)
            {
                return;
            }

            Object.Destroy(_view.gameObject);
            _view = null;
        }

        public void Dispose()
        {
            _pendingSuccess = null;
            DestroyView();
        }

        private void HandlePinKey(string key)
        {
            switch (key)
            {
                case "C":
                    _state.ClearInput();
                    break;
                case "<":
                    _state.RemoveLastCharacter();
                    break;
                default:
                    if (!string.IsNullOrEmpty(key))
                    {
                        _state.AppendDigit(key[0]);
                    }
                    break;
            }
        }

        private void ProcessKeyboardInput()
        {
            if (!CheatsKeyboardInputUtility.HasAnyKeyboardInputThisFrame())
            {
                return;
            }

            if (CheatsKeyboardInputUtility.IsKeyPressedThisFrame(KeyCode.Return) || CheatsKeyboardInputUtility.IsKeyPressedThisFrame(KeyCode.KeypadEnter))
            {
                Submit();
                return;
            }

            if (CheatsKeyboardInputUtility.IsKeyPressedThisFrame(KeyCode.Backspace))
            {
                _state.RemoveLastCharacter();
                return;
            }
            
            if (CheatsKeyboardInputUtility.IsKeyPressedThisFrame(KeyCode.Escape))
            {
                CancelPrompt();
                return;
            }

            HandleDigits();
        }

        private void HandleDigits()
        {
            for (var keyCode = KeyCode.Alpha0; keyCode <= KeyCode.Alpha9; keyCode++)
            {
                if (!CheatsKeyboardInputUtility.IsKeyPressedThisFrame(keyCode))
                {
                    continue;
                }

                _state.AppendDigit((char)('0' + (keyCode - KeyCode.Alpha0)));
                return;
            }

            for (var keyCode = KeyCode.Keypad0; keyCode <= KeyCode.Keypad9; keyCode++)
            {
                if (!CheatsKeyboardInputUtility.IsKeyPressedThisFrame(keyCode))
                {
                    continue;
                }

                _state.AppendDigit((char)('0' + (keyCode - KeyCode.Keypad0)));
                return;
            }
        }

        private static void BindButton(Button button, UnityEngine.Events.UnityAction listener)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(listener);
        }
    }
}
