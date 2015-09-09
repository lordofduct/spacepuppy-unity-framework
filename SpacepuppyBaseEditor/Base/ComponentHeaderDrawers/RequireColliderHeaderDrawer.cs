using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(RequireColliderAttribute))]
    public class RequireColliderHeaderDrawer : ComponentHeaderDrawer
    {
        private const string MSG_FRM = "Component of type '{0}' requires a collider or rigidbody to be attached.";

        public override float GetHeight(SerializedObject serializedObject)
        {
            var attrib = this.Attribute as RequireColliderAttribute;
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
            var attrib = this.Attribute as ComponentHeaderAttribute;
            if (attrib != null && !this.Validate(serializedObject))
            {
                EditorGUI.HelpBox(position, string.Format(MSG_FRM, this.ComponentType.Name), MessageType.Error);
            }
        }

        private bool Validate(SerializedObject serializedObject)
        {
            var go = GameObjectUtil.GetGameObjectFromSource(serializedObject.targetObject);
            if(go == null) return true;

            return (go.HasComponent<Rigidbody>() || go.HasComponent<Collider>() || go.HasComponent<Rigidbody2D>() || go.HasComponent<Collider2D>());
        }
    }

}
