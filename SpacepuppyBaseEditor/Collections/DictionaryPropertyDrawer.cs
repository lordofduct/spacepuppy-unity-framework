using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppyeditor.Collections
{

    [CustomPropertyDrawer(typeof(DrawableDictionary), true)]
    public class DictionaryPropertyDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if(property.isExpanded)
            {
                var keysProp = property.FindPropertyRelative("_keys");
                return (keysProp.arraySize + 4) * EditorGUIUtility.singleLineHeight;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool expanded = property.isExpanded;
            var r = GetNextRect(ref position);
            property.isExpanded = EditorGUI.Foldout(r, property.isExpanded, label);

            if (expanded)
            {
                EditorGUI.indentLevel++;

                var keysProp = property.FindPropertyRelative("_keys");
                var valuesProp = property.FindPropertyRelative("_values");

                int cnt = keysProp.arraySize;
                if (valuesProp.arraySize != cnt) valuesProp.arraySize = cnt;

                for(int i = 0; i < cnt; i++)
                {
                    r = GetNextRect(ref position);
                    r = EditorGUI.IndentedRect(r);
                    var w = r.width / 2f;
                    var r0 = new Rect(r.xMin, r.yMin, w, r.height);
                    var r1 = new Rect(r0.xMax, r.yMin, w, r.height);

                    var keyProp = keysProp.GetArrayElementAtIndex(i);
                    var valueProp = valuesProp.GetArrayElementAtIndex(i);
                    EditorGUI.PropertyField(r0, keyProp, GUIContent.none, false);
                    EditorGUI.PropertyField(r1, valueProp, GUIContent.none, false);
                }

                r = GetNextRect(ref position);
                var pRect = new Rect(r.xMax - 60f, r.yMin, 30f, EditorGUIUtility.singleLineHeight);
                var mRect = new Rect(r.xMax - 30f, r.yMin, 30f, EditorGUIUtility.singleLineHeight);

                if(GUI.Button(pRect, "+"))
                {
                    keysProp.arraySize++;
                    EditorHelper.SetPropertyValue(keysProp.GetArrayElementAtIndex(keysProp.arraySize - 1), null);
                    valuesProp.arraySize = keysProp.arraySize;
                }
                if(GUI.Button(mRect, "-"))
                {
                    keysProp.arraySize = Mathf.Max(keysProp.arraySize - 1, 0);
                    valuesProp.arraySize = keysProp.arraySize;
                }
            }
        }


        private Rect GetNextRect(ref Rect position)
        {
            var r = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            var h = EditorGUIUtility.singleLineHeight + 1f;
            position = new Rect(position.xMin, position.yMin + h, position.width, position.height = h);
            return r;
        }

    }
}
