using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor
{

    [CreateAssetMenu(fileName = "BuildSettings", menuName = "Build Settings", order = int.MaxValue)]
    public class BuildSettings : ScriptableObject
    {

        #region Fields

        [SerializeField]
        private SceneAsset _bootScene;

        [SerializeField]
        [ReorderableArray]
        private List<SceneAsset> _scenes;

        [SerializeField]
        private BuildTarget _buildTarget = BuildTarget.StandaloneWindows;

        [SerializeField]
        [EnumFlags]
        private BuildOptions _buildOptions;
        
        #endregion

        #region Properties

        public SceneAsset BootScene
        {
            get { return _bootScene; }
        }

        public IList<SceneAsset> Scenes
        {
            get { return _scenes; }
        }

        public BuildTarget BuildTarget
        {
            get { return _buildTarget; }
        }

        public BuildOptions BuildOptions
        {
            get { return _buildOptions; }
        }
        
        #endregion

    }

    [CustomEditor(typeof(BuildSettings))]
    public class BuildSettingsEditor : SPEditor
    {

        protected override void OnSPInspectorGUI()
        {
            //TODO - make editor more robust like the Build Settings screen
            this.DrawDefaultInspector();
            
            //build button
            if (this.serializedObject.isEditingMultipleObjects) return;
            
            EditorGUILayout.Space();
            if(GUILayout.Button("Build"))
            {
                var path = this.Build();
                EditorUtility.RevealInFinder(path);
            }
            if (GUILayout.Button("Build & Run"))
            {
                var path = this.Build();
                var proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = path;
                proc.Start();
            }
        }

        public string[] GetScenePaths()
        {
            using (var lst = TempCollection.GetList<string>())
            {
                var settings = this.target as BuildSettings;
                if (settings.BootScene != null) lst.Add(AssetDatabase.GetAssetPath(settings.BootScene));

                foreach (var scene in settings.Scenes)
                {
                    lst.Add(AssetDatabase.GetAssetPath(scene));
                }

                return lst.ToArray();
            }
        }

        public virtual string Build()
        {
            var settings = this.target as BuildSettings;
            var scenes = this.GetScenePaths();
            var path = EditorUtility.SaveFilePanel("Build", "", Application.productName + ".exe", "exe");

            BuildPipeline.BuildPlayer(scenes, path, settings.BuildTarget, settings.BuildOptions);
            return path;
        }

    }

}
