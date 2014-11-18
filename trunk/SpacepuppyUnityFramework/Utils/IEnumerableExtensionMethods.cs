using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

namespace com.spacepuppy.Utils
{
    public static class IEnumerableExtensionMethods
    {

        //public static System.Array ToArray(this IEnumerable lst)
        //{
        //    var result = new List<object>();
        //    foreach (var obj in lst) result.Add(obj);
        //    return result.ToArray();
        //}

        //public static T[] ToArray<T>(this IEnumerable<T> lst)
        //{
        //    var result = new List<T>();
        //    foreach (var obj in lst) result.Add(obj);
        //    return result.ToArray();
        //}

        //public static int Count(this IEnumerable lst)
        //{
        //    int cnt = 0;
        //    var enr = lst.GetEnumerator();
        //    while(enr.MoveNext())
        //    {
        //        cnt++;	
        //    }

        //    return cnt;
        //}

        //public static int Count<T>(this IEnumerable<T> lst)
        //{
        //    int cnt = 0;

        //    var enr = lst.GetEnumerator();
        //    while(enr.MoveNext())
        //    {
        //        cnt++;
        //    }

        //    return cnt;
        //}

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
                if (ObjUtil.IsType(obj.GetType(), tp)) yield return obj;
            }
        }

        public static T PickRandom<T>(this IEnumerable<T> lst)
        {
            return lst.PickRandom(1).FirstOrDefault();
        }

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> lst, int count)
        {
            return lst.Shuffle().Take(count);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> lst)
        {
            //return lst.OrderBy(x => System.Guid.NewGuid());

            //var r = new System.Random();
            //return lst.OrderBy(x => r.Next());

            return lst.OrderBy(x => Random.value);
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

        public static bool SimilarTo<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            return first.Except(second).Count() == 0;
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

        public static object GetAny(this System.Array lst)
        {
            if (lst.Length == 0) return null;
            //return lst.GetValue(RandomUtil.Range(lst.Length, 0));
            return lst.GetValue(Random.Range(0, lst.Length));
        }

        public static T GetAny<T>(this T[] lst)
        {
            if (lst.Length == 0) return default(T);
            //return lst[RandomUtil.Range(lst.Length, 0)];
            return lst[Random.Range(0, lst.Length)];
        }

        #endregion

    }
}

