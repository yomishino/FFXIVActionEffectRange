namespace ActionEffectRange.Actions.Data.Template
{
    public class BlacklistedActionDataItem : IDataItem
    {
        public uint ActionId { get; }

        public BlacklistedActionDataItem(uint actionId) => ActionId = actionId;

        public static implicit operator uint(BlacklistedActionDataItem item) 
            => item.ActionId;

        public static explicit operator BlacklistedActionDataItem(uint actionId)
            => new(actionId);
    }
}
