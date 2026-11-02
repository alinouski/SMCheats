using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SMCheats
{
    public static class CheatsUiUtility
    {
        public static void RebuildLayout(this RectTransform viewRoot)
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(viewRoot);
            Canvas.ForceUpdateCanvases();
        }

        public static IDisposable RebuildLayoutOnNextLateUpdate(this RectTransform viewRoot)
        {
            return new DelayedLayoutRebuild(viewRoot);
        }

        public static void RebindButton(Button button, UnityAction onClick)
        {
            button.onClick.RemoveAllListeners();
            if (onClick != null)
            {
                button.onClick.AddListener(onClick);
            }
        }

        public static void UpdateStateButton(Button button, Text label, bool isActive, string activeText, string inactiveText)
        {
            CheatsUiElementFactory.UpdateStateButton(button, label, isActive, activeText, inactiveText);
        }

        private sealed class DelayedLayoutRebuild : ICheatsLateUpdateTickable, IDisposable
        {
            private readonly RectTransform _viewRoot;
            private bool _isDisposed;

            public DelayedLayoutRebuild(RectTransform viewRoot)
            {
                _viewRoot = viewRoot;
                CheatsTicker.Add(this);
            }

            public void LateTick()
            {
                _viewRoot.RebuildLayout();
                Dispose();
            }

            public void Dispose()
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;
                CheatsTicker.Remove(this);
            }
        }
    }
}
