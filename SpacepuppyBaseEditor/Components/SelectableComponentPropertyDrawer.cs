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

        public bool AllowSceneObject;
        public bool ForceOnlySelf;
        public bool SearchChildren;
        public float PopupWidthScale = DEFAULT_POPUP_WIDTH_SCALE;
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
                var attrib = (this.attribute as SelectableComponentAttribute);
                this.AllowSceneObject = attrib.AllowSceneObjects;
                this.ForceOnlySelf = attrib.ForceOnlySelf;
                SearchChildren = attrib.SearchChildren;
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
            if (property.propertyType != SerializedPropertyType.ObjectReference || !TypeUtil.IsType(_restrictionType, typeof(Component), typeof(IComponent)))
            {
                this.DrawAsMismatchedAttribute(position, property, label);
                return;
            }

            this.Init();

            if(this.ForceOnlySelf)
            {
                var targGo = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                if(targGo == null)
                {
                    this.DrawAsMismatchedAttribute(position, property, label);
                    return;
                }

                if(property.objectReferenceValue == null)
                {
                    property.objectReferenceValue = targGo.GetFirstLikeComponent(_restrictionType);
                }
            }

            if (!this.ForceOnlySelf && property.objectReferenceValue == null)
            {
                //SPEditorGUI.DefaultPropertyField(position, property, label);
                this.DrawObjectRefField(position, property, label);
            }
            else
            {
                this.ChoiceSelector.BeforeGUI(this, property, _restrictionType);

                float w = (label == GUIContent.none) ? position.width : Mathf.Max(position.width - EditorGUIUtility.labelWidth, 0);
                var ra = new Rect(position.xMin, position.yMin, w * this.PopupWidthScale + EditorGUIUtility.labelWidth, position.height);
                var rb = new Rect(ra.xMax, position.yMin, position.width - ra.width, position.height);

                var components = this.ChoiceSelector.GetComponents();
                var names = this.ChoiceSelector.GetPopupEntries();
                int oi = this.ChoiceSelector.GetPopupIndexOfComponent(property.objectReferenceValue as Component);
                int ni = EditorGUI.Popup(ra, label, oi, names);
                if (oi != ni) property.objectReferenceValue = this.ChoiceSelector.GetComponentAtPopupIndex(ni);

                this.DrawObjectRefField(rb, property, GUIContent.none);

                this.ChoiceSelector.GUIComplete(property);
            }
            
        }

        private void DrawObjectRefField(Rect position, SerializedProperty property, GUIContent label)
        {
            if (TypeUtil.IsType(_restrictionType, typeof(Component)))
            {
                var obj = EditorGUI.ObjectField(position, label, property.objectReferenceValue, _restrictionType, this.AllowSceneObject);
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
                var ngo = EditorGUI.ObjectField(position, label, ogo, typeof(GameObject), this.AllowSceneObject) as GameObject;
                if (ogo != ngo)
                {
                    if(this.ForceOnlySelf)
                    {
                        var targGo = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                        if (targGo == ngo ||
                            (this.SearchChildren && targGo.IsParentOf(ngo)))
                        {
                            property.objectReferenceValue = ngo.GetFirstLikeComponent(_restrictionType);
                        }
                    }
                    else
                    {
                        property.objectReferenceValue = (ngo == null) ? null : ngo.GetFirstLikeComponent(_restrictionType);
                    }
                }
            }
        }


        private void DrawAsMismatchedAttribute(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.LabelField(position, label, EditorHelper.TempContent("Mismatched type of PropertyDrawer attribute with field."));
        }

    }

}
