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
                new(7439, Both),        // earthly star (AST) (player action of placing earthly star)
                // #8324 - stellar detonation (AST) (player action of "explosion" of earthly star; mapped to 7440 or 7441)
                new(7440, Both),        // stellar burst (Pet-like action from player executing #8324 stellar detonation)
                new(7441, Both),        // stellar explosion (Pet-like action from player executing #8324 stellar detonation)
                new (25874, Both),       // macrocosmos (AST) 

                new(29094, Both),       // Salted Earth (DRK PvP)
                new(29130, Harmful),    // Relentless Rush (GNB PvP)
                new(29234, Both),       // Deployment Tactics (SCH PvP)
                new(29253, Both),       // Macrocosmos (AST PvP)
                new(29255, Both),       // Celestial River (AST PvP)
                // 29423~29427 actually effective
                // (on buff from #29422 ends or on #29470 used), effect based on stacks
                // #29470 now in DirectMap
                new(29423, Both),       // Honing Ovation (DNC PvP) 
                new(29424, Both),       // Honing Ovation (DNC PvP) 
                new(29425, Both),       // Honing Ovation (DNC PvP) 
                new(29426, Both),       // Honing Ovation (DNC PvP) 
                new(29427, Both),       // Honing Ovation (DNC PvP) 
                new(29514, Harmful),    // Doton (NIN PvP)
                new(29673, Harmful),    // Summon Bahamut (SMN PvP)
                new(29704, Both),       // Southern Cross (RDM PvP) (white)
                new(29705, Both),       // Southern Cross (RDM PvP) (black)
            }.ToImmutableDictionary();
    }
}
