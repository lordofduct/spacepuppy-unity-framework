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

            var routines = targ.GetAllCoroutines().ToArray();
            EditorGUILayout.HelpBox(string.Format("Managing '{0}' RadicalCoroutines.", routines.Length), MessageType.Info);

            _expanded = EditorGUILayout.Foldout(_expanded, EditorHelper.TempContent("Coroutine Breakdown"));
            if(_expanded)
            {
                EditorGUI.indentLevel++;
                for(int i = 0; i < routines.Length; i++)
                {
                    var routine = routines[i];
                    EditorGUILayout.LabelField(string.Format("[{0:00}] Routine {1}", i, RadicalCoroutine.EditorHelper.GetInternalRoutineID(routine)));
                    EditorGUI.indentLevel += 2;
                    EditorGUILayout.LabelField("Component:", (routine.Operator != null) ? routine.Operator.GetType().Name : "UNKNOWN");
                    EditorGUILayout.LabelField("State:", routine.OperatingState.ToString());
                    EditorGUILayout.LabelField("Yield:", RadicalCoroutine.EditorHelper.GetYieldID(routine));
                    EditorGUILayout.LabelField("Derivative:", RadicalCoroutine.EditorHelper.GetDerivativeID(routine));
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
            if(targ == null) return;

            var routines = targ.GetAllCoroutines().ToArray();
            EditorGUILayout.HelpBox(string.Format("Managing '{0}' RadicalCoroutines.", routines.Length), MessageType.Info);
    
            _expanded = EditorGUILayout.Foldout(_expanded, "Coroutine Breakdown");
            if(_expanded)
            {
                EditorGUI.indentLevel++;
                for(int i = 0; i < routines.Length; i++)
                {
                    var routine = routines[i];
                    EditorGUILayout.LabelField(string.Format("[{0:00}] Routine {1}", i, RadicalCoroutine.EditorHelper.GetInternalRoutineID(routine)));
                    EditorGUI.indentLevel += 2;
                    EditorGUILayout.LabelField("Component:", (routine.Operator != null) ? routine.Operator.GetType().Name : "UNKNOWN");
                    EditorGUILayout.LabelField("State:", routine.OperatingState.ToString());
                    EditorGUILayout.LabelField("Yield:", RadicalCoroutine.EditorHelper.GetYieldID(routine));
                    EditorGUILayout.LabelField("Derivative:", RadicalCoroutine.EditorHelper.GetDerivativeID(routine));
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
