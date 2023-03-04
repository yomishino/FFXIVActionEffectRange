using ActionEffectRange.Actions.EffectRange;

namespace ActionEffectRange.Actions.Data.Predefined
{
    // The remaining cases for EffectRangeData overriding
    public static class EffectRangeCornerCases
    {
        public static EffectRangeData UpdateEffectRangeData(EffectRangeData erdata)
            => erdata.ActionId switch
            {
                // PvE

                // Liturgy of the bell (placing) (WHM)
                //  Use the effect range from #25863
                25862 => new CircleAoEEffectRangeData(
                        erdata.ActionId, (uint)erdata.Category,
                        erdata.IsGTAction, erdata.Harmfulness,
                        0, ActionData.GetActionExcelRow(25863)?.EffectRange ?? 0,
                        0, erdata.CastType, false),

                // PvP

                // Eventide (DRK PvP)
                //  Change to bidirected line AoE (front+back)
                //  The original effect range is for front + back in total.
                29097 => new BidirectedLineAoEEffectRangeData(
                        erdata.ActionId, (uint)erdata.Category,
                        erdata.IsGTAction, erdata.Harmfulness, erdata.Range, 
                        erdata.EffectRange, erdata.XAxisModifier, 
                        erdata.CastType, 0, isOriginal: true),

                // Hissatsu: Soten (SAM PvP)
                //  Providing EffectRange 
                //  EffectRange of 1 is from Excel data for the old PvP Soten skill
                29532 => new DashAoEEffectRangeData(
                        erdata.ActionId, (uint)erdata.Category,
                        erdata.IsGTAction, erdata.Harmfulness,
                        erdata.Range, 1, erdata.XAxisModifier,
                        erdata.CastType, isOriginal: false),

                // Southern Cross (White/Black) (RDM PvP)
                // Southern Cross (Black) (RDM PvP)
                //  Seems to have a 45-degree rotation offset
                29704 or 29705 => new CrossAoEEffectRangeData(
                        erdata, -MathF.PI / 4),

                _ => erdata
            };
    }
}
