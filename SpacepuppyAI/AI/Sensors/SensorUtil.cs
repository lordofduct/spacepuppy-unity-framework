using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.Sensors
{

    public static class SensorUtil
    {

        public static System.Comparison<IAspect> SortByPrecedenceDescending
        {
            get
            {
                return PrecedenceDelegateContainer<IAspect>.PrecedenceDescending;
            }
        }

        public static System.Comparison<IAspect> SortByPrecedenceAscending
        {
            get
            {
                return PrecedenceDelegateContainer<IAspect>.PrecedenceAscending;
            }
        }

        public static System.Comparison<T> GetSortByPrecedenceDescending<T>() where T : class, IAspect
        {
            return PrecedenceDelegateContainer<T>.PrecedenceDescending;
        }

        public static System.Comparison<T> GetSortByPrecedenceAscending<T>() where T : class, IAspect
        {
            return PrecedenceDelegateContainer<T>.PrecedenceAscending;
        }



        public static bool AnyVisible(this Sensor sensor, IEnumerable<IAspect> aspects)
        {
            if (sensor == null) throw new System.ArgumentNullException("sensor");
            if (aspects == null) return false;

            var e = new LightEnumerator<IAspect>(aspects);
            while (e.MoveNext())
            {
                if (e.Current != null && e.Current.IsActive)
                {
                    if (sensor.Visible(e.Current)) return true;
                }
            }

            return false;
        }

        public static IAspect[] SenseAll(this Sensor sensor, System.Func<IAspect, bool> predicate, System.Comparison<IAspect> sortby)
        {
            if (sensor == null) throw new System.ArgumentNullException("sensor");

            using (var lst = TempCollection.GetList<IAspect>())
            {
                if (sensor.SenseAll(lst, predicate) > 0)
                {
                    if (sortby != null) lst.Sort(sortby);
                    return lst.ToArray();
                }
                else
                {
                    return ArrayUtil.Empty<IAspect>();
                }
            }
        }

        public static T[] SenseAll<T>(this Sensor sensor, System.Func<T, bool> predicate, System.Comparison<T> sortby) where T : class, IAspect
        {
            if (sensor == null) throw new System.ArgumentNullException("sensor");

            using (var lst = TempCollection.GetList<T>())
            {
                if (sensor.SenseAll(lst, predicate) > 0)
                {
                    if (sortby != null) lst.Sort(sortby);
                    return lst.ToArray();
                }
                else
                {
                    return ArrayUtil.Empty<T>();
                }
            }
        }

        public static IAspect Sense(this Sensor sensor, System.Func<IAspect, bool> predicate = null, System.Comparison<IAspect> precedence = null)
        {
            if (sensor == null) throw new System.ArgumentNullException("sensor");

            if (precedence == null)
            {
                return sensor.Sense(predicate);
            }
            else
            {
                using (var lst = TempCollection.GetList<IAspect>())
                {
                    if (sensor.SenseAll(lst, predicate) > 0)
                    {
                        lst.Sort(precedence);
                        return lst[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public static T Sense<T>(this Sensor sensor, System.Func<T, bool> predicate = null, System.Comparison<T> precedence = null) where T : class, IAspect
        {
            if (sensor == null) throw new System.ArgumentNullException("sensor");

            using (var lst = TempCollection.GetList<T>())
            {
                if (sensor.SenseAll<T>(lst, predicate) > 0)
                {
                    if (precedence != null) lst.Sort(precedence);
                    return lst[0];
                }
                else
                {
                    return null;
                }
            }
        }

        public static IAspect SenseHighestPrecedence(this Sensor sensor, System.Func<IAspect, bool> predicate = null)
        {
            if (sensor == null) throw new System.ArgumentNullException("sensor");

            using (var lst = TempCollection.GetList<IAspect>())
            {
                if (sensor.SenseAll(lst, predicate) > 0)
                {
                    lst.Sort(PrecedenceDelegateContainer<IAspect>.PrecedenceDescending);
                    return lst[0];
                }
                else
                {
                    return null;
                }
            }
        }

        public static T SenseHighestPrecedence<T>(this Sensor sensor, System.Func<T, bool> predicate = null) where T : class, IAspect
        {
            if (sensor == null) throw new System.ArgumentNullException("sensor");

            using (var lst = TempCollection.GetList<T>())
            {
                if (sensor.SenseAll(lst, predicate) > 0)
                {
                    lst.Sort(PrecedenceDelegateContainer<T>.PrecedenceDescending);
                    return lst[0];
                }
                else
                {
                    return null;
                }
            }
        }





        private static class PrecedenceDelegateContainer<T> where T : class, IAspect
        {
            private static System.Comparison<T> _precedenceDesc;
            public static System.Comparison<T> PrecedenceDescending
            {
                get
                {
                    if (_precedenceDesc == null) _precedenceDesc = (a, b) =>
                    {
                        if (a == null) return b == null ? 0 : 1;
                        if (b == null) return -1;
                        return -a.Precedence.CompareTo(b.Precedence);
                    };

                    return _precedenceDesc;
                }
            }

            private static System.Comparison<T> _precedenceAsc;
            public static System.Comparison<T> PrecedenceAscending
            {
                get
                {
                    if (_precedenceAsc == null) _precedenceAsc = (a, b) =>
                    {
                        if (a == null) return b == null ? 0 : -1;
                        if (b == null) return 1;
                        return a.Precedence.CompareTo(b.Precedence);
                    };

                    return _precedenceAsc;
                }
            }

        }

    }

}
