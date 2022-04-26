using ActionEffectRange.Actions.Enums;

namespace ActionEffectRange.Actions.EffectRange
{
    public class ConeAoEEffectRangeData : EffectRangeData
    {
        public readonly float CentralAngleCycles;
        public readonly float RotationOffset;

        public ConeAoEEffectRangeData(uint actionId, 
            uint actionCategory, bool isGT, ActionHarmfulness harmfulness, 
            sbyte range, byte effectRange, byte xAxisModifier, byte castType, 
            float centralAngleCycles = 0, float rotationOffset = 0, 
            bool isOriginal = false)
            : base(actionId, actionCategory, isGT, harmfulness,
                  range, effectRange, xAxisModifier, castType, isOriginal)
        {
            CentralAngleCycles = centralAngleCycles;
            RotationOffset = rotationOffset;
        }

        public ConeAoEEffectRangeData(Lumina.Excel.GeneratedSheets.Action actionRow,
            float centralAngleCycles = 0, float rotationOffset = 0)
            : this(actionRow.RowId, actionRow.ActionCategory.Row, actionRow.TargetArea,
                  ActionData.GetActionHarmfulness(actionRow), actionRow.Range,
                  actionRow.EffectRange, actionRow.XAxisModifier, actionRow.CastType,
                  centralAngleCycles, rotationOffset, isOriginal: true)
        { }

        public ConeAoEEffectRangeData(EffectRangeData original,
            float centralAngleCycles = 0, float rotationOffset = 0,
            bool isOriginal = true)
            : this(original.ActionId, (uint)original.Category,
                  original.IsGTAction, original.Harmfulness,
                  original.Range, original.EffectRange, 
                  original.XAxisModifier, original.CastType,
                  centralAngleCycles, rotationOffset, isOriginal)
        { }

        protected override string AdditionalFieldsToString()
            => $"Angle: {CentralAngleCycles}, RotationOffset: {RotationOffset}";
    }
}
