using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    /// <summary>
    /// Currently only works with builtin unity types.
    /// </summary>
    [CustomPropertyDrawer(typeof(OneOrManyAttribute))]
    public class OneOrManyPropertyDrawer : PropertyDrawer, IArrayHandlingPropertyDrawer
    {
        private const float BTN_WIDTH = 42f;
        private const float SIZE_WIDTH = 50f;
        private GUIContent _moreBtnLabel = new GUIContent("Many", "Change between accepting a configured argument or not.");
        private GUIContent _oneBtnLabel = new GUIContent("One", "Change between accepting a configured argument or not.");

        private PropertyDrawer _internalDrawer;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isArray) return EditorGUIUtility.singleLineHeight;

            if (property.arraySize == 0) property.arraySize = 1;

            if (property.arraySize == 1)
            {
                return (_internalDrawer != null) ? _internalDrawer.GetPropertyHeight(property.GetArrayElementAtIndex(0), EditorHelper.TempContent("Element 0")) : EditorGUIUtility.singleLineHeight;
            }
            else
            {
                var h = EditorGUIUtility.singleLineHeight;
                var lbl = EditorHelper.TempContent("Element 0");
                for (int i = 0; i < property.arraySize; i++)
                {
                    h += (_internalDrawer != null) ? _internalDrawer.GetPropertyHeight(property.GetArrayElementAtIndex(i), lbl) : EditorGUIUtility.singleLineHeight;
                }
                return h;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(!property.isArray)
            {
                EditorGUI.PropertyField(position, property, label, false);
                return;
            }

            if (property.arraySize == 0) property.arraySize = 1;

            if (property.arraySize == 1)
            {
                var elementHeight = (_internalDrawer != null) ? _internalDrawer.GetPropertyHeight(property.GetArrayElementAtIndex(0), label) : EditorGUIUtility.singleLineHeight;
                var propArea = new Rect(position.xMin, position.yMin, Mathf.Max(0f, position.width - BTN_WIDTH), elementHeight);
                var btnArea = new Rect(propArea.xMax, position.yMin, Mathf.Min(BTN_WIDTH, position.width), EditorGUIUtility.singleLineHeight);

                if (_internalDrawer != null)
                    _internalDrawer.OnGUI(propArea, property.GetArrayElementAtIndex(0), label);
                else
                    SPEditorGUI.DefaultPropertyField(propArea, property.GetArrayElementAtIndex(0), label);
                if (GUI.Button(btnArea, _moreBtnLabel))
                {
                    property.arraySize = 2;
                }
            }
            else
            {
                var elementArea = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);

                //draw header
                var leftOverArea = EditorGUI.PrefixLabel(elementArea, label ?? GUIContent.none);
                var sizeArea = new Rect(Mathf.Max(leftOverArea.xMin, leftOverArea.xMax - BTN_WIDTH - SIZE_WIDTH),
                                        leftOverArea.yMin,
                                        Mathf.Min(SIZE_WIDTH, Mathf.Max(0f, leftOverArea.width - BTN_WIDTH)), EditorGUIUtility.singleLineHeight);
                var btnArea = new Rect(sizeArea.xMax, position.yMin, Mathf.Min(BTN_WIDTH, position.width), EditorGUIUtility.singleLineHeight);
                property.arraySize = Mathf.Max(EditorGUI.IntField(sizeArea, property.arraySize), 1);
                if(GUI.Button(btnArea, _oneBtnLabel))
                {
                    property.arraySize = 1;
                }

                EditorGUI.indentLevel++;
                for(int i = 0; i < property.arraySize; i++)
                {
                    var lbl = EditorHelper.TempContent("Element " + i.ToString());
                    var elementHeight = (_internalDrawer != null) ? _internalDrawer.GetPropertyHeight(property.GetArrayElementAtIndex(i), lbl) : EditorGUIUtility.singleLineHeight;
                    elementArea = new Rect(position.xMin, elementArea.yMax, position.width, elementHeight);

                    if (_internalDrawer != null)
                        _internalDrawer.OnGUI(elementArea, property.GetArrayElementAtIndex(i), lbl);
                    else
                        SPEditorGUI.DefaultPropertyField(elementArea, property.GetArrayElementAtIndex(i), lbl);
                }
                EditorGUI.indentLevel--;
            }
        }

        #region IArrayHandlingPropertyDrawer Interface

        PropertyDrawer IArrayHandlingPropertyDrawer.InternalDrawer
        {
            get
            {
                return _internalDrawer;
            }
            set
            {
                _internalDrawer = value;
            }
        }

        #endregion

    }

}
