using System;
using UnityEngine;

namespace SMCheats
{
    internal interface ICheatsOptionControlHandler
    {
        CheatsOptionControlKind Kind { get; }
        CheatsOptionControlView CreateView(CheatsOptionsControlFactory factory, Transform parent);
        void Bind(
            CheatsOptionControlView view,
            CheatOptionDefinition option,
            CheatsOptionBindingContext context,
            Action<Action> trackUpdate);
    }
}
