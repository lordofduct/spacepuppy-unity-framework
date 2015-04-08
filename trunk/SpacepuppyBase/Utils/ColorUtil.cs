using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Utils
{
    public static class ColorUtil
    {

        public static Color SetAlpha(this Color c, float a)
        {
            c.a = a;
            return c;
        }

        public static Color SetRed(this Color c, float r)
        {
            c.r = r;
            return c;
        }

        public static Color SetBlue(this Color c, float b)
        {
            c.b = b;
            return c;
        }

        public static Color SetGreen(this Color c, float g)
        {
            c.g = g;
            return c;
        }


    }
}
