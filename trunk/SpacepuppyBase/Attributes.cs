using UnityEngine;
using System.Reflection;

namespace com.spacepuppy
{

    #region Notification Attributes

    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class RequireNotificationReceiverAttribute : System.Attribute
    {

        public bool GlobalObserverConsidered;

    }

    //[System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    //public class AutoNotificationHandler : System.Attribute
    //{

    //}

    #endregion

    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class SPRemoteCallAttribute : System.Attribute
    {


        public static object CallMethod(GameObject target, string methodName, object[] args)
        {
            if (target == null) return null;

            var arglen = (args != null) ? args.Length : 0;
            var attribType = typeof(SPRemoteCallAttribute);
            const BindingFlags methodBinding = (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var comp in target.GetComponents<Component>())
            {
                var methods = comp.GetType().GetMethods(methodBinding);

                foreach (var meth in methods)
                {
                    if (meth.Name != methodName) continue;

                    var attrib = System.Attribute.GetCustomAttribute(meth, attribType);
                    if (attrib == null) continue;

                    var paramInfos = meth.GetParameters();
                    if (paramInfos.Length != arglen) continue;

                    bool bFail = false;
                    for (int i = 0; i < arglen; i++)
                    {
                        if (!com.spacepuppy.Utils.ObjUtil.IsType(args[i].GetType(), paramInfos[i].ParameterType))
                        {
                            bFail = true;
                            break;
                        }
                    }

                    if (bFail) continue;

                    return meth.Invoke(comp, args);
                }
            }

            return null;
        }

    }


    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class TypeReferenceRestrictionAttribute : System.Attribute
    {

        public System.Type InheritsFromType;
        public bool allowAbstractClasses = false;
        public bool allowInterfaces = false;
        public System.Type defaultType = null;

        public TypeReferenceRestrictionAttribute(System.Type inheritsFromType)
        {
            this.InheritsFromType = inheritsFromType;
        }

    }

}