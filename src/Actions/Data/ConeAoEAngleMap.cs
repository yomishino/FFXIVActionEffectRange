using System.Collections.Generic;
using System.Collections.Immutable;

namespace ActionEffectRange.Actions.Data
{
    // May be inaccurate
    public static class ConeAoEAngleMap
    {
        public const float DefaultAngleBy2pi_Range12 = .25f;
        public const float DefaultAngleBy2pi_Range8 = 1f / 3f;
        public const float DefaultAngleBy2pi_Range6 = .25f;
        public const float AngleBy2pi_120 = 1f / 3f;

        public static readonly ImmutableDictionary<uint, float> Dictionary = new KeyValuePair<uint, float>[]
        {
            //new(7385, DefaultAngleBy2pi_Range8),      // passage of arms (PLD)
            //new(41, AngleBy2pi_120),        // overpower (WAR)
            //new(7483, AngleBy2pi_120),      // fuga (SAM)
            //new(7488, AngleBy2pi_120),      // tenka goken (SAM)
            //new(16485, AngleBy2pi_120),     // kaeshi: goken (SAM)
            // TODO: check 25781 & 25782
            //new(25781, AngleBy2pi_120),     // ogi namikiri (SAM)
            //new(25782, AngleBy2pi_120),     // kaeshi namikiri (SAM)
            new(24392, .5f),                // Grim Swathe (RPR)
            new(24384, .5f),                // Guillotine (RPR)
            new(24397, .5f),                // Grim Reaping (RPR)
            new(24400, .5f),                // Lemure's Scythe (RPR)
            new(7418, .25f),                // Flamethrower (MCH)
            new(25791, AngleBy2pi_120),     // Fan Dance IV (DNC)
            //new(7513, AngleBy2pi_120),      // Moulinet (RDM)
            //new(7530, AngleBy2pi_120),      // Enchanted Moulinet (RDM)

            new(18899, AngleBy2pi_120),     // glory slash (PLD pvp)
            //new(8830, AngleBy2pi_120),      // tenka goken (SAM pvp)
            //new(17742, AngleBy2pi_120),     // kaeshi goken (SAM pvp)
            new(27813, .5f),                // Grim Swathe (RPR pvp)
            new(27804, .5f),                // Guillotine (RPR pvp)
            new(27805, .5f),                // Grim Reaping (RPR pvp)
            new(27816, .5f),                // Lemure's Scythe (RPR pvp)
            //new(17780, AngleBy2pi_120),     // wither (SMN pvp)
            //new(18944, AngleBy2pi_120),     // Enchanted Moulinet (RDM pvp)

            // Most BLU cone aoe with effect range 6 seems 90-degree, exceptions exist
            //new(11402, AngleBy2pi_120),   // flame thrower (BLU) #2
            new(11390, .25f),           // aqua breath (BLU) #3
            new(11403, .25f),           // faze (BLU) #23
            new(11399, AngleBy2pi_120),   // the look (BLU) #27
            new(11388, .25f),           // bad breath (BLU) #28
            new(11430, 2f / 3f),        // glass dance (BLU) #48
            new(18296, 1f / 12f),       // protean wave (BLU) #51
            new(18323, AngleBy2pi_120),   // surpanakha (BLU) #78
            new(23288, .25f),           // phantom flurry (BLU) #103
            new(23289, .5f),            // phantom flurry (2nd phase) (BLU)

        }.ToImmutableDictionary();
    }
}
