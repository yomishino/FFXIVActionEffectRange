using System.Collections.Immutable;

namespace ActionEffectRange.Actions.Data.Predefined
{
    public static class RuledOutActions
    {
        public static readonly ImmutableHashSet<uint> HashSet = new uint[]
        {
            2262,   // Shukuchi
            8812,   // Shukuchi PvP

            11401,  // Loom (BLU)

        }.ToImmutableHashSet();
    }
}
