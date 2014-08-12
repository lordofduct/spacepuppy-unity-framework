using UnityEngine;
using System.Reflection;

namespace com.spacepuppy
{

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequireLikeComponentAttribute : System.Attribute
    {

        private System.Type[] _types;

        public RequireLikeComponentAttribute(params System.Type[] tps)
        {
            _types = tps;
        }

        public System.Type[] Types
        {
            get { return _types; }
        }

    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequireComponentInEntityAttribute : System.Attribute
    {

        private System.Type[] _types;

        public RequireComponentInEntityAttribute(params System.Type[] tps)
        {
            _types = tps;
        }

        public System.Type[] Types
        {
            get { return _types; }
        }

    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UniqueToEntityAttribute : System.Attribute
    {

        public bool MustBeAttachedToRoot;

    }

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



    #region Property Drawer Attributes

    public class LabelAttribute : PropertyAttribute
    {

        public string Label;
        public string ToolTip;

        public LabelAttribute()
        {

        }

        public LabelAttribute(string lbl, string msg)
        {
            this.Label = lbl;
            this.ToolTip = msg;
        }

    }

    public class EulerRotationInspectorAttribute : PropertyAttribute
    {

        public bool UseRadians = false;

        public EulerRotationInspectorAttribute()
        {

        }

    }

    public class FieldInspectorOverrideAttribute : PropertyAttribute
    {

        private string _propertyName;
        private string _getMethodName;
        private string _setMethodName;
        private bool _isProperty;

        public FieldInspectorOverrideAttribute(string propertyName)
        {
            _isProperty = true;
            _propertyName = propertyName;
        }

        public FieldInspectorOverrideAttribute(string getMethodName, string setMethodName)
        {
            _isProperty = false;
            _getMethodName = getMethodName;
            _setMethodName = setMethodName;
        }

        public bool IsProeprty { get { return _isProperty; } }

        public string PropertyName { get { return _propertyName; } }
        public string GetMethodName { get { return _getMethodName; } }

        public string SetMethodName { get { return _setMethodName; } }

    }

    #endregion

}