namespace ActionEffectRange.Actions.EffectRange
{
    public class ConeAoEEffectRangeData : EffectRangeData
    {
        public readonly float CentralAngleBy2Pi;
        public readonly float RotationOffset;

        public ConeAoEEffectRangeData(uint actionId, uint actionCategory,
            bool isGT, bool isHarmful, sbyte range, byte effectRange,
            byte xAxisModifier, byte castType, float centralAngleBy2Pi = 0, 
            float rotationOffset = 0, bool isOriginal = false)
            : base(actionId, actionCategory, isGT, isHarmful,
                  range, effectRange, xAxisModifier, castType, isOriginal)
        {
            CentralAngleBy2Pi = centralAngleBy2Pi;
            RotationOffset = rotationOffset;
        }

        public ConeAoEEffectRangeData(Lumina.Excel.GeneratedSheets.Action actionRow,
            float centralAngleBy2Pi = 0, float rotationOffset = 0)
            : this(actionRow.RowId, actionRow.ActionCategory.Row, actionRow.TargetArea,
                  ActionData.IsHarmfulAction(actionRow), actionRow.Range,
                  actionRow.EffectRange, actionRow.XAxisModifier, actionRow.CastType,
                  centralAngleBy2Pi, rotationOffset, isOriginal: true)
        { }

        public ConeAoEEffectRangeData(EffectRangeData original,
            float centralAngleBy2Pi = 0, float rotationOffset = 0,
            bool isOriginal = true)
            : this(original.ActionId, (uint)original.Category,
                  original.IsGTAction, original.IsHarmfulAction,
                  original.Range, original.EffectRange, 
                  original.XAxisModifier, original.CastType,
                  centralAngleBy2Pi, rotationOffset, isOriginal)
        { }

        protected override string AdditionalFieldsToString()
            => $"Angle: {CentralAngleBy2Pi}, RotationOffset: {RotationOffset}";
    }
}
