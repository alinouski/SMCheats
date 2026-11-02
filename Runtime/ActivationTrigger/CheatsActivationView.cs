using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsActivationView : MonoBehaviour
    {
        public const string ViewPrefabPath = CheatsPrefabResourcePaths.ActivationView;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasScaler _canvasScaler;
        [SerializeField] private GraphicRaycaster _graphicRaycaster;
        [SerializeField] private Button _triggerButton;

        public Canvas Canvas => _canvas;
        public CanvasScaler CanvasScaler => _canvasScaler;
        public GraphicRaycaster GraphicRaycaster => _graphicRaycaster;
        public Button TriggerButton => _triggerButton;

        public void SetVisible(bool isVisible)
        {
            _canvas.enabled = isVisible;
        }
    }
}
