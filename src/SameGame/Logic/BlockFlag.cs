using System;

namespace SameGame.Logic
{
    [Flags]
    public enum BlockFlag
    {
        None = 0,

        Hidden = 1,

        X2 = 2,

        X3 = 4,

        X5 = 8,

        Selected = 16
    }
}
