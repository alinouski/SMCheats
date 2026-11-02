namespace SMCheats
{
    public static class CheatsPrefabResourcePaths
    {
        private const string Root = "SMCheats";
        private const string PanelRoot = Root + "/Panels";
        private const string UiElementRoot = Root + "/UiElements";
        public const string OptionControlRoot = PanelRoot + "/" + CheatsTabUiUtility.Options + "/Controls";

        public const string CheatsView = Root + "/CheatsView";
        public const string ActivationView = Root + "/CheatsActivationView";
        public const string PinPromptView = Root + "/CheatsPinPromptView";

        public const string Button = UiElementRoot + "/Button";
        public const string HeaderButton = UiElementRoot + "/HeaderButton";
        public const string TabButton = UiElementRoot + "/TabButton";
        public const string ToggleButton = UiElementRoot + "/ToggleButton";
        public const string ConsoleTabButton = UiElementRoot + "/ConsoleTabButton";
        public const string ConsoleFilterButton = UiElementRoot + "/ConsoleFilterButton";
        public const string ConsoleExceptionIndicator = UiElementRoot + "/ConsoleExceptionIndicator";

        public const string InfoCategory = PanelRoot + "/" + CheatsTabUiUtility.Info + "/InfoCategory";
        public const string ProfilerGraph = PanelRoot + "/" + CheatsTabUiUtility.Profiler + "/ProfilerGraph";
        public const string OptionsCategory = PanelRoot + "/" + CheatsTabUiUtility.Options + "/OptionCategory";
        public const string ConsolePinnedPanel = PanelRoot + "/" + CheatsTabUiUtility.Console + "/ConsolePinnedPanel";

        public const string ActionOption = OptionControlRoot + "/ActionOption";
        public const string ToggleOption = OptionControlRoot + "/ToggleOption";
        public const string DropdownOption = OptionControlRoot + "/DropdownOption";
        public const string SliderOption = OptionControlRoot + "/SliderOption";
        public const string SelectorOption = OptionControlRoot + "/SelectorOption";
        public const string InputOption = OptionControlRoot + "/InputOption";
        public const string ValueOption = OptionControlRoot + "/ValueOption";

        public static string Main(string panelId)
        {
            return $"{PanelRoot}/{panelId}/{panelId}Main";
        }

        public static string Pinned(string panelId)
        {
            return $"{PanelRoot}/{panelId}/{panelId}Pinned";
        }
    }
}
