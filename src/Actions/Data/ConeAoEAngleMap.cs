using System.Collections.Generic;
using System.Collections.Immutable;

namespace ActionEffectRange.Actions.Data
{
    // May be inaccurate
    public static class ConeAoEAngleMap
    {
        public const float DefaultAngleBy2pi = .25f;
        public const float AngleBy2pi_120 = 1f / 3f;

        public static readonly ImmutableDictionary<uint, float> Dictionary = new KeyValuePair<uint, float>[]
        {
            new(7385, AngleBy2pi_120),    // Passage of Arms (PLD)

            // These seems default:
            // 7418, flamethrower (MCH)

            // BLU other cone aoe skills seem all using default, up to 6.0x
            new(11399, AngleBy2pi_120),   // the look (BLU)
            new(11402, AngleBy2pi_120),   // flame thrower (BLU)
            new(11430, 2f / 3f),        // glass dance (BLU)
            new(18296, 1f / 12f),       // protean wave (BLU)
            new(18323, AngleBy2pi_120),   // surpanakha (BLU)
            new(23289, .5f),            // phantom flurry (2nd phase) (BLU)

        }.ToImmutableDictionary();
    }
}
