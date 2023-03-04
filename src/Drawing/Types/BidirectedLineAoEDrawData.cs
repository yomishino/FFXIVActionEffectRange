using ImGuiNET;

namespace ActionEffectRange.Drawing.Types
{
    public class BidirectedLineAoEDrawData : FacingDirectedLineAoEDrawData
    {
        public readonly Vector3 End2;

        // Draw like line AoE but extend on both directions
        // Using one end as the origin instead of the position of the actor
        public BidirectedLineAoEDrawData(Vector3 origin, float rotation,
            byte baseEffectRange, byte xAxisModifier, float rotationOffset,
            uint ringColour, uint fillColour)
            : base(origin, rotation, byte.DivRem(baseEffectRange, 2).Quotient,
                  xAxisModifier, rotationOffset, ringColour, fillColour)
        {
            End2 = CalcFarEndWorldPos(origin, -Direction, Length);
        }

        public override void Draw(ImDrawListPtr drawList)
          => DrawRect(drawList, Width, End2, End, Direction);
    }
}