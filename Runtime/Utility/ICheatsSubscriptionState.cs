using System;
using UnityEngine.UI;

namespace SMCheats
{
    public interface ICheatsSubscriptionState
    {
        void AddUpdate(Action action);
        void AddUpdate(Text text, Func<string> getter);
        void AddLifecycle(Action onSelected, Action onDeselected);
    }
}