using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [InitializeOnLoad()]
    [CustomEditor(typeof(TagData))]
    internal class TagDataInspector : SPEditor
    {

        #region Static Interface

        private static System.DateTime _lastUpdate;


        static TagDataInspector()
        {
            _lastUpdate = System.DateTime.Now;
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView scene)
        {
            if ((System.DateTime.Now - _lastUpdate).TotalSeconds < 1.0d) return;

            if (!Application.isPlaying)
            {
                SPMenu.SyncTagData();
                _lastUpdate = System.DateTime.Now;
            }
        }

        #endregion

        #region Inspector

        private TagData.EditorHelper _helper;

        void OnEnable()
        {
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
                _helper.UpdateTags(UnityEditorInternal.InternalEditorUtility.tags);
                EditorUtility.SetDirty(this.target);
                AssetDatabase.SaveAssets();
            }
        }

        #endregion

    }
}
