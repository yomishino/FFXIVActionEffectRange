using ActionEffectRange.Actions.Data;
using ActionEffectRange.Actions.Enums;

namespace ActionEffectRange.Actions.EffectRange
{
    public class CircleAoEEffectRangeData : EffectRangeData
    {
        public CircleAoEEffectRangeData(uint actionId, 
            uint actionCategory, bool isGT, ActionHarmfulness harmfulness, 
            sbyte range, byte effectRange, byte xAxisModifier, 
            byte castType, bool isOriginal = false)
            : base(actionId, actionCategory, isGT, harmfulness, 
                  range, effectRange, xAxisModifier, castType, isOriginal)
        { }

        public CircleAoEEffectRangeData(Lumina.Excel.GeneratedSheets.Action actionRow)
            : this(actionRow.RowId, actionRow.ActionCategory.Row, actionRow.TargetArea,
                  ActionData.GetActionHarmfulness(actionRow), actionRow.Range,
                  actionRow.EffectRange, actionRow.XAxisModifier,
                  actionRow.CastType, isOriginal: true)
        { }

    }
}
