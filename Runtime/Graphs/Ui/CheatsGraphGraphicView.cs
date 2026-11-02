using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class CheatsGraphGraphicView : MaskableGraphic, IPointerClickHandler
    {
        private CheatsGraphDataSource _source;

        public event Action Clicked;

        public void SetSource(CheatsGraphDataSource source)
        {
            _source = source;
            SetVerticesDirty();
        }

        public void Refresh()
        {
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            CheatsGraphMeshUtility.Populate(vh, rectTransform.rect, _source);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke();
        }
    }
}
