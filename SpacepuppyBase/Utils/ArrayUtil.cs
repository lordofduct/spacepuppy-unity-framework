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

        public static T LastOrDefault<T>(this IEnumerable<T> e, T defaultvalue)
        {
            var lst = e as IList<T>;
            if(lst != null)
            {
                int cnt = lst.Count;
                return cnt > 0 ? lst[cnt - 1] : defaultvalue;
            }
            else
            {
                var en = e.GetEnumerator();
                T result = defaultvalue;
                while(en.MoveNext())
                {
                    result = en.Current;
                }
                return result;
            }
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> e, T defaultvalue)
        {
            var lst = e as IList<T>;
            if (lst != null)
            {
                int cnt = lst.Count;
                return cnt > 0 ? lst[0] : defaultvalue;
            }
            else
            {
                var en = e.GetEnumerator();
                T result = defaultvalue;
                if (en.MoveNext())
                {
                    result = en.Current;
                }
                return result;
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
            var e = new LightEnumerator<T>(lst);
            while (e.MoveNext())
            {
                yield return e.Current;
            }
            yield return obj;
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> first, IEnumerable<T> next)
        {
            var e = new LightEnumerator<T>(first);
            while (e.MoveNext())
            {
                yield return e.Current;
            }
            e = new LightEnumerator<T>(next);
            while (e.MoveNext())
            {
                yield return e.Current;
            }
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> lst, T obj)
        {
            yield return obj;
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

        #region Random Methods

        public static void Shuffle<T>(T[] arr, IRandom rng = null)
        {
            if (arr == null) throw new System.ArgumentNullException("arr");
            if (rng == null) rng = RandomUtil.Standard;

            int j;
            T temp;
            for (int i = 0; i < arr.Length - 1; i++)
            {
                j = rng.Next(i, arr.Length);
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
            for (int i = 0; i < arr.Length - 1; i++)
            {
                int j = rng.Next(i, arr.Length);
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
            for (int i = 0; i < cnt - 1; i++)
            {
                j = rng.Next(i, cnt);
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
            for (int i = 0; i < cnt - 1; i++)
            {
                j = rng.Next(i, cnt);
                temp = lst[j];
                lst[j] = lst[i];
                lst[i] = temp;
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

