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
                    updatedDataSet.Add(new(originalData, isHarmful: true));
                    return updatedDataSet;
                case 7418:      // flamethrower (MCH)
                    // Override CastType as cone AoE, angle default
                    updatedDataSet.Add(new(originalData.ActionId, (uint)originalData.Category, originalData.IsGTAction, true,
                        originalData.Range, originalData.EffectRange, 3, originalData.XAxisModifier, isOriginal: false));
                    return updatedDataSet;
                case 7385:      // Passage of Arms (PLD)
                    // Overide it to be Cone and add rotation offset to adjust to cone towards back
                    //const float ratio7385 = 1f / 3f;
                    //updatedDataSet.Add(new(originalData.ActionId, (uint)originalData.Category, originalData.IsGTAction, originalData.IsHarmfulAction,
                    //    originalData.Range, originalData.EffectRange, 3, originalData.XAxisModifier, 
                    //    ratio: ratio7385, rotationOffset: System.MathF.PI, isOriginal: false));
                    updatedDataSet.Add(new(originalData, rotationOffset: System.MathF.PI));
                    return updatedDataSet;
                case 7439:      // earthly star
                    // Add also as harmful action
                    updatedDataSet.Add(new(originalData, isHarmful: true));
                    // Add original heal effect later to "prioritise" drawing it as beneficial
                    updatedDataSet.Add(originalData);
                    return updatedDataSet;
                case 16553:     // celestial opposition (AST)
                    // Force it to be "beneficial"
                    updatedDataSet.Add(new(originalData, isHarmful: false));
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
                    updatedDataSet.Add(new(originalData, isHarmful: false));
                    return updatedDataSet;
                case 24318:     // pneuma (SGE)
                case 27830:     // pneuma (SGE PvP)
                    updatedDataSet.Add(originalData);
                    // Add the additional heal effect range
                    updatedDataSet.Add(new(originalData.ActionId, (uint)originalData.Category, originalData.IsGTAction, false, 0, 20, 2, 0, isOriginal: false));
                    return updatedDataSet;
                //case 11399:     // the look (BLU)
                //    // Set customised central angle
                //    const float ratio11399 = 1f / 3f;
                //    updatedDataSet.Add(new(originalData, ratio: ratio11399));
                //    return updatedDataSet;
                //case 11402:     // flame thrower (BLU)
                //    // Set customised central angle
                //    const float ratio11402 = 1f / 3f;
                //    updatedDataSet.Add(new(originalData, ratio: ratio11402));
                //    return updatedDataSet;
                case 11420:     // dragon's voice (BLU)
                    updatedDataSet.Add(new(originalData, additionalEffectRange: 8));
                    return updatedDataSet;
                //case 11430:     // glass dance (BLU)
                //    // Set customised central angle
                //    const float ratio11430 = 2f / 3f;
                //    updatedDataSet.Add(new(originalData, ratio: ratio11430));
                //    return updatedDataSet;
                //case 18296:     // protean wave (BLU)
                //    const float ratio18296 = 1f / 12f;
                //    updatedDataSet.Add(new(originalData, ratio: ratio18296));
                //    return updatedDataSet;
                //case 18323:     // surpanakha (BLU)
                //    const float ratio18323 = 1f / 3f;
                //    updatedDataSet.Add(new(originalData, ratio: ratio18323));
                //    return updatedDataSet;
                //case 23289:     // phantom flurry (2nd phase) (BLU)
                //    // Set customised central angle
                //    updatedDataSet.Add(new(originalData, centralAngleBy2pi: .5f));
                //    return updatedDataSet;
                default: return updatedDataSet;
            }
        }
    }
}
