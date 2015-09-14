using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Utils
{
    public static class ColorUtil
    {

        #region Setters

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

        #endregion

        #region Extraction

        /// <summary>
        /// Luma is returned as a percentage from 0 to 1
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static float ExtractLuma(this Color c)
        {
            return 0.299f * c.r + 0.587f * c.g + 0.114f * c.b;
        }

        /// <summary>
        /// Hue is returned as an angle in degrees around the standard hue colour wheel.
        /// see: http://en.wikipedia.org/wiki/HSL_and_HSV
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static float ExtractHue(this Color c)
        {
            float sum = 0f;
            float mult = 1f;

            float x1, x2, x3;
            if(c.r >= c.g && c.r >= c.b)
            {
                if( c.g >= c.b)
                {
                    x1 = c.r;
                    x2 = c.g;
                    x3 = c.b;
                    sum = 0f;
                    mult = 1f;
                }
                else
                {
                    x1 = c.r;
                    x2 = c.b;
                    x3 = c.g;
                    sum = 6f;
                    mult = -1f;
                }
            }
            else if(c.g > c.r && c.g >= c.b)
            {
                if (c.r >= c.b)
                {
                    x1 = c.g;
                    x2 = c.r;
                    x3 = c.b;
                    sum = 2f;
                    mult = -1f;
                }
                else
                {
                    x1 = c.g;
                    x2 = c.b;
                    x3 = c.r;
                    sum = 2f;
                    mult = 1f;
                }
            }
            else
            {
                if(c.g > c.r)
                {
                    x1 = c.b;
                    x2 = c.g;
                    x3 = c.r;
                    sum = 4f;
                    mult = -1f;
                }
                else
                {
                    x1 = c.b;
                    x2 = c.r;
                    x3 = c.g;
                    sum = 4f;
                    mult = 1f;
                }
            }

            float fract = (x2 - x3) / (x1 - x3);
            return 60f * (sum + mult * fract);
        }

        /// <summary>
        /// Returns the value of an RGB color. This can be used in an HSV representation of a colour.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static float ExtractValue(this Color c)
        {
            return Mathf.Max(c.r, c.g, c.b);
        }

        public static float ExtractSaturation(this Color c)
        {
            var max = Mathf.Max(c.r, c.g, c.b);
            var min = Mathf.Min(c.r, c.g, c.b);
            var delta = max - min;
            if (Mathf.Abs(delta) < 0.0001f) return 0f;
            else
            {
                var light = (max + min) / 2f;
                return delta / (1 - Mathf.Abs(2 * light - 1f));
            }
        }

        #endregion

        #region Lerp

        /// <summary>
        /// Unity's Color.Lerp clamps between 0->1, this allows a true lerp of all ranges.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Color Lerp(Color a, Color b, float t)
        {
            return new Color(a.r + (b.r - a.r) * t, a.g + (b.g - a.g) * t, a.b + (b.b - a.b) * t, a.a + (b.a - a.a) * t);
        }

        /// <summary>
        /// Unity's Color32.Lerp clamps between 0->1, this allows a true lerp of all ranges.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Color32 Lerp(Color32 a, Color32 b, float t)
        {
            return new Color32((byte)MathUtil.Clamp((float)a.r + (float)((int)b.r - (int)a.r) * t, 0, 255), 
                               (byte)MathUtil.Clamp((float)a.g + (float)((int)b.g - (int)a.g) * t, 0, 255), 
                               (byte)MathUtil.Clamp((float)a.b + (float)((int)b.b - (int)a.b) * t, 0, 255), 
                               (byte)MathUtil.Clamp((float)a.a + (float)((int)b.a - (int)a.a) * t, 0, 255));
        }

        #endregion

    }
}
