using System.Collections.Immutable;

namespace ActionEffectRange.Actions.Data
{
    public static class RuledOutActions
    {
        public static readonly ImmutableHashSet<uint> HashSet = new uint[]
        {
            2262,   // Shukuchi
            8812,   // Shukuchi PvP

            //8324,   // stellar detonation   (from earthly star)

        }.ToImmutableHashSet();
    }
}
