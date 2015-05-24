using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

namespace com.spacepuppy.Utils
{
    public static class IEnumerableExtensionMethods
    {

        #region General Methods

        public static bool IsEmpty(this IEnumerable lst)
        {
            return !lst.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// Get how deep into the enumerable the first instance of the object is.
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int Depth(this IEnumerable lst, object obj)
        {
            int i = 0;
            foreach(var o in lst)
            {
                if (object.Equals(o, obj)) return i;
                i++;
            }
            return -1;
        }

        /// <summary>
        /// Get how deep into the enumerable the first instance of the value is.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Depth<T>(this IEnumerable<T> lst, T value)
        {
            int i = 0;
            foreach (var v in lst)
            {
                if (object.Equals(v, value)) return i;
                i++;
            }
            return -1;
        }

        public static IEnumerable<T> Like<T>(this IEnumerable lst)
        {
            foreach (var obj in lst)
            {
                if (obj is T) yield return (T)obj;
            }
        }

        public static IEnumerable Like(this IEnumerable lst, System.Type tp)
        {
            foreach (var obj in lst)
            {
                if (TypeUtil.IsType(obj.GetType(), tp)) yield return obj;
            }
        }

        public static bool Compare<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            var e1 = first.GetEnumerator();
            var e2 = second.GetEnumerator();

            while (true)
            {
                var b1 = e1.MoveNext();
                var b2 = e2.MoveNext();
                if (!b1 && !b2) break; //reached end of list

                if (b1 && b2)
                {
                    if (!object.Equals(e1.Current, e2.Current)) return false;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Each enumerable contains the same elements, not necessarily in the same order, or of the same count. Just the same elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool SimilarTo<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            return first.Except(second).Count() + second.Except(first).Count() == 0;
        }

        public static bool ContainsAny<T>(this IEnumerable<T> lst, params T[] objs)
        {
            if (objs == null) return false;
            return lst.Intersect(objs).Count() > 0;
        }

        public static bool ContainsAny<T>(this IEnumerable<T> lst, IEnumerable<T> objs)
        {
            return lst.Intersect(objs).Count() > 0;
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> lst, T obj)
        {
            foreach (var o in lst)
            {
                yield return o;
            }
            yield return obj;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> lst, T obj)
        {
            yield return obj;
            foreach (var o in lst)
            {
                yield return o;
            }
        }

        public static bool Contains(this IEnumerable lst, object obj)
        {
            foreach (var o in lst)
            {
                if (Object.Equals(o, obj)) return true;
            }
            return false;
        }

        public static void AddRange<T>(this ICollection<T> lst, IEnumerable<T> elements)
        {
            foreach (var e in elements)
            {
                lst.Add(e);
            }
        }

        #endregion

        #region Random Methods

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> lst)
        {
            if (lst == null) throw new System.ArgumentNullException("lst");
            return lst.ShuffleIterator(RandomUtil.Standard);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> lst, IRandom rng)
        {
            if (lst == null) throw new System.ArgumentNullException("lst");
            if (rng == null) throw new System.ArgumentNullException("rng");
            return lst.ShuffleIterator(rng);
        }

        private static IEnumerable<T> ShuffleIterator<T>(this IEnumerable<T> lst, IRandom rng)
        {
            var buffer = lst.ToList();
            int j;
            for (int i = 0; i < buffer.Count; i++)
            {
                j = rng.Next(i, buffer.Count);
                yield return buffer[j];
                buffer[j] = buffer[i];
            }
        }


        public static T PickRandom<T>(this IEnumerable<T> lst)
        {
            //return lst.PickRandom(1).FirstOrDefault();
            if (lst is IList<T>)
            {
                var a = lst as IList<T>;
                if (a.Count == 0) return default(T);
                return a[Random.Range(0, a.Count)];
            }
            else
            {
                return lst.PickRandom(1).FirstOrDefault();
            }
        }

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> lst, int count)
        {
            return lst.Shuffle().Take(count);
        }

        public static T PickRandom<T>(this IEnumerable<T> lst, System.Func<T, float> weightPredicate)
        {
            var arr = (lst is IList<T>) ? lst as IList<T> : lst.ToList();
            var weights = (from o in lst select weightPredicate(o)).ToArray();
            var total = weights.Sum();
            float r = Random.value;
            float s = 0f;

            for (int i = 0; i < weights.Length; i++)
            {
                s += weights[i] / total;
                if (s >= r)
                {
                    return arr[i];
                }
            }

            //should never get here
            return lst.Last();
        }

        public static object PickRandom(this System.Array lst)
        {
            if (lst.Length == 0) return null;
            //return lst.GetValue(RandomUtil.Range(lst.Length, 0));
            return lst.GetValue(Random.Range(0, lst.Length));
        }

        public static T PickRandom<T>(this T[] lst)
        {
            if (lst.Length == 0) return default(T);
            //return lst[RandomUtil.Range(lst.Length, 0)];
            return lst[Random.Range(0, lst.Length)];
        }

        #endregion

        #region Array Methods

        public static int IndexOf(this System.Array lst, object obj)
        {
            return System.Array.IndexOf(lst, obj);
        }

        public static int IndexOf<T>(this T[] lst, T obj)
        {
            return System.Array.IndexOf(lst, obj);
        }

        public static bool InBounds(this System.Array arr, int index)
        {
            return index >= 0 && index <= arr.Length - 1;
        }

        #endregion

    }
}

