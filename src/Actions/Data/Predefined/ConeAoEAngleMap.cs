using ActionEffectRange.Actions.Data.Template;
using System.Collections.Generic;
using System.Collections.Immutable;
using System;

namespace ActionEffectRange.Actions.Data.Predefined
{
    // May be inaccurate
    public static class ConeAoEAngleMap
    {
        // All central angles used internally are in cycles (1 cycle = 2pi)
        // RotaionOffsets are in radians though,
        // to be consistent and directly applicable with character rotation

        public const float DefaultAngleCycles = 1f / 3f;

        public static readonly ImmutableDictionary<byte, float> DefaultAnglesByRange
            = new KeyValuePair<byte, float>[]
            {
                new(6, .25f),
                new(8, 1f / 3f),
                new(12, .25f)
            }.ToImmutableDictionary();

        public static ImmutableDictionary<uint, ConeAoEAngleDataItem> PredefinedActionMap
            => new KeyValuePair<uint, ConeAoEAngleDataItem>[]
            {
                GeneratePair(7385, 1f / 3f, MathF.PI),  // Passage of Arms (PLD)
                GeneratePair(24392, .5f),               // Grim Swathe (RPR)
                GeneratePair(24384, .5f),               // Guillotine (RPR)
                GeneratePair(24397, .5f),               // Grim Reaping (RPR)
                GeneratePair(24400, .5f),               // Lemure's Scythe (RPR)
                GeneratePair(7418, .25f),               // Flamethrower (MCH)
                GeneratePair(25791, 1f / 3f),           // Fan Dance IV (DNC)

                GeneratePair(29428, .25f),              // Fan Dance (DNC PvP)      --- idk why devteam decided to make it smaller than PvE version but nah
                GeneratePair(29547, .5f),               // Grim Swathe (RPR PvP)
                GeneratePair(29548, .5f),               // Lemure's Slice (RPR PvP)

                // BLU up to #104
                // Most BLU cone aoe with effect range 6 seems 90-degree, exceptions exist
                GeneratePair(11390, .25f),           // aqua breath (BLU) #3
                GeneratePair(11403, .25f),           // faze (BLU) #23
                GeneratePair(11399, 1f / 3f),   // the look (BLU) #27
                GeneratePair(11388, .25f),           // bad breath (BLU) #28
                GeneratePair(11430, 2f / 3f),        // glass dance (BLU) #48
                GeneratePair(18296, 1f / 12f),       // protean wave (BLU) #51
                GeneratePair(18323, 1f / 3f),   // surpanakha (BLU) #78
                GeneratePair(23288, .25f),           // phantom flurry (BLU) #103
                GeneratePair(23289, .5f),            // phantom flurry (2nd phase) (BLU)
            }.ToImmutableDictionary();

        private static KeyValuePair<uint, ConeAoEAngleDataItem>
            GeneratePair(uint actionId, float angle, float rotationOffset = 0)
                => new(actionId, new(actionId, angle, rotationOffset));

    }
}
