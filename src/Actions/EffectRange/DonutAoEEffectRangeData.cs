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

    }
}
