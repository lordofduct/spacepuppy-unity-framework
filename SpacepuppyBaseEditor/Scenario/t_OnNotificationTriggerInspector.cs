using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Scenario;

namespace com.spacepuppyeditor.Scenario
{

    [CustomEditor(typeof(t_OnNotificationTrigger), true)]
    public class t_OnNotificationTriggerInspector : SPEditor
    {

        public const string PROP_NOTIFTYPE = "_notificationType";
        public const string PROP_USEGLOBAL = "_useGlobal";
        public const string PROP_TARGET = "_targetGameObject";


        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);


            
            var cache = SPGUI.DisableIfPlaying();

            this.DrawPropertyField(PROP_NOTIFTYPE);

            var useGlobalProp = this.serializedObject.FindProperty(PROP_USEGLOBAL);
            var targetProp = this.serializedObject.FindProperty(PROP_TARGET);

            SPEditorGUILayout.PropertyField(useGlobalProp);
            if(useGlobalProp.boolValue)
            {
                targetProp.objectReferenceValue = null;
            }
            else
            {
                SPEditorGUILayout.PropertyField(targetProp);
            }

            cache.Reset();




            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_NOTIFTYPE, PROP_USEGLOBAL, PROP_TARGET);

            this.serializedObject.ApplyModifiedProperties();
        }

    }
}
