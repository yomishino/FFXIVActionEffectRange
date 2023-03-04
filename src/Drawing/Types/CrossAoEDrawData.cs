using ImGuiNET;

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
            // TODO: do cross's have the extra +.5 as do lines?
            Length = baseEffectRange + .5f;
            Width = xAxisModifier;

            Origin = origin;
            // Basic direction is from origin to target
            //  then further rotate by the offset.
            // (This assumes the action's direction is target-based)
            Direction = CalcDirection(origin, target, rotationOffset);
            CrossDirection = new Vector3(
                Direction.Z, Direction.Y, -Direction.X);   // perpendicular

            // Endpoint is target based
            End = CalcFarEndWorldPos(target, Direction, Length);
            End2 = CalcFarEndWorldPos(target, -Direction, Length);
            CrossEnd = CalcFarEndWorldPos(target, CrossDirection, Length);
            CrossEnd2 = CalcFarEndWorldPos(target, -CrossDirection, Length);
        }

        public override void Draw(ImDrawListPtr drawList)
        {
#if DEBUG
            Projection.WorldToScreen(End, out var pe, out _);
            drawList.AddCircleFilled(pe, Config.Thickness * 2, 
                RingColour);
            Projection.WorldToScreen(End2, out var pe2, out _);
            drawList.AddCircleFilled(pe2, Config.Thickness * 2, 
                ImGui.ColorConvertFloat4ToU32(new(0,0,1,1)));
            Projection.WorldToScreen(CrossEnd, out var pc, out _);
            drawList.AddCircleFilled(pc, Config.Thickness * 2, 
                ImGui.ColorConvertFloat4ToU32(new(1, 0, 0, 1)));
            Projection.WorldToScreen(CrossEnd2, out var pc2, out _);
            drawList.AddCircleFilled(pc2, Config.Thickness * 2, 
                ImGui.ColorConvertFloat4ToU32(new(1, 1, 0, 1)));
#endif
            // TODO: Making the shapes not overlapped
            var w2 = Width / 2;
            DrawRect(drawList, w2, End, End2, Direction);
            DrawRect(drawList, w2, CrossEnd, CrossEnd2, CrossDirection);
        }
    }
}
