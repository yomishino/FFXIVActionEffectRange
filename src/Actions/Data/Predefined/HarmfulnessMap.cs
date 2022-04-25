using ActionEffectRange.Actions.Enums;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ActionEffectRange.Actions.Data.Predefined
{
    public class HarmfulnessMap
    {
        public static readonly ImmutableDictionary<uint, ActionHarmfulness> Dictionary =
            new KeyValuePair<uint, ActionHarmfulness>[]
            {
                new(29094, ActionHarmfulness.Both),     // Salted Earth (DRK PvP)
                new(29130, ActionHarmfulness.Harmful),  // Relentless Rush (GNB PvP)
                new(29234, ActionHarmfulness.Both),     // Deployment Tactics (SCH PvP)
                new(29253, ActionHarmfulness.Both),     // Macrocosmos (AST PvP)
                new(29255, ActionHarmfulness.Both),     // Celestial River (AST PvP)
                // 29423~29427 actually effective (on buff from #29422 ends or on #29470 used), effect based on stacks
                // #29470 now in DirectMap
                new(29423, ActionHarmfulness.Both),     // Honing Ovation (DNC PvP) 
                new(29424, ActionHarmfulness.Both),     // Honing Ovation (DNC PvP) 
                new(29425, ActionHarmfulness.Both),     // Honing Ovation (DNC PvP) 
                new(29426, ActionHarmfulness.Both),     // Honing Ovation (DNC PvP) 
                new(29427, ActionHarmfulness.Both),     // Honing Ovation (DNC PvP) 
                //new(29470, ActionHarmfulness.Both),     // Honing Ovation (DNC PvP) (player invocation)
                // Now in DirectMap
                //new(29499, ActionHarmfulness.Harmful),  // Sky Shatter (DRG PvP) (player invocation; alternative of auto-triggered #29498)
                new(29514, ActionHarmfulness.Harmful),  // Doton (NIN PvP)
                new(29673, ActionHarmfulness.Harmful),  // Summon Bahamut (SMN PvP)
                new(29704, ActionHarmfulness.Both),     // Southern Cross (RDM PvP) (white)
                new(29705, ActionHarmfulness.Both),     // Southern Cross (RDM PvP) (black)
            }.ToImmutableDictionary();
    }
}
