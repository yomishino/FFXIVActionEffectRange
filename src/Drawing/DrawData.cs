using System;
using System.Numerics;

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
        public float ElapsedSeconds => (float)(DateTime.Now - createTime).TotalSeconds;


        public abstract void Draw(ImGuiNET.ImDrawListPtr drawList);

        public static float CalculateRotation(Vector3 origin, Vector3 target)
            => MathF.Atan2(target.X - origin.X, target.Z - origin.Z);
    }
}
