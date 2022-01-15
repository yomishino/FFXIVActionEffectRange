using ActionEffectRange.Actions.Enums;
using System.Numerics;

namespace ActionEffectRange.Actions
{
    public class EffectRangeData
    {
        public readonly uint ActionId;
        public readonly ActionCategory Category;
        public readonly bool IsHarmfulAction;
        public readonly sbyte Range;
        public readonly byte EffectRange;
        public readonly byte CastType;
        public readonly ActionAoEType AoEType;
        public readonly byte XAxisModifier; // for straight line aoe, this is the width?
        public readonly byte AdditionalEffectRange; // basically for donut inner radius
        public readonly bool IsOriginal;

        public EffectRangeData(uint actionId, uint actionCategory, bool isHarmful, sbyte range, byte effectRange, byte castType, 
            byte xAxisModifier, byte additionalEffectRange = 0, bool isOriginal = true)
        {
            ActionId = actionId;
            Category = (ActionCategory)actionCategory;
            IsHarmfulAction = isHarmful;
            Range = range;
            EffectRange = effectRange;
            CastType = castType;
            AoEType = ActionData.GetActionAoEType(castType);
            XAxisModifier = xAxisModifier;
            AdditionalEffectRange = additionalEffectRange;
            IsOriginal = isOriginal;
        }

        public EffectRangeData(Lumina.Excel.GeneratedSheets.Action actionRow)
            : this(actionRow.RowId, actionRow.ActionCategory.Row, ActionData.IsHarmfulAction(actionRow), actionRow.Range, actionRow.EffectRange, actionRow.CastType, actionRow.XAxisModifier) { }
        
    }

}
