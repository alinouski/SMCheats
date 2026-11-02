using System;
using UnityEngine;

namespace SMCheats
{
    public interface ICheatsRuntimeUi
    {
        event Action<string> SelectPanelRequest; 
        event Action CloseRequest; 
        
        public RectTransform MainContentRoot { get; }
        public RectTransform PinnedContentRoot { get; }
        void SetActive(bool isActive);
        void SetHeaderTitle(string title);

        IDisposable AddHeaderElement(ICheatsViewElement cheatsViewElement);
        IDisposable AddTabButton(ICheatsTabButton tabButton);
    }
}