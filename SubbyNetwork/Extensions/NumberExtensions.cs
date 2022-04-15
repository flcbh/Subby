using System;

namespace SubbyNetwork.Extensions
{
    public static class NumberExtensions
    {
        public static int RoundOff (this int i)
        {
            return ((int)Math.Round(i / 10.0)) * 10;
        }
    }
}