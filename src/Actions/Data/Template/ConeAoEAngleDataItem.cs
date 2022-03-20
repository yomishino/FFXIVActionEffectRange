namespace ActionEffectRange.Actions.Data.Template
{
    public class ConeAoEAngleDataItem
    {
        public readonly uint ActionId;
        public readonly float CentralAngleBy2pi;
        public readonly float RotationOffset;

        public ConeAoEAngleDataItem(uint actionId, float centralAngleBy2pi, 
            float rotationOffset = 0)
        {
            ActionId = actionId;
            CentralAngleBy2pi = centralAngleBy2pi;
            RotationOffset = rotationOffset;
        }
    }
}
