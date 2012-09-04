using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple
{
    public static class IntExtensions
    {
        public static bool GreaterThan(this int my, int other)
        {
            return my > other;
        }

        public static bool IsPositive(this int number)
        {
            return number > 0;
        }

        public static int Seconds(this int minutes)
        {
            return minutes*60;
        }

        public static int Milliseconds(this int seconds)
        {
            return seconds*1000;
        }

        public static int MinutesInMilliseconds(this int minutes)
        {
            return Seconds(minutes).Milliseconds();
        }


    }
}
