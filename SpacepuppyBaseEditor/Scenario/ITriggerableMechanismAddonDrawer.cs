using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Scenario;

namespace com.spacepuppyeditor.Scenario
{
    [CustomAddonDrawer(typeof(ITriggerableMechanism), displayAsFooter = true)]
    public class ITriggerableMechanismAddonDrawer : SPEditorAddonDrawer
    {

        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying) return;

            var targ = this.SerializedObject.targetObject as ITriggerableMechanism;
            if (targ == null) return;

            var rect = GetActivateButtonControlRect();

            if(GUI.Button(rect, "Activate"))
            {
                targ.Trigger(null);
            }
        }


        public static Rect GetActivateButtonControlRect()
        {
            var rect = EditorGUILayout.GetControlRect(false, 24f);
            var w = rect.width * 0.6f;
            var pad = (rect.width - w) / 2f;
            rect = new Rect(rect.xMin + pad, rect.yMin + 2f, w, 20f);
            return rect;
        }

    }

}
