using ImGuiNET;
using System;
using System.Numerics;

namespace ActionEffectRange.Drawing.Types
{
    public abstract class ConeAoEDrawData : DrawData
    {
        public readonly Vector3 Origin;
        public readonly float Rotation;
        public readonly float Radius;
        public readonly byte Width;
        public readonly Vector3 End;
        public readonly float Ratio;


        public ConeAoEDrawData(Vector3 origin, byte baseEffectRange, byte xAxisModifier, float rotation, uint ringColour, uint fillColour, float ratio = .25f)
            : base(ringColour, fillColour)
        {
            Origin = origin;
            Radius = baseEffectRange + .5f; 
            Width = xAxisModifier;
            Rotation = rotation;
            End = new Vector3(Origin.X + Radius * MathF.Sin(Rotation), Origin.Y, Origin.Z + Radius * MathF.Cos(Rotation));
            
            Ratio = ratio;
        }

        private void DrawHalfCone(ImDrawListPtr drawList, Vector2 projectedOrigin, Vector2 projectedEnd, int numSegments, bool drawClockwise)
        {
            var points = new Vector2[numSegments];
            var rot = drawClockwise ? Rotation - Ratio * MathF.PI : Rotation + Ratio * MathF.PI;    // rotation +/- (ratio * 2 * pi) / 2
            drawList.PathLineTo(projectedOrigin);
            for (int i = 0; i < numSegments; i++)
            {
                var a = drawClockwise ? i * SegmentAngle + rot : rot - i * SegmentAngle;
                Projection.WorldToScreen(
                    new(Origin.X + Radius * MathF.Sin(a), Origin.Y, Origin.Z + Radius * MathF.Cos(a)),
                    out var p, out var pr);
                // Don't draw the whole range if some of the points may be projected to a weird position
                // We cannot simply ignore it like when we draw Circle AoE because that may truncate part of the cone 
                if (pr.Z < -1)
                {
                    drawList.PathClear();
                    return;
                }
                points[i] = p;
                drawList.PathLineTo(p);
            }
            drawList.PathLineTo(projectedEnd);
            if (Plugin.Config.Filled)
                drawList.PathFillConvex(FillColour);
            if (Plugin.Config.OuterRing)
            {
                if (Plugin.Config.Filled)
                {
                    drawList.PathLineTo(projectedOrigin);
                    foreach (var p in points)
                        drawList.PathLineTo(p);
                    drawList.PathLineTo(projectedEnd);
                }
                drawList.PathStroke(RingColour, ImDrawFlags.None, Plugin.Config.Thickness);
            }
            drawList.PathClear();
        }

        public override void Draw(ImDrawListPtr drawList)
        {
            Projection.WorldToScreen(Origin, out var p0);
            Projection.WorldToScreen(End, out var pe);
#if DEBUG
            drawList.AddCircleFilled(pe, Plugin.Config.Thickness * 2, RingColour);
#endif      

            var numSegmentsHalf = (int)(Ratio * Plugin.Config.NumSegments / 2);

            DrawHalfCone(drawList, p0, pe, numSegmentsHalf, true);
            DrawHalfCone(drawList, p0, pe, numSegmentsHalf, false);
            return;
        }
    }
}
