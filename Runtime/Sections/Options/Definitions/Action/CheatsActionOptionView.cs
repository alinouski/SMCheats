using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsActionOptionView : CheatsOptionControlView
    {
        [SerializeField] private Button _actionButton;

        public Button ActionButton => _actionButton;
        public Text ActionButtonLabel => TitleText;
    }
}
