using System;
using System.Numerics;

namespace ActionEffectRange.Drawing.Types
{
    public class TargetDirectedConeAoEDrawData : ConeAoEDrawData
    {
        public readonly Vector3 Target;

        public TargetDirectedConeAoEDrawData(Vector3 origin, Vector3 target, byte baseEffectRange, byte xAxisModifier, float centralAngleBy2pi, uint ringColour, uint fillColour)
            : base(origin, baseEffectRange, xAxisModifier, CalculateRotation(origin, target), centralAngleBy2pi, ringColour, fillColour) { }
    }
}
