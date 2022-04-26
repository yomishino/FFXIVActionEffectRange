using ActionEffectRange.Actions.Enums;

namespace ActionEffectRange.Actions.Data.Template
{
    public class AoETypeDataItem : IDataItem
    {
        public uint ActionId { get; }
        public byte CastType { get; }
        public ActionHarmfulness Harmfulness { get; }

        public AoETypeDataItem(
            uint actionId, byte castType, ActionHarmfulness harmfulness)
        {
            ActionId = actionId;
            CastType = castType;
            Harmfulness = harmfulness;
        }
    }
}
