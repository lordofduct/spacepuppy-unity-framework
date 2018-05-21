using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomEditor(typeof(GameLoopEntry))]
    public class GameLoopEntryInspector : SingletonInspector
    {

        protected override void OnSPInspectorGUI()
        {
            base.OnSPInspectorGUI();
            
            if(Application.isPlaying)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Runtime Info", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Running UpdatePump Hooks", GameLoopEntry.UpdatePump.Count.ToString());
                EditorGUILayout.LabelField("Running LateUpdatePump Hooks", GameLoopEntry.LateUpdatePump.Count.ToString());
                EditorGUILayout.LabelField("Running FixedUpdatePump Hooks", GameLoopEntry.FixedUpdatePump.Count.ToString());
            }
        }

        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying || base.RequiresConstantRepaint();
        }

    }

}
