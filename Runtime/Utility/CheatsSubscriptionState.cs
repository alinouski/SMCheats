using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SMCheats
{
    internal sealed class CheatsSubscriptionState : ICheatsSubscriptionState
    {
        private readonly List<IDisposable> _disposables = new();
        private readonly List<Func<IDisposable>> _subscriptionFactories = new();

        public void AddUpdate(Action action)
        {
            AddSubscription(() => UpdateUtility.BindAction(action));
        }

        public void AddUpdate(Text text, Func<string> getter)
        {
            AddSubscription(() => UpdateUtility.BindText(text, getter));
        }

        public void AddLifecycle(Action onSelected, Action onDeselected)
        {
            AddSubscription(() =>
            {
                onSelected?.Invoke();
                return onDeselected == null ? null : new DisposableAction(onDeselected);
            });
        }

        public void Start()
        {
            Stop();
            foreach (var factory in _subscriptionFactories)
            {
                factory?.Invoke().AddTo(_disposables);
            }
        }

        public void Stop()
        {
            _disposables.DisposeAndClear();
        }

        public void Clear()
        {
            Stop();
            _subscriptionFactories.Clear();
        }

        private void AddSubscription(Func<IDisposable> factory)
        {
            if (factory == null)
            {
                return;
            }

            _subscriptionFactories.Add(factory);
        }
    }
}