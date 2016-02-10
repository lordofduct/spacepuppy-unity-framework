using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    public abstract class SPPropertyAttribute : PropertyAttribute
    {

        public SPPropertyAttribute()
        {

        }

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

    /// <summary>
    /// Defines a script as requiring either a Collider or Rigidbody attached to it.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RequireColliderAttribute : ComponentHeaderAttribute
    {

    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ForceRootTagAttribute : ComponentHeaderAttribute
    {

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

    public class EnumPopupExcludingAttribute : SPPropertyAttribute
    {

        public readonly int[] excludedValues;

        public EnumPopupExcludingAttribute(params int[] excluded)
        {
            excludedValues = excluded;
        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class TagSelectorAttribute : SPPropertyAttribute
    {
        public bool AllowUntagged;
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class TimeUnitsSelectorAttribute : SPPropertyAttribute
    {
        public string DefaultUnits;

        public TimeUnitsSelectorAttribute()
        {
        }

        public TimeUnitsSelectorAttribute(string defaultUnits)
        {
            DefaultUnits = defaultUnits;
        }
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

    /// <summary>
    /// TODO - consider deprecating since 'SelectableComponentAttribute' mostly does the same thing as this... 
    /// only that this allows 'hiding the dropdown' and selecting the first component of the restriction type found on the source.
    /// This had a purpose at one point when there was different 'drop down' modes, but we devised a much better way of choosing the component.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class ComponentTypeRestrictionAttribute : SPPropertyAttribute
    {
        public System.Type InheritsFromType;
        public bool HideTypeDropDown;

        public ComponentTypeRestrictionAttribute(System.Type inheritsFromType)
        {
            this.InheritsFromType = inheritsFromType;
        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class SelectableComponentAttribute : SPPropertyAttribute
    {
        public System.Type InheritsFromType;
        public bool AllowSceneObjects = true;
        public bool ForceOnlySelf = false;
        public bool SearchChildren = false;

        public SelectableComponentAttribute()
        {

        }

        public SelectableComponentAttribute(System.Type inheritsFromType)
        {
            this.InheritsFromType = inheritsFromType;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class SelectableObjectAttribute : SPPropertyAttribute
    {
        public bool AllowSceneObjects = true;

        public SelectableObjectAttribute()
        {

        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class ReorderableArrayAttribute : SPPropertyAttribute
    {

        public string ElementLabelFormatString = null;
        public bool DisallowFoldout;
        public bool RemoveBackgroundWhenCollapsed;
        public bool Draggable = true;
        public float ElementPadding = 0f;

        public ReorderableArrayAttribute()
        {

        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple=false)]
    public class OneOrManyAttribute : SPPropertyAttribute
    {
        public OneOrManyAttribute()
        {

        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple=false)]
    public class UnitVectorAttribute : SPPropertyAttribute
    {
        
        public UnitVectorAttribute() : base()
        {

        }
        
    }

    /// <summary>
    /// Restrict a value to be no lesser than min.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class MinRangeAttribute : SPPropertyAttribute
    {
        public float Min;

        public MinRangeAttribute(float min)
        {
            this.Min = min;
        }
    }

    /// <summary>
    /// Restrict a value to be no greater than max.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class MaxRangeAttribute : SPPropertyAttribute
    {
        public float Max;

        public MaxRangeAttribute(float max)
        {
            this.Max = max;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class DisplayNestedPropertyAttribute : SPPropertyAttribute
    {

        public readonly string InnerPropName;
        public readonly string Label;
        public readonly string Tooltip;

        public DisplayNestedPropertyAttribute(string innerPropName)
        {
            InnerPropName = innerPropName;
        }

        public DisplayNestedPropertyAttribute(string innerPropName, string label)
        {
            InnerPropName = innerPropName;
            Label = label;
        }

        public DisplayNestedPropertyAttribute(string innerPropName, string label, string tooltip)
        {
            InnerPropName = innerPropName;
            Label = label;
            Tooltip = tooltip;
        }

    }

    #endregion

    #region Default Or Configured Property Drawer Attribute

    public abstract class DefaultOrConfiguredAttribute : PropertyAttribute
    {

        private System.Type _fieldType;
        private object _defaultValue;

        public DefaultOrConfiguredAttribute(System.Type tp)
        {
            _fieldType = tp;
            _defaultValue = tp.GetDefaultValue();
        }

        public DefaultOrConfiguredAttribute(System.Type tp, object defaultValue)
        {
            _fieldType = tp;
            _defaultValue = defaultValue;
        }

        public System.Type FieldType { get { return _fieldType; } }

        public virtual bool DrawAsDefault(object value)
        {
            return object.Equals(value, _defaultValue);
        }
        public virtual object GetDefaultValue()
        {
            return _defaultValue;
        }

        public virtual object GetValueToDisplayAsDefault()
        {
            return this.GetDefaultValue();
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

    /// <summary>
    /// While in the editor, if the value is ever null, an attempt is made to find the value on a GameObject in itself 
    /// that matches the name given.
    /// 
    /// You whil still have to initialize the value on Awake if null. The cost of doing it automatically is too high for all 
    /// components to test themselves for this attribute.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class FindInSelfAttribute : PropertyModifierAttribute
    {
        public string Name;
        public bool UseEntity = false;

        public FindInSelfAttribute(string name)
        {
            this.Name = name;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class ForceFromSelfAttribute : PropertyModifierAttribute
    {

        public EntityRelativity Relativity;

        public ForceFromSelfAttribute(EntityRelativity relativity)
        {
            this.Relativity = relativity;
        }
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

    #region NonSerialized Property Drawer Attributes

    public class ShowNonSerializedPropertyAttribute : System.Attribute
    {
        public string Label;
        public string Tooltip;
        public bool Readonly;

        public ShowNonSerializedPropertyAttribute(string label)
        {
            this.Label = label;
        }
    }

    #endregion

}