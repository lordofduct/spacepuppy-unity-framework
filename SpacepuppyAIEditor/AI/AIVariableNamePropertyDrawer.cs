using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.AI;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.AI
{

    [CustomPropertyDrawer(typeof(AIVariableNameAttribute))]
    public class AIVariableNamePropertyDrawer : PropertyDrawer
    {


        #region Methods

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(property.propertyType != SerializedPropertyType.String)
            {
                SPEditorGUI.DefaultPropertyField(position, property, label);
                return;
            }

            var tr = GameObjectUtil.GetTransformFromSource(property.serializedObject.targetObject);
            if (tr == null)
            {
                SPEditorGUI.DefaultPropertyField(position, property, label);
                return;
            }

            IAIController controller = tr.GetComponentInParent<IAIController>() ?? tr.FindComponent<IAIController>();
            if(controller == null)
            {
                SPEditorGUI.DefaultPropertyField(position, property, label);
                return;
            }

            var names = (controller.Variables == null) ? ArrayUtil.Empty<string>() : controller.Variables.Names.ToArray();
            /*
            var guiNames = (from n in names select EditorHelper.TempContent(n)).Append(EditorHelper.TempContent("Custom...")).ToArray();
            int index = names.IndexOf(property.stringValue);
            if (index < 0) index = names.Length;
            if(index == names.Length)
            {
                var fw = position.width - EditorGUIUtility.labelWidth;
                var wl = EditorGUIUtility.labelWidth + (fw / 4f);
                var wr = position.width - wl - 1f;

                var rl = new Rect(position.xMin, position.yMin, wl, EditorGUIUtility.singleLineHeight);
                var rr = new Rect(rl.xMax + 1f, rl.yMin, wr, EditorGUIUtility.singleLineHeight);

                index = EditorGUI.Popup(rl, label, index, guiNames);
                if (index >= 0 && index < names.Length)
                {
                    property.stringValue = names[index];
                }
                else
                {
                    property.stringValue = EditorGUI.TextField(rr, property.stringValue);
                }
            }
            else
            {
                index = EditorGUI.Popup(position, label, index, guiNames);
                property.stringValue = (index >= 0 && index < names.Length) ? names[index] : null;
            }
            */
            property.stringValue = SPEditorGUI.OptionPopupWithCustom(position, label, property.stringValue, names);
        }

        #endregion

    }
}
