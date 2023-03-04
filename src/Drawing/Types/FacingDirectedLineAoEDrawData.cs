namespace ActionEffectRange.Drawing.Types
{
    public class FacingDirectedLineAoEDrawData : LineAoEDrawData
    {
        public FacingDirectedLineAoEDrawData(Vector3 origin, float rotation, 
            byte baseEffectRange, byte xAxisModifier, float rotationOffset,
            uint ringColour, uint fillColour)
            : base(origin, GetDummyTarget(origin, rotation), baseEffectRange, 
                  xAxisModifier, rotationOffset, ringColour, fillColour) 
        {}

        protected static Vector3 GetDummyTarget(Vector3 origin, float rotation)
            => CalcFarEndWorldPos(
                origin, new(MathF.Sin(rotation), 0, MathF.Cos(rotation)), 1);
    }
}
