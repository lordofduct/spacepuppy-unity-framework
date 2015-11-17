using UnityEngine;
using UnityEditor;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor.Base.Inspectors
{

#if SP_LIB

    [CustomEditor(typeof(RadicalCoroutineManager))]
    public class RadicalCoroutineManagerInspector : SPEditor
    {

        private bool _expanded = true;

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
                    var info = infos[i];
                    EditorGUILayout.LabelField(string.Format("[{0:00}] Routine {1}", i, RadicalCoroutine.EditorHelper.GetInternalRoutineID(info.Routine)));
                    EditorGUI.indentLevel += 2;
                    EditorGUILayout.LabelField("Component:", info.Component.GetType().Name);
                    EditorGUILayout.LabelField("State:", info.Routine.OperatingState.ToString());
                    EditorGUILayout.LabelField("Yield:", RadicalCoroutine.EditorHelper.GetYieldID(info.Routine));
                    EditorGUILayout.LabelField("Derivative:", RadicalCoroutine.EditorHelper.GetDerivativeID(info.Routine));
                    EditorGUI.indentLevel -= 2;
                }
                EditorGUI.indentLevel--;
            }
        }

        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying && _expanded;
        }

    }

#else

    [CustomEditor(typeof(RadicalCoroutineManager))]
    public class RadicalCoroutineManagerInspector : Editor
    {

        private bool _expanded = true;

        public override void OnInspectorGUI()
        {
            var targ = this.target as RadicalCoroutineManager;
            if (targ == null) return;

            var infos = targ.GetCoroutineInfo().ToArray();
            EditorGUILayout.HelpBox(string.Format("Managing '{0}' RadicalCoroutines.", infos.Length), MessageType.Info);

            _expanded = EditorGUILayout.Foldout(_expanded, "Coroutine Breakdown");
            if (_expanded)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < infos.Length; i++)
                {
                    var info = infos[i];
                    EditorGUILayout.LabelField(string.Format("[{0:00}] Routine {1}", i, RadicalCoroutine.EditorHelper.GetInternalRoutineID(info.Routine)));
                    EditorGUI.indentLevel += 2;
                    EditorGUILayout.LabelField("Component:", info.Component.GetType().Name);
                    EditorGUILayout.LabelField("State:", info.Routine.OperatingState.ToString());
                    EditorGUILayout.LabelField("Yield:", RadicalCoroutine.EditorHelper.GetYieldID(info.Routine));
                    EditorGUILayout.LabelField("Derivative:", RadicalCoroutine.EditorHelper.GetDerivativeID(info.Routine));
                    EditorGUI.indentLevel -= 2;
                }
                EditorGUI.indentLevel--;
            }
        }

        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying && _expanded;
        }

    }

#endif

}
