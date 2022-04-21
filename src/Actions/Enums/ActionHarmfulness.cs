using System;

namespace ActionEffectRange.Actions.Enums
{
    [Flags]
    public enum ActionHarmfulness
    {
        Harmful = 1,
        Beneficial = 2,
        Both = Harmful | Beneficial,
    }
}
