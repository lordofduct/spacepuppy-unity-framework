using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.AI.BehaviourTree;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.AI.BehaviourTree
{

    [CustomPropertyDrawer(typeof(ParallelPassOptions))]
    public class ParallelPassOptionsPropertyDrawer : PropertyDrawer
    {
        private const ParallelPassOptions BOTH_ANY = ParallelPassOptions.FailOnAny | ParallelPassOptions.SucceedOnAny;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var e = (ParallelPassOptions)property.intValue;
            bool both = ((e & BOTH_ANY) == BOTH_ANY);

            bool failAny = e.HasFlag(ParallelPassOptions.FailOnAny);
            bool passAny = e.HasFlag(ParallelPassOptions.SucceedOnAny);
            bool passOnTie = e.HasFlag(ParallelPassOptions.SucceedOnTie);

            var r1 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            var r2 = new Rect(position.xMin, r1.yMax, position.width, EditorGUIUtility.singleLineHeight);
            var r3 = new Rect(position.xMin, r2.yMax, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();

            failAny = EditorGUI.Popup(r1, "Fail", (failAny) ? 1 : 0, new string[] { "All", "Any" }) == 1;
            passAny = EditorGUI.Popup(r2, "Succeed", (passAny) ? 1 : 0, new string[] { "All", "Any" }) == 1;

            var cache = SPGUI.DisableIf(both);
            passOnTie = EditorGUI.Popup(r3, "Tie Breaker", (passOnTie) ? 1 : 0, new string[] { "Fail", "Succeed" }) == 1;
            cache.Reset();

            if(EditorGUI.EndChangeCheck())
            {
                e = 0;
                if (failAny) e |= ParallelPassOptions.FailOnAny;
                if (passAny) e |= ParallelPassOptions.SucceedOnAny;
                if (passOnTie) e |= ParallelPassOptions.SucceedOnTie;
                property.intValue = (int)e;
            }
        }

    }

}
