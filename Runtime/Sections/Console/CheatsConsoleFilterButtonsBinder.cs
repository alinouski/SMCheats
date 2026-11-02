namespace SMCheats
{
    internal sealed class CheatsConsoleFilterButtonsBinder
    {
        private readonly CheatsConsoleViewState _state;
        private readonly CheatsConsoleFilterButtonView _logsButton;
        private readonly CheatsConsoleFilterButtonView _warningsButton;
        private readonly CheatsConsoleFilterButtonView _errorsButton;

        public CheatsConsoleFilterButtonsBinder(
            CheatsConsoleViewState state,
            CheatsConsoleFilterButtonView logsButton,
            CheatsConsoleFilterButtonView warningsButton,
            CheatsConsoleFilterButtonView errorsButton)
        {
            _state = state;
            _logsButton = logsButton;
            _warningsButton = warningsButton;
            _errorsButton = errorsButton;
        }

        public void Update(int logCount, int warningCount, int errorAndExceptionCount)
        {
            UpdateButton(_logsButton, _state.ShowLogs, logCount);
            UpdateButton(_warningsButton, _state.ShowWarnings, warningCount);
            UpdateButton(_errorsButton, _state.ShowErrors, errorAndExceptionCount);
        }

        private static void UpdateButton(CheatsConsoleFilterButtonView view, bool isActive, int count)
        {
            view.SetFilterActive(isActive);
            view.SetCount(count);
        }
    }
}
