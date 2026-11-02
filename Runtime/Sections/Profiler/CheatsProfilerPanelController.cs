using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace SMCheats
{
    internal sealed class CheatsProfilerPanelController : CheatsPanelControllerBase
    {
        private readonly CheatsCpuProfilerCategory[] CpuGraphCategories = {
            CheatsCpuProfilerCategory.Rendering,
            CheatsCpuProfilerCategory.Scripts,
            CheatsCpuProfilerCategory.Physics,
        };

        private CheatsToggleButtonView _pinModeToggleButton;

        public override string Id => CheatsTabUiUtility.Profiler;
        public override int Order => 2;
        public override CheatsBuiltInTabs BuiltInFlag => CheatsBuiltInTabs.Profiler;

        public override void OnUnregistered()
        {
            base.OnUnregistered();
            _pinModeToggleButton = null;
        }

        protected override List<IDisposable> BuildHeaderActions()
        {
            var uiSettings = UiSettings;
            _pinModeToggleButton = UiFactory.CreateToggleButton(
                null,
                uiSettings.ToggleActiveIcon,
                uiSettings.ToggleInactiveIcon);

            AddHeaderButton(_pinModeToggleButton);
            _pinModeToggleButton.Button.onClick.AddListener(TogglePinMode);

            return new List<IDisposable>();
        }

        protected override (GameObject, IDisposable) BuildMainContent()
        {
            var resourcePath = CheatsPrefabResourcePaths.Main(Id);
            var view = UiFactory.InstantiatePrefabView<CheatsProfilerPanelView>(resourcePath, RuntimeUi.MainContentRoot);

            BindFpsContent(view.FpsGraph);
            BindCpuContent(view.CpuGraph);

            RegisterPinnableView(view.FpsGraph, CreatePinnedGraphView, BindPinnedFpsContent);
            RegisterPinnableView(view.CpuGraph, CreatePinnedGraphView, BindPinnedCpuContent);

            return CheatsPanelUtility.GetDestroyDisposable(view.gameObject);
        }

        private void BindFpsContent(CheatsGraphView view)
        {
            BindSelectedGraph(view, CreateFpsGraphSource());
        }

        private void BindCpuContent(CheatsGraphView view)
        {
            BindSelectedGraph(view, CreateCpuGraphSource());
        }

        private IDisposable BindPinnedFpsContent(CheatsGraphView view)
        {
            return BindPinnedGraph(view, CreateFpsGraphSource());
        }

        private IDisposable BindPinnedCpuContent(CheatsGraphView view)
        {
            return BindPinnedGraph(view, CreateCpuGraphSource());
        }

        private void BindSelectedGraph(CheatsGraphView view, CheatsGraphDataSource graphSource)
        {
            graphSource.Tick();
            SelectionState.AddLifecycle(graphSource.Activate, graphSource.Deactivate);
            view.SetSource(graphSource);

            SelectionState.AddUpdate(() =>
            {
                graphSource.Tick();
                view.Refresh();
            });
        }

        private static IDisposable BindPinnedGraph(CheatsGraphView view, CheatsGraphDataSource graphSource)
        {
            graphSource.Tick();
            view.SetSource(graphSource);

            return UpdateUtility.BindAction(() =>
            {
                graphSource.Tick();
                view.Refresh();
            });
        }

        private CheatsGraphDataSource CreateFpsGraphSource()
        {
            var profiler = Services.Get<CheatsFpsProfilerService>();

            return new CheatsGraphDataSource(
                "FPS",
                Settings.ProfilerFpsGraphVisualConfig,
                new ICheatsGraphCurrentValueSource[]
                {
                    new CheatsGraphCurrentValueSource("Min", () => profiler.MinFps, static s => s.ToString("0.0")),
                    new CheatsGraphCurrentValueSource("Max", () => profiler.MaxFps, static s => s.ToString("0.0")),
                    new CheatsGraphCurrentValueSource("Avg", () => profiler.AverageFps, static s => s.ToString("0.0")),
                },
                new CheatsGraphFrameValueSource(() => CreateFpsFrameSample(profiler, Settings)),
                profiler.SampleCapacity);
        }

        private CheatsGraphDataSource CreateCpuGraphSource()
        {
            var cpuProfilerService = Services.Get<ICheatsCpuProfilerService>();
            var viewSettings = Settings.ProfilerCpuGraphVisualConfig;
            var colorConfigs = viewSettings.CreateDisplayConfig();

            return new CheatsGraphDataSource(
                "CPU",
                viewSettings,
                new ICheatsGraphCurrentValueSource[]
                {
                    new CheatsGraphCurrentValueSource(
                        "Min",
                        () => cpuProfilerService.MinFrameTimeMs,
                        static s => s.ToString("0.0 ms", CultureInfo.InvariantCulture)),

                    new CheatsGraphCurrentValueSource(
                        "Max",
                        () => cpuProfilerService.MaxFrameTimeMs,
                        static s => s.ToString("0.0 ms", CultureInfo.InvariantCulture)),

                    new CheatsGraphCurrentValueSource(
                        "Avg",
                        () => cpuProfilerService.AverageFrameTimeMs,
                        static s => s.ToString("0.0 ms", CultureInfo.InvariantCulture)),

                    new CheatsGraphCurrentValueSource(
                        "Scripts",
                        () => cpuProfilerService.GetCategoryMsUsage(CheatsCpuProfilerCategory.Scripts),
                        static s => s.ToString("0.0 ms", CultureInfo.InvariantCulture),
                        colorConfigs.GetDataColor(1)),

                    new CheatsGraphCurrentValueSource(
                        "Rendering",
                        () => cpuProfilerService.GetCategoryMsUsage(CheatsCpuProfilerCategory.Rendering),
                        static s => s.ToString("0.0 ms", CultureInfo.InvariantCulture),
                        colorConfigs.GetDataColor(0)),

                    new CheatsGraphCurrentValueSource(
                        "Physics",
                        () => cpuProfilerService.GetCategoryMsUsage(CheatsCpuProfilerCategory.Physics),
                        static s => s.ToString("0.0 ms", CultureInfo.InvariantCulture),
                        colorConfigs.GetDataColor(2)),
                },
                new CheatsGraphFrameValueSource(() => CreateCpuFrameSample(cpuProfilerService, CpuGraphCategories)),
                cpuProfilerService.SampleCapacity);
        }

        private CheatsGraphView CreatePinnedGraphView()
        {
            return UiFactory.InstantiatePrefabView<CheatsGraphView>(CheatsPrefabResourcePaths.ProfilerGraph, RuntimeUi.PinnedContentRoot);
        }

        private void TogglePinMode()
        {
            SetPinModeActive(!IsPinModeActive);
        }

        protected override void OnPinModeChanged(bool isActive)
        {
            _pinModeToggleButton.IsActive = isActive;
        }

        private static CheatsGraphFrameSample CreateFpsFrameSample(CheatsFpsProfilerService fpsProfiler, CheatsSettings settings)
        {
            var currentFps = fpsProfiler.CurrentFps;
            var frameSample = new CheatsGraphFrameSample(currentFps);

            if (currentFps > settings.FpsThreshold.x)
            {
                return frameSample;
            }

            var isMedium = currentFps > settings.FpsThreshold.y;

            frameSample.SetPart(0, 0);
            frameSample.SetPart(1, isMedium ? currentFps : 0);
            frameSample.SetPart(2, !isMedium ? currentFps : 0);

            return frameSample;
        }

        private static CheatsGraphFrameSample CreateCpuFrameSample(
            ICheatsCpuProfilerService profiler,
            CheatsCpuProfilerCategory[] categories)
        {
            var frameSample = new CheatsGraphFrameSample(profiler.CurrentFrameTimeMs);

            for (var i = 0; i < categories.Length; i++)
            {
                frameSample.SetPart(i, profiler.GetCategoryMsUsage(categories[i]));
            }

            return frameSample;
        }
    }
}
