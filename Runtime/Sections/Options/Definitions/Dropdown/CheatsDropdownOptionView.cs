using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsDropdownOptionView : CheatsOptionControlView
    {
        [SerializeField] private Dropdown _dropdown;

        public Dropdown Dropdown => _dropdown;
    }
}
