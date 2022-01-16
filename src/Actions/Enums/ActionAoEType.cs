namespace ActionEffectRange.Actions.Enums
{
    // Action.CastType
    public enum ActionAoEType : byte
    {
        None,
        Circle = 2,
        Cone = 3,
        Line = 4,
        GT = 7,
        DashAoE = 8,    // dash and do dmg on the route, e.g. soten
        Donut = 10,
    }

}
