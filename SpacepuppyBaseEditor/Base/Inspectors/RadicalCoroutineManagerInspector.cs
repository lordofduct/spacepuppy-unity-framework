using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base.Inspectors
{

    [CustomEditor(typeof(RadicalCoroutineManager))]
    public class RadicalCoroutineManagerInspector : SPEditor
    {

        private bool _expanded;

        protected override void OnSPInspectorGUI()
        {
            var targ = this.target as RadicalCoroutineManager;
            if(targ == null) return;

            var infos = targ.GetCoroutineInfo().ToArray();
            EditorGUILayout.HelpBox(string.Format("Managing '{0}' RadicalCoroutines.", infos.Length), MessageType.Info);

            _expanded = EditorGUILayout.Foldout(_expanded, EditorHelper.TempContent("Coroutine Breakdown"));
            if(_expanded)
            {
                EditorGUI.indentLevel++;
                for(int i = 0; i < infos.Length; i++)
                {
                    EditorGUILayout.LabelField(infos[i].Component.GetType().Name);
                }
                EditorGUI.indentLevel--;
            }
        }

    }

}
