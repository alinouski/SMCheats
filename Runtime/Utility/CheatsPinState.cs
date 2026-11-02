using System;
using System.Collections.Generic;

namespace SMCheats
{
    internal sealed class CheatsPinState
    {
        private readonly List<ICheatsViewElement> _pinnableViews = new();
        private readonly Dictionary<ICheatsViewElement, PinnedViewRegistration> _pinRegistrations = new();
        private readonly Dictionary<ICheatsViewElement, CheatsPinnedElementBinding> _pinnedBySource = new();

        public bool IsActive { get; private set; }

        public event Action<bool> ActiveChanged;

        public void SetActive(bool value)
        {
            IsActive = value;
            ActiveChanged?.Invoke(value);

            foreach (var view in _pinnableViews)
            {
                ApplySourcePinState(view);
            }
        }

        public void RegisterView<TView>(
            TView view,
            Func<TView> createPinnedView,
            Func<TView, IDisposable> bindPinnedView)
            where TView : class, ICheatsViewElement
        {
            if (view == null || createPinnedView == null || bindPinnedView == null || _pinRegistrations.ContainsKey(view))
            {
                return;
            }

            _pinnableViews.Add(view);
            _pinRegistrations[view] = new PinnedViewRegistration(
                () => createPinnedView.Invoke(),
                pinnedView => bindPinnedView.Invoke(pinnedView as TView));
            view.PinRequested += HandlePinRequested;
            ApplySourcePinState(view);
        }

        public void ClearRegisteredViews()
        {
            foreach (var view in _pinnableViews)
            {
                if (view != null)
                {
                    view.PinRequested -= HandlePinRequested;
                    view.PinState = CheatsViewElementPinState.None;
                }
            }

            _pinnableViews.Clear();
            _pinRegistrations.Clear();
        }

        public void ClearPinnedElements()
        {
            var bindings = new List<CheatsPinnedElementBinding>(_pinnedBySource.Values);
            foreach (var binding in bindings)
            {
                Unpin(binding);
            }

            _pinnedBySource.Clear();
        }

        private void HandlePinRequested(ICheatsViewElement view)
        {
            if (view == null)
            {
                return;
            }

            if (_pinnedBySource.TryGetValue(view, out var sourceBinding))
            {
                if (IsActive)
                {
                    Unpin(sourceBinding);
                }

                return;
            }

            if (IsActive && _pinRegistrations.TryGetValue(view, out var registration))
            {
                Pin(view, registration);
            }
        }

        private void Pin(ICheatsViewElement source, PinnedViewRegistration registration)
        {
            var pinnedView = registration.CreatePinnedView?.Invoke();
            if (pinnedView == null)
            {
                return;
            }

            pinnedView.PinState = CheatsViewElementPinState.PinnedScreen;

            var updateSubscription = registration.BindPinnedView?.Invoke(pinnedView);
            var binding = new CheatsPinnedElementBinding(source, pinnedView, updateSubscription);
            _pinnedBySource[source] = binding;

            ApplySourcePinState(source);
        }

        private void Unpin(CheatsPinnedElementBinding binding)
        {
            if (binding == null)
            {
                return;
            }

            _pinnedBySource.Remove(binding.Source);

            binding.Dispose();
            ApplySourcePinState(binding.Source);
        }

        private void ApplySourcePinState(ICheatsViewElement view)
        {
            if (view == null)
            {
                return;
            }

            if (!IsActive)
            {
                view.PinState = CheatsViewElementPinState.None;
                return;
            }

            view.PinState = _pinnedBySource.ContainsKey(view)
                ? CheatsViewElementPinState.Pinned
                : CheatsViewElementPinState.ReadyToPin;
        }

        private sealed class PinnedViewRegistration
        {
            public PinnedViewRegistration(
                Func<ICheatsViewElement> createPinnedView,
                Func<ICheatsViewElement, IDisposable> bindPinnedView)
            {
                CreatePinnedView = createPinnedView;
                BindPinnedView = bindPinnedView;
            }

            public Func<ICheatsViewElement> CreatePinnedView { get; }
            public Func<ICheatsViewElement, IDisposable> BindPinnedView { get; }
        }
    }
}
