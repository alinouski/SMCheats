using UnityEngine;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsProfilerPanelView : MonoBehaviour
    {
        [SerializeField] 
        private CheatsGraphView _fpsGraph;
        
        [SerializeField] 
        private CheatsGraphView _cpuGraph;

        public CheatsGraphView FpsGraph => _fpsGraph;
        public CheatsGraphView CpuGraph => _cpuGraph;
    }
}
