using System;
using System.Collections.Generic;
using UnityEngine;

namespace SMCheats
{
    internal sealed class CheatsOptionsControlBinder
    {
        private static readonly ICheatsOptionControlHandler[] Handlers =
        {
            new ActionOptionControlHandler(),
            new ToggleOptionControlHandler(),
            new ReadOnlyValueOptionControlHandler(),
            new DropdownOptionControlHandler(),
            new SliderOptionControlHandler(),
            new SelectorOptionControlHandler(),
            new NumericInputOptionControlHandler(),
            new TextInputOptionControlHandler(),
            new FallbackValueOptionControlHandler()
        };

        private readonly CheatsOptionsControlFactory _controlFactory;
        private readonly List<Action> _mainUpdateActions = new();
        private readonly List<Action> _pinnedUpdateActions = new();

        public CheatsOptionsControlBinder(CheatsOptionsControlFactory controlFactory)
        {
            _controlFactory = controlFactory;
        }

        public void ClearUpdates()
        {
            _mainUpdateActions.Clear();
            _pinnedUpdateActions.Clear();
        }

        public void RunUpdates()
        {
            RunMainUpdates();
            RunPinnedUpdates();
        }

        public void RunMainUpdates()
        {
            for (var i = 0; i < _mainUpdateActions.Count; i++)
            {
                _mainUpdateActions[i].Invoke();
            }
        }

        public void RunPinnedUpdates()
        {
            for (var i = 0; i < _pinnedUpdateActions.Count; i++)
            {
                _pinnedUpdateActions[i].Invoke();
            }
        }

        public CheatsOptionControlKind ResolveKind(CheatOptionDefinition option)
        {
            return CheatsOptionControlResolver.Resolve(option);
        }

        public CheatsOptionRowBinding CreateOptionRow(
            CheatOptionDefinition option,
            CheatsOptionControlKind kind,
            Transform parent)
        {
            var handler = GetHandler(kind);
            var view = handler.CreateView(_controlFactory, parent);
            return new CheatsOptionRowBinding(kind, view);
        }

        public void BindOptionRow(
            CheatsOptionRowBinding row,
            CheatOptionDefinition option,
            CheatsOptionBindingContext context)
        {
            if (option == null)
            {
                return;
            }

            var handler = GetHandler(row.Kind);
            handler.Bind(row.View, option, context, action => TrackOptionUpdate(action, context.PinnedContext));
        }

        private static ICheatsOptionControlHandler GetHandler(CheatsOptionControlKind kind)
        {
            for (var i = 0; i < Handlers.Length; i++)
            {
                if (Handlers[i].Kind == kind)
                {
                    return Handlers[i];
                }
            }

            return Handlers[^1];
        }

        private void TrackOptionUpdate(Action action, bool pinnedContext)
        {
            if (action == null)
            {
                return;
            }

            if (pinnedContext)
            {
                _pinnedUpdateActions.Add(action);
                return;
            }

            _mainUpdateActions.Add(action);
        }

        private sealed class ActionOptionControlHandler : ICheatsOptionControlHandler
        {
            public CheatsOptionControlKind Kind => CheatsOptionControlKind.Action;

            public CheatsOptionControlView CreateView(CheatsOptionsControlFactory factory, Transform parent)
            {
                return factory.CreateAction(parent);
            }

            public void Bind(
                CheatsOptionControlView view,
                CheatOptionDefinition option,
                CheatsOptionBindingContext context,
                Action<Action> trackUpdate)
            {
                if (view is not CheatsActionOptionView actionView || option.ActionData == null)
                {
                    return;
                }

                BindChrome(actionView, option, context);
                CheatsUiUtility.RebindButton(
                    actionView.ActionButton,
                    context.PinSelectionMode
                        ? () => context.ToggleOptionPin?.Invoke(option)
                        : () => option.ActionData.Callback?.Invoke());
                if (context.PinSelectionMode)
                {
                    actionView.ActionButton.enabled = true;
                    actionView.ActionButton.interactable = true;
                }

                actionView.ActionButtonLabel.text = option.Name;
            }
        }

        private sealed class ToggleOptionControlHandler : ICheatsOptionControlHandler
        {
            public CheatsOptionControlKind Kind => CheatsOptionControlKind.Toggle;

            public CheatsOptionControlView CreateView(CheatsOptionsControlFactory factory, Transform parent)
            {
                return factory.CreateToggle(parent);
            }

            public void Bind(
                CheatsOptionControlView view,
                CheatOptionDefinition option,
                CheatsOptionBindingContext context,
                Action<Action> trackUpdate)
            {
                if (view is not CheatsToggleOptionView toggleView || option.ToggleData == null)
                {
                    return;
                }

                BindChrome(toggleView, option, context);
                if (context.PinSelectionMode)
                {
                    CheatsUiUtility.RebindButton(
                        toggleView.ToggleButton,
                        () => context.ToggleOptionPin?.Invoke(option));
                    toggleView.ToggleButton.enabled = true;
                    toggleView.ToggleButton.interactable = true;

                    return;
                }

                var toggleData = option.ToggleData;
                CheatsUiUtility.RebindButton(toggleView.ToggleButton, () =>
                {
                    var current = toggleData.Getter?.Invoke() ?? false;
                    toggleData.Setter?.Invoke(!current);
                });
                trackUpdate?.Invoke(() => UpdateToggleButtonState(
                    toggleView,
                    toggleData.Getter?.Invoke() ?? false));
            }
        }

        private sealed class ReadOnlyValueOptionControlHandler : ICheatsOptionControlHandler
        {
            public CheatsOptionControlKind Kind => CheatsOptionControlKind.ReadOnlyValue;

            public CheatsOptionControlView CreateView(CheatsOptionsControlFactory factory, Transform parent)
            {
                return factory.CreateValue(parent);
            }

            public void Bind(
                CheatsOptionControlView view,
                CheatOptionDefinition option,
                CheatsOptionBindingContext context,
                Action<Action> trackUpdate)
            {
                if (view is not CheatsValueOptionView valueView || option.ValueData == null)
                {
                    return;
                }

                BindChrome(valueView, option, context);
                trackUpdate?.Invoke(() =>
                    valueView.TitleText.text = CheatsTabUiUtility.FormatValue(option.ValueData.Getter?.Invoke()));
            }
        }

        private sealed class DropdownOptionControlHandler : ICheatsOptionControlHandler
        {
            public CheatsOptionControlKind Kind => CheatsOptionControlKind.Dropdown;

            public CheatsOptionControlView CreateView(CheatsOptionsControlFactory factory, Transform parent)
            {
                return factory.CreateDropdown(parent);
            }

            public void Bind(
                CheatsOptionControlView view,
                CheatOptionDefinition option,
                CheatsOptionBindingContext context,
                Action<Action> trackUpdate)
            {
                if (view is not CheatsDropdownOptionView dropdownView || option.ValueData == null)
                {
                    return;
                }

                BindChrome(dropdownView, option, context);
                CheatsDropdownOptionBinder.Bind(dropdownView, option.ValueData, trackUpdate);
            }
        }

        private sealed class SliderOptionControlHandler : ICheatsOptionControlHandler
        {
            public CheatsOptionControlKind Kind => CheatsOptionControlKind.Slider;

            public CheatsOptionControlView CreateView(CheatsOptionsControlFactory factory, Transform parent)
            {
                return factory.CreateSlider(parent);
            }

            public void Bind(
                CheatsOptionControlView view,
                CheatOptionDefinition option,
                CheatsOptionBindingContext context,
                Action<Action> trackUpdate)
            {
                if (view is not CheatsSliderOptionView sliderView || option.ValueData == null)
                {
                    return;
                }

                BindChrome(sliderView, option, context);
                CheatsNumericOptionBinder.BindSlider(sliderView, option.ValueData, trackUpdate);
            }
        }

        private sealed class SelectorOptionControlHandler : ICheatsOptionControlHandler
        {
            public CheatsOptionControlKind Kind => CheatsOptionControlKind.Selector;

            public CheatsOptionControlView CreateView(CheatsOptionsControlFactory factory, Transform parent)
            {
                return factory.CreateSelector(parent);
            }

            public void Bind(
                CheatsOptionControlView view,
                CheatOptionDefinition option,
                CheatsOptionBindingContext context,
                Action<Action> trackUpdate)
            {
                if (view is not CheatsSelectorOptionView selectorView || option.ValueData == null)
                {
                    return;
                }

                BindChrome(selectorView, option, context);
                CheatsNumericOptionBinder.BindSelector(selectorView, option.ValueData, trackUpdate);
            }
        }

        private sealed class NumericInputOptionControlHandler : ICheatsOptionControlHandler
        {
            public CheatsOptionControlKind Kind => CheatsOptionControlKind.NumericInput;

            public CheatsOptionControlView CreateView(CheatsOptionsControlFactory factory, Transform parent)
            {
                return factory.CreateInput(parent);
            }

            public void Bind(
                CheatsOptionControlView view,
                CheatOptionDefinition option,
                CheatsOptionBindingContext context,
                Action<Action> trackUpdate)
            {
                if (view is not CheatsInputOptionView inputView || option.ValueData == null)
                {
                    return;
                }

                BindChrome(inputView, option, context);
                CheatsNumericOptionBinder.BindInput(inputView, option.ValueData, trackUpdate);
            }
        }

        private sealed class TextInputOptionControlHandler : ICheatsOptionControlHandler
        {
            public CheatsOptionControlKind Kind => CheatsOptionControlKind.TextInput;

            public CheatsOptionControlView CreateView(CheatsOptionsControlFactory factory, Transform parent)
            {
                return factory.CreateInput(parent);
            }

            public void Bind(
                CheatsOptionControlView view,
                CheatOptionDefinition option,
                CheatsOptionBindingContext context,
                Action<Action> trackUpdate)
            {
                if (view is not CheatsInputOptionView inputView || option.ValueData == null)
                {
                    return;
                }

                var valueData = option.ValueData;
                BindChrome(inputView, option, context);
                CheatsInputOptionBinder.Bind(
                    inputView,
                    value => valueData.Setter?.Invoke(value),
                    () => valueData.Getter?.Invoke() as string ?? string.Empty,
                    trackUpdate);
            }
        }

        private sealed class FallbackValueOptionControlHandler : ICheatsOptionControlHandler
        {
            public CheatsOptionControlKind Kind => CheatsOptionControlKind.FallbackValue;

            public CheatsOptionControlView CreateView(CheatsOptionsControlFactory factory, Transform parent)
            {
                return factory.CreateValue(parent);
            }

            public void Bind(
                CheatsOptionControlView view,
                CheatOptionDefinition option,
                CheatsOptionBindingContext context,
                Action<Action> trackUpdate)
            {
                if (view is not CheatsValueOptionView valueView || option.ValueData == null)
                {
                    return;
                }

                BindChrome(valueView, option, context);
                trackUpdate?.Invoke(() =>
                    valueView.TitleText.text = CheatsTabUiUtility.FormatValue(option.ValueData.Getter?.Invoke()));
            }
        }

        private static void BindChrome(
            CheatsOptionControlView view,
            CheatOptionDefinition option,
            CheatsOptionBindingContext context)
        {
            CheatsOptionVisualBinder.Bind(
                view,
                option,
                context.PinnedContext,
                context.PinSelectionMode,
                context.IsOptionPinned,
                context.IsCategoryPinned,
                context.ToggleOptionPin);
        }

        private static void UpdateToggleButtonState(CheatsToggleOptionView view, bool isActive)
        {
            CheatsUiUtility.UpdateStateButton(view.ToggleButton, null, isActive, null, null);

            var uiSettings = CheatsSettingsProxy.UiSettings;
            var icon = isActive
                ? uiSettings.ToggleActiveIcon
                : uiSettings.ToggleInactiveIcon;
            view.ToggleButtonIcon.sprite = icon;
            view.ToggleButtonIcon.preserveAspect = true;
            view.ToggleButtonIcon.gameObject.SetActive(icon != null);
        }
    }
}
