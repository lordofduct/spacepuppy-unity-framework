using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.AI.BehaviourTree;
using com.spacepuppy.AI.BehaviourTree.Components;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.AI.BehaviourTree.Components
{

    [CustomEditor(typeof(a_Sense), true)]
    public class a_SenseInspector : SPEditor
    {

        public const string PROP_REPEAT = "_repeat";
        public const string PROP_ALWAYSSUCCEED = "_alwaysSucceed";
        public const string PROP_SENSOR = "_sensor";
        public const string PROP_VARIABLE = "_variable";
        public const string PROP_VARIABLEUPDATEPARAMS = "_variableUpdateParams";

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);
            this.DrawPropertyField(PROP_REPEAT);
            this.DrawPropertyField(PROP_ALWAYSSUCCEED);
            this.DrawPropertyField(PROP_SENSOR);
            this.DrawPropertyField(PROP_VARIABLE);

            var prop = this.serializedObject.FindProperty(PROP_VARIABLEUPDATEPARAMS);
            var e = (a_Sense.VariableUpdateOptions)prop.intValue; //prop.GetEnumValue<a_Sense.VariableUpdateOptions>();
            EditorGUI.BeginChangeCheck();

            bool clearPrev = EditorGUILayout.Toggle(EditorHelper.TempContent("Clear Previous Aspect If Unseen", "Purge the currently stored aspect if it's not visible. If the current field is not an aspect, it may persist if no aspect is found and this sensor is not configured to always update."), e.HasFlag(a_Sense.VariableUpdateOptions.ClearPreviousAspectIfUnseen));
            bool onlyUpdate = EditorGUILayout.Toggle(EditorHelper.TempContent("Only Update Aspect If Previous Unseen", "The sensor will only test for a new aspect if, and only if, the previous aspect is null or unseen."), e.HasFlag(a_Sense.VariableUpdateOptions.OnlyUpdateAspectIfPreviousUnseen));
            bool storeOnly = EditorGUILayout.Toggle(EditorHelper.TempContent("Store Only Position", "Store only the current position of the found aspect."), e.HasFlag(a_Sense.VariableUpdateOptions.StoreOnlyPosition));
            bool alwaysUpdate = EditorGUILayout.Toggle(EditorHelper.TempContent("Always Update", "Update the variable even if no aspect is found."), e.HasFlag(a_Sense.VariableUpdateOptions.AlwaysUpdate));
            
            if(EditorGUI.EndChangeCheck())
            {
                e = 0;
                if (clearPrev) e |= a_Sense.VariableUpdateOptions.ClearPreviousAspectIfUnseen;
                if (onlyUpdate) e |= a_Sense.VariableUpdateOptions.OnlyUpdateAspectIfPreviousUnseen;
                if (storeOnly) e |= a_Sense.VariableUpdateOptions.StoreOnlyPosition;
                if (alwaysUpdate) e |= a_Sense.VariableUpdateOptions.AlwaysUpdate;
                prop.intValue = (int)e;
            }

            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_REPEAT, PROP_ALWAYSSUCCEED, PROP_SENSOR, PROP_VARIABLE, PROP_VARIABLEUPDATEPARAMS);

            this.serializedObject.ApplyModifiedProperties();
        }

    }
}
