using System;

namespace SMCheats
{
    internal readonly struct CheatsOptionBindingContext
    {
        public CheatsOptionBindingContext(
            bool pinnedContext,
            bool pinSelectionMode,
            Func<CheatOptionDefinition, bool> isOptionPinned,
            Func<string, bool> isCategoryPinned,
            Action<CheatOptionDefinition> toggleOptionPin)
        {
            PinnedContext = pinnedContext;
            PinSelectionMode = pinSelectionMode;
            IsOptionPinned = isOptionPinned;
            IsCategoryPinned = isCategoryPinned;
            ToggleOptionPin = toggleOptionPin;
        }

        public bool PinnedContext { get; }
        public bool PinSelectionMode { get; }
        public Func<CheatOptionDefinition, bool> IsOptionPinned { get; }
        public Func<string, bool> IsCategoryPinned { get; }
        public Action<CheatOptionDefinition> ToggleOptionPin { get; }
    }
}
