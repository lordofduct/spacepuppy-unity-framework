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
                int lvl = EditorGUI.indentLevel;
                EditorGUI.indentLevel = lvl + 1;

                var keysProp = property.FindPropertyRelative("_keys");
                var valuesProp = property.FindPropertyRelative("_values");

                int cnt = keysProp.arraySize;
                if (valuesProp.arraySize != cnt) valuesProp.arraySize = cnt;

                for(int i = 0; i < cnt; i++)
                {
                    r = GetNextRect(ref position);
                    //r = EditorGUI.IndentedRect(r);
                    var w0 = EditorGUIUtility.labelWidth; // r.width / 2f;
                    var w1 = r.width - w0;
                    var r0 = new Rect(r.xMin, r.yMin, w0, r.height);
                    var r1 = new Rect(r0.xMax, r.yMin, w1, r.height);

                    var keyProp = keysProp.GetArrayElementAtIndex(i);
                    var valueProp = valuesProp.GetArrayElementAtIndex(i);

                    this.DrawKey(r0, keyProp);
                    this.DrawValue(r1, valueProp);
                }

                EditorGUI.indentLevel = lvl;

                r = GetNextRect(ref position);
                var pRect = new Rect(r.xMax - 60f, r.yMin, 30f, EditorGUIUtility.singleLineHeight);
                var mRect = new Rect(r.xMax - 30f, r.yMin, 30f, EditorGUIUtility.singleLineHeight);

                if(GUI.Button(pRect, "+"))
                {
                    keysProp.arraySize++;
                    SetPropertyDefault(keysProp.GetArrayElementAtIndex(keysProp.arraySize - 1));
                    valuesProp.arraySize = keysProp.arraySize;
                }
                if(GUI.Button(mRect, "-"))
                {
                    keysProp.arraySize = Mathf.Max(keysProp.arraySize - 1, 0);
                    valuesProp.arraySize = keysProp.arraySize;
                }
            }
        }

        protected virtual void DrawKey(Rect area, SerializedProperty keyProp)
        {
#if SP_LIB
            SPEditorGUI.PropertyField(area, keyProp, GUIContent.none, false);
#else
            EditorGUI.PropertyField(area, keyProp, GUIContent.none, false);
#endif
        }

        protected virtual void DrawValue(Rect area, SerializedProperty valueProp)
        {
#if SP_LIB
            SPEditorGUI.PropertyField(area, valueProp, GUIContent.none, false);
#else
            EditorGUI.PropertyField(area, valueProp, GUIContent.none, false);
#endif
        }




        private Rect GetNextRect(ref Rect position)
        {
            var r = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            var h = EditorGUIUtility.singleLineHeight + 1f;
            position = new Rect(position.xMin, position.yMin + h, position.width, position.height = h);
            return r;
        }

        private static void SetPropertyDefault(SerializedProperty prop)
        {
            if (prop == null) throw new System.ArgumentNullException("prop");

            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    prop.intValue = 0;
                    break;
                case SerializedPropertyType.Boolean:
                    prop.boolValue = false;
                    break;
                case SerializedPropertyType.Float:
                    prop.floatValue = 0f;
                    break;
                case SerializedPropertyType.String:
                    prop.stringValue = string.Empty;
                    break;
                case SerializedPropertyType.Color:
                    prop.colorValue = Color.black;
                    break;
                case SerializedPropertyType.ObjectReference:
                    prop.objectReferenceValue = null;
                    break;
                case SerializedPropertyType.LayerMask:
                    prop.intValue = -1;
                    break;
                case SerializedPropertyType.Enum:
                    prop.enumValueIndex = 0;
                    break;
                case SerializedPropertyType.Vector2:
                    prop.vector2Value = Vector2.zero;
                    break;
                case SerializedPropertyType.Vector3:
                    prop.vector3Value = Vector3.zero;
                    break;
                case SerializedPropertyType.Vector4:
                    prop.vector4Value = Vector4.zero;
                    break;
                case SerializedPropertyType.Rect:
                    prop.rectValue = Rect.zero;
                    break;
                case SerializedPropertyType.ArraySize:
                    prop.arraySize = 0;
                    break;
                case SerializedPropertyType.Character:
                    prop.intValue = 0;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    prop.animationCurveValue = null;
                    break;
                case SerializedPropertyType.Bounds:
                    prop.boundsValue = default(Bounds);
                    break;
                case SerializedPropertyType.Gradient:
                    throw new System.InvalidOperationException("Can not handle Gradient types.");
            }
        }

    }
}
