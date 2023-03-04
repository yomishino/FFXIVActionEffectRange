using System;

namespace ActionEffectRange.Actions.Enums
{
    [Flags]
    public enum ActionHarmfulness
    {
        None = 0,
        Harmful = 1,
        Beneficial = 2,
        Both = Harmful | Beneficial,
    }
}
