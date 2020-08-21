using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

using com.spacepuppy.Collections;
using UnityEngine.Rendering;

namespace com.spacepuppy.Utils
{

    public interface IWeightedPair<T>
    {
        T Value { get; }
        float Weight { get; }
    }

    public interface IWeightedPairResolver<TPair, TValue> where TPair : IWeightedPair<TValue>
    {
        Func<TPair, float> WeightPredicate { get; }
        TValue GetValue(TPair pair);
        float GetWeight(TPair pair);
    }

    public class WeightedPairResolver<TPair, TValue> : IWeightedPairResolver<TPair, TValue> where TPair : IWeightedPair<TValue>
    {
        public static readonly WeightedPairResolver<TPair, TValue> Default = new WeightedPairResolver<TPair, TValue>();

        private Func<TPair, float> _weightPred = (p) => p != null ? p.Weight : 0f;
        public Func<TPair, float> WeightPredicate { get { return _weightPred; } }

        public TValue GetValue(TPair pair)
        {
            return pair != null ? pair.Value : default(TValue);
        }

        public float GetWeight(TPair pair)
        {
            return pair != null ? pair.Weight : 0f;
        }
    }

    public class WeightedPairResolver<T> : IWeightedPairResolver<IWeightedPair<T>, T>
    {
        public static readonly WeightedPairResolver<T> Default = new WeightedPairResolver<T>();

        private Func<IWeightedPair<T>, float> _weightPred = (p) => p != null ? p.Weight : 0f;
        public Func<IWeightedPair<T>, float> WeightPredicate { get { return _weightPred; } }

        public T GetValue(IWeightedPair<T> pair)
        {
            return pair != null ? pair.Value : default(T);
        }

        public float GetWeight(IWeightedPair<T> pair)
        {
            return pair != null ? pair.Weight : 0f;
        }
    }

    public static class RandomEnumUtil
    {

        #region Enumerable Methods

        public static IEnumerable<T> Shuffled<T>(this IEnumerable<T> coll, IRandom rng = null)
        {
            if (coll == null) throw new System.ArgumentNullException("coll");
            if (rng == null) rng = RandomUtil.Standard;

            using (var buffer = TempCollection.GetList<T>(coll))
            {
                int j;
                for (int i = 0; i < buffer.Count; i++)
                {
                    j = rng.Next(i, buffer.Count);
                    yield return buffer[j];
                    buffer[j] = buffer[i];
                }
            }
        }

        public static T PickRandom<T>(this IEnumerable<T> lst, IRandom rng = null)
        {
            //return lst.PickRandom(1).FirstOrDefault();
            if (lst is IList<T>)
            {
                if (rng == null) rng = RandomUtil.Standard;
                var a = lst as IList<T>;
                if (a.Count == 0) return default(T);
                return a[rng.Range(a.Count)];
            }
            else
            {
                return lst.PickRandom(1, rng).FirstOrDefault();
            }
        }

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> lst, int count, IRandom rng = null)
        {
            return lst.Shuffled(rng).Take(count);
        }

        public static T PickRandom<T>(this IEnumerable<T> lst, System.Func<T, float> weightPredicate, IRandom rng = null)
        {
            var arr = (lst is IList<T>) ? lst as IList<T> : lst.ToList();
            if (arr.Count == 0) return default(T);

            using (var weights = com.spacepuppy.Collections.TempCollection.GetList<float>(arr.Count))
            {
                int i;
                float w;
                float total = 0f;
                for (i = 0; i < arr.Count; i++)
                {
                    w = weightPredicate(arr[i]);
                    if (float.IsPositiveInfinity(w)) return arr[i];
                    else if (w >= 0f && !float.IsNaN(w)) total += w;
                    weights.Add(w);
                }

                if (rng == null) rng = RandomUtil.Standard;
                float r = rng.Next();
                float s = 0f;

                for (i = 0; i < weights.Count; i++)
                {
                    w = weights[i];
                    if (float.IsNaN(w) || w <= 0f) continue;

                    s += w / total;
                    if (s > r)
                    {
                        return arr[i];
                    }
                }

                //should only get here if last element had a zero weight, and the r was large
                i = arr.Count - 1;
                while (i > 0 && weights[i] <= 0f) i--;
                return arr[i];
            }
        }

        public static T PickRandomWeighted<T>(this IEnumerable<IWeightedPair<T>> lst, IRandom rng = null)
        {
            var resolver = WeightedPairResolver<T>.Default;
            return resolver.GetValue(PickRandom(lst, resolver.WeightPredicate, rng));
        }

        public static TValue PickRandomWeighted<TPair, TValue>(this IEnumerable<TPair> lst, IRandom rng = null, IWeightedPairResolver<TPair, TValue> resolver = null) where TPair : IWeightedPair<TValue>
        {
            if (resolver == null) resolver = WeightedPairResolver<TPair, TValue>.Default;
            return resolver.GetValue(PickRandom(lst, resolver.WeightPredicate, rng));
        }

        #endregion

    }
}
