using ActionEffectRange.Actions.Data.Template;
using ActionEffectRange.Actions.Enums;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ActionEffectRange.Actions.Data.Predefined
{
    public static class AoETypeOverridingMap
    {
        public static readonly ImmutableDictionary<uint, AoETypeDataItem> PredefinedSpecial
            = new KeyValuePair<uint, AoETypeDataItem>[]
            {
                GeneratePair(2270, ActionAoEType.Circle, true),     // Doton (NIN)
                GeneratePair(3639, ActionAoEType.Circle, true),     // salted earth (DRK)

                GeneratePair(7385, ActionAoEType.Cone, false),  // Passage of Arms (PLD)
                GeneratePair(7418, ActionAoEType.Cone, true),   // Flamethrower (MCH)

                GeneratePair(16553, ActionAoEType.Circle, false),   // celestial opposition (AST)
            }.ToImmutableDictionary();

        private static KeyValuePair<uint, AoETypeDataItem>
            GeneratePair(uint actionId, byte castType, bool isHarmful)
                => new(actionId, new(actionId, castType, isHarmful));

        private static KeyValuePair<uint, AoETypeDataItem>
            GeneratePair(uint actionId, ActionAoEType aoeType, bool isHarmful)
                => GeneratePair(actionId, (byte)aoeType, isHarmful);
    }
}
