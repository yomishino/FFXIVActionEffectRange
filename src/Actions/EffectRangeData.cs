using static ActionEffectRange.Actions.Data.ConeAoEAngleMap;
using ActionEffectRange.Actions.Enums;

namespace ActionEffectRange.Actions
{
    public class EffectRangeData
    {
        public readonly uint ActionId;
        public readonly ActionCategory Category;
        public readonly bool IsGTAction;
        public readonly bool IsHarmfulAction;
        public readonly sbyte Range;
        public readonly byte EffectRange;
        public readonly byte CastType;
        public readonly ActionAoEType AoEType;
        public readonly byte XAxisModifier; // for straight line aoe, this is the width?
        public readonly byte AdditionalEffectRange; // basically for donut inner radius
        public readonly float CentralAngleBy2Pi;    // for cone
        public readonly float RotationOffset;
        public readonly bool IsOriginal;


        public EffectRangeData(uint actionId, uint actionCategory, bool isGT, bool isHarmful, 
            sbyte range, byte effectRange, byte castType, byte xAxisModifier, 
            byte additionalEffectRange = 0, float centralAngleBy2Pi = DefaultAngle, 
            float rotationOffset = 0, bool isOriginal = false)
        {
            ActionId = actionId;
            Category = (ActionCategory)actionCategory;
            IsGTAction = isGT;
            IsHarmfulAction = isHarmful;
            Range = range;
            EffectRange = effectRange;
            CastType = castType;
            AoEType = (ActionAoEType)castType;
            XAxisModifier = xAxisModifier;
            AdditionalEffectRange = additionalEffectRange;
            CentralAngleBy2Pi = centralAngleBy2Pi;
            RotationOffset = rotationOffset;
            IsOriginal = isOriginal;
        }

        public EffectRangeData(Lumina.Excel.GeneratedSheets.Action actionRow)
            : this(actionRow.RowId, actionRow.ActionCategory.Row, actionRow.TargetArea, ActionData.IsHarmfulAction(actionRow),
                   actionRow.Range, actionRow.EffectRange, actionRow.CastType, actionRow.XAxisModifier, isOriginal: true) { }

        public EffectRangeData(EffectRangeData originalData, bool isHarmful, bool isOriginal = false)
            : this(originalData.ActionId, (uint)originalData.Category, originalData.IsGTAction, isHarmful,
                  originalData.Range, originalData.EffectRange, originalData.CastType, originalData.XAxisModifier, isOriginal: isOriginal) { }

        public EffectRangeData(EffectRangeData originalData, byte additionalEffectRange = 0, 
            float centralAngleBy2Pi = 1f / 3f, float rotationOffset = 0, bool isOriginal = false)
            : this(originalData.ActionId, (uint)originalData.Category, originalData.IsGTAction, originalData.IsHarmfulAction,
                  originalData.Range, originalData.EffectRange, originalData.CastType, originalData.XAxisModifier,
                  additionalEffectRange, centralAngleBy2Pi, rotationOffset, isOriginal: isOriginal) { }


        public static EffectRangeData OverrideAsConeAoE(EffectRangeData original,
            float centralAngleBy2Pi, float rotationOffset = 0)
            => new(original.ActionId, (uint)original.Category, original.IsGTAction,
                  original.IsHarmfulAction, original.Range, original.EffectRange,
                  (byte)ActionAoEType.Cone, original.XAxisModifier, original.AdditionalEffectRange,
                  centralAngleBy2Pi, rotationOffset, false);
    }

}
