using ActionEffectRange.Actions.Enums;
using System.Collections.Generic;
using System.Collections.Immutable;

using static ActionEffectRange.Actions.Enums.ActionHarmfulness;

namespace ActionEffectRange.Actions.Data.Predefined
{
    public class HarmfulnessMap
    {
        public static readonly ImmutableDictionary<uint, ActionHarmfulness> Dictionary =
            new KeyValuePair<uint, ActionHarmfulness>[]
            {
                new(2270, Harmful),     // Doton (NIN)
                new(3571, Both),        // Assize (WHM)
                new(3639, Harmful),     // salted earth (DRK)
                new(7439, Both),        // earthly star (AST) (player action of placing earthly star)
                // #8324 - stellar detonation (AST) (player action of "explosion" of earthly star; mapped to 7440 or 7441)
                new(7440, Both),        // stellar burst (Pet-like action from player executing #8324 stellar detonation)
                new(7441, Both),        // stellar explosion (Pet-like action from player executing #8324 stellar detonation)
                new(16193, Both),       // Single Technical Finish (DNC)
                new(16194, Both),       // Double Technical Finish (DNC)
                new(16195, Both),       // Triple Technical Finish (DNC)
                new(16196, Both),       // Quadruple Technical Finish (DNC)
                new(16544, Beneficial), // Fey Blessing (SCH) (action by pet)
                new(16553, Beneficial), // celestial opposition (AST)
                new (25874, Both),      // macrocosmos (AST) 

                new(29094, Both),       // Salted Earth (DRK PvP)
                new(29130, Harmful),    // Relentless Rush (GNB PvP)
                new(29229, Both),       // Seraph Strike (WHM PvP)
                new(29234, Both),       // Deployment Tactics (SCH PvP)
                new(29253, Both),       // Macrocosmos (AST PvP)
                new(29255, Both),       // Celestial River (AST PvP)
                new(29266, Both),       // Mesotes (SGE PvP)
                new(29267, Both),       // Mesotes (SGE PvP) (after #29266)
                new(29412, Both),       // Bishop Autoturret (MCH PvP)
                new(29413, Both),       // Aether Mortar (MCH PvP) (after #29412)
                new(29422, Harmful),    // Honing Dance (DNC PvP)
                // (#29423~29427 triggered on #29422 ended or on #29470 used), effect based on stacks
                new(29423, Both),       // Honing Ovation (DNC PvP) 
                new(29424, Both),       // Honing Ovation (DNC PvP) 
                new(29425, Both),       // Honing Ovation (DNC PvP) 
                new(29426, Both),       // Honing Ovation (DNC PvP) 
                new(29427, Both),       // Honing Ovation (DNC PvP) 
                new(29514, Harmful),    // Doton (NIN PvP)
                new(29673, Harmful),    // Summon Bahamut (SMN PvP)
                new(29685, Both),       // Verholy (RDM PvP) (white)
                new(29704, Both),       // Southern Cross (RDM PvP) (white)
                new(29705, Both),       // Southern Cross (RDM PvP) (black)
                new(29706, Beneficial), // Pneuma (SGE PvP) additional circle beneficial effect from #29260
            }.ToImmutableDictionary();
    }
}
