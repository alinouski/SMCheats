using UnityEngine;
using UnityEngine.Serialization;

namespace SMCheats
{
    [CreateAssetMenu(fileName = "CheatsGraphVisualConfig", menuName = "SMCheats/Graphs/Graph Visual Config")]
    public sealed class CheatsGraphVisualConfig : ScriptableObject
    {
        [SerializeField] private CheatsGraphMode _mode = CheatsGraphMode.Bars;
        [SerializeField] private Color _backgroundColor = new(0.1f, 0.36f, 0.44f, 0.86f);
        [FormerlySerializedAs("_normalColor")]
        [SerializeField] private Color _dataColor = new(0.52f, 0.82f, 0.42f, 0.9f);
        [SerializeField] private Color[] _auxiliaryDataColors =
        {
            new(0.87f, 0.83f, 0.31f, 0.9f),
            new(0.9f, 0.38f, 0.36f, 0.9f)
        };
        [SerializeField] private float _barGap = 1f;
        [SerializeField] private float _lineThickness = 3f;

        public CheatsGraphDisplayConfig CreateDisplayConfig()
        {
            return new CheatsGraphDisplayConfig(
                _mode,
                _backgroundColor,
                _dataColor,
                _auxiliaryDataColors,
                _barGap,
                _lineThickness);
        }

        public static CheatsGraphDisplayConfig CreateDefaultDisplayConfig()
        {
            return new CheatsGraphDisplayConfig(
                CheatsGraphMode.Bars,
                new Color(0.1f, 0.36f, 0.44f, 0.86f),
                new Color(0.52f, 0.82f, 0.42f, 0.9f),
                new[]
                {
                    new Color(0.87f, 0.83f, 0.31f, 0.9f),
                    new Color(0.9f, 0.38f, 0.36f, 0.9f)
                },
                1f,
                3f);
        }
    }
}
