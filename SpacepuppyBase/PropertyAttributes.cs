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
    
    /*
     * TODO
     * 
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ExecutionOrderAttribute : System.Attribute
    {

        public int Order;
        public bool Inherited;

        public ExecutionOrderAttribute(int order)
        {
            this.Order = order;
        }

    }
    */

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
        public bool IgnoreInactive;

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

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ConstantlyRepaintEditorAttribute : System.Attribute
    {
        public bool RuntimeOnly;
    }

    #endregion

    #region Property Drawer Attributes

    /// <summary>
    /// ScriptableObject doesn't draw vectors correctly for some reason... this allows you to coerce it to.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class VectorInspectorAttribute : SPPropertyAttribute
    {

        public VectorInspectorAttribute()
        {

        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class UnitVectorAttribute : SPPropertyAttribute
    {

        public UnitVectorAttribute() : base()
        {

        }

    }

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
    public class TypeRestrictionAttribute : SPPropertyAttribute
    {
        public System.Type InheritsFromType;
        public bool HideTypeDropDown;
        public bool AllowProxy;

        public TypeRestrictionAttribute(System.Type inheritsFromType)
        {
            this.InheritsFromType = inheritsFromType;
        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    [System.Obsolete("Use TypeRestrictionAttribute Instead")]
    public class ComponentTypeRestrictionAttribute : TypeRestrictionAttribute
    {
        public ComponentTypeRestrictionAttribute(System.Type inheritsFromType)
            : base(inheritsFromType)
        {

        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class SelectableComponentAttribute : SPPropertyAttribute
    {
        public System.Type InheritsFromType;
        public bool AllowSceneObjects = true;
        public bool ForceOnlySelf = false;
        public bool SearchChildren = false;
        public bool AllowProxy;

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
        public System.Type InheritsFromType;
        public bool AllowSceneObjects = true;
        public bool AllowProxy;

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
        public bool DrawElementAtBottom = false;
        public bool HideElementLabel = false;

        /// <summary>
        /// If DrawElementAtBottom is true, this child element can be displayed as the label in the reorderable list.
        /// </summary>
        public string ChildPropertyToDrawAsElementLabel;

        /// <summary>
        /// If DrawElementAtBottom is true, this child element can be displayed as the modifiable entry in the reorderable list.
        /// </summary>
        public string ChildPropertyToDrawAsElementEntry;

        /// <summary>
        /// A method on the serialized object that is called when a new entry is added to the list/array. Should accept the list member type 
        /// as a parameter, and then also return it (used for updating).
        /// 
        /// Like:
        /// object OnObjectAddedToList(object obj)
        /// </summary>
        public string OnAddCallback;

        /// <summary>
        /// If the array/list accepts UnityEngine.Objects, this will allow the dragging of objects onto the inspector to auto add without needing to click the + button.
        /// </summary>
        public bool AllowDragAndDrop = true;

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

    public class DisplayFlatAttribute : SPPropertyAttribute
    {

        public bool CanShrinkAndExpand;

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class InputIDAttribute : SPPropertyAttribute
    {

        public string[] RestrictedTo;
        public string[] Exclude;

        public InputIDAttribute()
        {

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
    /// While in the editor, if the value is ever null, an attempt is made to get the value from self. You will still 
    /// have to initialize the value on Awake if null. The cost of doing it automatically is too high for all components 
    /// to test themselves for this attribute.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class DefaultFromSelfAttribute : PropertyModifierAttribute
    {
        public EntityRelativity Relativity = EntityRelativity.Self;
        public bool HandleOnce = true;

        public DefaultFromSelfAttribute(EntityRelativity relativity = EntityRelativity.Self)
        {
            this.Relativity = relativity;
        }

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

        public EntityRelativity Relativity = EntityRelativity.Self;
        
        public ForceFromSelfAttribute(EntityRelativity relativity = EntityRelativity.Self)
        {
            this.Relativity = relativity;
        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class DisableOnPlayAttribute : PropertyModifierAttribute
    {

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class DisableIfAttribute : PropertyModifierAttribute
    {
        public readonly string MemberName;
        public bool DisableIfNot;
        
        public DisableIfAttribute(string memberName)
        {
            this.MemberName = memberName;
        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class ReadOnlyAttribute : PropertyModifierAttribute
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

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class InsertButtonAttribute : PropertyAttribute
    {

        public string Label;
        public string OnClick;
        public bool PrecedeProperty;
        public bool RuntimeOnly;

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