using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    public abstract class CheatsOptionControlView : MonoBehaviour, ILayoutElement
    {
        [SerializeField] private Text _titleText;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Button _selectionButton;

        public Text TitleText => _titleText;
        public Image BackgroundImage => _backgroundImage;
        public Button SelectionButton => _selectionButton;

        public float minWidth => preferredWidth;
        public float preferredWidth { get; private set; }
        public float flexibleWidth => -1f;
        public float minHeight => -1f;
        public float preferredHeight => -1f;
        public float flexibleHeight => -1f;
        public int layoutPriority => 2;

        public void CalculateLayoutInputHorizontal()
        {
            preferredWidth = Mathf.Max(100f, CalculatePreferredWidth());
        }

        public void CalculateLayoutInputVertical()
        {
        }

        protected virtual float CalculatePreferredWidth()
        {
            var width = _titleText.preferredWidth + 16f;
            return TryGetComponent<LayoutGroup>(out var layout) ? Mathf.Max(width, layout.preferredWidth) : width;
        }
    }
}
