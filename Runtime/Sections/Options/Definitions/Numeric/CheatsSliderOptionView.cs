using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsSliderOptionView : CheatsOptionControlView
    {
        [SerializeField]
        private Slider _slider;

        [SerializeField]
        private Text _valueLabel;

        public Slider Slider => _slider;
        public Text ValueLabel => _valueLabel;
    }
}