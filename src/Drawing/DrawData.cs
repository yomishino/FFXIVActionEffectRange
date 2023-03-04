using ImGuiNET;

namespace ActionEffectRange.Drawing
{
    public abstract class DrawData
    {
        public readonly uint RingColour;
        public readonly uint FillColour;

        public DrawData(uint ringColour, uint fillColour)
        {
            RingColour = ringColour;
            FillColour = fillColour;
        }


        private readonly DateTime createTime = DateTime.Now;
        public float ElapsedSeconds 
            => (float)(DateTime.Now - createTime).TotalSeconds;


        public abstract void Draw(ImDrawListPtr drawList);


        protected static float CalcRotation(Vector3 origin, Vector3 target)
            => MathF.Atan2(target.X - origin.X, target.Z - origin.Z);

        protected static float ArcSegmentAngle 
            => 2 * MathF.PI / Config.NumSegments;


        protected static Vector3 CalcDirection(Vector3 origin, Vector3 target, 
            float rotationOffset = 0, bool ignoreY = true)
        {
            var dir = Vector3.Normalize(target - origin);
            // Further rotate by offset
            if (rotationOffset > .0001f || rotationOffset < -.0001f)
                dir = new(
                MathF.Cos(rotationOffset) * dir.X
                        - MathF.Sin(rotationOffset) * dir.Z,
                dir.Y,
                MathF.Cos(rotationOffset) * dir.Z
                        + MathF.Sin(rotationOffset) * dir.X);
            if (ignoreY) dir.Y = 0;
            return dir;
        }

        // Return the world position of the far-end of the AoE shape;
        //  for rectangular shape this is the midpoint on the farthest side
        protected static Vector3 CalcFarEndWorldPos(
            Vector3 origin, Vector3 direction, float length)
            => direction * length + origin;

        protected static (Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4)
            CalcRectCornersWorldPos(
                Vector3 nearEnd, Vector3 farEnd, Vector3 direction, float halfWidth)
            => (Vector3.Normalize(new(direction.Z, 0, -direction.X)) 
                    * halfWidth + nearEnd,
                Vector3.Normalize(new(direction.Z, 0, -direction.X)) 
                    * halfWidth + farEnd,
                Vector3.Normalize(new(-direction.Z, 0, direction.X)) 
                    * halfWidth + farEnd,
                Vector3.Normalize(new(-direction.Z, 0, direction.X)) 
                    * halfWidth + nearEnd);


        #region DrawComponent

        protected void DrawRect(ImDrawListPtr drawList, float width, 
            Vector3 nearEndWorldPos, Vector3 farEndWorldPos, Vector3 direction)
        {
#if DEBUG
            Projection.WorldToScreen(farEndWorldPos, out var pe, out var per);
            drawList.AddCircleFilled(pe, Config.Thickness * 2, RingColour);
#endif

            var w2 = width / 2;
            var (p1w, p2w, p3w, p4w) = CalcRectCornersWorldPos(
                nearEndWorldPos, farEndWorldPos, direction, w2);

            Projection.WorldToScreen(p1w, out var p1s, out var p1r);
            Projection.WorldToScreen(p2w, out var p2s, out var p2r);
            Projection.WorldToScreen(p3w, out var p3s, out var p3r);
            Projection.WorldToScreen(p4w, out var p4s, out var p4r);

            // don't draw the whole range if some of the points
            // may be projected to a weird position
            if (p1r.Z < -1 || p2r.Z < -1 || p3r.Z < -1 || p4r.Z < -1) return;

            if (Config.Filled)
                drawList.AddQuadFilled(p1s, p2s, p3s, p4s, FillColour);
            if (Config.OuterRing)
                drawList.AddQuad(p1s, p2s, p3s, p4s, RingColour, Config.Thickness);
        }

        #endregion
    }
}
