using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.Tween.Accessors
{

    public class CustomTweenMemberAccessorFactory
    {
        
        private static ListDictionary<string, CustomAccessorData> _targetToCustomAccessor;

        private static void BuildAccessorDictionary()
        {
            _targetToCustomAccessor = new ListDictionary<string, CustomAccessorData>();
            foreach (var tp in TypeUtil.GetTypesAssignableFrom(typeof(ITweenMemberAccessor)))
            {
                var attribs = tp.GetCustomAttributes(typeof(CustomTweenMemberAccessorAttribute), false).Cast<CustomTweenMemberAccessorAttribute>().ToArray();
                foreach (var attrib in attribs)
                {
                    var data = new CustomAccessorData()
                    {
                        priority = attrib.priority,
                        TargetType = attrib.HandledTargetType,
                        MemberType = attrib.MemberType,
                        AccessorType = tp
                    };
                    _targetToCustomAccessor.Add(attrib.HandledPropName, data);
                }
            }

            foreach (var lst in _targetToCustomAccessor.Lists)
            {
                (lst as List<CustomAccessorData>).Sort((a, b) => b.priority.CompareTo(a.priority)); //sort descending
            }
        }

        public static bool TryGetMemberAccessor(object target, string name, out ITweenMemberAccessor accessor)
        {
            if(target == null)
            {
                accessor = null;
                return false;
            }
            if (_targetToCustomAccessor == null) BuildAccessorDictionary();

            IList<CustomAccessorData> lst;
            if (_targetToCustomAccessor.Lists.TryGetList(name, out lst))
            {
                var tp = target.GetType();
                CustomAccessorData data;
                int cnt = lst.Count;
                for (int i = 0; i < cnt; i++)
                {
                    data = lst[i];
                    if (data.TargetType.IsAssignableFrom(tp))
                    {
                        try
                        {
                            accessor = System.Activator.CreateInstance(data.AccessorType) as ITweenMemberAccessor;
                            return true;
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to create Custom MemberAccessor of type '" + tp.FullName + "'.");
                            break;
                        }
                    }
                }
            }

            accessor = null;
            return false;
        }

        public static bool TryGetMemberAccessorInfo(object target, string name, out CustomAccessorData data)
        {
            if (target == null)
            {
                data = default(CustomAccessorData);
                return false;
            }
            if (_targetToCustomAccessor == null) BuildAccessorDictionary();

            IList<CustomAccessorData> lst;
            if (_targetToCustomAccessor.Lists.TryGetList(name, out lst))
            {
                var tp = target.GetType();
                CustomAccessorData d2;
                int cnt = lst.Count;
                for (int i = 0; i < cnt; i++)
                {
                    d2 = lst[i];
                    if (d2.TargetType.IsAssignableFrom(tp))
                    {
                        try
                        {
                            data = d2;
                            return true;
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to create Custom MemberAccessor of type '" + tp.FullName + "'.");
                            break;
                        }
                    }
                }
            }

            data = default(CustomAccessorData);
            return false;
        }

        public static string[] GetCustomAccessorIds()
        {
            if (_targetToCustomAccessor == null) BuildAccessorDictionary();

            return _targetToCustomAccessor.Keys.ToArray();
        }

        public static string[] GetCustomAccessorIds(System.Type tp, System.Predicate<CustomAccessorData> predicate = null)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (_targetToCustomAccessor == null) BuildAccessorDictionary();

            using (var set = TempCollection.GetSet<string>())
            {
                var e = _targetToCustomAccessor.GetEnumerator();
                while (e.MoveNext())
                {
                    var lst = e.Current.Value;
                    for(int i = 0; i < lst.Count; i++)
                    {
                        if(lst[i].TargetType.IsAssignableFrom(tp) && 
                            (predicate == null || predicate(lst[i])))
                        {
                            set.Add(e.Current.Key);
                            break;
                        }
                    }
                }

                return set.ToArray();
            }
        }



        #region Special Types

        public struct CustomAccessorData
        {
            public int priority;
            public System.Type TargetType;
            public System.Type AccessorType;
            public System.Type MemberType;
        }
        
        #endregion

    }

}
