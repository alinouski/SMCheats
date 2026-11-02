using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SMCheats
{
    public sealed class CheatsRuntimeUi : ICheatsRuntimeUi
    {
        private CheatsView View { get; }

        public RectTransform MainContentRoot => View.MainContentRoot;
        public RectTransform PinnedContentRoot => View.PinnedScreen;
        private RectTransform HeaderActionsRoot => View.HeaderActionsRoot;
        private RectTransform TabsRoot => View.TabsRoot;
        public Button CloseButton => View.CloseButton;

        public event Action<string> SelectPanelRequest;
        public event Action CloseRequest;
        
        public CheatsRuntimeUi()
        {
            var view = CheatsPrefabFactory.InstantiateView<CheatsView>(CheatsView.ResourcePath);
            var instance = view.gameObject;
            Object.DontDestroyOnLoad(instance);

            View = view;
            CloseButton.onClick.RemoveAllListeners();
            CloseButton.onClick.AddListener(CallClose);
            SetActive(false);
        }

        public void SetActive(bool value)
        {
            View.CheatsScreen.gameObject.SetActive(value);
        }

        public void SetHeaderTitle(string title)
        {
            View.HeaderTitle.text = title ?? string.Empty;
        }

        public IDisposable AddHeaderElement(ICheatsViewElement cheatsViewElement)
        {
            var rectTransform = cheatsViewElement.Root;
            rectTransform.SetParent(HeaderActionsRoot, false);

            return new DisposableAction(() => { Object.Destroy(rectTransform.gameObject); });
        }

        public IDisposable AddTabButton(ICheatsTabButton tabButton)
        {
            var rectTransform = tabButton.Root;
            rectTransform.SetParent(TabsRoot, false);
            tabButton.OnClick += SelectPanel;

            return new DisposableAction(() =>
            {
                tabButton.OnClick -= SelectPanel;
                Object.Destroy(rectTransform.gameObject);
            });
        }
        
        private void CallClose()
        {
            CloseRequest?.Invoke();
        }
        
        private void SelectPanel(string tab)
        {
            SelectPanelRequest?.Invoke(tab);
        }
    }
}
