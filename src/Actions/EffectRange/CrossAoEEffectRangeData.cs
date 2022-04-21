namespace ActionEffectRange.Actions.EffectRange
{
    public class CrossAoEEffectRangeData : EffectRangeData
    {
        public byte Width => XAxisModifier;
        public readonly float RotationOffset;

        public CrossAoEEffectRangeData(uint actionId, uint actionCategory,
                bool isGT, bool isHarmful, sbyte range, byte effectRange,
                byte xAxisModifier, byte castType,
                float rotationOffset = 0, bool isOriginal = false)
                : base(actionId, actionCategory, isGT, isHarmful,
                      range, effectRange, xAxisModifier, castType, isOriginal)
        {
            RotationOffset = rotationOffset;
        }

        public CrossAoEEffectRangeData(Lumina.Excel.GeneratedSheets.Action actionRow,
            float rotationOffset = 0)
            : this(actionRow.RowId, actionRow.ActionCategory.Row, actionRow.TargetArea,
                  ActionData.IsHarmfulAction(actionRow), actionRow.Range,
                  actionRow.EffectRange, actionRow.XAxisModifier, actionRow.CastType,
                  rotationOffset, isOriginal: true)
        { }

        public CrossAoEEffectRangeData(EffectRangeData original, 
            float rotationOffset)
            : this(original.ActionId, (uint)original.Category, 
                  original.IsGTAction, original.IsHarmfulAction, 
                  original.Range, original.EffectRange,
                  original.XAxisModifier, original.CastType,
                  rotationOffset, isOriginal: false)
        { }

        protected override string AdditionalFieldsToString()
            => $"RotationOffset: {RotationOffset}";
    }
}
