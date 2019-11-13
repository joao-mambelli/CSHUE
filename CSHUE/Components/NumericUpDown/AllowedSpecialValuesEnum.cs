using System;

namespace CSHUE.Components.NumericUpDown
{
    [Flags]
    public enum AllowedSpecialValues
    {
        None = 0,
        NaN = 1,
        PositiveInfinity = 2,
        NegativeInfinity = 4,
        AnyInfinity = 6,
        Any = 7
    }
}