namespace ActionEffectRange.Drawing.Types
{
    public class TargetDirectedConeAoEDrawData : ConeAoEDrawData
    {
        public readonly Vector3 Target;

        public TargetDirectedConeAoEDrawData(
            Vector3 origin, Vector3 target, byte baseEffectRange, byte xAxisModifier, 
            float centralAngleCycles, float rotationOffset, uint ringColour, uint fillColour)
            : base(origin, baseEffectRange, xAxisModifier, 
                  CalcRotation(origin, target) + rotationOffset, centralAngleCycles, 
                  ringColour, fillColour) { }
    }
}
