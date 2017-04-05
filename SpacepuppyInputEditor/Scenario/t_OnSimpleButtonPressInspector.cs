using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using com.spacepuppy.Scenario;

namespace com.spacepuppyeditor.Scenario
{

    [CustomEditor(typeof(t_OnSimpleButtonPressInspector))]
    public class t_OnSimpleButtonPressInspector : SPEditor
    {

        public const string PROP_INPUTID = "_inputId";

        private string[] _inputIds;

        protected override void OnEnable()
        {
            base.OnEnable();

            _inputIds = com.spacepuppyeditor.Base.AdvancedInputManagerWindow.GetAllAvailableInputs();
        }

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);

            var prop = this.serializedObject.FindProperty(PROP_INPUTID);
            int i = System.Array.IndexOf(_inputIds, prop.stringValue);
            i = UnityEditor.EditorGUILayout.Popup("Input ID", i, _inputIds);
            prop.stringValue = (i >= 0) ? _inputIds[i] : string.Empty;

            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_INPUTID);

            this.serializedObject.ApplyModifiedProperties();
        }

    }
}
