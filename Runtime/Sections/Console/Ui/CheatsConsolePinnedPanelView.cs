using UnityEngine;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsConsolePinnedPanelView : MonoBehaviour
    {
        [SerializeField] private RectTransform _leftHeaderRoot;
        [SerializeField] private RectTransform _rightHeaderRoot;
        [SerializeField] private CheatsConsoleEntriesView _entriesView;

        public RectTransform LeftHeaderRoot => _leftHeaderRoot;
        public RectTransform RightHeaderRoot => _rightHeaderRoot;
        public CheatsConsoleEntriesView EntriesView => _entriesView;

        public void SetEntriesVisible(bool isVisible)
        {
            _entriesView.gameObject.SetActive(isVisible);
        }
    }
}
