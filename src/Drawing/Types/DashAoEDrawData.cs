using System.Numerics;
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

        // No extra addition to length of the AoE which is unlike LineAoE.
        // Seems for pvp players you can actually hit the target only if
        // position you move to is at least (exactly) the same or
        // go past the target's position, but dummies seem get dmg
        // as if the AoE has some extra added (but who cares dummies!
        // unless players are actually also like this in instances..)
        // For width tho, different from LineAoE, it seems using Action.EffectRange
        // for half of the width instead of using XAxisModifier as the full width?
        // 
        // Patch 6.1: for SAM's new Soten, while it makes action user
        //  pass through the target entirely, the extra area from target's
        //  position to action user's position seems has no effect at all?
        //  Looks like it just do the normal effect calculation for this type 
        //  and then makes character "warp" to the new position???
        // 
        // TODO: Also, idk about the correct calculation of width for new Soten,
        //  but if we make it as having EffectRange of 1 as old Soten,
        //  it's effectively the same width as using XAxisModifer anyway (which is 4)
        //  Probably it is designed to be consistent? Since new Soten has EffectRange 0,
        //  possibly also a change reflecting it being no longer a GT action.
        public DashAoEDrawData(Vector3 origin, Vector3 target, byte baseEffectRange, byte xAxisModifier, uint ringColour, uint fillColour)
        : base(ringColour, fillColour)
        {
            Origin = origin;
            Target = target;
            Direction = target - origin;
            EffectRange = baseEffectRange;
            Width = baseEffectRange * 2;
        }


        public override void Draw(ImDrawListPtr drawList)
        {
#if DEBUG
            Projection.WorldToScreen(Target, out var pe, out var per);
            drawList.AddCircleFilled(pe, Plugin.Config.Thickness * 2, RingColour);
#endif

            var w2 = Width / 2;

            var p1w = Vector3.Normalize(new Vector3(Direction.Z, 0, -Direction.X)) * w2 + Origin;
            var p2w = Vector3.Normalize(new Vector3(Direction.Z, 0, -Direction.X)) * w2 + Target;
            var p3w = Vector3.Normalize(new Vector3(-Direction.Z, 0, Direction.X)) * w2 + Target;
            var p4w = Vector3.Normalize(new Vector3(-Direction.Z, 0, Direction.X)) * w2 + Origin;

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
