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
                    // Override CastType as cone AoE and force it to be "harmful"
                    updatedDataSet.Add(new(originalData.ActionId, (uint)originalData.Category, originalData.IsGTAction, true,
                        originalData.Range, originalData.EffectRange, 3, originalData.XAxisModifier, isOriginal: false));
                    return updatedDataSet;
                case 7439:      // earthly star
                    // Add also as harmful action
                    updatedDataSet.Add(new(originalData.ActionId, (uint)originalData.Category, originalData.IsGTAction, true,
                        originalData.Range, originalData.EffectRange, originalData.CastType, originalData.XAxisModifier, isOriginal: false));
                    // Add original heal effect later to "prioritise" drawing it as beneficial
                    updatedDataSet.Add(originalData);
                    return updatedDataSet;
                case 17991:     // celestial opposition (AST PvP)
                    updatedDataSet.Add(originalData);
                    // Add also the additional heal effect range
                    updatedDataSet.Add(new(originalData.ActionId, (uint)originalData.Category, originalData.IsGTAction, false,
                        originalData.Range, originalData.EffectRange, originalData.CastType, originalData.XAxisModifier, isOriginal: false));
                    return updatedDataSet;
                case 25874:     // macrocosmos (AST) 
                    updatedDataSet.Add(originalData);
                    // Add also the additional heal effect range
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
                    updatedDataSet.Add(new(originalData, additionalEffectRange: 8));
                    return updatedDataSet;
                case 11430:     // glass dance (BLU)
                    // Set customised central angle
                    const float ratio11430 = 2f / 3f;
                    updatedDataSet.Add(new(originalData, ratio: ratio11430));
                    return updatedDataSet;
                default: return updatedDataSet;
            }
        }
    }
}
