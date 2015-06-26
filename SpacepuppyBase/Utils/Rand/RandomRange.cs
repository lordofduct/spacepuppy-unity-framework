using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Utils.Rand
{
    public struct IntRange
    {

        public int Min;
        public int Max;
        public float Weight;

    }

    public struct FloatRange
    {
        public float Min;
        public float Max;
        public float Weight;
    }

    public static class RandomRange
    {

        public static int Range(this IRandom rng, params IntRange[] ranges)
        {
            if (ranges.Length == 0) throw new System.ArgumentException("At least one range must be included.");
            if (ranges.Length == 1) return rng.Range(ranges[0].Max, ranges[0].Min);

            float total = 0f;
            for (int i = 0; i < ranges.Length; i++) total += ranges[i].Weight;

            float r = rng.Next();
            float s = 0f;
            float t;

            int cnt = ranges.Length - 1;
            for (int i = 0; i < cnt; i++)
            {
                s += ranges[i].Weight / total;
                if (s >= r)
                {
                    float s2 = (s + ranges[i + 1].Weight) / total;
                    t = (r - s) / (s2 - s);
                    return (int)((ranges[i].Max - ranges[i].Min) * t) + ranges[i].Min;
                }
            }

            t = (r - s) / (1f - s);
            return (int)((ranges[cnt].Max - ranges[cnt].Min) * t) + ranges[cnt].Min;
        }

        public static float Range(this IRandom rng, params FloatRange[] ranges)
        {
            if (ranges.Length == 0) throw new System.ArgumentException("At least one range must be included.");
            if (ranges.Length == 1) return rng.Range(ranges[0].Max, ranges[0].Min);

            float total = 0f;
            for (int i = 0; i < ranges.Length; i++) total += ranges[i].Weight;

            float r = rng.Next();
            float s = 0f;
            float t;

            int cnt = ranges.Length - 1;
            for (int i = 0; i < cnt; i++)
            {
                s += ranges[i].Weight / total;
                if (s >= r)
                {
                    float s2 = (s + ranges[i + 1].Weight) / total;
                    t = (r - s) / (s2 - s);
                    return (ranges[i].Max - ranges[i].Min) * t + ranges[i].Min;
                }
            }

            t = (r - s) / (1f - s);
            return (ranges[cnt].Max - ranges[cnt].Min) * t + ranges[cnt].Min;
        }






    }

}
