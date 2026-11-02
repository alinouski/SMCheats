using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsConsolePanelView : MonoBehaviour
    {
        [SerializeField] private Button _copySelectedButton;
        [SerializeField] private CheatsInputOptionView _searchInput;
        [SerializeField] private CheatsConsoleEntriesView _entriesView;
        [SerializeField] private CheatsConsoleDetailsWindowView _detailsWindow;

        public Button CopySelectedButton => _copySelectedButton;
        public CheatsInputOptionView SearchInput => _searchInput;
        public CheatsConsoleEntriesView EntriesView => _entriesView;
        public CheatsConsoleDetailsWindowView DetailsWindow => _detailsWindow;
    }
}
