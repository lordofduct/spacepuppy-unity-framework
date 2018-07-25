using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor.Base
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

        private List<int> _memberLookupTable = new List<int>(); 

        #endregion

        #region CONSTRUCTOR

        private void StartOnGUI(SerializedProperty property, GUIContent label)
        {
            _currentProp = property;
            _label = label;
            _currentKeysProp = _currentProp.FindPropertyRelative("_keys");
            _currentValuesProp = _currentProp.FindPropertyRelative("_values");

            _currentValuesProp.arraySize = _currentKeysProp.arraySize;

            _lst = CachedReorderableList.GetListDrawer(_memberLookupTable, _currentProp, _lst_DrawHeader, _lst_DrawElement, _lst_OnAdd, _lst_OnRemove, null, null, _lst_OnReorder);
            //_lst.draggable = false;
            _memberLookupTable.Clear();
            for(int i = 0; i < _currentKeysProp.arraySize; i++)
            {
                _memberLookupTable.Add(i);
            }

            if (this.fieldInfo != null)
            {
                var asPropAttrib = this.fieldInfo.GetCustomAttributes(typeof(VariantCollection.AsPropertyListAttribute), false).FirstOrDefault() as VariantCollection.AsPropertyListAttribute;
                if(asPropAttrib != null && asPropAttrib.TargetType != null)
                {
                    this.ConfigurePropertyList(asPropAttrib.TargetType);
                }

                var asTypedList = this.fieldInfo.GetCustomAttributes(typeof(VariantCollection.AsTypedList), false).FirstOrDefault() as VariantCollection.AsTypedList;
                if(asTypedList != null && asTypedList.TargetType != null && VariantReference.AcceptableType(asTypedList.TargetType))
                {
                    _variantDrawer.RestrictVariantType = true;
                    _variantDrawer.TypeRestrictedTo = asTypedList.TargetType;
                    _variantDrawer.ForcedObjectType = asTypedList.TargetType;
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

            if(this.fieldInfo != null)
            {
                _propertyListTargetType = null;
                _propertyListMembers = null;
                _propertyListNames = null;
            }

            _variantDrawer.RestrictVariantType = false;
            _variantDrawer.ForcedObjectType = null;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void ConfigurePropertyList(System.Type tp)
        {
            _propertyListTargetType = tp;
            if(tp != null)
            {
                _propertyListMembers = (from m
                                         in DynamicUtil.GetEasilySerializedMembersFromType(_propertyListTargetType, System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Property, DynamicMemberAccess.Write)
                                        select m).ToArray();
                _propertyListNames = (from m in _propertyListMembers select m.Name).ToArray();
            }
            else
            {
                _propertyListMembers = null;
                _propertyListNames = null;
            }
        }

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
                    var propType = com.spacepuppy.Dynamic.DynamicUtil.GetReturnType(_propertyListMembers[i]);
                    _variantDrawer.RestrictVariantType = true;
                    _variantDrawer.TypeRestrictedTo = propType;
                    _variantDrawer.ForcedObjectType = propType;
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
        
        private void _lst_OnReorder(ReorderableList lst)
        {
            if (lst.index < 0 || lst.index >= _memberLookupTable.Count) return;

            int i = lst.index;
            int j = _memberLookupTable[lst.index];

            _currentKeysProp.MoveArrayElement(j, i);
            _currentValuesProp.MoveArrayElement(j, i);
            _currentProp.serializedObject.ApplyModifiedProperties();
            _currentProp.serializedObject.Update();
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
