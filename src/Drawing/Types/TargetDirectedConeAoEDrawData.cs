﻿using System;
using System.Numerics;

namespace ActionEffectRange.Drawing.Types
{
    public class TargetDirectedConeAoEDrawData : ConeAoEDrawData
    {
        public readonly Vector3 Target;

        public TargetDirectedConeAoEDrawData(Vector3 origin, Vector3 target, byte baseEffectRange, byte xAxisModifier, uint ringColour, uint fillColour, float ratio = .25f)
            : base(origin, baseEffectRange, xAxisModifier, CalculateRotation(origin, target), ringColour, fillColour, ratio) { }
    }
}
