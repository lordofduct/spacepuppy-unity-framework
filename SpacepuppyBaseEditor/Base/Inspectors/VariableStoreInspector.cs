using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomEditor(typeof(VariableStore))]
    public class VariableStoreInspector : SPEditor
    {

        public const string PROP_REFLECTNAMES = "_reflectNamesFromType";
        public const string PROP_VARIABLES = "_variables";

        private VariantCollectionPropertyDrawer _variablesDrawer = new VariantCollectionPropertyDrawer();

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            var propReflect = this.serializedObject.FindProperty(PROP_REFLECTNAMES);
            SPEditorGUILayout.PropertyField(propReflect);

            var propVars = this.serializedObject.FindProperty(PROP_VARIABLES);
            var lbl_Vars = EditorHelper.TempContent("Variables");
            _variablesDrawer.ConfigurePropertyList((TypeReferencePropertyDrawer.GetTypeFromTypeReference(propReflect)));
            var h = _variablesDrawer.GetPropertyHeight(propVars, lbl_Vars);
            var r = EditorGUILayout.GetControlRect(true, _variablesDrawer.GetPropertyHeight(propVars, lbl_Vars));
            _variablesDrawer.OnGUI(r, propVars, lbl_Vars);

            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_REFLECTNAMES, PROP_VARIABLES);

            this.serializedObject.ApplyModifiedProperties();
        }

    }

    [CustomEditor(typeof(VariableStoreAsset))]
    public class VariableStoreAssetInspector : VariableStoreInspector
    {

        /*
        public const string PROP_REFLECTNAMES = "_reflectNamesFromType";
        public const string PROP_VARIABLES = "_variables";

        private VariantCollectionPropertyDrawer _variablesDrawer = new VariantCollectionPropertyDrawer();

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            var propReflect = this.serializedObject.FindProperty(PROP_REFLECTNAMES);
            SPEditorGUILayout.PropertyField(propReflect);

            var propVars = this.serializedObject.FindProperty(PROP_VARIABLES);
            var lbl_Vars = EditorHelper.TempContent("Variables");
            _variablesDrawer.ConfigurePropertyList((TypeReferencePropertyDrawer.GetTypeFromTypeReference(propReflect)));
            var h = _variablesDrawer.GetPropertyHeight(propVars, lbl_Vars);
            var r = EditorGUILayout.GetControlRect(true, _variablesDrawer.GetPropertyHeight(propVars, lbl_Vars));
            _variablesDrawer.OnGUI(r, propVars, lbl_Vars);

            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_REFLECTNAMES, PROP_VARIABLES);

            this.serializedObject.ApplyModifiedProperties();
        }
        */

    }

}
