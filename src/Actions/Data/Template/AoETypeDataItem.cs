namespace ActionEffectRange.Actions.Data.Template
{
    public class AoETypeDataItem : IDataItem
    {
        public uint ActionId { get; }
        public byte CastType { get; }
        public bool IsHarmful { get; }

        public AoETypeDataItem(uint actionId, byte castType, bool isHarmful)
        {
            ActionId = actionId;
            CastType = castType;
            IsHarmful = isHarmful;
        }
    }
}
