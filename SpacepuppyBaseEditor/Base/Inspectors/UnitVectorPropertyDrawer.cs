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
            float w = (position.width - btnw) / 2f;
            float h = position.height;
            float y = position.yMin;

            var r1 = new Rect(position.xMin, y, w, h);
            var r2 = new Rect(r1.xMax, y, w, h);
            var rbtn = new Rect(r2.xMax, y, btnw, h);

            var v = property.vector2Value;
            EditorGUI.LabelField(r1, "X: " + v.x.ToString("0.#######"));
            EditorGUI.LabelField(r2, "Y: " + v.y.ToString("0.#######"));
            int i = EditorGUI.Popup(rbtn, -1, new string[] {"Up",
                                                            "Down",
                                                            "Right",
                                                            "Left",
                                                            "Configure"
                                                            });
            switch(i)
            {
                case 0:
                    property.vector2Value = Vector2.up;
                    break;
                case 1:
                    property.vector2Value = -Vector2.up;
                    break;
                case 2:
                    property.vector2Value = Vector2.right;
                    break;
                case 3:
                    property.vector2Value = -Vector2.right;
                    break;
                case 4:
                    //TODO
                    break;
            }
        }

        private void DrawVector3(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            float btnw = Mathf.Min(position.width, BTN_WIDTH);
            float w = (position.width - btnw) / 3f;
            float h = position.height;
            float y = position.yMin;

            var r1 = new Rect(position.xMin, y, w, h);
            var r2 = new Rect(r1.xMax, y, w, h);
            var r3 = new Rect(r2.xMax, y, w, h);
            var rbtn = new Rect(r3.xMax, y, btnw, h);

            var v = property.vector3Value;
            EditorGUI.LabelField(r1, "X: " + v.x.ToString("0.#######"));
            EditorGUI.LabelField(r2, "Y: " + v.y.ToString("0.#######"));
            EditorGUI.LabelField(r3, "Z: " + v.z.ToString("0.#######"));
            int i = EditorGUI.Popup(rbtn, -1, new string[] {"Up",
                                                            "Down",
                                                            "Right",
                                                            "Left",
                                                            "Forward",
                                                            "Backward",
                                                            "Configure"
                                                            });
            switch (i)
            {
                case 0:
                    property.vector3Value = Vector3.up;
                    break;
                case 1:
                    property.vector3Value = Vector3.down;
                    break;
                case 2:
                    property.vector3Value = Vector3.right;
                    break;
                case 3:
                    property.vector3Value = Vector3.left;
                    break;
                case 4:
                    property.vector3Value = Vector3.forward;
                    break;
                case 5:
                    property.vector3Value = Vector3.back;
                    break;
                case 6:
                    //TODO - configure
                    break;
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
