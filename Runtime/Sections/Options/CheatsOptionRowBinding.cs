using UnityEngine;

namespace SMCheats
{
    internal sealed class CheatsOptionRowBinding
    {
        public CheatsOptionRowBinding(CheatsOptionControlKind kind, CheatsOptionControlView view)
        {
            Kind = kind;
            View = view;
        }

        public CheatsOptionControlKind Kind { get; }
        public CheatsOptionControlView View { get; }
        public GameObject Root => View.gameObject;

        public void Activate(Transform parent)
        {
            var viewTransform = View.transform;
            if (parent != null && viewTransform.parent != parent)
            {
                viewTransform.SetParent(parent, false);
            }

            viewTransform.SetAsLastSibling();
            View.gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            View.gameObject.SetActive(false);
        }
    }
}
