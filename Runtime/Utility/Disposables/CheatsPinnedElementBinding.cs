using System;
using Object = UnityEngine.Object;

namespace SMCheats
{
    internal sealed class CheatsPinnedElementBinding : IDisposable
    {
        public CheatsPinnedElementBinding(
            ICheatsViewElement source,
            ICheatsViewElement pinnedView,
            IDisposable updateSubscription)
        {
            Source = source;
            PinnedView = pinnedView;
            UpdateSubscription = updateSubscription;
        }

        public ICheatsViewElement Source { get; }
        public ICheatsViewElement PinnedView { get; }
        public IDisposable UpdateSubscription { get; }

        public void Dispose()
        {
            UpdateSubscription?.Dispose();
            Object.Destroy(PinnedView.Root.gameObject);
        }
    }
}
