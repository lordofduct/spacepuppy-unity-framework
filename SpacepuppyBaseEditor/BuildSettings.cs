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

    [CustomEditor(typeof(BuildSettings), true)]
    public class BuildSettingsEditor : SPEditor
    {

        public const string PROP_BOOTSCENE = "_bootScene";
        public const string PROP_SCENES = "_scenes";
        public const string PROP_BUILDTARGET = "_buildTarget";
        public const string PROP_BUILDOPTIONS = "_buildOptions";


        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawScenes();

            this.DrawBuildOptions();

            this.serializedObject.ApplyModifiedProperties();
            
            //build button
            if (this.serializedObject.isEditingMultipleObjects) return;
            
            EditorGUILayout.Space();

            this.DrawBuildButtons();
        }

        public virtual void DrawScenes()
        {
            this.DrawPropertyField(PROP_BOOTSCENE);
            this.DrawPropertyField(PROP_SCENES);
        }

        public virtual void DrawBuildOptions()
        {
            //TODO - upgrade this to more specialized build options gui
            this.DrawPropertyField(PROP_BUILDTARGET);
            this.DrawPropertyField(PROP_BUILDOPTIONS);
        }

        public virtual void DrawBuildButtons()
        {
            if (GUILayout.Button("Build"))
            {
                var path = this.Build();
                EditorUtility.RevealInFinder(path);
            }
            if (GUILayout.Button("Build & Run"))
            {
                var path = this.Build();
                if (!string.IsNullOrEmpty(path))
                {
                    var proc = new System.Diagnostics.Process();
                    proc.StartInfo.FileName = path;
                    proc.Start();
                }
            }
            if (GUILayout.Button("Sync To Global Build"))
            {
                this.SyncToGlobalBuild();
            }
        }

        public virtual string[] GetScenePaths()
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

        public virtual void SyncToGlobalBuild()
        {
            var lst = new List<EditorBuildSettingsScene>();
            var settings = this.target as BuildSettings;
            foreach (var sc in this.GetScenePaths())
            {
                lst.Add(new EditorBuildSettingsScene(sc, true));
            }
            EditorBuildSettings.scenes = lst.ToArray();
        }

        public virtual string Build()
        {
            var settings = this.target as BuildSettings;
            var scenes = this.GetScenePaths();

            var dir = EditorProjectPrefs.Local.GetString("LastBuildDirectory", string.Empty);
            var path = EditorUtility.SaveFilePanel("Build", dir, Application.productName + ".exe", "exe");
            if(!string.IsNullOrEmpty(path))
            {
                EditorProjectPrefs.Local.SetString("LastBuildDirectory", System.IO.Path.GetDirectoryName(path));

                BuildPipeline.BuildPlayer(scenes, path, settings.BuildTarget, settings.BuildOptions);
                return path;
            }
            else
            {
                return string.Empty;
            }
        }

    }

}
