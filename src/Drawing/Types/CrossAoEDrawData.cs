using ImGuiNET;
using System;
using System.Numerics;

namespace ActionEffectRange.Drawing.Types
{
    public class CrossAoEDrawData : DrawData
    {
        public readonly Vector3 Origin;
        public readonly Vector3 Direction;
        public readonly float Length;
        public readonly float Width;
        public readonly Vector3 End;
        public readonly Vector3 CrossDirection;
        public readonly Vector3 CrossEnd;


        public CrossAoEDrawData(Vector3 origin, Vector3 target, 
            byte baseEffectRange, byte xAxisModifier, 
            bool calculateY, uint ringColour, uint fillColour)
            : base(ringColour, fillColour)
        {
            Origin = origin;
            Direction = Vector3.Normalize(target - origin);
            if (!calculateY) Direction.Y = 0;
            Length = baseEffectRange + .5f; // TODO: do cross has the extra .5 as line?
            Width = xAxisModifier;
            End = Direction * Length + origin;
            CrossDirection = new Vector3(Direction.Z, Direction.Y, -Direction.X);   // TODO: perpendicular?
            CrossEnd = CrossDirection * Length + origin;
        }


        public override void Draw(ImDrawListPtr drawList)
        {
#if DEBUG
            Projection.WorldToScreen(End, out var pe, out var _);
            drawList.AddCircleFilled(pe, Plugin.Config.Thickness * 2, RingColour);
            Projection.WorldToScreen(CrossEnd, out var pc, out var _);
            drawList.AddCircleFilled(pc, Plugin.Config.Thickness * 2, RingColour);
#endif
            // TODO: would be better if not making the shapes overlapped
            var w2 = Width / 2;
            DrawOneLine(drawList, w2, Direction);
            DrawOneLine(drawList, w2, CrossDirection);
        }

        private void DrawOneLine(
            ImDrawListPtr drawList, float halfWidth, Vector3 direction)
        {
            // TODO: -End makes the shape extend to the other end of the direction?
            var p1w = Vector3.Normalize(new Vector3(direction.Z, 0, -direction.X)) * halfWidth - End;
            var p2w = Vector3.Normalize(new Vector3(direction.Z, 0, -direction.X)) * halfWidth + End;
            var p3w = Vector3.Normalize(new Vector3(-direction.Z, 0, direction.X)) * halfWidth + End;
            var p4w = Vector3.Normalize(new Vector3(-direction.Z, 0, direction.X)) * halfWidth - End;

            Projection.WorldToScreen(p1w, out var p1s, out var p1r);
            Projection.WorldToScreen(p2w, out var p2s, out var p2r);
            Projection.WorldToScreen(p3w, out var p3s, out var p3r);
            Projection.WorldToScreen(p4w, out var p4s, out var p4r);

            // don't draw the whole range if some of the points may be projected to a weird position
            if (p1r.Z < -1 || p2r.Z < -1 || p3r.Z < -1 || p4r.Z < -1) return;

            if (Plugin.Config.Filled) drawList.AddQuadFilled(p1s, p2s, p3s, p4s, FillColour);
            if (Plugin.Config.OuterRing) drawList.AddQuad(p1s, p2s, p3s, p4s, RingColour, Plugin.Config.Thickness);
        }
    }
}
