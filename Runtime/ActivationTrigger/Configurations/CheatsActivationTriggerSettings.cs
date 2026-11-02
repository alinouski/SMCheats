using System;
using UnityEngine;

namespace SMCheats
{
    [Serializable]
    public class CheatsActivationTriggerSettings
    {
        
        [SerializeField]
        private CheatsOpenMethod _openMethods = CheatsOpenMethod.KeyboardShortcut | CheatsOpenMethod.ScreenTrigger;

        [SerializeField]
        private KeyCode _keyboardShortcut = KeyCode.P;

        public CheatsOpenMethod OpenMethods => _openMethods;
        public KeyCode KeyboardShortcut => _keyboardShortcut;
    }
}
