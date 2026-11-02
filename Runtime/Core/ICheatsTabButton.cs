using System;

namespace SMCheats
{
    public interface ICheatsTabButton : ICheatsViewElement
    {
        event Action<string> OnClick;
        int Order { get; }
        bool IsSelected { get; set; }
    }
}
