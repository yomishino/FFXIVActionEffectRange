using System;
using System.Numerics;

namespace ActionEffectRange.Drawing.Types
{
    public class FacingDirectedConeAoEDrawData : ConeAoEDrawData
    {
        public FacingDirectedConeAoEDrawData(Vector3 origin, float rotation, byte baseEffectRange, byte xAxisModifier, float centralAngleBy2pi, uint ringColour, uint fillColour)
            : base(origin, baseEffectRange, xAxisModifier, rotation, centralAngleBy2pi, ringColour, fillColour) { }
    }
}
