using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsDropdown : Dropdown
    {
        protected override GameObject CreateDropdownList(GameObject template)
        {
            var list = Instantiate(template, template.transform.parent, false);
            var rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
            var popupCanvas = list.GetComponent<Canvas>();
            popupCanvas.overrideSorting = true;
            popupCanvas.sortingLayerID = rootCanvas.sortingLayerID;
            popupCanvas.sortingOrder = rootCanvas.sortingOrder + 2;
            return list;
        }
    }
}
