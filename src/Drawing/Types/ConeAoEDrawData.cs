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

        public readonly float Angle;
        public readonly float SegmentRatio;


        public ConeAoEDrawData(Vector3 origin, byte baseEffectRange, byte xAxisModifier, float rotation, uint ringColour, uint fillColour, float centralAngle = (98f / 180f) * MathF.PI)
            : base(ringColour, fillColour)
        {
            Origin = origin;
            Radius = baseEffectRange + .5f; 
            Width = xAxisModifier;
            Rotation = rotation;
            End = new Vector3(Origin.X + Radius * MathF.Sin(Rotation), Origin.Y, Origin.Z + Radius * MathF.Cos(Rotation));
            Angle = centralAngle;   // not sure about the default but close enough; normally its slightly larger than pi/2
            SegmentRatio = Angle / (MathF.PI * 2);
        }


        public override void Draw(ImDrawListPtr drawList)
        {
#if DEBUG
            Projection.WorldToScreen(End, out var pe);
            drawList.AddCircleFilled(pe, Plugin.Config.Thickness * 2, RingColour);
#endif      

            var numSegments = (int)(Plugin.Config.NumSegments * SegmentRatio);
            var points = new Vector2[numSegments + 1];
            var seg = Angle / numSegments;
            var a = Rotation - Angle / 2;
            for (int i = 0; i < numSegments; i++)
            {
                Projection.WorldToScreen(
                    new(Origin.X + Radius * MathF.Sin(i * seg + a),
                        Origin.Y,
                        Origin.Z + Radius * MathF.Cos(i * seg + a)),
                    out var p,
                    out var pr);
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
            Projection.WorldToScreen(Origin, out var p0);
            points[^1] = p0;
            drawList.PathLineTo(p0);

            if (Plugin.Config.Filled)
            {
                drawList.PathFillConvex(FillColour);
                foreach (var p in points)
                    drawList.PathLineTo(p);
            }
            if (Plugin.Config.OuterRing)
                drawList.PathStroke(RingColour, ImDrawFlags.Closed, Plugin.Config.Thickness);
            drawList.PathClear();
        }
    }
}
