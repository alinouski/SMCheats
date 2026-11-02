
namespace SMCheats
{
    public struct CheatsGraphFrameSample
    {
        public CheatsGraphFrameSample(float value)
        {
            Value = value;
            FramePart0 = float.NegativeInfinity;
            FramePart1 = float.NegativeInfinity;
            FramePart2 = float.NegativeInfinity;
            FramePart3 = float.NegativeInfinity;
            FramePart4 = float.NegativeInfinity;
            FramePart5 = float.NegativeInfinity;
        }

        public float Value { get; }
        public float FramePart0 { get; set; }
        public float FramePart1 { get; set; }
        public float FramePart2 { get; set; }
        public float FramePart3 { get; set; }
        public float FramePart4 { get; set; }
        public float FramePart5 { get; set; }

        public int PartsCount
        {
            get
            {
                if (float.IsNegativeInfinity(FramePart0))
                    return 0;
                if (float.IsNegativeInfinity(FramePart1))
                    return 1;
                if (float.IsNegativeInfinity(FramePart2))
                    return 2;
                if (float.IsNegativeInfinity(FramePart3))
                    return 3;
                if (float.IsNegativeInfinity(FramePart4))
                    return 4;
                if (float.IsNegativeInfinity(FramePart5))
                    return 5;

                return 6;
            }
        }

        public void SetPart(int index, float value)
        {
            switch (index)
            {
                case 0:
                {
                    FramePart0 = value;
                    return;
                }
                case 1:
                {
                    FramePart1 = value;
                    return;
                }
                case 2:
                {
                    FramePart2 = value;
                    return;
                }
                case 3:
                {
                    FramePart3 = value;
                    return;
                }
                case 4:
                {
                    FramePart4 = value;
                    return;
                }
                case 5:
                {
                    FramePart5 = value;
                    return;
                }
            }
        }

        public bool TryGetPart(int index, out float value)
        {
            switch (index)
            {
                case 0:
                {
                    value = FramePart0;
                    return float.IsNegativeInfinity(FramePart0);
                }
                case 1:
                {
                    value = FramePart1;
                    return float.IsNegativeInfinity(FramePart1);
                }
                case 2:
                {
                    value = FramePart2;
                    return float.IsNegativeInfinity(FramePart2);
                }
                case 3:
                {
                    value = FramePart3;
                    return float.IsNegativeInfinity(FramePart3);
                }
                case 4:
                {
                    value = FramePart4;
                    return float.IsNegativeInfinity(FramePart5);
                }
                default:
                    value = 0;
                    return false;
            }
        }
    }
}