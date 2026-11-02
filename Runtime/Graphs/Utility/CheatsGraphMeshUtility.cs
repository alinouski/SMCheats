using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    public static class CheatsGraphMeshUtility
    {
        public static void Populate(VertexHelper vertexHelper, Rect rect, CheatsGraphDataSource source)
        {
            vertexHelper.Clear();
            if (source == null || rect.width <= 0f || rect.height <= 0f)
            {
                return;
            }

            var config = source.DisplayConfig;
            if (source.SampleCount <= 0)
            {
                return;
            }

            if (config.Mode == CheatsGraphMode.Line)
            {
                var minValue = source.MinValue;
                var maxValue = source.MaxValue;
                if (Mathf.Approximately(minValue, maxValue))
                {
                    minValue = Mathf.Min(0f, minValue);
                }

                maxValue = Mathf.Max(maxValue, minValue + 0.0001f);
                PopulateLine(vertexHelper, rect, source, config, minValue, maxValue);
                return;
            }

            PopulateBars(vertexHelper, rect, source, config, 0f, Mathf.Max(source.DisplayMaxValue, 0.0001f));
        }

        private static void PopulateBars(
            VertexHelper vertexHelper,
            Rect rect,
            CheatsGraphDataSource source,
            CheatsGraphDisplayConfig config,
            float minValue,
            float maxValue)
        {
            var sampleCount = source.SampleCount;
            var step = rect.width / sampleCount;
            var barGap = Mathf.Clamp(config.BarGap, 0f, step * 0.75f);
            var barWidth = Mathf.Max(1f, step - barGap);

            for (var i = 0; i < sampleCount; i++)
            {
                var sample = source.GetSample(i);
                var xMin = rect.xMin + i * step + barGap * 0.5f;
                var xMax = Mathf.Min(xMin + barWidth, rect.xMax);
                if (sample.PartsCount > 0)
                {
                    AddStackedBar(vertexHelper, rect, xMin, xMax, sample, config, minValue, maxValue);
                    continue;
                }

                var value = Mathf.InverseLerp(minValue, maxValue, sample.Value);
                var yMax = rect.yMin + rect.height * Mathf.Clamp01(value);
                AddRect(vertexHelper, xMin, rect.yMin, xMax, yMax, config.GetDataColor(0));
            }
        }

        private static void AddStackedBar(
            VertexHelper vertexHelper,
            Rect rect,
            float xMin,
            float xMax,
            CheatsGraphFrameSample sample,
            CheatsGraphDisplayConfig config,
            float minValue,
            float maxValue)
        {
            var totalValue = Mathf.Max(0f, sample.Value);
            if (totalValue <= 0f)
            {
                return;
            }

            var totalHeight = rect.height * Mathf.Clamp01(Mathf.InverseLerp(minValue, maxValue, sample.Value));
            var y = rect.yMin;
            for (var i = 0; i < sample.PartsCount; i++)
            {
                sample.TryGetPart(i, out var part);
                var height = totalHeight * Mathf.Clamp01(Mathf.Max(0f, part) / totalValue);
                var yMax = Mathf.Min(y + height, rect.yMax);
                AddRect(vertexHelper, xMin, y, xMax, yMax, config.GetDataColor(i));
                y = yMax;
                if (y >= rect.yMax)
                {
                    return;
                }
            }
        }

        private static void PopulateLine(
            VertexHelper vertexHelper,
            Rect rect,
            CheatsGraphDataSource source,
            CheatsGraphDisplayConfig config,
            float minValue,
            float maxValue)
        {
            if (source.SampleCount < 2)
            {
                return;
            }

            for (var i = 1; i < source.SampleCount; i++)
            {
                var fromSample = source.GetSample(i - 1);
                var toSample = source.GetSample(i);
                var from = GetPoint(rect, i - 1, source.SampleCount, fromSample.Value, minValue, maxValue);
                var to = GetPoint(rect, i, source.SampleCount, toSample.Value, minValue, maxValue);
                AddLineSegment(vertexHelper, from, to, config.LineThickness, config.GetDataColor(0));
            }
        }

        private static Vector2 GetPoint(Rect rect, int sampleIndex, int sampleCount, float sample, float minValue, float maxValue)
        {
            var x = rect.xMin + (sampleIndex / (sampleCount - 1f)) * rect.width;
            var y = rect.yMin + Mathf.InverseLerp(minValue, maxValue, sample) * rect.height;
            return new Vector2(x, y);
        }

        private static void AddRect(VertexHelper vertexHelper, float xMin, float yMin, float xMax, float yMax, Color color)
        {
            if (xMax <= xMin || yMax <= yMin || color.a <= 0f)
            {
                return;
            }

            var vertex = UIVertex.simpleVert;
            vertex.color = color;

            var startIndex = vertexHelper.currentVertCount;
            vertex.position = new Vector2(xMin, yMin);
            vertexHelper.AddVert(vertex);
            vertex.position = new Vector2(xMin, yMax);
            vertexHelper.AddVert(vertex);
            vertex.position = new Vector2(xMax, yMax);
            vertexHelper.AddVert(vertex);
            vertex.position = new Vector2(xMax, yMin);
            vertexHelper.AddVert(vertex);

            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex, startIndex + 2, startIndex + 3);
        }

        private static void AddLineSegment(VertexHelper vertexHelper, Vector2 from, Vector2 to, float thickness, Color color)
        {
            var direction = (to - from).normalized;
            if (direction.sqrMagnitude <= 0f || color.a <= 0f)
            {
                return;
            }

            var normal = new Vector2(-direction.y, direction.x) * (Mathf.Max(1f, thickness) * 0.5f);
            var vertex = UIVertex.simpleVert;
            vertex.color = color;

            var startIndex = vertexHelper.currentVertCount;
            vertex.position = from - normal;
            vertexHelper.AddVert(vertex);
            vertex.position = from + normal;
            vertexHelper.AddVert(vertex);
            vertex.position = to + normal;
            vertexHelper.AddVert(vertex);
            vertex.position = to - normal;
            vertexHelper.AddVert(vertex);

            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex, startIndex + 2, startIndex + 3);
        }
    }
}
