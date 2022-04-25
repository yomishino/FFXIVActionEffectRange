using ActionEffectRange.Actions.EffectRange;
using System.Collections.Generic;

namespace ActionEffectRange.Actions.Data.Predefined
{
    // The remaining cases for EffectRangeData overriding
    // that are not templatised yet
    public static class EffectRangeCornerCases
    {
        public static List<EffectRangeData> GetUpdatedEffectDataList(EffectRangeData original)
        {
            var updated = new List<EffectRangeData>();
            switch (original.ActionId)
            {
                case 7439:      // earthly star (AST)
                //case 8324:      // stellar detonation (AST) (player action of "explosion" of earthly star)
                case 7440:      // stellar burst (Pet-like action from player executing #8324 stellar detonation)
                case 7441:      // stellar explosion (Pet-like action from player executing #8324 stellar detonation)
                    // Add both harmful and beneficial
                    // (Earthly star is originally treated as beneficial while #7440 and #7441 are as harmful)
                    updated.Add(new CircleAoEEffectRangeData(
                        original.ActionId, (uint)original.Category, original.IsGTAction, 
                        true, original.Range, original.EffectRange, original.XAxisModifier,
                        original.CastType, isOriginal: false));
                    updated.Add(new CircleAoEEffectRangeData(
                        original.ActionId, (uint)original.Category, original.IsGTAction,
                        false, original.Range, original.EffectRange, original.XAxisModifier,
                        original.CastType, isOriginal: false));
                    return updated;
                case 25874:     // macrocosmos (AST) 
                    updated.Add(original);
                    // Add also the additional heal effect range
                    updated.Add(new CircleAoEEffectRangeData(
                        original.ActionId, (uint)original.Category, original.IsGTAction,
                        false, original.Range, original.EffectRange, original.XAxisModifier,
                        original.CastType, isOriginal: false));
                    return updated;
                case 24318:     // pneuma (SGE)
                    // Add the additional heal effect range
                    updated.Add(new CircleAoEEffectRangeData(
                        original.ActionId, (uint)original.Category, 
                        original.IsGTAction, false, 0, 20, 0, 2, isOriginal: false));
                    // Add back the original attack effect
                    updated.Add(original);
                    return updated;

                // PvP cases
                case 29097:     // Eventide (DRK PvP)
                    // Add the additional line effect range to the back;
                    //  also halve the effect range:
                    //  the original effect range is for front + back
                    // This would result in a "horizontal" line drawn in the centre
                    // if users choose to make shape outline visible though
                    updated.Add(new LineAoEEffectRangeData(
                        original.ActionId, (uint)original.Category, 
                        original.IsGTAction, original.IsHarmfulAction, 
                        original.Range, (byte)(original.EffectRange / 2), 
                        original.XAxisModifier,original.CastType, 
                        System.MathF.PI, isOriginal: false));
                    updated.Add(new LineAoEEffectRangeData(
                        original.ActionId, (uint)original.Category, 
                        original.IsGTAction,original.IsHarmfulAction, 
                        original.Range, (byte)(original.EffectRange / 2), 
                        original.XAxisModifier, original.CastType, isOriginal: false));
                    return updated;
                case 29260:     // Pneuma (SGE PvP)
                    // Add the additional heal effect range
                    updated.Add(new CircleAoEEffectRangeData(
                        original.ActionId, (uint)original.Category,
                        original.IsGTAction, false, 0, 20, 0, 2, isOriginal: false));
                    // Add back the original attack effect
                    updated.Add(original);
                    return updated;
                case 29422:     // Honing Dance (DNC PvP)
                    // Override as Circle AoE, also providing EffectRange
                    updated.Add(new CircleAoEEffectRangeData(
                        original.ActionId, (uint)original.Category,
                        original.IsGTAction, true, 0, 5, 0, 2, isOriginal: false));
                    return updated;
                case 29532:     // Hissatsu: Soten (SAM PvP)
                    // Providing EffectRange 
                    // EffectRange of 1 is from Excel data for the old PvP Soten skill
                    updated.Add(new DashAoEEffectRangeData(
                        original.ActionId, (uint)original.Category,
                        original.IsGTAction, original.IsHarmfulAction,
                        original.Range, 1, original.XAxisModifier,
                        original.CastType, isOriginal: false));
                    return updated;
                case 29704:     // Southern Cross (White/Black) (RDM PvP)
                case 29705:     // Southern Cross (Black) (RDM PvP)
                    // Seems to have a 45-degree rotation offset
                    updated.Add(new CrossAoEEffectRangeData(
                        original, -System.MathF.PI / 4));
                    return updated;
                default: return updated;
            }
        }
    }
}
