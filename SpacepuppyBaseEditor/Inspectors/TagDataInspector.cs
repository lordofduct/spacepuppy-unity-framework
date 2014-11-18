using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Tools
{

    [InitializeOnLoad()]
    [CustomEditor(typeof(TagData))]
    internal class TagDataInspector : Editor
    {

        #region Static Interface

        private static System.DateTime _lastUpdate;


        static TagDataInspector()
        {
            //SceneView.onSceneGUIDelegate -= OnSceneGUI;
            //SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        //private static void OnSceneGUI(SceneView scene)
        //{
        //    if ((System.DateTime.Now - _lastUpdate).TotalSeconds < 1.0d) return;

        //    if (!Application.isPlaying)
        //    {
        //        SyncTags();
        //    }
        //}

        public static void SyncTags()
        {
            //var tagData = (TagData)AssetDatabase.LoadAssetAtPath(@"Assets/Resources/TagData.asset", typeof(TagData));
            //if(tagData == null)
            //{
            //    tagData = ScriptableObjectHelper.CreateAsset<TagData>(@"Assets/Resources/TagData.asset");
            //}

            //if(!tagData.SimilarTo(UnityEditorInternal.InternalEditorUtility.tags))
            //{
            //    var helper = new TagData.EditorHelper(tagData);
            //    helper.UpdateTags(UnityEditorInternal.InternalEditorUtility.tags);
            //    EditorUtility.SetDirty(tagData);
            //    AssetDatabase.SaveAssets();
            //}

            //_lastUpdate = System.DateTime.Now;
        }

        #endregion

        #region Inspector

        private TagData.EditorHelper _helper;

        void OnEnable()
        {
            _helper = new TagData.EditorHelper(this.target as TagData);
        }

        public override void OnInspectorGUI()
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
                TagDataInspector.SyncTags();
            }
        }

        #endregion

    }
}
