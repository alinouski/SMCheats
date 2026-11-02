using System;
using System.Globalization;
using UnityEngine;

namespace SMCheats
{
    internal static class CheatsNumericOptionBinder
    {
        public static void BindSlider(CheatsSliderOptionView view, CheatValueOptionData valueData, Action<Action> trackUpdate)
        {
            var slider = view.Slider;
            var isRefreshing = false;
            slider.wholeNumbers = valueData.ValueType == typeof(int);
            slider.minValue = (float)(valueData.MinimumValue ?? 0d);
            slider.maxValue = (float)(valueData.MaximumValue ?? 1d);

            slider.onValueChanged.RemoveAllListeners();
            slider.onValueChanged.AddListener(value =>
            {
                if (isRefreshing)
                {
                    return;
                }

                if (valueData.ValueType == typeof(int))
                {
                    valueData.Setter?.Invoke(Mathf.RoundToInt(value));
                }
                else
                {
                    valueData.Setter?.Invoke(value);
                }
            });

            trackUpdate?.Invoke(() =>
            {
                isRefreshing = true;
                try
                {
                    var rawValue = valueData.Getter?.Invoke();
                    var numericValue = valueData.ValueType == typeof(int)
                        ? rawValue is int intValue ? intValue : 0f
                        : rawValue is float floatValue
                            ? floatValue
                            : 0f;
                    slider.SetValueWithoutNotify(numericValue);
                    view.ValueLabel.text = CheatsTabUiUtility.FormatValue(rawValue);
                }
                finally
                {
                    isRefreshing = false;
                }
            });
        }

        public static void BindSelector(CheatsSelectorOptionView view, CheatValueOptionData valueData, Action<Action> trackUpdate)
        {
            CheatsUiUtility.RebindButton(view.PreviousButton, () => NudgeNumericOption(valueData, -1));
            CheatsUiUtility.RebindButton(view.NextButton, () => NudgeNumericOption(valueData, 1));
            trackUpdate?.Invoke(() => view.ValueLabel.text = CheatsTabUiUtility.FormatValue(valueData.Getter?.Invoke()));
        }

        public static void BindInput(CheatsInputOptionView view, CheatValueOptionData valueData, Action<Action> trackUpdate)
        {
            CheatsInputOptionBinder.Bind(
                view,
                value => ApplyNumericOptionText(valueData, value),
                () => CheatsTabUiUtility.FormatValue(valueData.Getter?.Invoke()),
                trackUpdate);
        }

        private static void NudgeNumericOption(CheatValueOptionData valueData, int direction)
        {
            if (valueData.ValueType == typeof(int))
            {
                var current = valueData.Getter?.Invoke() is int currentValue ? currentValue : 0;
                var step = valueData.Step.HasValue ? Mathf.Max(1, Mathf.RoundToInt((float)valueData.Step.Value)) : 1;
                var next = current + (step * direction);
                if (valueData.MinimumValue.HasValue)
                {
                    next = Mathf.Max(next, Mathf.RoundToInt((float)valueData.MinimumValue.Value));
                }

                if (valueData.MaximumValue.HasValue)
                {
                    next = Mathf.Min(next, Mathf.RoundToInt((float)valueData.MaximumValue.Value));
                }

                valueData.Setter?.Invoke(next);
                return;
            }

            if (valueData.ValueType == typeof(float))
            {
                var current = valueData.Getter?.Invoke() is float currentValue ? currentValue : 0f;
                var step = valueData.Step.HasValue ? (float)valueData.Step.Value : 0.1f;
                var next = current + (step * direction);
                if (valueData.MinimumValue.HasValue)
                {
                    next = Mathf.Max(next, (float)valueData.MinimumValue.Value);
                }

                if (valueData.MaximumValue.HasValue)
                {
                    next = Mathf.Min(next, (float)valueData.MaximumValue.Value);
                }

                valueData.Setter?.Invoke(next);
            }
        }

        private static void ApplyNumericOptionText(CheatValueOptionData valueData, string value)
        {
            if (valueData.ValueType == typeof(int) &&
                int.TryParse(value, out var intValue))
            {
                valueData.Setter?.Invoke(intValue);
                return;
            }

            if (valueData.ValueType == typeof(float) &&
                float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var floatValue))
            {
                valueData.Setter?.Invoke(floatValue);
            }
        }
    }
}
