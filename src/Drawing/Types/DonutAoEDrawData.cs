using ImGuiNET;

namespace ActionEffectRange.Drawing.Types
{
    public class DonutAoEDrawData : DrawData
    {
        public readonly Vector3 Centre;
        public readonly int Radius;
        public readonly int InnerRadius;

        public DonutAoEDrawData(Vector3 centre, byte baseEffectRange, 
            byte xAxisModifier, byte innerRadius, uint ringColour, uint fillColour)
            : base(ringColour, fillColour)
        {
            Centre = centre;
            Radius = baseEffectRange + xAxisModifier;
            InnerRadius = innerRadius;
        }

        public override void Draw(ImDrawListPtr drawList)
        {
            if (Config.LargeDrawOpt == 1 && Radius >= Config.LargeThreshold) 
                return;  // no draw large

            var outerPoints = new Vector2[Config.NumSegments];
            var innerPoints = new Vector2[Config.NumSegments];
            var seg = 2 * MathF.PI / Config.NumSegments;

            for (int i = 0; i < Config.NumSegments; i++)
            {
                Projection.WorldToScreen(
                    new(Centre.X + Radius * MathF.Sin(i * seg),
                        Centre.Y,
                        Centre.Z + Radius * MathF.Cos(i * seg)),
                    out var pOuter, out var prOuter);
                // Don't add points that may be projected to weird positions
                outerPoints[i] = prOuter.Z < -.5f 
                    ? new(float.NaN, float.NaN) : pOuter;
                Projection.WorldToScreen(
                    new(Centre.X + InnerRadius * MathF.Sin(i * seg),
                        Centre.Y,
                        Centre.Z + InnerRadius * MathF.Cos(i * seg)),
                    out var pInner, out var prInner);
                innerPoints[i] = prInner.Z < -.5f 
                    ? new(float.NaN, float.NaN) : pInner;
            }

            if (Config.Filled 
                && (Config.LargeDrawOpt == 0 || Radius < Config.LargeThreshold))
            {
                for (int i = 0; i < Config.NumSegments - 1; i++)
                {
                    var j = i + 1;
                    if (!float.IsNaN(outerPoints[i].X) 
                        && !float.IsNaN(outerPoints[j].X)
                        && !float.IsNaN(innerPoints[i].X) 
                        && !float.IsNaN(innerPoints[j].X))
                        drawList.AddQuadFilled(outerPoints[i], outerPoints[j], 
                            innerPoints[j], innerPoints[i], FillColour);
                }
                if (!float.IsNaN(outerPoints[0].X) 
                    && !float.IsNaN(outerPoints[^1].X)
                    && !float.IsNaN(innerPoints[0].X) 
                    && !float.IsNaN(innerPoints[^1].X))
                    drawList.AddQuadFilled(outerPoints[0], outerPoints[^1], 
                        innerPoints[^1], innerPoints[0], FillColour);
            }

            if (Config.OuterRing)
            {
                // outer
                for (int i = 0; i < Config.NumSegments; i++)
                    if (!float.IsNaN(outerPoints[i].X)) 
                        drawList.PathLineTo(outerPoints[i]);
                drawList.PathStroke(RingColour, ImDrawFlags.Closed, Config.Thickness);

                // inner
                for (int i = 0; i < Config.NumSegments; i++)
                    if (!float.IsNaN(innerPoints[i].X)) 
                        drawList.PathLineTo(innerPoints[i]);
                drawList.PathStroke(RingColour, ImDrawFlags.Closed, Config.Thickness);
            }
        }
    }
}
