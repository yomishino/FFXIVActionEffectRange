using ActionEffectRange.Actions.Enums;

namespace ActionEffectRange.Actions.EffectRange
{
    public abstract class EffectRangeData
    {
        public readonly uint ActionId;
        public readonly ActionCategory Category;
        public readonly bool IsGTAction;
        public readonly ActionHarmfulness Harmfulness;
        public readonly sbyte Range;
        public readonly byte EffectRange;
        // XAxisModifier decides the width of line aoe?
        public readonly byte XAxisModifier; 
        public readonly byte CastType;

        public readonly bool IsOriginal;


        protected EffectRangeData(uint actionId, uint actionCategory, bool isGT, 
            ActionHarmfulness harmfulness, sbyte range, byte effectRange, 
            byte xAxisModifier, byte castType, bool isOriginal = false)
        {
            ActionId = actionId;
            Category = (ActionCategory)actionCategory;
            IsGTAction = isGT;
            Harmfulness = harmfulness;
            Range = range;
            EffectRange = effectRange;
            XAxisModifier = xAxisModifier;
            CastType = castType;
            IsOriginal = isOriginal;
        }

        public static EffectRangeData Create(uint actionId, 
            uint actionCategory, bool isGT, ActionHarmfulness harmfulness, 
            sbyte range, byte effectRange, byte xAxisModifier, 
            byte castType, bool isOriginal = false)
            => (ActionAoEType)castType switch
            {
                ActionAoEType.Circle or ActionAoEType.Circle2 or ActionAoEType.GT
                    => new CircleAoEEffectRangeData(actionId, actionCategory, 
                        isGT, harmfulness, range, effectRange, xAxisModifier, 
                        castType, isOriginal: isOriginal),
                ActionAoEType.Cone or ActionAoEType.Cone2
                    => new ConeAoEEffectRangeData(actionId, actionCategory, 
                        isGT, harmfulness, range, effectRange, xAxisModifier, 
                        castType, isOriginal: isOriginal),
                ActionAoEType.Line or ActionAoEType.Line2
                    => new LineAoEEffectRangeData(actionId, actionCategory, 
                        isGT, harmfulness, range, effectRange, xAxisModifier, 
                        castType, isOriginal: isOriginal),
                ActionAoEType.DashAoE
                    => new DashAoEEffectRangeData(actionId, actionCategory, 
                        isGT, harmfulness, range, effectRange, xAxisModifier, 
                        castType, isOriginal: isOriginal),
                ActionAoEType.Donut
                    => new DonutAoEEffectRangeData(actionId, actionCategory, 
                        isGT, harmfulness, range, effectRange, xAxisModifier, 
                        castType, isOriginal: isOriginal),
                ActionAoEType.Cross
                    => new CrossAoEEffectRangeData(actionId, actionCategory, 
                        isGT, harmfulness, range, effectRange, xAxisModifier, 
                        castType, isOriginal: isOriginal),
                _ => new NonAoEEffectRangeData(actionId, actionCategory, 
                    isGT, harmfulness, range, effectRange, xAxisModifier, 
                    castType, isOriginal: isOriginal)
            };

        public override string ToString()
            => $"{GetType().Name}{{ ActionId: {ActionId}, Category: {Category}, " +
            $"IsGT: {IsGTAction}, Harmfulness: {Harmfulness}, " +
            $"Range: {Range}, EffectRange: {EffectRange}, XAxisModifier: {XAxisModifier}, " +
            $"CastType: {CastType}, {AdditionalFieldsToString()}, IsOriginal: {IsOriginal} }}";

        protected virtual string AdditionalFieldsToString() => string.Empty;
    }

}
