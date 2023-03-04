using ImGuiNET;

namespace ActionEffectRange.Drawing.Types
{
    public class DashAoEDrawData : DrawData
    {
        public readonly Vector3 Origin;
        public readonly Vector3 Target;
        public readonly Vector3 Direction;
        public readonly byte EffectRange;
        public readonly float Width;

        // No padding to length of the AoE which is unlike LineAoE.
        // Seems for pvp actions, an actor can actually hit the an enemy only if
        //  the targeted position is at least (exactly) the same or further past
        //  the enemy's position. (Tested on duelling ground.)
        // Dummies seem to still get dmg as if the AoE had the padded length.
        // Actually, not sure about player enemies in instances... 
        // 
        // Patch 6.1~: for SAM's new Soten, while action user will pass through
        //  the target entirely, there seems to be no dmg or other effects happen
        //  within the extra area from target's position to user's final position?
        // Perhaps it is just the effect calculation as usual for this type 
        //  and then character somehow "warps" to the final position???
        // 
        // TODO: Width for new Soten?
        // Old Soten has EffectRange=1 and XAxisModifier=4 while the new one has 0 and 4,
        //  but the EffectRange for old Soten may be for the GT action hitbox circle instead.
        // The old Soten seemed to have a width of 2; not sure about the new one.
        // Using XAxisModifier/2 for now just to be consistent numerically;
        //  but this may be incorrect... 
        public DashAoEDrawData(Vector3 origin, Vector3 target, 
            byte effectRange, byte xAxisModifier, uint ringColour, uint fillColour)
        : base(ringColour, fillColour)
        {
            Origin = origin;
            Target = target;
            Direction = target - origin;
            EffectRange = effectRange;
            //Width = effectRange * 2;
            Width = xAxisModifier / 2;
        }


        public override void Draw(ImDrawListPtr drawList)
            => DrawRect(drawList, Width, Origin, Target, Direction);
    }
}
