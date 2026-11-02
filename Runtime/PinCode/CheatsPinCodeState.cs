using UnityEngine;

namespace SMCheats
{
    internal sealed class CheatsPinCodeState
    {
        public const int MaxInputLength = 12;

        private readonly CheatsPinCodeSettings _settings;
        private string _input = string.Empty;
        private int _failedAttempts;
        private float _lockoutUntilRealtime;

        public bool IsSessionStarted { get; private set; }
        public bool HasInput => !string.IsNullOrEmpty(_input);
        public int InputLength => _input.Length;
        public CheatsPinCodeSubmitResult LastSubmitResult { get; private set; }

        public CheatsPinCodeState(CheatsPinCodeSettings settings)
        {
            _settings = settings;
        }
        
        public void BeginPrompt()
        {
            _input = string.Empty;
            LastSubmitResult = IsLockedOut()
                ? CheatsPinCodeSubmitResult.LockedOut
                : CheatsPinCodeSubmitResult.None;
        }

        public void ClearInput()
        {
            _input = string.Empty;
            LastSubmitResult = IsLockedOut()
                ? CheatsPinCodeSubmitResult.LockedOut
                : CheatsPinCodeSubmitResult.None;
        }

        public void RemoveLastCharacter()
        {
            if (_input.Length > 0)
            {
                _input = _input[..^1];
            }

            LastSubmitResult = IsLockedOut()
                ? CheatsPinCodeSubmitResult.LockedOut
                : CheatsPinCodeSubmitResult.None;
        }

        public void AppendDigit(char digit)
        {
            if (_settings == null || _input.Length >= MaxInputLength)
            {
                return;
            }

            _input += digit;
            LastSubmitResult = CheatsPinCodeSubmitResult.None;
        }

        public CheatsPinCodeSubmitResult Submit()
        {
            if (IsLockedOut())
            {
                LastSubmitResult = CheatsPinCodeSubmitResult.LockedOut;
                return LastSubmitResult;
            }

            if (_input == _settings.PinCode)
            {
                IsSessionStarted = true;
                _failedAttempts = 0;
                _input = string.Empty;
                LastSubmitResult = CheatsPinCodeSubmitResult.Granted;
                return LastSubmitResult;
            }

            _failedAttempts++;
            _input = string.Empty;
            if (_failedAttempts >= _settings.MaximumPinAttempts)
            {
                _failedAttempts = 0;
                _lockoutUntilRealtime = Time.realtimeSinceStartup + _settings.PinLockoutDurationSeconds;
                LastSubmitResult = _settings.PinLockoutDurationSeconds > 0f
                    ? CheatsPinCodeSubmitResult.LockedOutStarted
                    : CheatsPinCodeSubmitResult.InvalidPin;
                return LastSubmitResult;
            }

            LastSubmitResult = CheatsPinCodeSubmitResult.InvalidPin;
            return LastSubmitResult;
        }

        public bool IsLockedOut()
        {
            return Time.realtimeSinceStartup < _lockoutUntilRealtime;
        }

        public float GetRemainingLockoutSeconds()
        {
            return Mathf.Max(0f, _lockoutUntilRealtime - Time.realtimeSinceStartup);
        }

        public int GetRemainingAttempts()
        {
            return Mathf.Max(0, _settings.MaximumPinAttempts - _failedAttempts);
        }
    }
}
