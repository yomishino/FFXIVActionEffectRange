using ActionEffectRange.Actions.Data.Template;
using System.Collections.Generic;
using System.Collections.Immutable;
using System;

namespace ActionEffectRange.Actions.Data.Predefined
{
    // May be inaccurate
    public static class ConeAoEAngleMap
    {
        public const float DefaultAngle = 1f / 3f;

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

                // TODO: 25781 - ogi namikiri (SAM), 25782 - kaeshi namikiri (SAM): 120deg (as default) ?

                GeneratePair(18899, 1f / 3f),     // glory slash (PLD pvp)
                GeneratePair(27813, .5f),                // Grim Swathe (RPR pvp)
                GeneratePair(27804, .5f),                // Guillotine (RPR pvp)
                GeneratePair(27805, .5f),                // Grim Reaping (RPR pvp)
                GeneratePair(27816, .5f),                // Lemure's Scythe (RPR pvp)

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
