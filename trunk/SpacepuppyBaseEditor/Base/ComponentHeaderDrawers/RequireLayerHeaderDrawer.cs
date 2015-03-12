using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor.Base
{

    [CustomPropertyDrawer(typeof(RequireLayerAttribute))]
    public class RequireLayerHeaderDrawer : ComponentHeaderDrawer
    {

        public override float GetHeight(SerializedObject serializedObject)
        {
            var attrib = this.Attribute as RequireLayerAttribute;
            if(attrib == null) return 0f;

            GUIStyle style = GUI.skin.GetStyle("HelpBox");
            return Mathf.Max(40f, style.CalcHeight(EditorHelper.TempContent("This component requires the current layer to be set to '" + LayerMask.LayerToName(attrib.Layer) + "'."), EditorGUIUtility.currentViewWidth));
        }

        public override void OnGUI(Rect position, SerializedObject serializedObject)
        {
            var attrib = this.Attribute as RequireLayerAttribute;

            if (attrib != null)
            {
                var go = (serializedObject.targetObject as Component).gameObject;
                if(go.layer != attrib.Layer)
                {
                    go.layer = attrib.Layer;
                    EditorUtility.SetDirty(go);
                }
            }

            EditorGUI.HelpBox(position, "This component requires the current layer to be set to '" + LayerMask.LayerToName(attrib.Layer) + "'.", MessageType.Info);
        }

    }
}
