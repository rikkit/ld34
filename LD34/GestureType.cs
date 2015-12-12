using System;

namespace LD34
{
    [Flags]
    public enum GestureType
    {
        None = 0,
        Click = 1,
        Swipe = 2,
        Scroll = 4,
        KeyPress = 8,
        Move = 16
    }
}