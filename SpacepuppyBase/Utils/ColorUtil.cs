using UnityEngine;

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
            var max = Mathf.Max(c.r, Mathf.Max(c.g, c.b));
            var min = Mathf.Min(c.r, Mathf.Min(c.g, c.b));
            var delta = max - min;
            if (Mathf.Abs(delta) < 0.0001f)
            {
                return 0f;
            }
            else if(c.r >= c.g && c.r >= c.b)
            {
                return 60f * (((c.g - c.b) / delta) % 6f);
            }
            else if(c.g >= c.b)
            {
                return 60f * ((c.b - c.r) / delta + 2f);
            }
            else
            {
                return 60f * ((c.r - c.g) / delta + 4f);
            }
        }

        /// <summary>
        /// Returns the value of an RGB color. This can be used in an HSV representation of a colour.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static float ExtractValue(this Color c)
        {
            return Mathf.Max(c.r, Mathf.Max(c.g, c.b));
        }

        public static float ExtractSaturation(this Color c)
        {
            ////ala HSL formula
            //var max = Mathf.Max(c.r, c.g, c.b);
            //var min = Mathf.Min(c.r, c.g, c.b);
            //var delta = max - min;
            //if (Mathf.Abs(delta) < 0.0001f) return 0f;
            //else
            //    return delta / (1f - Mathf.Abs(max + min - 1f));

            //ala HSV formula
            var max = Mathf.Max(c.r, Mathf.Max(c.g, c.b));
            if (Mathf.Abs(max) < 0.0001f) return 0f;
            var min = Mathf.Min(c.r, Mathf.Min(c.g, c.b));
            return (max - min) / max;
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

        public static Color Lerp(float t, params Color[] colors)
        {
            if (colors == null || colors.Length == 0) return Color.black;
            if (colors.Length == 1) return colors[0];

            int i = Mathf.FloorToInt(colors.Length * t);
            if (i < 0) i = 0;
            if (i >= colors.Length - 1) return colors[colors.Length - 1];
            
            t %= 1f / (float)(colors.Length - 1);
            return Color.Lerp(colors[i], colors[i + 1], t);
        }

        public static Color Slerp(Color a, Color b, float t)
        {
            return (Color)ColorHSV.Slerp((ColorHSV)a, (ColorHSV)b, t);
        }

        public static Color Slerp(float t, params Color[] colors)
        {
            if (colors == null || colors.Length == 0) return Color.black;
            if (colors.Length == 1) return colors[0];

            int i = Mathf.FloorToInt(colors.Length * t);
            if (i < 0) i = 0;
            if (i >= colors.Length - 1) return colors[colors.Length - 1];

            t %= 1f / (float)(colors.Length - 1);
            return (Color)ColorHSV.Slerp((ColorHSV)colors[i], (ColorHSV)colors[i + 1], t);
        }
        
        #endregion
        
    }
}
