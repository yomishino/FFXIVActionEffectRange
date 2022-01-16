using System;
using System.Numerics;

namespace ActionEffectRange.Drawing.Types
{
    public class TargetDirectedLineAoEDrawData : LineAoEDrawData
    {
        public readonly Vector3 Target;

        public TargetDirectedLineAoEDrawData(Vector3 origin, Vector3 target, byte baseEffectRange, byte xAxisModifier, bool calculateY, uint ringColour, uint fillColour)
            : base(origin, target, baseEffectRange, xAxisModifier, calculateY, ringColour, fillColour) { }
    }
}
