using System;
using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    internal static class CheatsOptionVisualBinder
    {
        public static void Bind(
            CheatsOptionControlView view,
            CheatOptionDefinition option,
            bool pinnedContext,
            bool pinSelectionMode,
            Func<CheatOptionDefinition, bool> isOptionPinned,
            Func<string, bool> isCategoryPinned,
            Action<CheatOptionDefinition> toggleOptionPin)
        {
            view.TitleText.text = option.Name;

            var selectionModeActive = pinSelectionMode;
            view.SelectionButton.onClick.RemoveAllListeners();
            view.SelectionButton.interactable = selectionModeActive;
            if (selectionModeActive)
            {
                view.SelectionButton.onClick.AddListener(() => toggleOptionPin?.Invoke(option));
            }

            SetOptionControlInteractivity(view, !selectionModeActive);
            UpdateOptionChromeState(view, option, pinnedContext, pinSelectionMode, isOptionPinned, isCategoryPinned);
        }

        private static void UpdateOptionChromeState(
            CheatsOptionControlView view,
            CheatOptionDefinition option,
            bool pinnedContext,
            bool pinSelectionMode,
            Func<CheatOptionDefinition, bool> isOptionPinned,
            Func<string, bool> isCategoryPinned)
        {
            if (!pinSelectionMode)
            {
                var uiSettings = CheatsSettingsProxy.UiSettings;
                view.BackgroundImage.color = uiSettings.OptionBackgroundColor;
                UpdateControlButtonChrome(view, uiSettings.OptionControlBackgroundColor);
                return;
            }

            var isPinned = (isCategoryPinned?.Invoke(option.Category) ?? false) ||
                           (isOptionPinned?.Invoke(option) ?? false);
            var color = CheatsSettingsProxy.UiSettings.GetPinStateColor(
                pinnedContext || isPinned
                    ? CheatsViewElementPinState.Pinned
                    : CheatsViewElementPinState.ReadyToPin);
            view.BackgroundImage.color = color;
            UpdateControlButtonChrome(view, color);
        }

        private static void SetOptionControlInteractivity(CheatsOptionControlView view, bool isInteractive)
        {
            switch (view)
            {
                case CheatsActionOptionView actionView:
                    SetSelectableInteractivity(actionView.ActionButton, isInteractive);
                    break;

                case CheatsToggleOptionView toggleView:
                    SetSelectableInteractivity(toggleView.ToggleButton, isInteractive);
                    break;

                case CheatsDropdownOptionView dropdownView:
                    SetSelectableInteractivity(dropdownView.Dropdown, isInteractive);
                    break;

                case CheatsSliderOptionView sliderView:
                    SetSelectableInteractivity(sliderView.Slider, isInteractive);
                    break;

                case CheatsSelectorOptionView selectorView:
                    SetSelectableInteractivity(selectorView.PreviousButton, isInteractive);
                    SetSelectableInteractivity(selectorView.NextButton, isInteractive);
                    break;

                case CheatsInputOptionView inputView:
                    SetSelectableInteractivity(inputView.InputField, isInteractive);
                    break;
            }
        }

        private static void SetSelectableInteractivity(Selectable selectable, bool isInteractive)
        {
            selectable.interactable = isInteractive;
            selectable.enabled = isInteractive;
        }

        private static void UpdateControlButtonChrome(CheatsOptionControlView view, Color color)
        {
            if (view is CheatsActionOptionView actionView)
            {
                actionView.ActionButton.targetGraphic.color = color;
                return;
            }

            if (view is CheatsToggleOptionView toggleView)
            {
                toggleView.ToggleButton.targetGraphic.color = color;
            }
        }
    }
}
