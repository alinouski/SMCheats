using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsConsoleTabButtonView : CheatsButtonView
    {
        [SerializeField] private Text _logCountText;
        [SerializeField] private Text _warningCountText;
        [SerializeField] private Text _errorCountText;

        public void SetCounts(int logCount, int warningCount, int errorAndExceptionCount)
        {
            SetCount(_logCountText, logCount);
            SetCount(_warningCountText, warningCount);
            SetCount(_errorCountText, errorAndExceptionCount);
        }

        private static void SetCount(Text text, int count)
        {
            text.text = count.ToString(CultureInfo.InvariantCulture);
        }
    }
}
