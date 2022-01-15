using ImGuiNET;
using System;
using System.Numerics;

namespace ActionEffectRange.Drawing.Types
{
    public class DonutAoEDrawData : DrawData
    {
        public readonly Vector3 Centre;
        public readonly int Radius;
        public readonly int InnerRadius;

        public DonutAoEDrawData(Vector3 centre, byte baseEffectRange, byte xAxisModifier, byte innerRadius, uint ringColour, uint fillColour)
            : base(ringColour, fillColour)
        {
            Centre = centre;
            Radius = baseEffectRange + xAxisModifier;
            InnerRadius = innerRadius;
        }

        public override void Draw(ImDrawListPtr drawList)
        {
            if (Plugin.Config.LargeDrawOpt == 1 && Radius >= Plugin.Config.LargeThreshold) return;  // no draw large

            var outerPoints = new Vector2[Plugin.Config.NumSegments];
            var innerPoints = new Vector2[Plugin.Config.NumSegments];
            var seg = 2 * MathF.PI / Plugin.Config.NumSegments;

            var lastValidOuter = new Vector2(float.NaN, float.NaN);
            var lastValidInner = new Vector2(float.NaN, float.NaN);
            var pad = new Vector2(Plugin.Config.Thickness, Plugin.Config.Thickness) / 2;
            for (int i = 0; i < Plugin.Config.NumSegments; i++)
            {
                Projection.WorldToScreen(
                    new(Centre.X + Radius * MathF.Sin(i * seg),
                        Centre.Y,
                        Centre.Z + Radius * MathF.Cos(i * seg)),
                    out var pOuter, out var prOuter);

                // Dont add points that may be projected to weird positions
                if (prOuter.Z < -.1f)
                {
                    outerPoints[i] = new(float.NaN, float.NaN);
                    innerPoints[i] = new(float.NaN, float.NaN);
                }
                else
                {
                    Projection.WorldToScreen(
                    new(Centre.X + InnerRadius * MathF.Sin(i * seg),
                        Centre.Y,
                        Centre.Z + InnerRadius * MathF.Cos(i * seg)),
                    out var pInner, out var _);
                    outerPoints[i] = pOuter;
                    innerPoints[i] = pInner;

                    if (!float.IsNaN(lastValidInner.X) && !float.IsNaN(lastValidOuter.X) 
                        && Plugin.Config.Filled && (Plugin.Config.LargeDrawOpt == 0 || Radius < Plugin.Config.LargeThreshold))
                        drawList.AddQuadFilled(lastValidInner, lastValidOuter, pOuter, pInner, FillColour);
                    
                    lastValidOuter = pOuter - pad;
                    lastValidInner = pInner - pad;
                }
            }

            if (Plugin.Config.OuterRing)
            {
                foreach (var p in innerPoints)
                    if (!float.IsNaN(p.X)) drawList.PathLineTo(p);
                drawList.PathStroke(RingColour, ImDrawFlags.Closed, Plugin.Config.Thickness);
                foreach (var p in outerPoints)
                    if (!float.IsNaN(p.X)) drawList.PathLineTo(p);
                drawList.PathStroke(RingColour, ImDrawFlags.Closed, Plugin.Config.Thickness);
            }
        }
    }
}
