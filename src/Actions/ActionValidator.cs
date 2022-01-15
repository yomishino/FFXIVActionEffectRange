using System.Collections.Generic;
using System.Linq;

namespace ActionEffectRange.Actions
{
    public static partial class ActionValidator
    {
        public static bool ShouldDrawForActionType(uint actionType) => actionType == 0x1 || actionType == 0xE; // pve 0x1, pvp 0xE

        // Only check for circle (2) and donut (10)
        public static bool ShouldDrawForEffectRange(byte castType, byte effectRange) =>
            effectRange > 0 && (!(castType == 2 || castType == 10) || (Plugin.Config.LargeDrawOpt != 1 || effectRange < Plugin.Config.LargeThreshold));


        //public static bool IsBlacklistedAction(uint actionId)


        public static bool CheckPetAction(uint ownerActionId, out HashSet<EffectRangeData?>? petActionEffectRangeDataSet)
        {
            petActionEffectRangeDataSet = null;
            if (!Plugin.BuddyList.PetBuddyPresent 
                || !ActionData.TryGetMappedPetAction(ownerActionId, out var petActionIds) || petActionIds == null) return false;

            petActionEffectRangeDataSet = petActionIds
                .Select(id => ActionData.GetActionEffectRangeData(id))
                .Where(data => data != null && ShouldDrawForEffectRange(data.CastType, data.EffectRange))    
                .ToHashSet();

            return petActionEffectRangeDataSet.Any();
        }


        public static HashSet<EffectRangeData> UpdateEffectRangeData(EffectRangeData originalData)
        {
            if (!CheckCornerCases(originalData, out var dataSet) 
                && ShouldDrawForEffectRange(originalData.CastType, originalData.EffectRange)) 
                dataSet.Add(originalData);
            return dataSet;
        }


        // TODO: dnc waltz partner's

        private static bool CheckCornerCases(EffectRangeData originalData, out HashSet<EffectRangeData> updatedDataSet)
        {
            updatedDataSet = new();
            switch (originalData.ActionId)
            {
                case 7418:      // flamethrower (MCH)
                    if (ShouldDrawForEffectRange(originalData.CastType, originalData.EffectRange))
                        updatedDataSet.Add(
                            new(originalData.ActionId, (uint)originalData.Category, originalData.IsHarmfulAction,
                                originalData.Range, originalData.EffectRange, 3, originalData.XAxisModifier, isOriginal: false));
                    return true;
                case 25874:  // macrocosmos (AST)   
                    // TODO: macrocosmos need test
                    if (ShouldDrawForEffectRange(originalData.CastType, originalData.EffectRange))
                    {
                        if (Plugin.Config.DrawHarmful)
                            updatedDataSet.Add(originalData);
                        else if (Plugin.Config.DrawBeneficial)  // don't draw it twice since both effect are of same effect range
                            updatedDataSet.Add(
                                new(originalData.ActionId, (uint)originalData.Category, false, originalData.Range, originalData.EffectRange,
                                    originalData.CastType, originalData.XAxisModifier, isOriginal: false));
                    }
                    return true;
                case 24318:     // pneuma (SGE)
                case 27830:     // pneuma (SGE PvP)
                    if (ShouldDrawForEffectRange(originalData.CastType, originalData.EffectRange))
                        updatedDataSet.Add(originalData);
                    // Add the additional heal effect range
                    if (Plugin.ClientState.LocalPlayer != null && ShouldDrawForEffectRange(2, 20))
                        updatedDataSet.Add(new(originalData.ActionId, (uint)originalData.Category, false, 0, 20, 2, 0, isOriginal: false));
                    return true;
                case 11420:     // dragon's voice (BLU)
                    if (ShouldDrawForEffectRange(originalData.CastType, originalData.EffectRange))
                        updatedDataSet.Add(
                            new(originalData.ActionId, (uint)originalData.Category, originalData.IsHarmfulAction, originalData.Range, 
                                originalData.EffectRange, originalData.CastType, originalData.XAxisModifier, additionalEffectRange: 8, isOriginal: false));
                    return true;
                default: return false;
            }
        }
    }
}
