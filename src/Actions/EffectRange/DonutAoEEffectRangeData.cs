namespace ActionEffectRange.Actions.EffectRange
{
    public class DonutAoEEffectRangeData : EffectRangeData
    {
        public readonly byte InnerRadius;

        public DonutAoEEffectRangeData(uint actionId, uint actionCategory,
            bool isGT, bool isHarmful, sbyte range, byte effectRange,
            byte xAxisModifier, byte castType, byte innerRadius = 0, bool isOriginal = false)
            : base(actionId, actionCategory, isGT, isHarmful,
                  range, effectRange, xAxisModifier, castType, isOriginal)
        {
            InnerRadius = innerRadius;
        }

        public DonutAoEEffectRangeData(
            Lumina.Excel.GeneratedSheets.Action actionRow, byte innerRadius = 0)
            : this(actionRow.RowId, actionRow.ActionCategory.Row, actionRow.TargetArea,
                  ActionData.IsHarmfulAction(actionRow), actionRow.Range,
                  actionRow.EffectRange, actionRow.XAxisModifier, actionRow.CastType,
                  innerRadius, isOriginal: true)
        { }

        public DonutAoEEffectRangeData(EffectRangeData original,
            byte innerRadius, bool isOriginal = false)
            : this(original.ActionId, (uint)original.Category, original.IsGTAction,
                  original.IsHarmfulAction, original.Range, original.EffectRange,
                  original.XAxisModifier, original.CastType, innerRadius, isOriginal)
        { }

        protected override string AdditionalFieldsToString()
            => $"InnerRadius: {InnerRadius}";
    }
}
