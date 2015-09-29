using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor.Collections
{

    [CustomPropertyDrawer(typeof(DrawableEnumKeyedCollection), true)]
    public class EnumKeyedCollectionPropertyDrawer : PropertyDrawer
    {

        public const string PROP_VALUES = "_values";


        public PropertyDrawer OverridePropertyDrawer;

        private bool _unsafe;
        private ReorderableList _lst;
        private GUIContent _label;
        private System.Array _keys;



        public EnumKeyedCollectionPropertyDrawer()
        {

        }

        public EnumKeyedCollectionPropertyDrawer(bool unsafeUse)
        {
            if(unsafeUse)
            {
                _unsafe = true;
            }
        }


        public ReorderableList UnsafeList { get { return _lst; } }

        public object GetKeyAt(int index)
        {
            return _keys.GetValue(index);
        }

        private void BeginProperty(SerializedProperty property, GUIContent label)
        {
            var valuesProp = property.FindPropertyRelative(PROP_VALUES);

            _label = label;

            _lst = CachedReorderableList.GetListDrawer(valuesProp, _lst_DrawHeader, _lst_DrawElement);
            _lst.displayAdd = false;
            _lst.displayRemove = false;
            _lst.draggable = false;

            if (valuesProp.arraySize > 0)
            {
                var el = valuesProp.GetArrayElementAtIndex(0);
                if (OverridePropertyDrawer != null)
                    _lst.elementHeight = OverridePropertyDrawer.GetPropertyHeight(el, GUIContent.none);
                else
                    _lst.elementHeight = SPEditorGUI.GetPropertyHeight(el);
            }
            else
            {
                _lst.elementHeight = EditorGUIUtility.singleLineHeight;
            }
        }

        private void EndProperty()
        {
            _label = null;
            if(!_unsafe)
                _lst = null;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float h;
            if (EditorHelper.AssertMultiObjectEditingNotSupportedHeight(property, label, out h)) return h;

            if (property.isExpanded)
            {
                this.BeginProperty(property, label);
                h = _lst.GetHeight();
                this.EndProperty();
                return h;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorHelper.AssertMultiObjectEditingNotSupported(position, property, label)) return;

            property.isExpanded = EditorGUI.Foldout(new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, GUIContent.none);

            if (property.isExpanded)
            {
                _keys = GetKeys(property);
                if (_keys == null)
                {
                    EditorGUI.LabelField(position, "Failed to determine enum key type.");
                    return;
                }

                var valuesProp = property.FindPropertyRelative(PROP_VALUES);
                if (valuesProp.arraySize != _keys.Length) valuesProp.arraySize = _keys.Length;

                this.BeginProperty(property, label);
                _lst.DoList(position);
                this.EndProperty();
            }
            else
            {
                ReorderableListHelper.DrawRetractedHeader(position, label);
            }
        }



        #region Masks ReorderableList Handlers

        private void _lst_DrawHeader(Rect area)
        {
            EditorGUI.LabelField(area, _label);
        }

        private void _lst_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            string id = System.Convert.ToString(_keys.GetValue(index));
            var el = _lst.serializedProperty.GetArrayElementAtIndex(index);

            if (this.OverridePropertyDrawer != null)
                this.OverridePropertyDrawer.OnGUI(area, el, EditorHelper.TempContent(id));
            else
                SPEditorGUI.PropertyField(area, el, EditorHelper.TempContent(id));
        }

        #endregion


        #region Static Utils

        private static System.Array GetKeys(SerializedProperty property)
        {
            var targ = EditorHelper.GetTargetObjectOfProperty(property);
            if (targ == null)
                return null;

            var tp = targ.GetType();
            var ptp = typeof(DrawableEnumKeyedCollection);
            while(tp != null && tp.BaseType != ptp)
            {
                tp = tp.BaseType;
            }

            if (tp == null)
                return null;

            tp = tp.GetGenericArguments().FirstOrDefault();
            if (tp == null || !tp.IsEnum)
                return null;

            var arr = System.Enum.GetValues(tp);
            System.Array.Sort(arr);
            return arr;
        }

        #endregion

    }

}
