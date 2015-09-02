using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor.Base.Inspectors
{

    [CustomPropertyDrawer(typeof(VariantCollection))]
    public class VariantCollectionPropertyDrawer : PropertyDrawer
    {

        #region Fields

        private VariantReferencePropertyDrawer _variantDrawer = new VariantReferencePropertyDrawer();

        private ReorderableList _lst;
        private GUIContent _label;
        private SerializedProperty _currentProp;
        private SerializedProperty _currentKeysProp;
        private SerializedProperty _currentValuesProp;

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
            _lst.draggable = false;

        }

        private void EndOnGUI(SerializedProperty property, GUIContent label)
        {
            _lst = null;
            _currentProp = null;
            _currentKeysProp = null;
            _currentValuesProp = null;
            _label = null;
        }

        #endregion

        #region Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            this.StartOnGUI(property, label);

            var h = _lst.GetHeight();

            this.EndOnGUI(property, label);

            return h;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
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
            var nameRect = new Rect(area.xMin, area.yMin, w, EditorGUIUtility.singleLineHeight);
            keyProp.stringValue = EditorGUI.TextField(nameRect, keyProp.stringValue);

            var variantRect = new Rect(nameRect.xMax + 1f, area.yMin, area.width - (nameRect.width + 1f), area.height);
            _variantDrawer.DrawValueField(variantRect, variantProp);

        }

        private void _lst_OnAdd(ReorderableList lst)
        {
            int i = _currentKeysProp.arraySize;
            _currentKeysProp.arraySize = i + 1;
            _currentKeysProp.GetArrayElementAtIndex(i).stringValue = "Entry " + i.ToString();
            _currentValuesProp.arraySize = i + 1;
        }
        
        private void _lst_OnRemove(ReorderableList lst)
        {
            var index = lst.index;
            if(index < 0 || index >= _currentKeysProp.arraySize) return;
            _currentKeysProp.DeleteArrayElementAtIndex(index);
            if (index < _currentValuesProp.arraySize) _currentValuesProp.DeleteArrayElementAtIndex(index);
        }

        #endregion

    }
}
