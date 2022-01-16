using Dalamud.Logging;
using System.Collections.Generic;

namespace ActionEffectRange.Actions.Data
{
    // Corner cases that needs modification in generating EffectRangeData
    public static class EffectRangeCornerCases
    {
        // TODO: dnc waltz partner's
        public static HashSet<EffectRangeData> GetUpdatedEffectDataSet(EffectRangeData originalData)
        {
            var updatedDataSet = new HashSet<EffectRangeData>();
            switch (originalData.ActionId)
            {
                case 2270:      // Doton (NIN)
                case 3639:      // salted earth (DRK)
                    // Force it to be "harmful"
                    updatedDataSet.Add(new(originalData.ActionId, (uint)originalData.Category, originalData.IsGTAction, true,
                        originalData.Range, originalData.EffectRange, originalData.CastType, originalData.XAxisModifier, isOriginal: false));
                    return updatedDataSet;
                case 7418:      // flamethrower (MCH)
                    updatedDataSet.Add(new(originalData.ActionId, (uint)originalData.Category, originalData.IsGTAction, originalData.IsHarmfulAction,
                        originalData.Range, originalData.EffectRange, 3, originalData.XAxisModifier, isOriginal: false));
                    return updatedDataSet;
                case 25774:     // phantom kamaitachi (NIN)
                    // Override with 25775's data which is the actual action used by Bunshin
                    var a25775 = ActionData.GetActionEffectRangeDataRaw(25775);
                    if (a25775 == null)
                    {
                        PluginLog.Error($"CheckCornerCases: No excel row found for action of id {25775}");
                        return updatedDataSet;
                    }
                    updatedDataSet.Add(new(originalData.ActionId, (uint)originalData.Category, a25775.IsGTAction, a25775.IsHarmfulAction,
                        a25775.Range, a25775.EffectRange, a25775.CastType, a25775.XAxisModifier, isOriginal: false));
                    return updatedDataSet;
                case 25874:  // macrocosmos (AST)   
                    // TODO: macrocosmos need test
                    updatedDataSet.Add(originalData);
                    updatedDataSet.Add(new(originalData.ActionId, (uint)originalData.Category, originalData.IsGTAction, false, 
                        originalData.Range, originalData.EffectRange, originalData.CastType, originalData.XAxisModifier, isOriginal: false));
                    return updatedDataSet;
                case 24318:     // pneuma (SGE)
                case 27830:     // pneuma (SGE PvP)
                    updatedDataSet.Add(originalData);
                    // Add the additional heal effect range
                    updatedDataSet.Add(new(originalData.ActionId, (uint)originalData.Category, originalData.IsGTAction, false, 0, 20, 2, 0, isOriginal: false));
                    return updatedDataSet;
                case 11420:     // dragon's voice (BLU)
                    updatedDataSet.Add(new(originalData.ActionId, (uint)originalData.Category, originalData.IsGTAction, originalData.IsHarmfulAction, 
                        originalData.Range, originalData.EffectRange, originalData.CastType, originalData.XAxisModifier, additionalEffectRange: 8, isOriginal: false));
                    return updatedDataSet;
                default: return updatedDataSet;
            }
        }
    }
}
