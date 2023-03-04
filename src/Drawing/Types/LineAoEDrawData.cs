using ImGuiNET;

namespace ActionEffectRange.Drawing.Types
{
    public abstract class LineAoEDrawData : DrawData
    {
        public readonly Vector3 Origin;
        public readonly Vector3 Direction;
        public readonly float Rotation;
        public readonly float Length;
        public readonly float Width;
        public readonly Vector3 End;


        // Length (depth) of LineAoE seems has a small factor added to Action.EffectRange
        //  so its slightly longer: maybe 0.5?
        // Sometimes visually different on diffent enemies/hitbox radii.
        // Seems not applied to dummies (except for dummies in instances in explore mode).
        public LineAoEDrawData(Vector3 origin, Vector3 target, 
            byte baseEffectRange, byte xAxisModifier, float rotationOffset, 
            uint ringColour, uint fillColour)
            : base(ringColour, fillColour)
        {
            Origin = origin;
            Direction = CalcDirection(origin, target, rotationOffset);
            Length = baseEffectRange + .5f; 
            Width = xAxisModifier;
            End = CalcFarEndWorldPos(origin, Direction, Length);
        }

        public override void Draw(ImDrawListPtr drawList)
            => DrawRect(drawList, Width, Origin, End, Direction);
    }
}
