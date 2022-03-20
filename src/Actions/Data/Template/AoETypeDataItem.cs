namespace ActionEffectRange.Actions.Data.Template
{
    public class AoETypeDataItem
    {
        public readonly uint ActionId;
        public readonly byte CastType;
        public readonly bool IsHarmful;

        public AoETypeDataItem(uint actionId, byte castType, bool isHarmful)
        {
            ActionId = actionId;
            CastType = castType;
            IsHarmful = isHarmful;
        }
    }
}
