using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    public abstract class CheatsOptionControlView : MonoBehaviour
    {
        [SerializeField] private Text _titleText;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Button _selectionButton;

        public Text TitleText => _titleText;
        public Image BackgroundImage => _backgroundImage;
        public Button SelectionButton => _selectionButton;
    }
}
