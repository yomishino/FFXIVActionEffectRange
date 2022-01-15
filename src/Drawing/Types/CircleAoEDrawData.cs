using ImGuiNET;
using System;
using System.Numerics;

namespace ActionEffectRange.Drawing.Types
{
    public class CircleAoEDrawData : DrawData
    {
        public readonly Vector3 Centre;
        public readonly int Radius;

        public CircleAoEDrawData(Vector3 centre, byte baseEffectRange, byte xAxisModifier, uint ringColour, uint fillColour)
            : base(ringColour, fillColour)
        {
            Centre = centre;
            Radius = baseEffectRange + xAxisModifier;
        }

        public override void Draw(ImDrawListPtr drawList)
        {
            if (Plugin.Config.LargeDrawOpt == 1 && Radius >= Plugin.Config.LargeThreshold) return;  // no draw large

            var points = new Vector2[Plugin.Config.NumSegments];
            var seg = 2 * MathF.PI / Plugin.Config.NumSegments;
            for (int i = 0; i < Plugin.Config.NumSegments; i++)
            {
                Projection.WorldToScreen(
                    new(Centre.X + Radius * MathF.Sin(i * seg),
                        Centre.Y,
                        Centre.Z + Radius * MathF.Cos(i * seg)),
                    out var p, out var pr);

                // Dont add points that may be projected to weird positions
                if (pr.Z < -.1f) points[i] = new(float.NaN, float.NaN);
                else 
                {
                    points[i] = p;
                    drawList.PathLineTo(p);
                }
            }

            if (Plugin.Config.Filled && (Plugin.Config.LargeDrawOpt == 0 || Radius < Plugin.Config.LargeThreshold))
            {
                drawList.PathFillConvex(FillColour);
                foreach (var p in points)
                    if (!float.IsNaN(p.X)) drawList.PathLineTo(p);
            }
            if (Plugin.Config.OuterRing)
                drawList.PathStroke(RingColour, ImDrawFlags.None, Plugin.Config.Thickness);
            drawList.PathClear();
        }
    }
}
