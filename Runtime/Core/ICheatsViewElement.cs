using System;
using UnityEngine;

namespace SMCheats
{
    public interface ICheatsViewElement
    {
        RectTransform Root { get; }
        CheatsViewElementPinState PinState { get; set; }
        event Action<ICheatsViewElement> PinRequested;
    }
}
