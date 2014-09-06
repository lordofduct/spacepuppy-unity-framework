using UnityEngine;
using System.Reflection;

namespace com.spacepuppy
{

    [System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AutoNotificationHandler : System.Attribute
    {

    }

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



    #region Property Drawer Attributes

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class EulerRotationInspectorAttribute : PropertyAttribute
    {

        public bool UseRadians = false;

        public EulerRotationInspectorAttribute()
        {

        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class EnumFlagsAttribute : PropertyAttribute
    {

        public System.Type EnumType;

        public EnumFlagsAttribute()
        {

        }

        public EnumFlagsAttribute(System.Type enumType)
        {
            this.EnumType = enumType;
        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class TagSelectorAttribute : PropertyAttribute
    {
        public bool AllowUntagged;
    }

    public class ComponentTypeRestrictionAttribute : PropertyAttribute
    {
        public System.Type InheritsFromType;
        public TypeDropDownListingStyle MenuListingStyle = TypeDropDownListingStyle.ComponentMenu;

        public ComponentTypeRestrictionAttribute(System.Type inheritsFromType)
        {
            this.InheritsFromType = inheritsFromType;
        }

    }

    #endregion

    #region ModifierDrawer Attributes

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public abstract class PropertyModifierAttribute : PropertyAttribute
    {
        public bool IncludeChidrenOnDraw;
    }

    /// <summary>
    /// Process a series of PropertyModifierAttributes before drawing the inspector for this property. The order of processing 
    /// is determined by the 'order' value of the attribute, with the highest order acting as the visual drawn.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class ModifierChainAttribute : PropertyAttribute
    {

        public ModifierChainAttribute()
        {
            this.order = int.MinValue;
        }

    }

    /// <summary>
    /// While in the editor, if the value is ever null, an attempt is made to get the value from self. You will still 
    /// have to initialize the value on Awake if null. The cost of doing it automatically is too high for all components 
    /// to test themselves for this attribute.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class DefaultFromSelfAttribute : PropertyModifierAttribute
    {
        public bool FindInEntity = false;
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class DisableOnPlayAttribute : PropertyModifierAttribute
    {

    }

    #endregion

    #region Decorator Attributes

    public class InfoboxAttribute : PropertyAttribute
    {
        public string Message;

        public InfoboxAttribute(string msg)
        {
            this.Message = msg;
        }

    }

    #endregion

}