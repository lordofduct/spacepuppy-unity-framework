using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Components
{

    [CustomPropertyDrawer(typeof(SelectableComponentAttribute))]
    public class SelectableComponentPropertyDrawer : PropertyDrawer
    {

        #region Fields

        public const float DEFAULT_POPUP_WIDTH_SCALE = 0.4f;

        public bool AllowSceneObject = true;
        public bool ForceOnlySelf;
        public bool SearchChildren;
        public bool ShowXButton = true;
        public bool XButtonOnRightSide = true;
        public IComponentChoiceSelector ChoiceSelector;

        private System.Type _restrictionType = typeof(Component);

        public System.Type RestrictionType
        {
            get { return _restrictionType; }
            set
            {
                if (value == null || !TypeUtil.IsType(value, typeof(Component), typeof(IComponent))) throw new TypeArgumentMismatchException(value, typeof(Component), "value");
                _restrictionType = value;
            }
        }

        #endregion

        #region CONSTRUCTOR

        private void Init()
        {
            if (this.fieldInfo != null)
            {
                var tp = this.fieldInfo.FieldType;
                if (tp.IsListType()) tp = tp.GetElementTypeOfListType();
                _restrictionType = tp;
            }

            if (this.attribute != null && this.attribute is SelectableComponentAttribute)
            {
                //created as part as a PropertyHandler
                var attrib = (this.attribute as SelectableComponentAttribute);
                this.AllowSceneObject = attrib.AllowSceneObjects;
                this.ForceOnlySelf = attrib.ForceOnlySelf;
                this.SearchChildren = attrib.SearchChildren;
                if (attrib.InheritsFromType != null) _restrictionType = attrib.InheritsFromType;
            }

            if (this.ChoiceSelector == null)
            {
                this.ChoiceSelector = new DefaultComponentChoiceSelector();
            }
        }

        #endregion


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            this.OnGUI(position, property);
        }

        public void OnGUI(Rect position, SerializedProperty property)
        {
            //if (property.propertyType != SerializedPropertyType.ObjectReference || !TypeUtil.IsType(_restrictionType, typeof(Component), typeof(IComponent)))
            if (property.propertyType != SerializedPropertyType.ObjectReference || !(TypeUtil.IsType(_restrictionType, typeof(Component)) || _restrictionType.IsInterface))
            {
                this.DrawAsMismatchedAttribute(position, property);
                return;
            }

            this.Init();

            if (this.ForceOnlySelf)
            {
                var targGo = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                if (targGo == null)
                {
                    this.DrawAsMismatchedAttribute(position, property);
                    return;
                }

                if (property.objectReferenceValue == null)
                {
                    property.objectReferenceValue = targGo.GetComponent(_restrictionType);
                }
            }

            if (!this.ForceOnlySelf && property.objectReferenceValue == null)
            {
                //SPEditorGUI.DefaultPropertyField(position, property, label);
                this.DrawObjectRefField(position, property);
            }
            else
            {
                this.ChoiceSelector.BeforeGUI(this, property, _restrictionType);

                var fullsize = position;
                if (this.ShowXButton && SPEditorGUI.XButton(ref position, "Clear Selected GameObject", this.XButtonOnRightSide))
                {
                    property.objectReferenceValue = null;
                    this.DrawObjectRefField(fullsize, property);
                }
                else
                {
                    var components = this.ChoiceSelector.GetComponents();
                    var names = this.ChoiceSelector.GetPopupEntries();
                    int oi = this.ChoiceSelector.GetPopupIndexOfComponent(property.objectReferenceValue as Component);
                    int ni = EditorGUI.Popup(position, oi, names);
                    if (oi != ni) property.objectReferenceValue = this.ChoiceSelector.GetComponentAtPopupIndex(ni);
                }

                this.ChoiceSelector.GUIComplete(property);
            }
        }

        private void DrawObjectRefField(Rect position, SerializedProperty property)
        {
            if (TypeUtil.IsType(_restrictionType, typeof(Component)))
            {
                var obj = EditorGUI.ObjectField(position, property.objectReferenceValue, _restrictionType, this.AllowSceneObject);
                if(this.ForceOnlySelf)
                {
                    var targGo = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                    var ngo = GameObjectUtil.GetGameObjectFromSource(obj);
                    if(targGo == ngo ||
                       (this.SearchChildren && targGo.IsParentOf(ngo)))
                    {
                        property.objectReferenceValue = obj;
                    }
                }
                else
                {
                    property.objectReferenceValue = obj;
                }
            }
            else
            {
                var ogo = GameObjectUtil.GetGameObjectFromSource(property.objectReferenceValue);
                var ngo = EditorGUI.ObjectField(position, ogo, typeof(GameObject), this.AllowSceneObject) as GameObject;
                if (ogo != ngo)
                {
                    if(this.ForceOnlySelf)
                    {
                        var targGo = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                        if (targGo == ngo ||
                            (this.SearchChildren && targGo.IsParentOf(ngo)))
                        {
                            property.objectReferenceValue = ngo.GetComponent(_restrictionType);
                        }
                    }
                    else
                    {
                        property.objectReferenceValue = (ngo == null) ? null : ngo.GetComponent(_restrictionType);
                    }
                }
            }
        }


        private void DrawAsMismatchedAttribute(Rect position, SerializedProperty property)
        {
            EditorGUI.LabelField(position, EditorHelper.TempContent("Mismatched type of PropertyDrawer attribute with field."));
        }

    }

}
