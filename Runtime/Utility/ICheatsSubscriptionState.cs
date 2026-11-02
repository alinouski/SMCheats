using System;
using UnityEngine.UI;

namespace SMCheats
{
    internal interface ICheatsSubscriptionState
    {
        void AddUpdate(Action action);
        void AddUpdate(Text text, Func<string> getter);
        void AddLifecycle(Action onSelected, Action onDeselected);
    }
}