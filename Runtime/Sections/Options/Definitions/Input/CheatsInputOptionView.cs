using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsInputOptionView : CheatsOptionControlView
    {
        [SerializeField] private InputField _inputField;

        public InputField InputField => _inputField;
    }
}
