using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SMCheats
{
    internal static class CheatsDropdownOptionBinder
    {
        private static readonly Dictionary<Type, string[]> EnumLabelsByType = new();

        public static void Bind(CheatsDropdownOptionView view, CheatValueOptionData valueData, Action<Action> trackUpdate)
        {
            var dropdown = view.Dropdown;
            var state = new DropdownBindingState();
            var isRefreshing = false;

            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener(index =>
            {
                if (isRefreshing)
                {
                    return;
                }

                SetDropdownOptionValue(valueData, index);
            });

            trackUpdate?.Invoke(() =>
            {
                isRefreshing = true;
                try
                {
                    RefreshDropdownView(dropdown, valueData, state);
                }
                finally
                {
                    isRefreshing = false;
                }
            });
        }

        private static void RefreshDropdownView(Dropdown dropdown, CheatValueOptionData valueData, DropdownBindingState state)
        {
            FillDropdownLabels(valueData, state);
            SyncDropdownOptions(dropdown, state.Labels);

            var selectedIndex = GetSelectedDropdownIndex(valueData, state);
            dropdown.SetValueWithoutNotify(selectedIndex);
            dropdown.RefreshShownValue();
        }

        private static void FillDropdownLabels(CheatValueOptionData valueData, DropdownBindingState state)
        {
            state.Labels.Clear();
            state.Choices = null;

            if (valueData.DropdownChoicesGetter != null)
            {
                var choices = valueData.DropdownChoicesGetter.Invoke() ?? Array.Empty<CheatDropdownChoice>();
                state.Choices = choices;
                for (var i = 0; i < choices.Count; i++)
                {
                    var choice = choices[i];
                    state.Labels.Add(string.IsNullOrWhiteSpace(choice.Label)
                        ? CheatsTabUiUtility.FormatValue(choice.Value)
                        : choice.Label);
                }

                return;
            }

            if (valueData.ValueType.IsEnum)
            {
                var enumLabels = GetEnumLabels(valueData.ValueType);
                for (var i = 0; i < enumLabels.Length; i++)
                {
                    state.Labels.Add(enumLabels[i]);
                }
            }
        }

        private static void SyncDropdownOptions(Dropdown dropdown, List<string> labels)
        {
            if (dropdown.options.Count != labels.Count)
            {
                dropdown.options.Clear();
                for (var i = 0; i < labels.Count; i++)
                {
                    dropdown.options.Add(new Dropdown.OptionData(labels[i]));
                }

                return;
            }

            for (var i = 0; i < labels.Count; i++)
            {
                dropdown.options[i].text = labels[i];
            }
        }

        private static string[] GetEnumLabels(Type enumType)
        {
            if (!EnumLabelsByType.TryGetValue(enumType, out var labels))
            {
                labels = Enum.GetNames(enumType);
                EnumLabelsByType[enumType] = labels;
            }

            return labels;
        }

        private static int GetSelectedDropdownIndex(CheatValueOptionData valueData, DropdownBindingState state)
        {
            if (state.Labels.Count <= 0)
            {
                return 0;
            }

            var currentValue = valueData.Getter?.Invoke();
            if (valueData.DropdownChoicesGetter != null)
            {
                var choices = state.Choices ?? Array.Empty<CheatDropdownChoice>();
                for (var i = 0; i < choices.Count; i++)
                {
                    if (Equals(choices[i].Value, currentValue))
                    {
                        return i;
                    }
                }

                return 0;
            }

            if (!valueData.ValueType.IsEnum || currentValue == null)
            {
                return 0;
            }

            var values = Enum.GetValues(valueData.ValueType);
            var index = Array.IndexOf(values, currentValue);
            return index >= 0 ? index : 0;
        }

        private static void SetDropdownOptionValue(CheatValueOptionData valueData, int index)
        {
            if (valueData.DropdownChoicesGetter != null)
            {
                var choices = valueData.DropdownChoicesGetter.Invoke() ?? Array.Empty<CheatDropdownChoice>();
                if (index >= 0 && index < choices.Count)
                {
                    valueData.Setter?.Invoke(choices[index].Value);
                }

                return;
            }

            if (valueData.ValueType.IsEnum)
            {
                var values = Enum.GetValues(valueData.ValueType);
                if (index >= 0 && index < values.Length)
                {
                    valueData.Setter?.Invoke(values.GetValue(index));
                }
            }
        }

        private sealed class DropdownBindingState
        {
            public readonly List<string> Labels = new();
            public IReadOnlyList<CheatDropdownChoice> Choices;
        }
    }
}
