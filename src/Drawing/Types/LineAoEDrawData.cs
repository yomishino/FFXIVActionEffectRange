using ImGuiNET;
using System.Numerics;

namespace ActionEffectRange.Drawing.Types
{
    public abstract class LineAoEDrawData : DrawData
    {
        public readonly Vector3 Origin;
        public readonly Vector3 Direction;
        //public readonly float Rotation;
        public readonly float Length;
        public readonly float Width;
        public readonly Vector3 End;


        // Length (depth) of LineAoE seems has a small factor added to Action.EffectRange so its slightly longer: maybe 0.5 but not sure;
        // it sometimes look visually different on diffent enemies/hitbox radia.
        // The addition seems not applied to dummies on the field (only to dummies in instances in explore mode).
        public LineAoEDrawData(Vector3 origin, Vector3 target, byte baseEffectRange, byte xAxisModifier, bool calculateY, uint ringColour, uint fillColour)
        //public LineAoEDrawData(Vector3 origin, Vector3 target, byte baseEffectRange, byte xAxisModifier, float rotation, bool calculateY, uint ringColour, uint fillColour)
            : base(ringColour, fillColour)
        {
            Origin = origin;
            Direction = Vector3.Normalize(target - origin);
            if (!calculateY) Direction.Y = 0;
            Length = baseEffectRange + .5f; 
            Width = xAxisModifier;
            //Rotation = rotation;
            //End = new Vector3(Origin.X + Length * MathF.Sin(Rotation), Origin.Y, Origin.Z + Length * MathF.Cos(Rotation));
            End = Direction * Length + origin;
        }

        
        public override void Draw(ImDrawListPtr drawList)
        {
#if DEBUG
            Projection.WorldToScreen(End, out var pe, out var per);
            drawList.AddCircleFilled(pe, Plugin.Config.Thickness * 2, RingColour);
#endif

            var w2 = Width / 2;
            
            var p1w = Vector3.Normalize(new Vector3(Direction.Z, 0, -Direction.X)) * w2 + Origin;
            var p2w = Vector3.Normalize(new Vector3(Direction.Z, 0, -Direction.X)) * w2 + End;
            var p3w = Vector3.Normalize(new Vector3(-Direction.Z, 0, Direction.X)) * w2 + End;
            var p4w = Vector3.Normalize(new Vector3(-Direction.Z, 0, Direction.X)) * w2 + Origin;

            //var p1w = new Vector3(Origin.X - w2 * MathF.Cos(Rotation), Origin.Y, Origin.Z + w2 * MathF.Sin(Rotation));
            //var p2w = new Vector3(Origin.X + w2 * MathF.Cos(Rotation), Origin.Y, Origin.Z - w2 * MathF.Sin(Rotation));
            //var p3w = new Vector3(End.X + w2 * MathF.Cos(Rotation), End.Y, End.Z - w2 * MathF.Sin(Rotation));
            //var p4w = new Vector3(End.X - w2 * MathF.Cos(Rotation), End.Y, End.Z + w2 * MathF.Sin(Rotation));

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
