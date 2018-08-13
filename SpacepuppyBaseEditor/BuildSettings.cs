#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor
{

    [CreateAssetMenu(fileName = "BuildSettings", menuName = "Spacepuppy Build Pipeline/Build Settings")]
    public class BuildSettings : ScriptableObject
    {

        [System.Flags]
        public enum PostBuildOption
        {
            Nothing = 0,
            OpenFolder = 1,
            Run = 2,
            OpenFolderAndRun = 3
        }

        #region Fields

        [SerializeField]
        [Tooltip("Leave blank if you want to be asked for a filename every time you build.")]
        public string BuildFileName;
        [SerializeField]
        [Tooltip("Paths can be relative to the 'Assets' folder.\nLeave blank if you want to be asked for a directory every time you build.")]
        public string BuildDirectory;

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

        [SerializeField]
        private bool _defineSymbols;

        [SerializeField]
        [Tooltip("Semi-colon delimited symbols.")]
        private string _symbols;

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

        public bool DefineSymbols
        {
            get { return _defineSymbols; }
            set { _defineSymbols = value; }
        }

        /// <summary>
        /// Semi-colon delimited symbols.
        /// </summary>
        public string Symbols
        {
            get { return _symbols; }
            set { _symbols = value; }
        }

        #endregion


        #region Methods

        public string GetBuildFileNameWithExtension()
        {
            if (string.IsNullOrEmpty(this.BuildFileName)) return string.Empty;

            string extension = GetExtension(this.BuildTarget);
            string fileName = this.BuildFileName;
            if (!string.IsNullOrEmpty(extension))
            {
                string ext = "." + extension;
                if (!fileName.EndsWith(ext)) fileName += ext;
            }
            return fileName;
        }

        public virtual string[] GetScenePaths()
        {
            using (var lst = TempCollection.GetList<string>())
            {
                if (this.BootScene != null) lst.Add(AssetDatabase.GetAssetPath(this.BootScene));

                foreach (var scene in this.Scenes)
                {
                    lst.Add(AssetDatabase.GetAssetPath(scene));
                }

                return lst.ToArray();
            }
        }

        public virtual bool Build(PostBuildOption option)
        {
            string path;
            try
            {
                //get output directory
                var dir = EditorProjectPrefs.Local.GetString("LastBuildDirectory", string.Empty);
                if (string.IsNullOrEmpty(this.BuildFileName))
                {
                    string extension = GetExtension(this.BuildTarget);
                    path = EditorUtility.SaveFilePanel("Build", dir, string.IsNullOrEmpty(extension) ? Application.productName + "." + extension : Application.productName, extension);
                    if(!string.IsNullOrEmpty(path))
                    {
                        return false;
                    }
                }
                else
                {
                    string possiblePath = this.BuildDirectory;
                    if (!string.IsNullOrEmpty(possiblePath) && possiblePath.StartsWith(".")) possiblePath = System.IO.Path.Combine(Application.dataPath, possiblePath);
                    if(!string.IsNullOrEmpty(possiblePath) && System.IO.Directory.Exists(possiblePath))
                    {
                        path = System.IO.Path.Combine(possiblePath, this.GetBuildFileNameWithExtension());
                        path = System.IO.Path.GetFullPath(path);
                    }
                    else
                    {
                        path = EditorUtility.OpenFolderPanel("Build", dir, string.Empty);
                        if (!string.IsNullOrEmpty(path))
                        {
                            path = System.IO.Path.Combine(path, this.GetBuildFileNameWithExtension());
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }

            return this.Build(path, option);
        }

        public virtual bool Build(string path, PostBuildOption option)
        {
            try
            {
                var scenes = this.GetScenePaths();
                var buildGroup = BuildPipeline.GetBuildTargetGroup(this.BuildTarget);

                //set version
                this.Version.Build++;
                EditorUtility.SetDirty(this);
                PlayerSettings.bundleVersion = this.Version.ToString();
                AssetDatabase.SaveAssets();
                
                //build
                if (!string.IsNullOrEmpty(path))
                {
                    //save last build directory
                    EditorProjectPrefs.Local.SetString("LastBuildDirectory", System.IO.Path.GetDirectoryName(path));


                    //do build
                    InputSettings cacheInputs = null;
                    string cacheSymbols = null;

                    if (this.InputSettings != null)
                    {
                        cacheInputs = InputSettings.LoadGlobalInputSettings(false);
                        this.InputSettings.ApplyToGlobal();
                    }
                    if (this.DefineSymbols)
                    {
                        cacheSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildGroup) ?? string.Empty;
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildGroup, this.Symbols);
                    }

                    BuildPipeline.BuildPlayer(scenes, path, this.BuildTarget, this.BuildOptions);

                    if (cacheInputs != null)
                    {
                        cacheInputs.ApplyToGlobal();
                    }
                    if (cacheSymbols != null)
                    {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildGroup, cacheSymbols);
                    }


                    //save
                    if ((option & PostBuildOption.OpenFolder) != 0)
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
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }

            return false;
        }

        #endregion

        #region Special Utils

        public static string GetExtension(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "exe";
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinuxUniversal:
                    return "x86";
                case BuildTarget.StandaloneLinux64:
                    return "x86_64";
                case BuildTarget.StandaloneOSX:
                //case BuildTarget.StandaloneOSXIntel:
                //case BuildTarget.StandaloneOSXIntel64:
                //case BuildTarget.StandaloneOSXUniversal:
                    return "app";
                default:
                    return string.Empty;
            }
        }

        #endregion

    }

    [CustomEditor(typeof(BuildSettings), true)]
    public class BuildSettingsEditor : SPEditor
    {
        
        public const string PROP_BUILDFILENAME = "BuildFileName";
        public const string PROP_BUILDDIR = "BuildDirectory";
        public const string PROP_VERSION = "Version";
        public const string PROP_BOOTSCENE = "_bootScene";
        public const string PROP_SCENES = "_scenes";
        public const string PROP_BUILDTARGET = "_buildTarget";
        public const string PROP_BUILDOPTIONS = "_buildOptions";
        public const string PROP_INPUTSETTINGS = "_inputSettings";
        public const string PROP_DEFINESYMBOLS = "_defineSymbols";
        public const string PROP_SYMBOLS = "_symbols";

        #region Fields

        private com.spacepuppyeditor.Base.ReorderableArrayPropertyDrawer _scenesDrawer = new com.spacepuppyeditor.Base.ReorderableArrayPropertyDrawer();

        #endregion

        #region Properties

        public com.spacepuppyeditor.Base.ReorderableArrayPropertyDrawer ScenesDrawer
        {
            get { return _scenesDrawer; }
        }

        #endregion

        #region Methods

        protected override void OnEnable()
        {
            base.OnEnable();

            _scenesDrawer.FormatElementLabel = (p, i, b1, b2) =>
            {
                return string.Format("Scene #{0}", i + 1);
            };
        }

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);

            var propFileName = this.serializedObject.FindProperty(PROP_BUILDFILENAME);
            EditorGUILayout.PropertyField(propFileName);
            if(!string.IsNullOrEmpty(propFileName.stringValue))
            {
                var propBuildDir = this.serializedObject.FindProperty(PROP_BUILDDIR);
                propBuildDir.stringValue = SPEditorGUILayout.FolderPathTextfield(EditorHelper.TempContent(propBuildDir.displayName, propBuildDir.tooltip), propBuildDir.stringValue, "Build Directory");
            }
            
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
            //this.DrawPropertyField(PROP_BOOTSCENE);
            //this.DrawPropertyField(PROP_SCENES);

            this.DrawPropertyField(PROP_BOOTSCENE, "Boot Scene #0", false);

            var propScenes = this.serializedObject.FindProperty(PROP_SCENES);
            var lblScenes = EditorHelper.TempContent(propScenes.displayName, propScenes.tooltip);
            var h = _scenesDrawer.GetPropertyHeight(propScenes, lblScenes);
            _scenesDrawer.OnGUI(EditorGUILayout.GetControlRect(true, h), propScenes, lblScenes);
        }

        public virtual void DrawBuildOptions()
        {
            //TODO - upgrade this to more specialized build options gui
            this.DrawPropertyField(PROP_BUILDTARGET);
            this.DrawPropertyField(PROP_BUILDOPTIONS);

            var propDefineSymbols = this.serializedObject.FindProperty(PROP_DEFINESYMBOLS);
            SPEditorGUILayout.PropertyField(propDefineSymbols);
            if (propDefineSymbols.boolValue)
            {
                this.DrawPropertyField(PROP_SYMBOLS);
            }
        }

        public virtual void DrawInputSettings()
        {
            this.DrawPropertyField(PROP_INPUTSETTINGS);
        }

        public virtual void DrawBuildButtons()
        {
            if (GUILayout.Button("Build"))
            {
                EditorCoroutine.StartEditorCoroutine(this.DoBuild(BuildSettings.PostBuildOption.OpenFolder));
            }
            if (GUILayout.Button("Build & Run"))
            {
                EditorCoroutine.StartEditorCoroutine(this.DoBuild(BuildSettings.PostBuildOption.OpenFolderAndRun));
            }
            if (GUILayout.Button("Sync To Global Build"))
            {
                this.SyncToGlobalBuild();
            }
        }

        protected virtual System.Collections.IEnumerator DoBuild(BuildSettings.PostBuildOption postBuildOption)
        {
            var settings = this.target as BuildSettings;
            if(settings != null)
            {
                settings.Build(postBuildOption);
            }

            yield break;
        }



        
        public virtual void SyncToGlobalBuild()
        {
            var lst = new List<EditorBuildSettingsScene>();
            var settings = this.target as BuildSettings;
            foreach (var sc in settings.GetScenePaths())
            {
                lst.Add(new EditorBuildSettingsScene(sc, true));
            }
            EditorBuildSettings.scenes = lst.ToArray();
        }
        
        #endregion
        
    }

}
