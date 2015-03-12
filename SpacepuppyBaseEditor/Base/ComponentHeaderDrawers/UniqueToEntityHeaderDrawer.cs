using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(UniqueToEntityAttribute))]
    public class UniqueToEntityHeaderDrawer : ComponentHeaderDrawer
    {

        private const string MSG_FRM = "Component of type '{0}' must be unique to the entity.";

        public override float GetHeight(SerializedObject serializedObject)
        {
            var attrib = this.Attribute as UniqueToEntityAttribute;
            if (attrib == null || this.Validate(serializedObject))
            {
                return 0f;
            }
            else
            {
                GUIStyle style = GUI.skin.GetStyle("HelpBox");
                return Mathf.Max(40f, style.CalcHeight(EditorHelper.TempContent(string.Format(MSG_FRM, this.ComponentType.Name)), EditorGUIUtility.currentViewWidth));
            }
        }

        public override void OnGUI(Rect position, SerializedObject serializedObject)
        {
            var attrib = this.Attribute as UniqueToEntityAttribute;
            if (attrib != null && !this.Validate(serializedObject))
            {
                EditorGUI.HelpBox(position, string.Format(MSG_FRM, this.ComponentType.Name), MessageType.Error);
            }
        }

        private bool Validate(SerializedObject serializedObject)
        {
            var c = serializedObject.targetObject as Component;
            if (c == null) return true;
            return !com.spacepuppy.Utils.Assertions.AssertUniqueToEntityAttrib(c, true);
        }

    }

}
