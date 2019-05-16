using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Modifiers;

namespace com.spacepuppyeditor.Internal
{
    internal class MultiPropertyAttributePropertyHandler : UnityInternalPropertyHandler
    {

        #region Fields

        private System.Reflection.FieldInfo _fieldInfo;
        private bool _propertyIsArray;

        private PropertyDrawer _drawer;
        private List<PropertyModifier> _modifiers;
        private string _customTooltip;

        #endregion

        #region CONSTRUCTOR
        
        public MultiPropertyAttributePropertyHandler(System.Reflection.FieldInfo fieldInfo, bool propertyIsArray, PropertyAttribute[] attribs)
        {
            if (fieldInfo == null) throw new System.ArgumentNullException("fieldInfo");
            if (attribs == null) throw new System.ArgumentNullException("attribs");
            _fieldInfo = fieldInfo;
            _propertyIsArray = propertyIsArray;

            this.Init(attribs);
        }

        #endregion

        #region Properties

        public System.Reflection.FieldInfo Field { get { return _fieldInfo; } }

        public bool PropertyIsArray
        {
            get { return _propertyIsArray; }
        }

        #endregion

        #region Methods

        protected virtual void Init(PropertyAttribute[] attribs)
        {
            var fieldType = _fieldInfo.FieldType;
            if (fieldType.IsListType()) fieldType = fieldType.GetElementTypeOfListType();

            var fieldTypePropertyDrawerType = ScriptAttributeUtility.GetDrawerTypeForType(fieldType);
            if (fieldTypePropertyDrawerType != null && TypeUtil.IsType(fieldTypePropertyDrawerType, typeof(PropertyDrawer)))
            {
                _drawer = PropertyDrawerActivator.Create(fieldTypePropertyDrawerType, null, _fieldInfo);
                if (_drawer != null && _propertyIsArray) _drawer = new ArrayPropertyDrawer(_drawer);
                this.InternalDrawer = _drawer;
            }

            if (attribs != null)
            {
                foreach (var attrib in attribs)
                {
                    this.HandleAttribute(attrib, _fieldInfo, fieldType);
                }
            }

            if(_drawer == null)
            {
                if (_modifiers != null && _modifiers.Count > 0)
                {
                    var modifier = _modifiers.Last();
                    modifier.IsDrawer = true;
                    _drawer = modifier;
                    if (_propertyIsArray) _drawer = new ArrayPropertyDrawer(_drawer);
                    this.InternalDrawer = _drawer;
                }
                else
                {
                    _drawer = DefaultPropertyDrawer.SharedInstance;
                    if (_drawer != null && _propertyIsArray) _drawer = new ArrayPropertyDrawer(_drawer);
                    this.InternalDrawer = _drawer;
                }
            }
        }

        protected override void HandleAttribute(PropertyAttribute attribute, System.Reflection.FieldInfo field, System.Type propertyType)
        {
            if(attribute is PropertyModifierAttribute)
            {
                var mtp = ScriptAttributeUtility.GetDrawerTypeForType(attribute.GetType());
                if (TypeUtil.IsType(mtp, typeof(PropertyModifier)))
                {
                    var modifier = PropertyDrawerActivator.Create(mtp, attribute, field) as PropertyModifier;
                    if (_modifiers == null) _modifiers = new List<PropertyModifier>();
                    _modifiers.Add(modifier);
                }
            }
            else if(attribute is TooltipAttribute)
            {
                _customTooltip = (attribute as TooltipAttribute).tooltip;
                base.HandleAttribute(attribute, field, propertyType);
            }
            else if(attribute is ContextMenuItemAttribute)
            {
                base.HandleAttribute(attribute, field, propertyType);
            }
            else
            {
                var drawerTypeForType = ScriptAttributeUtility.GetDrawerTypeForType(attribute.GetType());
                if (drawerTypeForType == null)
                    return;
                else if (typeof(PropertyDrawer).IsAssignableFrom(drawerTypeForType))
                {
                    base.HandleAttribute(attribute, field, propertyType);
                    var drawer = this.InternalDrawer; //this retrieves the drawer that was selected by called 'base.HandleAttribute'
                    this.AppendDrawer(drawer);
                }
                else if (typeof(DecoratorDrawer).IsAssignableFrom(drawerTypeForType))
                {
                    DecoratorDrawer instance = (DecoratorDrawer)System.Activator.CreateInstance(drawerTypeForType);
                    com.spacepuppy.Dynamic.DynamicUtil.SetValue(instance, "m_Attribute", attribute);
                    if (this.DecoratorDrawers == null)
                        this.DecoratorDrawers = new List<DecoratorDrawer>();
                    this.DecoratorDrawers.Add(instance);
                }
            }
        }

        protected void AppendDrawer(PropertyDrawer drawer)
        {
            if (_drawer == null)
            {
                //no drawer has been set before... lets see if we got one
                if(drawer is PropertyModifier)
                {
                    if (_modifiers == null) _modifiers = new List<PropertyModifier>();
                    _modifiers.Add(drawer as PropertyModifier);
                    
                    if(_propertyIsArray)
                    {
                        _drawer = new ArrayPropertyDrawer(null);
                    }
                }
                else if(drawer != null)
                {
                    //we got a new drawer, set it
                    if (!(drawer is IArrayHandlingPropertyDrawer) && _propertyIsArray) drawer = new ArrayPropertyDrawer(drawer);
                    _drawer = drawer;
                }
            }
            else if (drawer != _drawer)
            {
                //a new drawer was created, lets see what we need to do with it compared to the last one
                if (drawer is PropertyModifier)
                {
                    if (_modifiers == null) _modifiers = new List<PropertyModifier>();
                    _modifiers.Add(drawer as PropertyModifier);
                }
                else if (drawer is IArrayHandlingPropertyDrawer)
                {
                    //got an array drawer, this overrides previous drawers
                    if (_drawer is IArrayHandlingPropertyDrawer)
                    {
                        var temp = _drawer as IArrayHandlingPropertyDrawer;
                        _drawer = drawer;
                        (_drawer as IArrayHandlingPropertyDrawer).InternalDrawer = temp.InternalDrawer;
                    }
                    else if (_drawer != null)
                    {
                        var temp = _drawer;
                        _drawer = drawer;
                        (_drawer as IArrayHandlingPropertyDrawer).InternalDrawer = temp;
                    }
                    else
                    {
                        _drawer = drawer;
                    }
                }
                else if (_drawer is IArrayHandlingPropertyDrawer)
                {
                    //got an internal drawer for the existing array drawer
                    (_drawer as IArrayHandlingPropertyDrawer).InternalDrawer = drawer;
                }
                else
                {
                    //we got a new drawer, set it
                    if (_propertyIsArray)
                    {
                        _drawer = new ArrayPropertyDrawer(drawer);
                    }
                    else
                    {
                        _drawer = drawer;
                    }
                }
            }

            //ensure internal drawer is set appropriately
            this.InternalDrawer = _drawer;
        }





        public override bool OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if(label == null)
            {
                label = EditorHelper.TempContent(property.displayName, _customTooltip ?? property.tooltip);
            }
            else if(string.IsNullOrEmpty(label.tooltip) && !string.IsNullOrEmpty(_customTooltip))
            {
                label = EditorHelper.CloneContent(label);
                label.tooltip = _customTooltip;
            }

            bool cancelDraw = false;

            if (_modifiers != null)
            {
                for (int i = 0; i < _modifiers.Count; i++)
                {
                    _modifiers[i].OnBeforeGUI(property, ref cancelDraw);
                }
            }

            bool result = false;
            if (!cancelDraw) result = base.OnGUI(position, property, label, includeChildren);
            PropertyHandlerValidationUtility.AddAsHandled(property, this);

            if (_modifiers != null)
            {
                for (int i = 0; i < _modifiers.Count; i++)
                {
                    _modifiers[i].OnPostGUI(property);
                }
            }

            return result;
        }

        public override bool OnGUILayout(SerializedProperty property, GUIContent label, bool includeChildren, GUILayoutOption[] options)
        {
            if (label == null)
            {
                label = EditorHelper.TempContent(property.displayName, _customTooltip ?? property.tooltip);
            }
            else if (string.IsNullOrEmpty(label.tooltip) && !string.IsNullOrEmpty(_customTooltip))
            {
                label = EditorHelper.CloneContent(label);
                label.tooltip = _customTooltip;
            }

            bool cancelDraw = false;

            if (_modifiers != null)
            {
                for (int i = 0; i < _modifiers.Count; i++)
                {
                    _modifiers[i].OnBeforeGUI(property, ref cancelDraw);
                }
            }

            bool result = false;
            if (!cancelDraw) result = base.OnGUILayout(property, label, includeChildren, options);
            PropertyHandlerValidationUtility.AddAsHandled(property, this);

            if (_modifiers != null)
            {
                for (int i = 0; i < _modifiers.Count; i++)
                {
                    _modifiers[i].OnPostGUI(property);
                }
            }

            return result;
        }

        public override void OnValidate(SerializedProperty property)
        {
            if (_modifiers != null)
            {
                for (int i = 0; i < _modifiers.Count; i++)
                {
                    _modifiers[i].OnValidate(property);
                }
            }
        }

        #endregion




        #region Special Types

        private class ArrayPropertyDrawer : PropertyDrawer, IArrayHandlingPropertyDrawer
        {

            #region Fields

            private PropertyDrawer _drawer;

            #endregion

            #region CONSTRUCTOR

            public ArrayPropertyDrawer(PropertyDrawer drawer)
            {
                _drawer = drawer;
            }

            #endregion

            #region PropertyDrawer Interface

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                if (label == null) label = EditorHelper.TempContent(property.displayName);

                if (!property.isArray)
                {
                    if (_drawer != null)
                        return _drawer.GetPropertyHeight(property, label);
                    else
                        return SPEditorGUI.GetDefaultPropertyHeight(property);
                }
                else
                {
                    float h = SPEditorGUI.GetSinglePropertyHeight(property, label);
                    if (!property.isExpanded) return h;

                    h += EditorGUIUtility.singleLineHeight + 2f;

                    for (int i = 0; i < property.arraySize; i++)
                    {
                        var pchild = property.GetArrayElementAtIndex(i);
                        if(_drawer != null)
                            h += _drawer.GetPropertyHeight(pchild, EditorHelper.TempContent(pchild.displayName)) + 2f;
                        else
                            h += SPEditorGUI.GetPropertyHeight(pchild, EditorHelper.TempContent(pchild.displayName)) + 2f;
                    }
                    return h;
                }
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (label == null) label = EditorHelper.TempContent(property.displayName);

                if (!property.isArray)
                {
                    if (_drawer != null)
                        _drawer.OnGUI(position, property, label);
                    else
                        SPEditorGUI.DefaultPropertyField(position, property, label);
                }
                else
                {
                    if (property.isExpanded)
                    {
                        var rect = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
                        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);

                        EditorGUI.indentLevel++;
                        rect = new Rect(rect.xMin, rect.yMax + 2f, rect.width, EditorGUIUtility.singleLineHeight);
                        property.arraySize = Mathf.Max(0, EditorGUI.IntField(rect, "Size", property.arraySize));

                        var lbl = EditorHelper.TempContent("");
                        for (int i = 0; i < property.arraySize; i++)
                        {
                            var pchild = property.GetArrayElementAtIndex(i);
                            lbl.text = pchild.displayName;
                            if(_drawer != null)
                            {
                                var h = _drawer.GetPropertyHeight(pchild, lbl);
                                rect = new Rect(rect.xMin, rect.yMax + 2f, rect.width, h);
                                _drawer.OnGUI(rect, pchild, lbl);
                            }
                            else
                            {
                                var h = SPEditorGUI.GetDefaultPropertyHeight(pchild, lbl);
                                rect = new Rect(rect.xMin, rect.yMax + 2f, rect.width, h);
                                SPEditorGUI.DefaultPropertyField(rect, pchild, lbl);
                            }
                        }

                        EditorGUI.indentLevel--;
                    }
                    else
                    {
                        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
                    }
                }
            }

            #endregion

            #region IArrayHandlingPropertyDrawer Interface

            public PropertyDrawer InternalDrawer
            {
                get
                {
                    return _drawer;
                }
                set
                {
                    if (value != null) _drawer = value;
                }
            }

            #endregion

        }

        

        private class DefaultPropertyDrawer : PropertyDrawer
        {
            public readonly static DefaultPropertyDrawer SharedInstance = new DefaultPropertyDrawer();

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return ScriptAttributeUtility.SharedNullPropertyHandler.GetHeight(property, label, property.hasVisibleChildren);
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                ScriptAttributeUtility.SharedNullPropertyHandler.OnGUI(position, property, label, property.hasVisibleChildren);
            }
        }

        #endregion

    }

}
