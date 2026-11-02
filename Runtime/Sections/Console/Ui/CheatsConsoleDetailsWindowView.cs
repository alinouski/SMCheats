using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsConsoleDetailsWindowView : MonoBehaviour
    {
        [SerializeField] private Text _detailsText;

        public void SetDetails(string details)
        {
            _detailsText.text = details ?? string.Empty;
        }

        public void Clear()
        {
            _detailsText.text = string.Empty;
        }
    }
}
