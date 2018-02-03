using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor
{

    [CreateAssetMenu(fileName = "BuildSettings", menuName = "Spacepuppy/Build Settings", order = int.MaxValue)]
    public class BuildSettings : ScriptableObject
    {

        #region Fields

        [SerializeField]
        public VersionInfo Version;

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

        [SerializeField]
        [Tooltip("Leave blank if you want to use default settings found in the Input Settings screen.")]
        private InputSettings _inputSettings;
        
        #endregion

        #region Properties
        
        public SceneAsset BootScene
        {
            get { return _bootScene; }
            set { _bootScene = value; }
        }

        public IList<SceneAsset> Scenes
        {
            get { return _scenes; }
        }

        public BuildTarget BuildTarget
        {
            get { return _buildTarget; }
            set { _buildTarget = value; }
        }

        public BuildOptions BuildOptions
        {
            get { return _buildOptions; }
            set { _buildOptions = value; }
        }
        
        public InputSettings InputSettings
        {
            get { return _inputSettings; }
            set { _inputSettings = value; }
        }

        #endregion

    }

    [CustomEditor(typeof(BuildSettings), true)]
    public class BuildSettingsEditor : SPEditor
    {

        [System.Flags]
        public enum PostBuildOption
        {
            Nothing = 0,
            OpenFolder = 1,
            Run = 2,
            OpenFolderAndRun = 3
        }

        public const string PROP_VERSION = "Version";
        public const string PROP_BOOTSCENE = "_bootScene";
        public const string PROP_SCENES = "_scenes";
        public const string PROP_BUILDTARGET = "_buildTarget";
        public const string PROP_BUILDOPTIONS = "_buildOptions";
        public const string PROP_INPUTSETTINGS = "_inputSettings";


        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(PROP_VERSION);

            this.DrawScenes();

            this.DrawBuildOptions();

            this.DrawInputSettings();

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

        public virtual void DrawInputSettings()
        {
            this.DrawPropertyField(PROP_INPUTSETTINGS);
        }

        public virtual void DrawBuildButtons()
        {
            if (GUILayout.Button("Build"))
            {
                EditorCoroutine.StartEditorCoroutine(this.DoBuild(PostBuildOption.OpenFolder));
            }
            if (GUILayout.Button("Build & Run"))
            {
                EditorCoroutine.StartEditorCoroutine(this.DoBuild(PostBuildOption.OpenFolderAndRun));
            }
            if (GUILayout.Button("Sync To Global Build"))
            {
                this.SyncToGlobalBuild();
            }
        }

        protected virtual System.Collections.IEnumerator DoBuild(PostBuildOption postBuildOption)
        {
            this.Build(postBuildOption);

            yield break;
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
        
        public bool Build(PostBuildOption option)
        {
            try
            {
                var settings = this.target as BuildSettings;
                var scenes = this.GetScenePaths();

                //set version
                settings.Version.Build++;
                EditorUtility.SetDirty(settings);
                PlayerSettings.bundleVersion = settings.Version.ToString();
                AssetDatabase.SaveAssets();

                //get output directory
                var dir = EditorProjectPrefs.Local.GetString("LastBuildDirectory", string.Empty);
                string path;
                switch(settings.BuildTarget)
                {
                    case BuildTarget.StandaloneWindows:
                    case BuildTarget.StandaloneWindows64:
                        path = EditorUtility.SaveFilePanel("Build", dir, Application.productName + ".exe", "exe");
                        break;
                    case BuildTarget.StandaloneLinux:
                    case BuildTarget.StandaloneLinuxUniversal:
                        path = EditorUtility.SaveFilePanel("Build", dir, Application.productName + ".x86", "x86");
                        break;
                    case BuildTarget.StandaloneLinux64:
                        path = EditorUtility.SaveFilePanel("Build", dir, Application.productName + ".x86_64", "x86_64");
                        break;
                    case BuildTarget.StandaloneOSXIntel:
                    case BuildTarget.StandaloneOSXIntel64:
                    case BuildTarget.StandaloneOSXUniversal:
                        path = EditorUtility.SaveFilePanel("Build", dir, Application.productName + ".app", "app");
                        break;
                    default:
                        path = EditorUtility.SaveFilePanel("Build", dir, Application.productName, "");
                        break;
                }

                //build
                if(!string.IsNullOrEmpty(path))
                {
                    EditorProjectPrefs.Local.SetString("LastBuildDirectory", System.IO.Path.GetDirectoryName(path));
                
                    if(settings.InputSettings != null)
                    {
                        var copy = InputSettings.LoadGlobalInputSettings(false);
                        settings.InputSettings.ApplyToGlobal();

                        BuildPipeline.BuildPlayer(scenes, path, settings.BuildTarget, settings.BuildOptions);

                        copy.ApplyToGlobal();
                    }
                    else
                    {
                        BuildPipeline.BuildPlayer(scenes, path, settings.BuildTarget, settings.BuildOptions);
                    }
                
                    if((option & PostBuildOption.OpenFolder) != 0)
                    {
                        EditorUtility.RevealInFinder(path);
                    }
                    if ((option & PostBuildOption.Run) != 0)
                    {
                        var proc = new System.Diagnostics.Process();
                        proc.StartInfo.FileName = path;
                        proc.Start();
                    }

                    return true;
                }

            }
            catch(System.Exception ex)
            {
                Debug.LogException(ex);
            }

            return false;
        }

    }

}
