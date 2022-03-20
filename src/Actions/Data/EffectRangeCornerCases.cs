using ActionEffectRange.Actions.EffectRange;
using System.Collections.Generic;

namespace ActionEffectRange.Actions.Data
{
    // The remaining cases for EffectRangeData overriding
    // that are not templatised yet
    public static class EffectRangeCornerCases
    {
        public static HashSet<EffectRangeData> GetUpdatedEffectDataSet(EffectRangeData original)
        {
            var updatedDataSet = new HashSet<EffectRangeData>();
            switch (original.ActionId)
            {
                case 7439:      // earthly star (AST)
                //case 8324:      // stellar detonation (AST) (player action of "explosion" of earthly star)
                case 7440:      // stellar burst (Pet-like action from player executing #8324 stellar detonation)
                case 7441:      // stellar explosion (Pet-like action from player executing #8324 stellar detonation)
                    // Add both harmful and beneficial
                    // (Earthly star is originally treated as beneficial while #7440 and #7441 are as harmful)
                    updatedDataSet.Add(new CircleAoEEffectRangeData(
                        original.ActionId, (uint)original.Category, original.IsGTAction, 
                        true, original.Range, original.EffectRange, original.XAxisModifier,
                        original.CastType, isOriginal: false));
                    updatedDataSet.Add(new CircleAoEEffectRangeData(
                        original.ActionId, (uint)original.Category, original.IsGTAction,
                        false, original.Range, original.EffectRange, original.XAxisModifier,
                        original.CastType, isOriginal: false));
                    return updatedDataSet;
                case 17991:     // celestial opposition (AST PvP)
                    updatedDataSet.Add(original);
                    // Add also the additional heal effect range
                    updatedDataSet.Add(new CircleAoEEffectRangeData(
                        original.ActionId, (uint)original.Category, original.IsGTAction, 
                        false, original.Range, original.EffectRange, original.XAxisModifier,
                        original.CastType, isOriginal: false));
                    return updatedDataSet;
                case 25874:     // macrocosmos (AST) 
                    updatedDataSet.Add(original);
                    // Add also the additional heal effect range
                    updatedDataSet.Add(new CircleAoEEffectRangeData(
                        original.ActionId, (uint)original.Category, original.IsGTAction,
                        false, original.Range, original.EffectRange, original.XAxisModifier,
                        original.CastType, isOriginal: false));
                    return updatedDataSet;

                case 24318:     // pneuma (SGE)
                case 27830:     // pneuma (SGE PvP)
                    // Add the additional heal effect range
                    updatedDataSet.Add(new CircleAoEEffectRangeData(
                        original.ActionId, (uint)original.Category, 
                        original.IsGTAction, false, 0, 20, 0, 2, isOriginal: false));
                    // Add back the original attack effect
                    updatedDataSet.Add(original);
                    return updatedDataSet;

                default: return updatedDataSet;
            }
        }
    }
}
