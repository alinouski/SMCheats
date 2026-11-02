using System;
using UnityEngine;

namespace SMCheats
{
    [Serializable]
    public class CheatsPinCodeSettings
    {
        [SerializeField]
        private bool _requirePinCode;

        [SerializeField]
        private string _pinCode = "0000";

        [SerializeField]
        [Min(1)]
        private int _maximumPinAttempts = 5;

        [SerializeField]
        [Min(0f)]
        private float _pinLockoutDurationSeconds = 60f;
        
        public bool RequirePinCode => _requirePinCode;
        public string PinCode => _pinCode;
        public int MaximumPinAttempts => _maximumPinAttempts;
        public float PinLockoutDurationSeconds => _pinLockoutDurationSeconds;
    }
}