using System.Collections.Generic;
using System.Collections.Immutable;

namespace ActionEffectRange.Actions.Data.Predefined
{
    public static class DonutAoERadiusMap
    {
        // InnerRadius
        public static readonly ImmutableDictionary<uint, byte> Predefined
            = new KeyValuePair<uint, byte>[]
            {
                new(11420, 8),      // dragon's voice (BLU)

            }.ToImmutableDictionary();
    }
}
