using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Utils
{

    public static class ArrayUtil
    {

        #region General Methods

        public static bool IsEmpty(this IEnumerable lst)
        {
            if(lst is IList)
            {
                return (lst as IList).Count == 0;
            }
            else
            {
                return !lst.GetEnumerator().MoveNext();
            }
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
            //foreach (var o in lst)
            //{
            //    yield return o;
            //}
            var e = new LightEnumerator<T>(lst);
            while (e.MoveNext())
            {
                yield return e.Current;
            }
            yield return obj;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> lst, T obj)
        {
            yield return obj;
            //foreach (var o in lst)
            //{
            //    yield return o;
            //}
            var e = new LightEnumerator<T>(lst);
            while(e.MoveNext())
            {
                yield return e.Current;
            }
        }

        public static bool Contains(this IEnumerable lst, object obj)
        {
            //foreach (var o in lst)
            //{
            //    if (Object.Equals(o, obj)) return true;
            //}
            var e = new LightEnumerator(lst);
            while(e.MoveNext())
            {
                if (Object.Equals(e.Current, obj)) return true;
            }
            return false;
        }

        public static bool Contains<T>(this T[,] arr, T value)
        {
            for(int i = 0; i < arr.GetLength(0); i++)
            {
                for(int j = 0; j < arr.GetLength(1); j++)
                {
                    if (EqualityComparer<T>.Default.Equals(arr[i, j], value)) return true;
                }
            }

            return false;
        }

        public static void AddRange<T>(this ICollection<T> lst, IEnumerable<T> elements)
        {
            //foreach (var e in elements)
            //{
            //    lst.Add(e);
            //}
            var e = new LightEnumerator<T>(elements);
            while(e.MoveNext())
            {
                lst.Add(e.Current);
            }
        }

        public static T GetValueAfterOrDefault<T>(this IEnumerable<T> lst, T element, bool loop = false)
        {
            if (lst is IList<T>)
            {
                var arr = lst as IList<T>;
                if (arr.Count == 0) return default(T);

                int i = arr.IndexOf(element) + 1;
                if (loop) i = i % arr.Count;
                else if (i >= arr.Count) return default(T);
                return arr[i];
            }
            else
            {
                var e = lst.GetEnumerator();
                if (!e.MoveNext()) return default(T);
                var first = e.Current;
                if (object.Equals(e.Current, element))
                {
                    if (e.MoveNext())
                    {
                        return e.Current;
                    }
                    else if (loop)
                    {
                        return first;
                    }
                    else
                    {
                        return default(T);
                    }
                }

                while (e.MoveNext())
                {
                    if (object.Equals(e.Current, element))
                    {
                        if (e.MoveNext())
                        {
                            return e.Current;
                        }
                        else if (loop)
                        {
                            return first;
                        }
                        else
                        {
                            return default(T);
                        }
                    }
                }
                return default(T);
            }
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> lst, T element)
        {
            if (lst == null) throw new System.ArgumentNullException("lst");
            foreach(var e in lst)
            {
                if (!object.Equals(e, element)) yield return e;
            }
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> lst, T element, IEqualityComparer<T> comparer)
        {
            if (lst == null) throw new System.ArgumentNullException("lst");
            if (comparer == null) throw new System.ArgumentNullException("comparer");
            foreach (var e in lst)
            {
                if (!comparer.Equals(e, element)) yield return e;
            }
        }

        #endregion

        #region Random Methods

        public static void Shuffle<T>(T[] arr, IRandom rng = null)
        {
            if (arr == null) throw new System.ArgumentNullException("arr");
            if (rng == null) rng = RandomUtil.Standard;
            
            int j;
            T temp;
            for (int i = 0; i < arr.Length; i++)
            {
                j = rng.Next(arr.Length);
                temp = arr[j];
                arr[j] = arr[i];
                arr[i] = temp;
            }
        }

        public static void Shuffle<T>(T[,] arr, IRandom rng = null)
        {
            if (arr == null) throw new System.ArgumentNullException("arr");
            if (rng == null) rng = RandomUtil.Standard;
            
            int width = arr.GetLength(0);
            for (int i = 0; i < arr.Length; i++)
            {
                int j = rng.Next(arr.Length);
                int ix = i % width;
                int iy = (int)(i / width);
                int jx = j % width;
                int jy = (int)(j / width);
                T temp = arr[jx, jy];
                arr[jx, jy] = arr[ix, iy];
                arr[ix, iy] = temp;
            }
        }

        public static void Shuffle(IList lst, IRandom rng = null)
        {
            if (lst == null) throw new System.ArgumentNullException("lst");
            if (rng == null) rng = RandomUtil.Standard;

            int j;
            object temp;
            int cnt = lst.Count;
            for (int i = 0; i < cnt; i++)
            {
                j = rng.Next(cnt);
                temp = lst[j];
                lst[j] = lst[i];
                lst[i] = temp;
            }
        }

        public static void Shuffle<T>(IList<T> lst, IRandom rng = null)
        {
            if (lst == null) throw new System.ArgumentNullException("lst");
            if (rng == null) rng = RandomUtil.Standard;

            int j;
            T temp;
            int cnt = lst.Count;
            for (int i = 0; i < cnt; i++)
            {
                j = rng.Next(cnt);
                temp = lst[j];
                lst[j] = lst[i];
                lst[i] = temp;
            }
        }

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
            //var arr = (lst is IList<T>) ? lst as IList<T> : lst.ToList();
            //if (arr.Count == 0) return default(T);

            //var weights = (from o in lst select weightPredicate(o)).ToArray();
            //var total = weights.Sum();
            //if (total <= 0) return arr[0];

            //if (rng == null) rng = RandomUtil.Standard;
            //float r = rng.Next();
            //float s = 0f;

            //int i;
            //for (i = 0; i < weights.Length; i++)
            //{
            //    s += weights[i] / total;
            //    if (s >= r)
            //    {
            //        return arr[i];
            //    }
            //}

            ////should only get here if last element had a zero weight, and the r was large
            //i = arr.Count - 1;
            //while (i > 0 || weights[i] <= 0f) i--;
            //return arr[i];


            var arr = (lst is IList<T>) ? lst as IList<T> : lst.ToList();
            if (arr.Count == 0) return default(T);

            using (var weights = com.spacepuppy.Collections.TempCollection.GetList<float>())
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
                    if (s >= r)
                    {
                        return arr[i];
                    }
                }

                //should only get here if last element had a zero weight, and the r was large
                i = arr.Count - 1;
                while (i > 0 || weights[i] <= 0f) i--;
                return arr[i];
            }
        }

        #endregion

        #region Array Methods

        public static T[] Empty<T>()
        {
            return TempArray<T>.Empty;
        }

        public static T[] Temp<T>(T value)
        {
            return TempArray<T>.Temp(value);
        }

        public static T[] Temp<T>(T value1, T value2)
        {
            return TempArray<T>.Temp(value1, value2);
        }

        public static T[] Temp<T>(T value1, T value2, T value3)
        {
            return TempArray<T>.Temp(value1, value2, value3);
        }

        public static T[] Temp<T>(T value1, T value2, T value3, T value4)
        {
            return TempArray<T>.Temp(value1, value2, value3, value4);
        }

        public static void ReleaseTemp<T>(T[] arr)
        {
            TempArray<T>.Release(arr);
        }






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

        public static void Clear(this System.Array arr)
        {
            if (arr == null) return;
            System.Array.Clear(arr, 0, arr.Length);
        }

        public static void Copy<T>(IEnumerable<T> source, System.Array destination, int index)
        {
            if (source is System.Collections.ICollection)
                (source as System.Collections.ICollection).CopyTo(destination, index);
            else
            {
                int i = 0;
                foreach(var el in source)
                {
                    destination.SetValue(el, i + index);
                    i++;
                }
            }
        }
        

        #endregion

        #region HashSet Methods

        public static T Pop<T>(this HashSet<T> set)
        {
            if (set == null) throw new System.ArgumentNullException("set");

            var e = set.GetEnumerator();
            if(e.MoveNext())
            {
                set.Remove(e.Current);
                return e.Current;
            }

            throw new System.ArgumentException("HashSet must not be empty.");
        }

        #endregion

        #region Special Types

        private class TempArray<T>
        {

            private static object _lock = new object();
            private static volatile T[] _empty;
            private static volatile T[] _oneArray;
            private static volatile T[] _twoArray;
            private static volatile T[] _threeArray;
            private static volatile T[] _fourArray;

            public static T[] Empty
            {
                get
                {
                    if (_empty == null) _empty = new T[0];
                    return _empty;
                }
            }

            public static T[] Temp(T value)
            {
                T[] arr;

                lock (_lock)
                {
                    if(_oneArray != null)
                    {
                        arr = _oneArray;
                        _oneArray = null;
                    }
                    else
                    {
                        arr = new T[1];
                    }
                }

                arr[0] = value;
                return arr;
            }

            public static T[] Temp(T value1, T value2)
            {
                T[] arr;

                lock (_lock)
                {
                    if (_oneArray != null)
                    {
                        arr = _twoArray;
                        _twoArray = null;
                    }
                    else
                    {
                        arr = new T[2];
                    }
                }

                arr[0] = value1;
                arr[1] = value2;
                return arr;
            }

            public static T[] Temp(T value1, T value2, T value3)
            {
                T[] arr;

                lock (_lock)
                {
                    if (_oneArray != null)
                    {
                        arr = _threeArray;
                        _threeArray = null;
                    }
                    else
                    {
                        arr = new T[3];
                    }
                }

                arr[0] = value1;
                arr[1] = value2;
                arr[2] = value3;
                return arr;
            }

            public static T[] Temp(T value1, T value2, T value3, T value4)
            {
                T[] arr;

                lock (_lock)
                {
                    if (_oneArray != null)
                    {
                        arr = _fourArray;
                        _fourArray = null;
                    }
                    else
                    {
                        arr = new T[4];
                    }
                }

                arr[0] = value1;
                arr[1] = value2;
                arr[2] = value3;
                arr[3] = value4;
                return arr;
            }


            public static void Release(T[] arr)
            {
                if (arr == null) return;
                System.Array.Clear(arr, 0, arr.Length);

                lock(_lock)
                {
                    switch (arr.Length)
                    {
                        case 1:
                            _oneArray = arr;
                            break;
                        case 2:
                            _twoArray = arr;
                            break;
                        case 3:
                            _threeArray = arr;
                            break;
                        case 4:
                            _fourArray = arr;
                            break;
                    }
                }
            }
        }

        #endregion

    }

}

