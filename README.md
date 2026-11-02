# SM Cheats

Runtime cheats panel for Unity: option controls, console, profiler graphs, device info and PIN-protected activation.

## Dependencies

- `com.unity.ugui`
- `com.unity.inputsystem`

## Setup

1. Add the package (embedded in `Packages/` or by git URL in `Packages/manifest.json`).
2. Run `Tools/Cheats/Create Settings Asset`. It creates `Assets/Resources/SMCheats/CheatsSettings.asset`
   unless a `CheatsSettings` asset already exists at any `Assets/**/Resources/SMCheats/CheatsSettings.asset`.
3. Register cheats from game code through the runtime API (`SMCheatsUtility`, `CheatsController`).

## Layout

- `Runtime/` — runtime assembly `SMCheats.Runtime`.
- `Runtime/Resources/SMCheats/` — prefabs, UI configs and icons loaded through `Resources.Load`.
- `Editor/` — settings inspector, prefab generation and validation (`SMCheats.Editor`).

Project-specific settings stay in the project; the package only ships shared prefabs and configs.