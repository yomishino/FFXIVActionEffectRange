using ImGuiNET;
using System;
using System.Numerics;

namespace ActionEffectRange.Drawing.Types
{
    public class FacingDirectedLineAoEDrawData : LineAoEDrawData
    {
        public FacingDirectedLineAoEDrawData(Vector3 origin, float rotation, byte baseEffectRange, byte xAxisModifier, uint ringColour, uint fillColour)
            : base(origin, baseEffectRange, xAxisModifier, rotation, ringColour, fillColour) { }
    }
}
