using System;

namespace SameGame.Logic
{
    [Flags]
    public enum BlockFlag
    {
        None = 0,
        
        X2 = 1,

        X3 = 2,

        X5 = 4,

        Selected = 8
    }
}
