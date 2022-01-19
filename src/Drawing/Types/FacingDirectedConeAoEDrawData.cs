using System;
using System.Numerics;

namespace ActionEffectRange.Drawing.Types
{
    public class FacingDirectedConeAoEDrawData : ConeAoEDrawData
    {
        public FacingDirectedConeAoEDrawData(Vector3 origin, float rotation, byte baseEffectRange, byte xAxisModifier, uint ringColour, uint fillColour, float ratio = .25f)
            : base(origin, baseEffectRange, xAxisModifier, rotation, ringColour, fillColour, ratio) { }
    }
}
