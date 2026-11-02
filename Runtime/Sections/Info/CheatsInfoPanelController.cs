using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SMCheats
{
    internal sealed class CheatsInfoPanelController : CheatsPanelControllerBase
    {
        private readonly List<InfoValueBinding> _valueBindings = new();
        private CheatsSystemPanelView _view;
        private IDisposable _layoutRebuildDisposable;

        public override string Id => CheatsTabUiUtility.Info;
        public override int Order => 0;
        public override CheatsBuiltInTabs BuiltInFlag => CheatsBuiltInTabs.Info;

        protected override List<IDisposable> BuildHeaderActions()
        {
            var refreshButton = UiFactory.CreateHeaderActionButton(null, UiSettings.UpdateIcon, null);
            AddHeaderButton(refreshButton);
            refreshButton.Button.onClick.AddListener(RefreshValues);

            return new List<IDisposable>();
        }

        protected override (GameObject, IDisposable) BuildMainContent()
        {
            var resourcePath = CheatsPrefabResourcePaths.Main(Id);
            var view = UiFactory.InstantiatePrefabView<CheatsSystemPanelView>(resourcePath, RuntimeUi.MainContentRoot);
            _view = view;
            BuildCategories(view);
            RefreshValues();

            var contentDisposable = CheatsPanelUtility.GetDestroyDisposable(view.gameObject).Item2;
            return (view.gameObject, new DisposableAction(() =>
            {
                _layoutRebuildDisposable?.Dispose();
                _layoutRebuildDisposable = null;
                _view = null;
                contentDisposable.Dispose();
            }));
        }

        public override void OnSelected()
        {
            base.OnSelected();
            RefreshValues();
            RebuildLayoutOnNextLateUpdate();
        }

        private void RebuildLayoutOnNextLateUpdate()
        {
            _layoutRebuildDisposable?.Dispose();
            _layoutRebuildDisposable = _view.CategoriesRoot.RebuildLayoutOnNextLateUpdate();
        }

        private void BuildCategories(CheatsSystemPanelView view)
        {
            _valueBindings.Clear();
            foreach (var categoryDefinition in CreateCategoryDefinitions())
            {
                var categoryView = view.AddCategory(categoryDefinition.Title);
                foreach (var valueDefinition in categoryDefinition.Values)
                {
                    var entryView = categoryView.AddValue(valueDefinition.Title, string.Empty);
                    _valueBindings.Add(new InfoValueBinding(entryView, valueDefinition.Getter));
                }
            }
        }

        private void RefreshValues()
        {
            foreach (var binding in _valueBindings)
            {
                binding.Refresh();
            }
        }

        private static InfoCategoryDefinition[] CreateCategoryDefinitions()
        {
            return new[]
            {
                new InfoCategoryDefinition("System", new[]
                {
                    Value("Operating System", () => Text(SystemInfo.operatingSystem)),
                    Value("Device Name", () => Text(SystemInfo.deviceName)),
                    Value("Device Type", () => SystemInfo.deviceType.ToString()),
                    Value("Device Model", () => Text(SystemInfo.deviceModel)),
                    Value("CPU Type", () => Text(SystemInfo.processorType)),
                    Value("CPU Count", () => Number(SystemInfo.processorCount)),
                    Value("System Memory", () => Megabytes(SystemInfo.systemMemorySize))
                }),
                new InfoCategoryDefinition("Battery", new[]
                {
                    Value("Status", () => SystemInfo.batteryStatus.ToString()),
                    Value("Battery Level", () => Percent(SystemInfo.batteryLevel))
                }),
                new InfoCategoryDefinition("Unity", new[]
                {
                    Value("Version", () => Text(Application.unityVersion)),
                    Value("Debug", () => Bool(UnityEngine.Debug.isDebugBuild)),
                    Value("Unity Pro", () => Bool(Application.HasProLicense())),
                    Value("Genuine", () => Bool(Application.genuine)),
                    Value("System Language", () => Application.systemLanguage.ToString()),
                    Value("Platform", () => Application.platform.ToString()),
                    Value("Install Mode", () => Application.installMode.ToString()),
                    Value("Sandbox", () => Application.sandboxType.ToString()),
                    Value("IL2CPP", GetIl2CppState),
                    Value("Application Version", () => Text(Application.version)),
                    Value("Application Id", () => Text(Application.identifier))
                }),
                new InfoCategoryDefinition("Display", new[]
                {
                    Value("Resolution", () => $"{Screen.width}x{Screen.height}"),
                    Value("DPI", () => Screen.dpi > 0f ? Decimal(Screen.dpi) : "Unknown"),
                    Value("Fullscreen", () => Bool(Screen.fullScreen)),
                    Value("Fullscreen Mode", () => Screen.fullScreenMode.ToString()),
                    Value("Orientation", () => Screen.orientation.ToString())
                }),
                new InfoCategoryDefinition("Runtime", new[]
                {
                    Value("Play Time", () => Duration(Time.realtimeSinceStartupAsDouble)),
                    Value("Level Play Time", () => Duration(Time.timeSinceLevelLoadAsDouble)),
                    Value("Current Level", GetCurrentLevel),
                    Value("Quality Level", GetQualityLevel)
                }),
                new InfoCategoryDefinition("Features", new[]
                {
                    Value("Location", () => Bool(SystemInfo.supportsLocationService)),
                    Value("Accelerometer", () => Bool(SystemInfo.supportsAccelerometer)),
                    Value("Gyroscope", () => Bool(SystemInfo.supportsGyroscope)),
                    Value("Vibration", () => Bool(SystemInfo.supportsVibration)),
                    Value("Audio", () => Bool(SystemInfo.supportsAudio))
                }),
                new InfoCategoryDefinition("Graphics - Device", new[]
                {
                    Value("Device Name", () => Text(SystemInfo.graphicsDeviceName)),
                    Value("Device Vendor", () => Text(SystemInfo.graphicsDeviceVendor)),
                    Value("Device Version", () => Text(SystemInfo.graphicsDeviceVersion)),
                    Value("Graphics Memory", () => Megabytes(SystemInfo.graphicsMemorySize)),
                    Value("Max Tex Size", () => Number(SystemInfo.maxTextureSize))
                }),
                new InfoCategoryDefinition("Graphics - Features", new[]
                {
                    Value("UV Starts at top", () => Bool(SystemInfo.graphicsUVStartsAtTop)),
                    Value("Shader Level", () => Number(SystemInfo.graphicsShaderLevel)),
                    Value("Multi Threaded", () => Bool(SystemInfo.graphicsMultiThreaded)),
                    Value("Hidden Surface Removal (GPU)", () => Bool(SystemInfo.hasHiddenSurfaceRemovalOnGPU)),
                    Value("Uniform Array Indexing (Fragment Shaders)",
                        () => Bool(SystemInfo.hasDynamicUniformArrayIndexingInFragmentShaders)),
                    Value("Shadows", () => Bool(SystemInfo.supportsShadows)),
                    Value("Raw Depth Sampling (Shadows)", () => Bool(SystemInfo.supportsRawShadowDepthSampling)),
                    Value("Motion Vectors", () => Bool(SystemInfo.supportsMotionVectors)),
                    Value("Cubemaps", () => Bool(true)),
                    Value("Image Effects", () => Bool(true)),
                    Value("3D Textures", () => Bool(SystemInfo.supports3DTextures)),
                    Value("2D Array Textures", () => Bool(SystemInfo.supports2DArrayTextures)),
                    Value("3D Render Textures", () => Bool(SystemInfo.supports3DRenderTextures)),
                    Value("Cubemap Array Textures", () => Bool(SystemInfo.supportsCubemapArrayTextures)),
                    Value("Copy Texture Support", () => SystemInfo.copyTextureSupport.ToString()),
                    Value("Compute Shaders", () => Bool(SystemInfo.supportsComputeShaders)),
                    Value("Instancing", () => Bool(SystemInfo.supportsInstancing)),
                    Value("Hardware Quad Topology", () => Bool(SystemInfo.supportsHardwareQuadTopology)),
                    Value("32-bit index buffer", () => Bool(SystemInfo.supports32bitsIndexBuffer)),
                    Value("Sparse Textures", () => Bool(SystemInfo.supportsSparseTextures)),
                    Value("Render Target Count", () => Number(SystemInfo.supportedRenderTargetCount)),
                    Value("Separated Render Targets Blend", () => Bool(SystemInfo.supportsSeparatedRenderTargetsBlend)),
                    Value("Multisampled Textures", () => Number(SystemInfo.supportsMultisampledTextures)),
                    Value("Texture Wrap Mirror Once", () => Number(SystemInfo.supportsTextureWrapMirrorOnce)),
                    Value("Reversed Z Buffer", () => Bool(SystemInfo.usesReversedZBuffer))
                })
            };
        }

        private static InfoValueDefinition Value(string title, Func<string> getter)
        {
            return new InfoValueDefinition(title, getter);
        }

        private static string Text(string value)
        {
            return string.IsNullOrEmpty(value) ? "Unknown" : value;
        }

        private static string Bool(bool value)
        {
            return value ? "True" : "False";
        }

        private static string Number(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        private static string Megabytes(int value)
        {
            return value > 0 ? $"{value.ToString(CultureInfo.InvariantCulture)} MB" : "Unknown";
        }

        private static string Percent(float value)
        {
            return value >= 0f ? $"{(value * 100f).ToString("0.#", CultureInfo.InvariantCulture)}%" : "Unknown";
        }

        private static string Decimal(float value)
        {
            return value.ToString("0.##", CultureInfo.InvariantCulture);
        }

        private static string Duration(double seconds)
        {
            var duration = TimeSpan.FromSeconds(Math.Max(0d, seconds));
            return duration.TotalHours >= 1d
                ? duration.ToString(@"h\:mm\:ss", CultureInfo.InvariantCulture)
                : duration.ToString(@"m\:ss", CultureInfo.InvariantCulture);
        }

        private static string GetCurrentLevel()
        {
            var scene = SceneManager.GetActiveScene();
            return string.IsNullOrEmpty(scene.name) ? scene.buildIndex.ToString(CultureInfo.InvariantCulture) : scene.name;
        }

        private static string GetQualityLevel()
        {
            var qualityLevel = QualitySettings.GetQualityLevel();
            var qualityNames = QualitySettings.names;
            if (qualityNames != null && qualityLevel >= 0 && qualityLevel < qualityNames.Length)
            {
                return qualityNames[qualityLevel];
            }

            return qualityLevel.ToString(CultureInfo.InvariantCulture);
        }

        private static string GetIl2CppState()
        {
#if ENABLE_IL2CPP
            return "True";
#else
            return "False";
#endif
        }

        private readonly struct InfoCategoryDefinition
        {
            public InfoCategoryDefinition(string title, InfoValueDefinition[] values)
            {
                Title = title;
                Values = values ?? Array.Empty<InfoValueDefinition>();
            }

            public string Title { get; }
            public InfoValueDefinition[] Values { get; }
        }

        private readonly struct InfoValueDefinition
        {
            public InfoValueDefinition(string title, Func<string> getter)
            {
                Title = title;
                Getter = getter;
            }

            public string Title { get; }
            public Func<string> Getter { get; }
        }

        private readonly struct InfoValueBinding
        {
            private readonly CheatsInfoEntryView _view;
            private readonly Func<string> _getter;

            public InfoValueBinding(CheatsInfoEntryView view, Func<string> getter)
            {
                _view = view;
                _getter = getter;
            }

            public void Refresh()
            {
                _view.SetValue(_view.TitleText.text, _getter.Invoke());
            }
        }
    }
}
