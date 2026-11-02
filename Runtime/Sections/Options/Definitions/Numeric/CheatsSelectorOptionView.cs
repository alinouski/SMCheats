using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsSelectorOptionView : CheatsOptionControlView
    {
        [SerializeField]
        private Button _previousButton;

        [SerializeField]
        private Button _nextButton;

        [SerializeField]
        private Text _valueLabel;

        public Button PreviousButton => _previousButton;
        public Button NextButton => _nextButton;
        public Text ValueLabel => _valueLabel;
    }
}