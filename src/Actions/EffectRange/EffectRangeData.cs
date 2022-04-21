using ActionEffectRange.Actions.Enums;

namespace ActionEffectRange.Actions.EffectRange
{
    public abstract class EffectRangeData
    {
        public readonly uint ActionId;
        public readonly ActionCategory Category;
        public readonly bool IsGTAction;
        public readonly bool IsHarmfulAction;
        public readonly sbyte Range;
        public readonly byte EffectRange;
        public readonly byte XAxisModifier; // for straight line aoe, this is the width?
        public readonly byte CastType;

        public readonly bool IsOriginal;


        protected EffectRangeData(uint actionId, uint actionCategory, 
            bool isGT, bool isHarmful, sbyte range, byte effectRange, 
            byte xAxisModifier, byte castType, bool isOriginal = false)
        {
            ActionId = actionId;
            Category = (ActionCategory)actionCategory;
            IsGTAction = isGT;
            IsHarmfulAction = isHarmful;
            Range = range;
            EffectRange = effectRange;
            XAxisModifier = xAxisModifier;
            CastType = castType;
            IsOriginal = isOriginal;
        }

        protected EffectRangeData(Lumina.Excel.GeneratedSheets.Action actionRow)
            : this(actionRow.RowId, actionRow.ActionCategory.Row, actionRow.TargetArea, 
                  ActionData.IsHarmfulAction(actionRow), actionRow.Range, actionRow.EffectRange, 
                  actionRow.XAxisModifier, actionRow.CastType, isOriginal: true) { }

        public static EffectRangeData Create(uint actionId, uint actionCategory, 
            bool isGT, bool isHarmful, sbyte range, byte effectRange, byte xAxisModifier, 
            byte castType, bool isOriginal = false)
            => (ActionAoEType)castType switch
            {
                ActionAoEType.Circle or ActionAoEType.Circle2 or ActionAoEType.GT
                    => new CircleAoEEffectRangeData(actionId, actionCategory, isGT, isHarmful,
                        range, effectRange, xAxisModifier, castType, isOriginal: isOriginal),
                ActionAoEType.Cone or ActionAoEType.Cone2
                    => new ConeAoEEffectRangeData(actionId, actionCategory, isGT, isHarmful,
                        range, effectRange, xAxisModifier, castType, isOriginal: isOriginal),
                ActionAoEType.Line or ActionAoEType.Line2
                    => new LineAoEEffectRangeData(actionId, actionCategory, isGT, isHarmful,
                        range, effectRange, xAxisModifier, castType, isOriginal: isOriginal),
                ActionAoEType.DashAoE
                    => new DashAoEEffectRangeData(actionId, actionCategory, isGT, isHarmful,
                        range, effectRange, xAxisModifier, castType, isOriginal: isOriginal),
                ActionAoEType.Donut
                    => new DonutAoEEffectRangeData(actionId, actionCategory, isGT, isHarmful,
                        range, effectRange, xAxisModifier, castType, isOriginal: isOriginal),
                _ => new NonAoEEffectRangeData(actionId, actionCategory, isGT, isHarmful,
                        range, effectRange, xAxisModifier, castType, isOriginal: isOriginal)
            };

        public static EffectRangeData Create(Lumina.Excel.GeneratedSheets.Action actionRow)
            => Create(actionRow.RowId, actionRow.ActionCategory.Row, 
                actionRow.TargetArea, ActionData.IsHarmfulAction(actionRow), 
                actionRow.Range, actionRow.EffectRange, actionRow.XAxisModifier, 
                actionRow.CastType, isOriginal: true);

        public static EffectRangeData CreateChangeHarmfulness(
            EffectRangeData original, bool isHarmful)
            => Create(original.ActionId, (uint)original.Category, original.IsGTAction, 
                isHarmful, original.Range, original.EffectRange, original.XAxisModifier, 
                original.CastType, isOriginal: false);

        public override string ToString()
            => $"{GetType().Name}{{ ActionId: {ActionId}, Category: {Category}, " +
            $"IsGT: {IsGTAction}, IsHarmful: {IsHarmfulAction}, " +
            $"Range: {Range}, EffectRange: {EffectRange}, XAxisModifier: {XAxisModifier}, " +
            $"CastType: {CastType}, {AdditionalFieldsToString()}, IsOriginal: {IsOriginal} }}";

        protected virtual string AdditionalFieldsToString() => string.Empty;
    }

}
