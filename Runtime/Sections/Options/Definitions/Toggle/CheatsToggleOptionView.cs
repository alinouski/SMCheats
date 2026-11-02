using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsToggleOptionView : CheatsOptionControlView
    {
        [SerializeField] private Button _toggleButton;
        [SerializeField] private Image _toggleButtonIcon;

        public Button ToggleButton => _toggleButton;
        public Image ToggleButtonIcon => _toggleButtonIcon;
    }
}
