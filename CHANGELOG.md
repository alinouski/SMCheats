# Changelog

## 0.2.0

- Cheats are disabled in non-development builds unless `Enable In Release Builds` is set in `CheatsSettings`.
  The `SMCHEATS_DISABLED` scripting define turns them off everywhere. New `SMCheatsUtility.IsEnabled`.
- Options can be added without attributes: `SMCheatsUtility.AddAction`, `AddToggle`, `AddValue<T>`, `RemoveOption`.
- `CheatsPanelControllerBase` and `CheatsPanelTabButton` are public, so custom tabs can reuse them.
  `Title` and `Icon` of a panel are overridable.
- Options are ordered by `sortOrder`, then by declaration order. Previously `sortOrder` competed with source line numbers.
- Console captures logs from background threads (`Application.logMessageReceivedThreaded`).
- `Fps Threshold` defaults to `(50, 30)` for new settings assets.
- Settings inspector shows `Enable In Release Builds` and the memory graph config.
- Editor namespace renamed from `Cheats.Editor` to `SMCheats.Editor`.
- A missing `CheatsUiColorSettings` is created in `Assets/Resources/SMCheats/Configs` instead of the read-only package folder.

## 0.1.0

- Moved from `Assets/Project/SourceCode/SMCheats` and `Assets/Project/Resources/SMCheats` into an embedded package.
  Script and asset GUIDs are preserved.
- Editor tools resolve prefabs inside the package and find `CheatsSettings` in any project `Resources/SMCheats` folder.
- Color settings fallback loads `SMCheats/Configs/CheatsUiColorSettings`, where the asset actually lives.