# Changelog

## 0.1.0

- Moved from `Assets/Project/SourceCode/SMCheats` and `Assets/Project/Resources/SMCheats` into an embedded package.
  Script and asset GUIDs are preserved.
- Editor tools resolve prefabs inside the package and find `CheatsSettings` in any project `Resources/SMCheats` folder.
- Color settings fallback loads `SMCheats/Configs/CheatsUiColorSettings`, where the asset actually lives.