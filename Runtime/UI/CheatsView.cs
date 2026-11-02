using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsView : MonoBehaviour
    {
        public const string ResourcePath = CheatsPrefabResourcePaths.CheatsView;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasScaler _canvasScaler;
        [SerializeField] private GraphicRaycaster _graphicRaycaster;
        [SerializeField] private RectTransform _root;
        [SerializeField] private RectTransform _cheatsScreen;
        [SerializeField] private RectTransform _tabsRoot;
        [SerializeField] private RectTransform _mainContentRoot;
        [SerializeField] private RectTransform _pinnedScreen;
        [SerializeField] private RectTransform _headerActionsRoot;
        [SerializeField] private Text _headerTitle;
        [SerializeField] private Button _closeButton;

        public Canvas Canvas => _canvas;
        public CanvasScaler CanvasScaler => _canvasScaler;
        public GraphicRaycaster GraphicRaycaster => _graphicRaycaster;
        public RectTransform Root => _root;
        public RectTransform CheatsScreen => _cheatsScreen;
        public RectTransform TabsRoot => _tabsRoot;
        public RectTransform MainContentRoot => _mainContentRoot;
        public RectTransform PinnedScreen => _pinnedScreen;
        public RectTransform HeaderActionsRoot => _headerActionsRoot;
        public Text HeaderTitle => _headerTitle;
        public Button CloseButton => _closeButton;
    }
}
