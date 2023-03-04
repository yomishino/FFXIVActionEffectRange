using ActionEffectRange.Actions.Data;
using ActionEffectRange.Actions.Enums;

namespace ActionEffectRange.Actions.EffectRange
{
    public class CrossAoEEffectRangeData : EffectRangeData
    {
        public byte Width => XAxisModifier;
        public readonly float RotationOffset;

        public CrossAoEEffectRangeData(uint actionId, 
            uint actionCategory, bool isGT, ActionHarmfulness harmfulness, 
            sbyte range, byte effectRange, byte xAxisModifier, byte castType,
            float rotationOffset = 0, bool isOriginal = false)
            : base(actionId, actionCategory, isGT, harmfulness,
                  range, effectRange, xAxisModifier, castType, isOriginal)
        {
            RotationOffset = rotationOffset;
        }

        public CrossAoEEffectRangeData(Lumina.Excel.GeneratedSheets.Action actionRow,
            float rotationOffset = 0)
            : this(actionRow.RowId, actionRow.ActionCategory.Row, actionRow.TargetArea,
                  ActionData.GetActionHarmfulness(actionRow), actionRow.Range,
                  actionRow.EffectRange, actionRow.XAxisModifier, actionRow.CastType,
                  rotationOffset, isOriginal: true)
        { }

        public CrossAoEEffectRangeData(EffectRangeData original,
            float rotationOffset)
            : this(original.ActionId, (uint)original.Category,
                  original.IsGTAction, original.Harmfulness,
                  original.Range, original.EffectRange,
                  original.XAxisModifier, original.CastType,
                  rotationOffset, isOriginal: false)
        { }

        protected override string AdditionalFieldsToString()
            => $"RotationOffset: {RotationOffset}";
    }
}
