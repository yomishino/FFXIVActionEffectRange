namespace ActionEffectRange.Drawing.Types
{
    public class FacingDirectedConeAoEDrawData : ConeAoEDrawData
    {
        public FacingDirectedConeAoEDrawData(
            Vector3 origin, float rotation, byte baseEffectRange, byte xAxisModifier, 
            float centralAngleCycles, uint ringColour, uint fillColour)
            : base(origin, baseEffectRange, xAxisModifier, 
                  rotation, centralAngleCycles, ringColour, fillColour) { }
    }
}
