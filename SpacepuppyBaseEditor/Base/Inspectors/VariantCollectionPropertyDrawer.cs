using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor.Base.Inspectors
{

    [CustomPropertyDrawer(typeof(VariantCollection), true)]
    public class VariantCollectionPropertyDrawer : PropertyDrawer
    {

        #region Fields

        private VariantReferencePropertyDrawer _variantDrawer = new VariantReferencePropertyDrawer();

        private ReorderableList _lst;
        private GUIContent _label;
        private SerializedProperty _currentProp;
        private SerializedProperty _currentKeysProp;
        private SerializedProperty _currentValuesProp;

        private System.Type _propertyListTargetType; //if the VariantCollection is treated as a PropertyList
        private System.Reflection.MemberInfo[] _propertyListMembers;
        private string[] _propertyListNames;

        #endregion

        #region CONSTRUCTOR

        private void StartOnGUI(SerializedProperty property, GUIContent label)
        {
            _currentProp = property;
            _label = label;
            _currentKeysProp = _currentProp.FindPropertyRelative("_keys");
            _currentValuesProp = _currentProp.FindPropertyRelative("_values");

            _currentValuesProp.arraySize = _currentKeysProp.arraySize;

            _lst = CachedReorderableList.GetListDrawer(_currentKeysProp, _lst_DrawHeader, _lst_DrawElement, _lst_OnAdd, _lst_OnRemove);
            //_lst.draggable = false;




            if(this.fieldInfo != null)
            {
                var attrib = this.fieldInfo.GetCustomAttributes(typeof(VariantCollection.AsPropertyListAttribute), false).FirstOrDefault() as VariantCollection.AsPropertyListAttribute;
                _propertyListTargetType = (attrib != null) ? attrib.TargetType : null;
                if(attrib != null && attrib.TargetType != null)
                {
                    _propertyListTargetType = attrib.TargetType;

                    _propertyListMembers = (from m
                                             in DynamicUtil.GetEasilySerializedMembersFromType(_propertyListTargetType, System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Property, DynamicMemberAccess.Write)
                                            select m).ToArray();
                    _propertyListNames = (from m in _propertyListMembers select m.Name).ToArray();
                }
            }
        }

        private void EndOnGUI(SerializedProperty property, GUIContent label)
        {
            _lst = null;
            _currentProp = null;
            _currentKeysProp = null;
            _currentValuesProp = null;
            _label = null;


            _propertyListTargetType = null;
            _propertyListMembers = null;
            _propertyListNames = null;

            _variantDrawer.RestrictVariantType = false;
            _variantDrawer.ForcedComponentType = null;
        }

        #endregion

        #region Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float h;
            if (EditorHelper.AssertMultiObjectEditingNotSupportedHeight(property, label, out h)) return h;

            this.StartOnGUI(property, label);

            h = _lst.GetHeight();

            this.EndOnGUI(property, label);

            return h;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorHelper.AssertMultiObjectEditingNotSupported(position, property, label)) return;

            this.StartOnGUI(property, label);

            _lst.DoList(position);

            this.EndOnGUI(property, label);
        }

        #endregion

        #region ReorderableList Handlers

        private void _lst_DrawHeader(Rect area)
        {
            EditorGUI.LabelField(area, _label);
        }

        private void _lst_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            if (_currentValuesProp.arraySize <= index) _currentValuesProp.arraySize = index + 1;
            var keyProp = _currentKeysProp.GetArrayElementAtIndex(index);
            var variantProp = _currentValuesProp.GetArrayElementAtIndex(index);

            var w = area.width / 3f;
            if(_propertyListTargetType != null)
            {
                var nameRect = new Rect(area.xMin, area.yMin, w, EditorGUIUtility.singleLineHeight);
                EditorGUI.BeginChangeCheck();
                int i = EditorGUI.Popup(nameRect, _propertyListNames.IndexOf(keyProp.stringValue), _propertyListNames);
                if (EditorGUI.EndChangeCheck() && i >= 0 && !NameIsInUse(_currentKeysProp, _propertyListNames[i]))
                    keyProp.stringValue = _propertyListNames[i];

                var variantRect = new Rect(nameRect.xMax + 1f, area.yMin, area.width - (nameRect.width + 1f), area.height);
                if (i >= 0)
                {
                    var propType = com.spacepuppy.Dynamic.DynamicUtil.GetParameters(_propertyListMembers[i]).FirstOrDefault();
                    var argType = VariantReference.GetVariantType(propType);
                    _variantDrawer.RestrictVariantType = true;
                    _variantDrawer.VariantTypeRestrictedTo = argType;
                    _variantDrawer.ForcedComponentType = (TypeUtil.IsType(propType, typeof(Component))) ? propType : null;
                    _variantDrawer.DrawValueField(variantRect, variantProp);
                }
                else
                {
                    EditorGUI.LabelField(variantRect, "Select Property");
                }
            }
            else
            {
                var nameRect = new Rect(area.xMin, area.yMin, w, EditorGUIUtility.singleLineHeight);
                keyProp.stringValue = EditorGUI.TextField(nameRect, keyProp.stringValue);

                var variantRect = new Rect(nameRect.xMax + 1f, area.yMin, area.width - (nameRect.width + 1f), area.height);
                _variantDrawer.DrawValueField(variantRect, variantProp);
            }
        }

        private void _lst_OnAdd(ReorderableList lst)
        {
            if (_propertyListTargetType != null)
            {
                int i = _currentKeysProp.arraySize;
                _currentKeysProp.arraySize = i + 1;
                _currentKeysProp.GetArrayElementAtIndex(i).stringValue = null;
                _currentValuesProp.arraySize = i + 1;
            }
            else
            {
                int i = _currentKeysProp.arraySize;
                _currentKeysProp.arraySize = i + 1;
                _currentKeysProp.GetArrayElementAtIndex(i).stringValue = "Entry " + i.ToString();
                _currentValuesProp.arraySize = i + 1;
            }
        }
        
        private void _lst_OnRemove(ReorderableList lst)
        {
            var index = lst.index;
            if(index < 0 || index >= _currentKeysProp.arraySize) return;
            _currentKeysProp.DeleteArrayElementAtIndex(index);
            if (index < _currentValuesProp.arraySize) _currentValuesProp.DeleteArrayElementAtIndex(index);
        }

        #endregion

        #region Static Utils

        private static bool NameIsInUse(SerializedProperty keysArrayProp, string name)
        {
            for(int i = 0; i < keysArrayProp.arraySize; i++)
            {
                if (keysArrayProp.GetArrayElementAtIndex(i).stringValue == name) return true;
            }
            return false;
        }

        #endregion

    }
}
