using UnityEngine;
using System.Reflection;

namespace com.spacepuppy
{

    public abstract class SPPropertyAttribute : PropertyAttribute
    {
        private bool _handlesEntireArray;

        public SPPropertyAttribute()
        {

        }

        public SPPropertyAttribute(bool handlesEntireArray)
        {
            _handlesEntireArray = handlesEntireArray;
        }

        public bool HandlesEntireArray { get { return _handlesEntireArray; } }
    }


    #region Component Attributes

    public abstract class ComponentHeaderAttribute : PropertyAttribute
    {

    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequireLikeComponentAttribute : ComponentHeaderAttribute
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
    public class RequireComponentInEntityAttribute : ComponentHeaderAttribute
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
    public class UniqueToEntityAttribute : ComponentHeaderAttribute
    {

        public bool MustBeAttachedToRoot;

    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RequireLayerAttribute : ComponentHeaderAttribute
    {
        public int Layer;

        public RequireLayerAttribute(int layer)
        {
            this.Layer = layer;
        }
    }

    #endregion

    #region Property Drawer Attributes

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class EulerRotationInspectorAttribute : SPPropertyAttribute
    {

        public bool UseRadians = false;

        public EulerRotationInspectorAttribute()
        {

        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class EnumFlagsAttribute : SPPropertyAttribute
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
    public class TagSelectorAttribute : SPPropertyAttribute
    {
        public bool AllowUntagged;
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class GenericMaskAttribute : SPPropertyAttribute
    {

        private string[] _maskNames;

        public GenericMaskAttribute(params string[] names)
        {
            _maskNames = names;
        }

        public string[] MaskNames { get { return _maskNames; } }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class ComponentTypeRestrictionAttribute : SPPropertyAttribute
    {
        public System.Type InheritsFromType;
        public TypeDropDownListingStyle MenuListingStyle = TypeDropDownListingStyle.ComponentMenu;

        public ComponentTypeRestrictionAttribute(System.Type inheritsFromType)
        {
            this.InheritsFromType = inheritsFromType;
        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class SelectableComponentAttribute : SPPropertyAttribute
    {
        public bool AllowSceneObjects = true;
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class ReorderableArrayAttribute : SPPropertyAttribute
    {

        public string ElementLabelFormatString = null;

        public ReorderableArrayAttribute() : base(true)
        {

        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple=false)]
    public class OneOrManyAttribute : SPPropertyAttribute
    {
        public OneOrManyAttribute() : base(true)
        {

        }
    }

    #endregion

    #region ModifierDrawer Attributes

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public abstract class PropertyModifierAttribute : SPPropertyAttribute
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
        public bool UseEntity = false;
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class DisableOnPlayAttribute : PropertyModifierAttribute
    {

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class OnChangedInEditorAttribute : PropertyModifierAttribute
    {

        public readonly string MethodName;
        public bool OnlyAtRuntime;

        public OnChangedInEditorAttribute(string methodName)
        {
            this.MethodName = methodName;
        }

    }

    #endregion

    #region Decorator Attributes

    public class InsertButtonAttribute : PropertyAttribute
    {

        public string Label;
        public string OnClick;
        public bool PrecedeProperty;

        public InsertButtonAttribute(string label, string onClick)
        {
            this.Label = label;
            this.OnClick = onClick;
        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Class, AllowMultiple = false)]
    public class InfoboxAttribute : ComponentHeaderAttribute
    {
        public string Message;
        public InfoBoxMessageType MessageType = InfoBoxMessageType.Info;

        public InfoboxAttribute(string msg)
        {
            this.Message = msg;
        }

    }

    #endregion

}