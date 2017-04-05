using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Waypoints;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Waypoints
{

    [CustomEditor(typeof(i_MoveOnPath))]
    public class i_MoveOnPathInspector : SPEditor
    {

        private const string PROP_MODIFIERS = "_updateModifierTypes";

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            var iterator = serializedObject.GetIterator();
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                if(iterator.name == PROP_MODIFIERS)
                {
                    this.DrawModifierTypes(iterator);
                }
                else
                {
                    SPEditorGUILayout.PropertyField(iterator, true);
                }
            }

            this.serializedObject.ApplyModifiedProperties();
        }



        private void DrawModifierTypes(SerializedProperty prop)
        {
            //TODO - we want to include a drop down that only lists the IStateModifiers that actually exist on the nodes...

            SPEditorGUILayout.PropertyField(prop, true);
        }

    }
}
