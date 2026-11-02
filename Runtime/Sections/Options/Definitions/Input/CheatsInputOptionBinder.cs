using System;

namespace SMCheats
{
    internal static class CheatsInputOptionBinder
    {
        public static void Bind(
            CheatsInputOptionView view,
            Action<string> applyValue,
            Func<string> getText,
            Action<Action> trackUpdate)
        {
            var inputField = view.InputField;
            inputField.onEndEdit.RemoveAllListeners();
            inputField.onEndEdit.AddListener(value => applyValue?.Invoke(value));
            trackUpdate?.Invoke(() =>
            {
                if (!inputField.isFocused)
                {
                    inputField.text = getText?.Invoke() ?? string.Empty;
                }
            });
        }
    }
}
