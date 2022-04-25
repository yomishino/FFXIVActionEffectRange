using System.Collections.Generic;
using System.Collections.Immutable;

namespace ActionEffectRange.Actions.Data.Predefined
{
    // A -> {A'}: on A used, directly mapped to and wait for any in {A'}
    public class DirectMap
    {
        public static readonly ImmutableDictionary<uint, ImmutableArray<uint>> Dictionary =
            new KeyValuePair<uint, ImmutableArray<uint>>[]
            {
                new(29470, BuildArray(29423, 29424, 29425, 29426, 29427)),    // Honing Ovation (DNC PvP) (player -> auto)
                new(29499, BuildArray(29498)),        // Sky Shatter (DRG PvP) (player -> auto)
                new(29469, BuildArray(29131)),        // Terminal Trigger (GNB PvP) (player -> auto)
            }.ToImmutableDictionary();

        private static ImmutableArray<uint> BuildArray(params uint[] actionId)
            => actionId.ToImmutableArray();
    }
}
