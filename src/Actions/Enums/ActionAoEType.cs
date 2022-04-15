namespace ActionEffectRange.Actions.Enums
{
    // Action.CastType
    public enum ActionAoEType : byte
    {
        None,
        
        Circle = 2,
        Cone = 3,
        Line = 4,
        Circle2 = 5,

        GT = 7,
        DashAoE = 8,    // dash and do dmg on the route, e.g. soten
                        // actually may be Line AoE with length adjusted to target position, not always dashing
        
        Donut = 10,
        Cross = 11,
        Line2 = 12,
        Cone2 = 13,
    }

}
