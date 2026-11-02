using System.Collections.Generic;
using UnityEngine;

namespace SMCheats
{
    public readonly struct CheatsGraphDisplayConfig
    {
        public CheatsGraphDisplayConfig(
            CheatsGraphMode mode,
            Color backgroundColor,
            Color dataColor,
            IReadOnlyList<Color> auxiliaryDataColors,
            float barGap,
            float lineThickness)
        {
            Mode = mode;
            BackgroundColor = backgroundColor;
            DataColor = dataColor;
            AuxiliaryDataColors = CopyColors(auxiliaryDataColors);
            BarGap = barGap;
            LineThickness = lineThickness;
        }

        public CheatsGraphMode Mode { get; }
        public Color BackgroundColor { get; }
        public Color DataColor { get; }
        public IReadOnlyList<Color> AuxiliaryDataColors { get; }
        public float BarGap { get; }
        public float LineThickness { get; }

        public Color GetDataColor(int dataIndex)
        {
            if (dataIndex <= 0 || AuxiliaryDataColors == null || AuxiliaryDataColors.Count == 0)
            {
                return DataColor;
            }

            return AuxiliaryDataColors[(dataIndex - 1) % AuxiliaryDataColors.Count];
        }

        private static Color[] CopyColors(IReadOnlyList<Color> colors)
        {
            if (colors == null || colors.Count == 0)
            {
                return new Color[0];
            }

            var copy = new Color[colors.Count];
            for (var i = 0; i < colors.Count; i++)
            {
                copy[i] = colors[i];
            }

            return copy;
        }
    }
}