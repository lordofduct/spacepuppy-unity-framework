using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Scenario
{

    [CustomEditor(typeof(i_PlayAudio), true)]
    public class i_PlayAudioInspector : SPEditor
    {

        public const string PROP_TARGET = "_targetAudioSource";
        public const string PROP_SETTINGSMASK = "_settingsMask";
        public const string PROP_SETTINGS = "_settings";
        public const string PROP_INTERRUPT = "_interrupt";
        public const string PROP_DELAY = "_delay";

        private static string[] PROPS_ANIMSETTINGS = new string[] { "clip", "volume", "loop" };

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);
            this.DrawPropertyField(EditorHelper.PROP_ORDER);
            this.DrawPropertyField(EditorHelper.PROP_ACTIVATEON);
            this.DrawPropertyField(PROP_TARGET);
            this.DrawAnimSettings();
            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, EditorHelper.PROP_ORDER, EditorHelper.PROP_ACTIVATEON, PROP_TARGET, PROP_SETTINGSMASK, PROP_SETTINGS);

            this.serializedObject.ApplyModifiedProperties();
        }


        private void DrawAnimSettings()
        {
            var propMask = this.serializedObject.FindProperty(PROP_SETTINGSMASK);
            var propSettings = this.serializedObject.FindProperty(PROP_SETTINGS);

            int mask = propMask.intValue;
            propSettings.isExpanded = EditorGUILayout.Foldout(propSettings.isExpanded, mask != 0 ? "Custom Settings : Active" : "Custom Settings");

            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < PROPS_ANIMSETTINGS.Length; i++)
            {
                int m = 1 << i;
                bool active = (mask & m) != 0;
                if (!propSettings.isExpanded && !active) continue;

                var propSet = propSettings.FindPropertyRelative(PROPS_ANIMSETTINGS[i]);
                EditorGUILayout.BeginHorizontal();
                if (EditorGUILayout.Toggle(active, GUILayout.MaxWidth(20f)))
                {
                    mask |= m;
                    EditorGUILayout.PropertyField(propSet);
                }
                else
                {
                    mask &= ~m;
                    EditorGUILayout.PrefixLabel(EditorHelper.TempContent(propSet.displayName, propSet.tooltip));
                }

                EditorGUILayout.EndHorizontal();
            }
            if (EditorGUI.EndChangeCheck())
                propMask.intValue = mask;
        }

    }

}
