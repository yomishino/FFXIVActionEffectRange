using System.Collections.Generic;
using System.Collections.Immutable;

namespace ActionEffectRange.Actions.Data.Predefined
{
    // Actions here should not try to match cached SeqInfo
    //  but always use current position info
    internal static class NoCachedSeqActions
    {
        public static ImmutableHashSet<uint> Set => new HashSet<uint>
        {
            29557,  // Relentless Rush (GNB PvP) (persistent effect following #29130)
            29558,  // Honing Dance (DNC PvP) (persistent effect following #29422)
        }.ToImmutableHashSet();
    }
}
