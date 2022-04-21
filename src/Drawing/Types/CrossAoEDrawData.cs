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
        public readonly Vector3 End2;
        public readonly Vector3 CrossDirection;
        public readonly Vector3 CrossEnd;
        public readonly Vector3 CrossEnd2;


        public CrossAoEDrawData(Vector3 origin, Vector3 target, 
            byte baseEffectRange, byte xAxisModifier, float rotationOffset,
            uint ringColour, uint fillColour)
            : base(ringColour, fillColour)
        {
            // TODO: do cross has the extra +.5 as do line?
            Length = baseEffectRange + .5f;
            Width = xAxisModifier;

            Origin = origin;
            // Basic direction is from origin to target
            // (or perhaps origin's facing; can't tell as the only skill of
            //  this type require player actually facing the target anyway)
            // then further rotate by the offset
            Direction = Vector3.Normalize(target - origin);
            if (rotationOffset > .0001f || rotationOffset < -.0001f)
                Direction = new(
                    MathF.Cos(rotationOffset) * Direction.X 
                        - MathF.Sin(rotationOffset) * Direction.Z,
                    Direction.Y,
                    MathF.Cos(rotationOffset) * Direction.Z 
                        + MathF.Sin(rotationOffset) * Direction.X);
            CrossDirection = new Vector3(
                Direction.Z, Direction.Y, -Direction.X);   // perpendicular

            // Endpoint is target based
            End = Direction * Length + target;
            End2 = -Direction * Length + target;
            CrossEnd = CrossDirection * Length + target;
            CrossEnd2 = -CrossDirection * Length + target;
        }

        public override void Draw(ImDrawListPtr drawList)
        {
#if DEBUG
            Projection.WorldToScreen(End, out var pe, out var _);
            drawList.AddCircleFilled(pe, Plugin.Config.Thickness * 2, 
                RingColour);
            Projection.WorldToScreen(End2, out var pe2, out var _);
            drawList.AddCircleFilled(pe2, Plugin.Config.Thickness * 2, 
                ImGui.ColorConvertFloat4ToU32(new(0,0,1,1)));
            Projection.WorldToScreen(CrossEnd, out var pc, out var _);
            drawList.AddCircleFilled(pc, Plugin.Config.Thickness * 2, 
                ImGui.ColorConvertFloat4ToU32(new(1, 0, 0, 1)));
            Projection.WorldToScreen(CrossEnd2, out var pc2, out var _);
            drawList.AddCircleFilled(pc2, Plugin.Config.Thickness * 2, 
                ImGui.ColorConvertFloat4ToU32(new(1, 1, 0, 1)));
#endif
            // TODO: would be better if not making the shapes overlapped
            var w2 = Width / 2;
            DrawOneLine(drawList, w2, Direction, End, End2);
            DrawOneLine(drawList, w2, CrossDirection, CrossEnd, CrossEnd2);
        }

        private void DrawOneLine(
            ImDrawListPtr drawList, float halfWidth, Vector3 direction, Vector3 end, Vector3 end2)
        {
            var p1w = Vector3.Normalize(new Vector3(direction.Z, 0, -direction.X)) * halfWidth + end;
            var p2w = Vector3.Normalize(new Vector3(direction.Z, 0, -direction.X)) * halfWidth + end2;
            var p3w = Vector3.Normalize(new Vector3(-direction.Z, 0, direction.X)) * halfWidth + end2;
            var p4w = Vector3.Normalize(new Vector3(-direction.Z, 0, direction.X)) * halfWidth + end;
            Projection.WorldToScreen(p1w, out var p1s, out var p1r);
            Projection.WorldToScreen(p2w, out var p2s, out var p2r);
            Projection.WorldToScreen(p3w, out var p3s, out var p3r);
            Projection.WorldToScreen(p4w, out var p4s, out var p4r);

            // don't draw the whole range if some of the points may be projected to a weird position
            if (p1r.Z < -1 || p2r.Z < -1 || p3r.Z < -1 || p4r.Z < -1) return;

            if (Plugin.Config.Filled) 
                drawList.AddQuadFilled(p1s, p2s, p3s, p4s, FillColour);
            if (Plugin.Config.OuterRing) 
                drawList.AddQuad(p1s, p2s, p3s, p4s, 
                    RingColour, Plugin.Config.Thickness);
        }
    }
}
