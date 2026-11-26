using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsValueOptionView : CheatsOptionControlView
    {
        [SerializeField]
        private Text _valueText;

        public Text ValueText => _valueText;

        protected override float CalculatePreferredWidth()
        {
            return Mathf.Max(base.CalculatePreferredWidth(), TitleText.preferredWidth + _valueText.preferredWidth + 24f);
        }
    }
}
