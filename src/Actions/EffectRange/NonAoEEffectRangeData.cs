namespace ActionEffectRange.Actions.EffectRange
{
    // Temporarily holding data to allow overriding later
    public class NonAoEEffectRangeData : EffectRangeData
    {
        public NonAoEEffectRangeData(uint actionId, uint actionCategory,
            bool isGT, bool isHarmful, sbyte range, byte effectRange,
            byte xAxisModifier, byte castType, bool isOriginal = false)
            : base(actionId, actionCategory, isGT, isHarmful,
                  range, effectRange, xAxisModifier, castType, isOriginal)
        { }

        public NonAoEEffectRangeData(Lumina.Excel.GeneratedSheets.Action actionRow)
            : this(actionRow.RowId, actionRow.ActionCategory.Row, actionRow.TargetArea,
                  ActionData.IsHarmfulAction(actionRow), actionRow.Range,
                  actionRow.EffectRange, actionRow.XAxisModifier,
                  actionRow.CastType, isOriginal: true)
        { }
    }
}
