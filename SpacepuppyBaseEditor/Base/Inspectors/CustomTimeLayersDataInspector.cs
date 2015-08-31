using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [CustomEditor(typeof(CustomTimeLayersData))]
    internal class CustomTimeLayersDataInspector : SPEditor
    {

        #region Inspector

        protected override void OnSPInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            this.DrawPropertyField("_customTimeLayers");
            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this.target);
                AssetDatabase.SaveAssets();
            }
        }

        #endregion

    }
}
