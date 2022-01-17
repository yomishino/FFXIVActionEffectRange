using System.Collections.Generic;
using System.Collections.Immutable;

namespace ActionEffectRange.Actions.Data
{
    // Actions implemented as pet actions
    public class PetLikeActionMap
    {
        public static readonly ImmutableDictionary<uint, ImmutableHashSet<uint>> Dictionary = new KeyValuePair<uint, ImmutableHashSet<uint>>[]
        {
            new(25774, new uint[]{ 25775 }.ToImmutableHashSet()),     // phantom kamaitachi (NIN)
            
            new(8324, new uint[]{ 7440, 7441 }.ToImmutableHashSet()),   // stellar detonation (player); stellar burst, stellar explosion (pet) (<- earthly star) (AST)

        }.ToImmutableDictionary();
    }
}
