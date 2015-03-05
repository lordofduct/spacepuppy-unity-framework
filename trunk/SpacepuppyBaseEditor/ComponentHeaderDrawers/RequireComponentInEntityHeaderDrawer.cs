using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor.ComponentHeaderDrawers
{

    [CustomPropertyDrawer(typeof(RequireComponentInEntityAttribute))]
    public class RequireComponentInEntityHeaderDrawer : ComponentHeaderDrawer
    {

        private const string MSG_FRM = "Component of type '{0}' requires a '{1}' component to be attached somewhere in the entity.";

        public override float GetHeight(SerializedObject serializedObject)
        {
            var attrib = this.Attribute as RequireComponentInEntityAttribute;
            System.Type missingComponentType;
            if (attrib == null || this.Validate(serializedObject, out missingComponentType))
            {
                return 0f;
            }
            else
            {
                GUIStyle style = GUI.skin.GetStyle("HelpBox");
                return Mathf.Max(40f, style.CalcHeight(EditorHelper.TempContent(string.Format(MSG_FRM, this.ComponentType.Name, missingComponentType.Name)), EditorGUIUtility.currentViewWidth));
            }
        }

        public override void OnGUI(Rect position, SerializedObject serializedObject)
        {
            var attrib = this.Attribute as RequireComponentInEntityAttribute;
            System.Type missingComponentType;
            if (attrib != null && !this.Validate(serializedObject, out missingComponentType))
            {
                EditorGUI.HelpBox(position, string.Format(MSG_FRM, this.ComponentType.Name, missingComponentType.Name), MessageType.Error);
            }
        }

        private bool Validate(SerializedObject serializedObject, out System.Type missingComponentType)
        {
            var c = serializedObject.targetObject as Component;
            missingComponentType = null;
            if (c == null) return true;
            return !com.spacepuppy.Utils.Assertions.AssertRequireComponentInEntityAttrib(c, out missingComponentType, true);
        }

    }

}
