#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(EulerRotationInspectorAttribute))]
    public class EulerRotationInspectorPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginProperty(position, label, property);

            var attr = this.attribute as EulerRotationInspectorAttribute;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Quaternion:
                    property.quaternionValue = SPEditorGUI.QuaternionField(position, label, property.quaternionValue, attr.UseRadians);
                    break;
                case SerializedPropertyType.Vector3:
                    //3d rotation
                    var v = property.vector3Value;
                    v.x = MathUtil.NormalizeAngle(v.x, attr.UseRadians);
                    v.y = MathUtil.NormalizeAngle(v.y, attr.UseRadians);
                    v.z = MathUtil.NormalizeAngle(v.z, attr.UseRadians);

                    v = EditorGUI.Vector3Field(position, label, v);

                    v.x = MathUtil.NormalizeAngle(v.x, attr.UseRadians);
                    v.y = MathUtil.NormalizeAngle(v.y, attr.UseRadians);
                    v.z = MathUtil.NormalizeAngle(v.z, attr.UseRadians);

                    property.vector3Value = v;
                    break;
                case SerializedPropertyType.Float:
                    //2d rotation
                    float a = MathUtil.NormalizeAngle(property.floatValue, attr.UseRadians);

                    a = EditorGUI.FloatField(position, label, a);

                    a = MathUtil.NormalizeAngle(a, attr.UseRadians);

                    property.floatValue = a;

                    //property.floatValue = FloatAngle(new Rect(0, 0, 30, 30), property.floatValue);
                    break;
            }

            EditorGUI.EndProperty();
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
        }




        //private static Texture KnobBack;
        private static Texture Knob;
        private static Vector2 mousePosition;

        public static float FloatAngle(Rect rect, float value)
        {
            return FloatAngle(rect, value, -1, -1, -1);
        }

        public static float FloatAngle(Rect rect, float value, float snap)
        {
            return FloatAngle(rect, value, snap, -1, -1);
        }

        public static float FloatAngle(Rect rect, float value, float snap, float min, float max)
        {
            if (Knob == null)
            {
                Knob = EditorGUIUtility.Load("../Assets/Knob.png") as Texture;
            }


            int id = GUIUtility.GetControlID(FocusType.Passive, rect);

            Rect knobRect = new Rect(rect.x, rect.y, rect.height, rect.height);

            float delta;
            if (min != max)
                delta = ((max - min) / 360);
            else
                delta = 1;

            if (Event.current != null)
            {
                if (Event.current.type == EventType.MouseDown && knobRect.Contains(Event.current.mousePosition))
                {
                    GUIUtility.hotControl = id;
                    mousePosition = Event.current.mousePosition;
                }
                else if (Event.current.type == EventType.MouseUp && GUIUtility.hotControl == id)
                    GUIUtility.hotControl = 0;
                else if (Event.current.type == EventType.MouseDrag && GUIUtility.hotControl == id)
                {
                    Vector2 move = mousePosition - Event.current.mousePosition;
                    value += delta * (-move.x - move.y);

                    if (snap > 0)
                    {
                        float mod = value % snap;

                        if (mod < (delta * 3) || Mathf.Abs(mod - snap) < (delta * 3))
                            value = Mathf.Round(value / snap) * snap;
                    }

                    mousePosition = Event.current.mousePosition;
                    GUI.changed = true;
                }
            }

            //GUI.DrawTexture(knobRect, KnobBack);
            Matrix4x4 matrix = GUI.matrix;

            if (min != max)
                GUIUtility.RotateAroundPivot(value * (360 / (max - min)), knobRect.center);
            else
                GUIUtility.RotateAroundPivot(value, knobRect.center);

            GUI.DrawTexture(knobRect, Knob);
            GUI.matrix = matrix;

            Rect label = new Rect(rect.x + rect.height, rect.y + (rect.height / 2) - 9, rect.height, 18);
            value = EditorGUI.FloatField(label, value);

            if (min != max)
                value = Mathf.Clamp(value, min, max);

            return value;
        }

    }

}