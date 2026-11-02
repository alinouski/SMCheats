using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsPinPromptView : MonoBehaviour
    {
        public const string ResourcePath = CheatsPrefabResourcePaths.PinPromptView;

        [SerializeField] private Text _statusLabel;
        [SerializeField] private Text _maskedText;
        [SerializeField] private Button _submitButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button[] _digitButtons;

        public Text StatusLabel => _statusLabel;
        public Text MaskedText => _maskedText;
        public Button SubmitButton => _submitButton;
        public Button CancelButton => _cancelButton;
        public Button[] DigitButtons => _digitButtons;
    }
}
