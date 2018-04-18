using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(Constraint))]
    public class ConstraintPropertyDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
                return EditorGUIUtility.singleLineHeight * 3f;
            else
                return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(property.isExpanded)
            {
                var r1 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
                var r2 = new Rect(position.xMin, r1.yMax, position.width, EditorGUIUtility.singleLineHeight);
                var r3 = new Rect(position.xMin, r2.yMax, position.width, EditorGUIUtility.singleLineHeight);

                property.isExpanded = EditorGUI.Foldout(r1, property.isExpanded, label);

                EditorGUI.indentLevel++;

                const float TOGGLE_WIDTH = 30f;
                var e = property.GetEnumValue<Constraint>();
                bool b;

                //POSITION
                r2 = EditorGUI.PrefixLabel(r2, EditorHelper.TempContent("Position"));

                b = (e & Constraint.XPosition) != 0;
                EditorGUI.BeginChangeCheck();
                b = GUI.Toggle(new Rect(r2.xMin, r2.yMin, TOGGLE_WIDTH, r2.height), b, "X");
                if (EditorGUI.EndChangeCheck())
                {
                    if (b)
                        e |= Constraint.XPosition;
                    else
                        e &= ~Constraint.XPosition;
                }

                b = (e & Constraint.YPosition) != 0;
                EditorGUI.BeginChangeCheck();
                b = GUI.Toggle(new Rect(r2.xMin + TOGGLE_WIDTH, r2.yMin, TOGGLE_WIDTH, r2.height), b, "Y");
                if (EditorGUI.EndChangeCheck())
                {
                    if (b)
                        e |= Constraint.YPosition;
                    else
                        e &= ~Constraint.YPosition;
                }

                b = (e & Constraint.ZPosition) != 0;
                EditorGUI.BeginChangeCheck();
                b = GUI.Toggle(new Rect(r2.xMin + TOGGLE_WIDTH + TOGGLE_WIDTH, r2.yMin, TOGGLE_WIDTH, r2.height), b, "Z");
                if (EditorGUI.EndChangeCheck())
                {
                    if (b)
                        e |= Constraint.ZPosition;
                    else
                        e &= ~Constraint.ZPosition;
                }


                //ROTATION
                r3 = EditorGUI.PrefixLabel(r3, EditorHelper.TempContent("Rotation"));

                b = (e & Constraint.XRotation) != 0;
                EditorGUI.BeginChangeCheck();
                b = GUI.Toggle(new Rect(r3.xMin, r3.yMin, TOGGLE_WIDTH, r3.height), b, "X");
                if (EditorGUI.EndChangeCheck())
                {
                    if (b)
                        e |= Constraint.XRotation;
                    else
                        e &= ~Constraint.XRotation;
                }

                b = (e & Constraint.YRotation) != 0;
                EditorGUI.BeginChangeCheck();
                b = GUI.Toggle(new Rect(r3.xMin + TOGGLE_WIDTH, r3.yMin, TOGGLE_WIDTH, r3.height), b, "Y");
                if (EditorGUI.EndChangeCheck())
                {
                    if (b)
                        e |= Constraint.YRotation;
                    else
                        e &= ~Constraint.YRotation;
                }

                b = (e & Constraint.ZRotation) != 0;
                EditorGUI.BeginChangeCheck();
                b = GUI.Toggle(new Rect(r3.xMin + TOGGLE_WIDTH + TOGGLE_WIDTH, r3.yMin, TOGGLE_WIDTH, r3.height), b, "Z");
                if (EditorGUI.EndChangeCheck())
                {
                    if (b)
                        e |= Constraint.ZRotation;
                    else
                        e &= ~Constraint.ZRotation;
                }


                property.SetEnumValue<Constraint>(e & Constraint.All);

                EditorGUI.indentLevel--;
            }
            else
            {
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
            }
        }

    }
}
