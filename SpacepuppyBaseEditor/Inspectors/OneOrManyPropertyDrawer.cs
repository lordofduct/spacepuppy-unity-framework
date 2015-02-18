using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Inspectors
{

    [CustomPropertyDrawer(typeof(OneOrManyAttribute))]
    public class OneOrManyPropertyDrawer : PropertyDrawer
    {
        private const float BTN_WIDTH = 42f;
        private const float SIZE_WIDTH = 50f;
        private GUIContent _moreBtnLabel = new GUIContent("Many", "Change between accepting a configured argument or not.");
        private GUIContent _oneBtnLabel = new GUIContent("One", "Change between accepting a configured argument or not.");

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isArray) return EditorGUIUtility.singleLineHeight;

            if (property.arraySize == 0) property.arraySize = 1;
            if (property.arraySize == 1)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            else
            {
                return (property.arraySize + 1) * EditorGUIUtility.singleLineHeight;
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
                var propArea = new Rect(position.xMin, position.yMin, Mathf.Max(0f, position.width - BTN_WIDTH), EditorGUIUtility.singleLineHeight);
                var btnArea = new Rect(propArea.xMax, position.yMin, Mathf.Min(BTN_WIDTH, position.width), EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(propArea, property.GetArrayElementAtIndex(0), label);
                if(GUI.Button(btnArea, _moreBtnLabel))
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
                    elementArea = new Rect(position.xMin, elementArea.yMax, position.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(elementArea, property.GetArrayElementAtIndex(i), new GUIContent("Element " + i.ToString()));
                }
                EditorGUI.indentLevel--;
            }
        }

    }

}
