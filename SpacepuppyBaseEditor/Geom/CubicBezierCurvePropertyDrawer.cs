using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Geom;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Geom
{

    [CustomPropertyDrawer(typeof(CubicBezierCurve))]
    public class CubicBezierCurvePropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            var curve = EditorHelper.GetTargetObjectOfProperty(property) as CubicBezierCurve;
            if (curve == null) return;

            DoCurveField(position, curve, Color.green, property);
        }



        internal static void DoCurveField(Rect position, CubicBezierCurve curve, Color color, SerializedProperty property)
        {
            int id = GUIUtility.GetControlID(EditorGUIUtility.native, position);
            Event current = Event.current;
            position.width = Mathf.Max(position.width, 2f);
            position.height = Mathf.Max(position.height, 2f);
            
            //TODO - update open curve editor window

            EventType typeForControl = current.GetTypeForControl(id);
            switch(typeForControl)
            {
                case EventType.KeyDown:

                    break;
                case EventType.Repaint:
                    Rect position1 = position;
                    ++position1.y;
                    --position1.height;
                    SPEditorGUI.DrawCurveSwatch(position1, curve, color, Color.gray);
                    //EditorStyles.colorPickerBox.Draw(position1, GUIContent.none, id, false);
                    break;
                case EventType.mouseDown:
                    if(position.Contains(current.mousePosition))
                    {
                        //TODO - show CubicBezierCurveEditorWindow

                        current.Use();
                        GUIUtility.ExitGUI();
                    }
                    break;
            }
        }

    }

}
