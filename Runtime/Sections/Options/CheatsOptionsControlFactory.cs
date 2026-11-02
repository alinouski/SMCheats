using UnityEngine;

namespace SMCheats
{
    internal sealed class CheatsOptionsControlFactory
    {
        private readonly ICheatsUiElementFactory _uiFactory;

        public CheatsOptionsControlFactory(ICheatsUiElementFactory uiFactory)
        {
            _uiFactory = uiFactory;
        }

        public CheatsActionOptionView CreateAction(Transform parent)
        {
            return _uiFactory.InstantiatePrefabView<CheatsActionOptionView>(CheatsPrefabResourcePaths.ActionOption, parent);
        }

        public CheatsToggleOptionView CreateToggle(Transform parent)
        {
            return _uiFactory.InstantiatePrefabView<CheatsToggleOptionView>(CheatsPrefabResourcePaths.ToggleOption, parent);
        }

        public CheatsDropdownOptionView CreateDropdown(Transform parent)
        {
            return _uiFactory.InstantiatePrefabView<CheatsDropdownOptionView>(CheatsPrefabResourcePaths.DropdownOption, parent);
        }

        public CheatsSliderOptionView CreateSlider(Transform parent)
        {
            return _uiFactory.InstantiatePrefabView<CheatsSliderOptionView>(CheatsPrefabResourcePaths.SliderOption, parent);
        }

        public CheatsSelectorOptionView CreateSelector(Transform parent)
        {
            return _uiFactory.InstantiatePrefabView<CheatsSelectorOptionView>(CheatsPrefabResourcePaths.SelectorOption, parent);
        }

        public CheatsInputOptionView CreateInput(Transform parent)
        {
            return _uiFactory.InstantiatePrefabView<CheatsInputOptionView>(CheatsPrefabResourcePaths.InputOption, parent);
        }

        public CheatsValueOptionView CreateValue(Transform parent)
        {
            return _uiFactory.InstantiatePrefabView<CheatsValueOptionView>(CheatsPrefabResourcePaths.ValueOption, parent);
        }

        public CheatsOptionControlView Create(CheatsOptionControlKind kind, Transform parent)
        {
            switch (kind)
            {
                case CheatsOptionControlKind.Action:
                    return CreateAction(parent);
                case CheatsOptionControlKind.Toggle:
                    return CreateToggle(parent);
                case CheatsOptionControlKind.Dropdown:
                    return CreateDropdown(parent);
                case CheatsOptionControlKind.Slider:
                    return CreateSlider(parent);
                case CheatsOptionControlKind.Selector:
                    return CreateSelector(parent);
                case CheatsOptionControlKind.NumericInput:
                case CheatsOptionControlKind.TextInput:
                    return CreateInput(parent);
                case CheatsOptionControlKind.ReadOnlyValue:
                case CheatsOptionControlKind.FallbackValue:
                default:
                    return CreateValue(parent);
            }
        }
    }
}
