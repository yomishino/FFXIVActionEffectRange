using System.Collections.Generic;
using System.Collections.Immutable;

namespace ActionEffectRange.Actions.Data.Predefined
{
    // Map to actions for additional effects of the original actions
    public static class AdditionalEffectsMap
    {
        public static readonly ImmutableDictionary<uint, ImmutableArray<uint>> 
            Dictionary = new KeyValuePair<uint, ImmutableArray<uint>>[]
            {
                // PvE

                new(24318, new uint[]{ 27524 }.ToImmutableArray()), // Pneuma (SGE)

                // PvP

                new(29260, new uint[]{ 29706 }.ToImmutableArray()), // Pneuma (SGE PvP)

            }.ToImmutableDictionary();

    }
}
