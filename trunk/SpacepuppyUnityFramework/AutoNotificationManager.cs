using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{
    internal class AutoNotificationManager
    {

        private MonoBehaviour _behaviour;
        private Dictionary<System.Type, System.Delegate> _handlers;

        public AutoNotificationManager(MonoBehaviour behaviour)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            _behaviour = behaviour;
            _handlers = new Dictionary<System.Type, System.Delegate>();
            this.Init();
        }

        private void Init()
        {
            var baseDelType = typeof(System.Action<>);
            foreach (var m in GetMethodInfos(_behaviour.GetType()))
            {
                var notifType = m.GetParameters()[0].ParameterType;
                var delType = baseDelType.MakeGenericType(notifType);
                var deleg = System.Delegate.CreateDelegate(delType, _behaviour, m, false);
                if (deleg == null) continue;

                if (!_handlers.ContainsKey(notifType))
                {
                    _handlers.Add(notifType, deleg);
                }
                else
                {
                    _handlers[notifType] = System.Delegate.Combine(_handlers[notifType], deleg);
                }
            }
        }

        public void OnNotification(Notification n)
        {
            var notifType = n.GetType();
            var baseNotifType = typeof(Notification);
            while (baseNotifType.IsAssignableFrom(notifType))
            {
                if (_handlers.ContainsKey(notifType))
                {
                    _handlers[notifType].DynamicInvoke(n);
                    n.AutoReceived = true;
                }
                notifType = notifType.BaseType;
            }
        }




        private static Dictionary<System.Type, System.Reflection.MethodInfo[]> _quickLookupTable;
        private static void LoadLookupTable()
        {
            _quickLookupTable = new Dictionary<System.Type, System.Reflection.MethodInfo[]>();

            var behTp = typeof(MonoBehaviour);
            var attribType = typeof(AutoNotificationHandler);
            var baseNotifType = typeof(Notification);
            var voidType = typeof(void);
            var types = from ass in System.AppDomain.CurrentDomain.GetAssemblies()
                        from tp in ass.GetTypes()
                        where behTp.IsAssignableFrom(tp) && System.Attribute.IsDefined(tp, attribType, true)
                        select tp;
            foreach (var tp in types)
            {
                var methods = from m in tp.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
                              where m.ReturnType == voidType &&
                              System.Attribute.IsDefined(m, attribType, true)
                              let p = m.GetParameters()
                              where p.Length == 1 && baseNotifType.IsAssignableFrom(p[0].ParameterType)
                              select m;
                if (methods.Count() > 0)
                {
                    _quickLookupTable.Add(tp, methods.ToArray());
                }
            }
        }

        private static System.Reflection.MethodInfo[] GetMethodInfos(System.Type tp)
        {
            if (_quickLookupTable == null) LoadLookupTable();

            if (_quickLookupTable.ContainsKey(tp)) return _quickLookupTable[tp];
            else return new System.Reflection.MethodInfo[0];
        }

        public static bool TypeHasAutoHandlers(System.Type tp)
        {
            if (_quickLookupTable == null) LoadLookupTable();

            return _quickLookupTable.ContainsKey(tp);
        }

    }
}
