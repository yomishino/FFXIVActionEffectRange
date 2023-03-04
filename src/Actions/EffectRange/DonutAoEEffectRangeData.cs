using ActionEffectRange.Actions.Data;
using ActionEffectRange.Actions.Enums;

namespace ActionEffectRange.Actions.EffectRange
{
    public class DonutAoEEffectRangeData : EffectRangeData
    {
        public readonly byte InnerRadius;

        public DonutAoEEffectRangeData(uint actionId, 
            uint actionCategory, bool isGT, ActionHarmfulness harmfulness, 
            sbyte range, byte effectRange, byte xAxisModifier, byte castType, 
            byte innerRadius = 0, bool isOriginal = false)
            : base(actionId, actionCategory, isGT, harmfulness,
                  range, effectRange, xAxisModifier, castType, isOriginal)
        {
            InnerRadius = innerRadius;
        }

        public DonutAoEEffectRangeData(
            Lumina.Excel.GeneratedSheets.Action actionRow, byte innerRadius = 0)
            : this(actionRow.RowId, actionRow.ActionCategory.Row, actionRow.TargetArea,
                  ActionData.GetActionHarmfulness(actionRow), actionRow.Range,
                  actionRow.EffectRange, actionRow.XAxisModifier, actionRow.CastType,
                  innerRadius, isOriginal: true)
        { }

        public DonutAoEEffectRangeData(EffectRangeData original,
            byte innerRadius, bool isOriginal = false)
            : this(original.ActionId, (uint)original.Category, original.IsGTAction,
                  original.Harmfulness, original.Range, original.EffectRange,
                  original.XAxisModifier, original.CastType, innerRadius, isOriginal)
        { }

        protected override string AdditionalFieldsToString()
            => $"InnerRadius: {InnerRadius}";
    }
}
