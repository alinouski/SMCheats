using System;
using UnityEngine.UI;

namespace SMCheats
{
    public static class UpdateUtility
    {
        public static IDisposable BindText(Text text, Func<string> getter)
        {
            if (text == null || getter == null)
            {
                return null;
            }

            return BindAction(() => text.text = getter.Invoke());
        }

        public static IDisposable BindAction(Action action)
        {
            return action == null ? null : new TickableAction(action);
        }
    }
}
