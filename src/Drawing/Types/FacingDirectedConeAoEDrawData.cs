using System;
using System.Numerics;

namespace ActionEffectRange.Drawing.Types
{
    public class FacingDirectedConeAoEDrawData : ConeAoEDrawData
    {
        public FacingDirectedConeAoEDrawData(Vector3 origin, float rotation, byte baseEffectRange, byte xAxisModifier, uint ringColour, uint fillColour, float centralAngle = (98f / 180f) * MathF.PI)
            : base(origin, baseEffectRange, xAxisModifier, rotation, ringColour, fillColour, centralAngle) { }
    }
}
