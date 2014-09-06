using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Inspectors.Scenario
{

    [CustomEditor(typeof(Trigger), true)]
    public class TriggerInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            this.DrawDefaultInspector(EditorHelper.PROP_SCRIPT);

            this.DrawOnlyTriggerBasics();

            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, "Triggerable", "TriggerableArg", "TriggerAllOnTarget");
        }

        public void DrawOnlyTriggerBasics()
        {
            var targ = this.target as Trigger;
            if(targ.HasComponent<MultiTargetTrigger>())
            {
                //EditorGUILayout.LabelField("This trigger is linked to a multi-target trigger.");
                EditorGUILayout.HelpBox("This trigger is linked to a multi-target trigger.", MessageType.Info);
                targ.Triggerable = null;
                targ.TriggerableArg = null;
            }
            else
            {
                this.DrawDefaultInspector("Triggerable");
                this.DrawDefaultInspector("TriggerableArg");
                this.DrawDefaultInspector("TriggerAllOnTarget");

                if (targ.Triggerable == null) targ.Triggerable = targ.GetFirstLikeComponent<ITriggerableMechanism>() as Component;
            }
        }

    }
}
