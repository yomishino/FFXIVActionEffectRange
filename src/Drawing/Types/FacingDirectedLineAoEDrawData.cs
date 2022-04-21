using System;
using System.Numerics;

namespace ActionEffectRange.Drawing.Types
{
    public class FacingDirectedLineAoEDrawData : LineAoEDrawData
    {
        public FacingDirectedLineAoEDrawData(Vector3 origin, float rotation, 
            byte baseEffectRange, byte xAxisModifier, float rotationOffset,
            uint ringColour, uint fillColour)
            : base(origin, GetDummyTarget(origin, rotation), baseEffectRange, 
                  xAxisModifier, false, rotationOffset, ringColour, fillColour) 
        { }

        private static Vector3 GetDummyTarget(Vector3 origin, float rotation) 
            => new(origin.X + MathF.Sin(rotation), origin.Y, origin.Z + MathF.Cos(rotation));
    }
}
