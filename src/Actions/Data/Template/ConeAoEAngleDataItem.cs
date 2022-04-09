namespace ActionEffectRange.Actions.Data.Template
{
    public class ConeAoEAngleDataItem : IDataItem
    {
        public uint ActionId { get; }
        public float CentralAngleCycles { get; }
        public float RotationOffset { get; }

        public ConeAoEAngleDataItem(uint actionId, 
            float centralAngleCycles, float rotationOffset = 0)
        {
            ActionId = actionId;
            CentralAngleCycles = centralAngleCycles;
            RotationOffset = rotationOffset;
        }
    }
}
