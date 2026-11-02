using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsInfoEntryView : MonoBehaviour
    {
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _bodyText;

        public Text TitleText => _titleText;
        public Text BodyText => _bodyText;

        public void SetValue(string title, string body)
        {
            _titleText.text = title ?? string.Empty;
            _bodyText.text = body ?? string.Empty;
        }
    }
}
