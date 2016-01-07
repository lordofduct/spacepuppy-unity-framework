using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Project;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Project
{

    [InitializeOnLoad()]
    [CustomEditor(typeof(TagData))]
    internal class TagDataInspector : SPEditor
    {

        #region Static Interface
        
        

        #endregion

        #region Inspector

        private TagData.EditorHelper _helper;
        
        protected override void OnEnable()
        {
            base.OnEnable();

            _helper = new TagData.EditorHelper(this.target as TagData);
        }

        protected override void OnSPInspectorGUI()
        {

            EditorGUILayout.HelpBox("This holds a reference to all the available tags for access at runtime. The data is kept in Assets/Resources and should be the ONLY one that exists. Do not delete.\n\nSpacepuppy Framework", MessageType.Info);

            EditorGUILayout.LabelField("Tags");
            EditorGUI.indentLevel++;
            foreach(var tag in _helper.Tags)
            {
                EditorGUILayout.LabelField(tag);
            }
            EditorGUI.indentLevel--;

            if(GUILayout.Button("Sync Tags"))
            {
                SPMenu.SyncTagData(_helper.Target);
            }
        }

        #endregion

    }
}
