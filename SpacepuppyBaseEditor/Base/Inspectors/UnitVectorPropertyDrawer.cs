using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(UnitVectorAttribute))]
    public class UnitVectorPropertyDrawer : PropertyDrawer
    {

        public const float BTN_WIDTH = 100f;

        private enum Vector2Directions
        {
            Up,
            Down,
            Right,
            Left,
            Configure
        }

        private enum Vector3Directions
        {
            Up,
            Down,
            Right,
            Left,
            Forward,
            Backward,
            Configure
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch(property.propertyType)
            {
                case SerializedPropertyType.Vector2:
                    this.DrawVector2(position, property, label);
                    break;
                case SerializedPropertyType.Vector3:
                    this.DrawVector3(position, property, label);
                    break;
                case SerializedPropertyType.Vector4:
                    this.DrawVector4(position, property, label);
                    break;
                default:
                    SPEditorGUI.DefaultPropertyField(position, property, label);
                    break;
            }
        }



        private void DrawVector2(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            float btnw = Mathf.Min(position.width, BTN_WIDTH);
            var rbtn = new Rect(position.xMax - btnw, position.yMin, btnw, EditorGUIUtility.singleLineHeight);

            var v = property.vector2Value;
            if (v == Vector2.zero)
            {
                v = Vector2.right;
                property.vector2Value = Vector2.right;
            }

            Vector2Directions i = Vector2Directions.Configure;
            if (v == Vector2.up)
                i = Vector2Directions.Up;
            else if (v == Vector2.down)
                i = Vector2Directions.Down;
            else if (v == Vector2.right)
                i = Vector2Directions.Right;
            else if (v == Vector2.left)
                i = Vector2Directions.Left;
            else
                i = Vector2Directions.Configure;

            EditorGUI.BeginChangeCheck();
            i = (Vector2Directions)EditorGUI.EnumPopup(rbtn, i);
            if (EditorGUI.EndChangeCheck())
            {
                switch (i)
                {
                    case Vector2Directions.Up:
                        property.vector2Value = Vector2.up;
                        break;
                    case Vector2Directions.Down:
                        property.vector2Value = Vector2.down;
                        break;
                    case Vector2Directions.Right:
                        property.vector2Value = Vector2.right;
                        break;
                    case Vector2Directions.Left:
                        property.vector2Value = Vector2.left;
                        break;
                }
            }

            if (i < Vector2Directions.Configure)
            {
                float w = (position.width - btnw) / 2f;
                var r1 = new Rect(position.xMin, position.yMin, w, EditorGUIUtility.singleLineHeight);
                var r2 = new Rect(r1.xMax, position.yMin, w, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(r1, "X: " + v.x.ToString("0.#######"));
                EditorGUI.LabelField(r2, "Y: " + v.y.ToString("0.#######"));
            }
            else
            {
                //TODO - need way to make this work effectively
                var r = new Rect(position.xMin, position.yMin, position.width - btnw, EditorGUIUtility.singleLineHeight);
                EditorGUI.BeginChangeCheck();
                v = EditorGUI.Vector2Field(r, GUIContent.none, v);
                if (EditorGUI.EndChangeCheck())
                {
                    property.vector2Value = v.normalized;
                }
            }
        }

        private void DrawVector3(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            float btnw = Mathf.Min(position.width, BTN_WIDTH);
            var rbtn = new Rect(position.xMax - btnw, position.yMin, btnw, EditorGUIUtility.singleLineHeight);

            var v = property.vector3Value;
            if (v == Vector3.zero)
            {
                v = Vector3.forward;
                property.vector3Value = Vector3.forward;
            }

            Vector3Directions i = Vector3Directions.Configure;
            if (v == Vector3.up)
                i = Vector3Directions.Up;
            else if (v == Vector3.down)
                i = Vector3Directions.Down;
            else if (v == Vector3.right)
                i = Vector3Directions.Right;
            else if (v == Vector3.left)
                i = Vector3Directions.Left;
            else if (v == Vector3.forward)
                i = Vector3Directions.Forward;
            else if (v == Vector3.back)
                i = Vector3Directions.Backward;
            else
                i = Vector3Directions.Configure;

            EditorGUI.BeginChangeCheck();
            i = (Vector3Directions)EditorGUI.EnumPopup(rbtn, i);
            if(EditorGUI.EndChangeCheck())
            {
                switch (i)
                {
                    case Vector3Directions.Up:
                        property.vector3Value = Vector3.up;
                        break;
                    case Vector3Directions.Down:
                        property.vector3Value = Vector3.down;
                        break;
                    case Vector3Directions.Right:
                        property.vector3Value = Vector3.right;
                        break;
                    case Vector3Directions.Left:
                        property.vector3Value = Vector3.left;
                        break;
                    case Vector3Directions.Forward:
                        property.vector3Value = Vector3.forward;
                        break;
                    case Vector3Directions.Backward:
                        property.vector3Value = Vector3.back;
                        break;
                }
            }

            if (i < Vector3Directions.Configure)
            {
                float w = (position.width - btnw) / 3f;
                var r1 = new Rect(position.xMin, position.yMin, w, EditorGUIUtility.singleLineHeight);
                var r2 = new Rect(r1.xMax, position.yMin, w, EditorGUIUtility.singleLineHeight);
                var r3 = new Rect(r2.xMax, position.yMin, w, EditorGUIUtility.singleLineHeight);

                EditorGUI.LabelField(r1, "X: " + v.x.ToString("0.#######"));
                EditorGUI.LabelField(r2, "Y: " + v.y.ToString("0.#######"));
                EditorGUI.LabelField(r3, "Z: " + v.z.ToString("0.#######"));
            }
            else
            {
                //TODO - need way to make this work effectively
                var r = new Rect(position.xMin, position.yMin, position.width - btnw, EditorGUIUtility.singleLineHeight);
                EditorGUI.BeginChangeCheck();
                v = SPEditorGUI.DelayedVector3Field(r, GUIContent.none, v);
                if(EditorGUI.EndChangeCheck())
                {
                    property.vector3Value = v.normalized;
                }
            }
        }

        private void DrawVector4(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            float btnw = Mathf.Min(position.width, BTN_WIDTH);
            float w = (position.width - btnw) / 4f;
            float h = position.height;
            float y = position.yMin;

            var r1 = new Rect(position.xMin, y, w, h);
            var r2 = new Rect(r1.xMax, y, w, h);
            var r3 = new Rect(r2.xMax, y, w, h);
            var r4 = new Rect(r3.xMax, y, w, h);
            var rbtn = new Rect(r4.xMax, y, btnw, h);

            var v = property.vector4Value;
            EditorGUI.LabelField(r1, "X: " + v.x.ToString("0.#######"));
            EditorGUI.LabelField(r2, "Y: " + v.y.ToString("0.#######"));
            EditorGUI.LabelField(r3, "Z: " + v.z.ToString("0.#######"));
            EditorGUI.LabelField(r4, "W: " + v.w.ToString("0.#######"));
            
            if(GUI.Button(rbtn, "Configure"))
            {
                //TODO
            }
        }

    }

}
