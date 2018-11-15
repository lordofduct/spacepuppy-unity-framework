#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

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

        [SerializeField]
        [ReorderableArray]
        private List<PlayerSettingOverride> _playerSettingOverrides = new List<PlayerSettingOverride>();

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

        public List<PlayerSettingOverride> PlayerSettingsOverrides
        {
            get { return _playerSettingOverrides; }
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
                AssetDatabase.SaveAssets();

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
                    Dictionary<BuildSettings.PlayerSettingOverride, object> cachePlayerSettings = null;

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

                    if (_playerSettingOverrides.Count > 0)
                    {
                        cachePlayerSettings = new Dictionary<PlayerSettingOverride, object>();
                        foreach(var setting in _playerSettingOverrides)
                        {
                            if (setting.SettingInfo != null)
                            {
                                cachePlayerSettings[setting] = setting.SettingInfo.GetValue(null, null);
                                try
                                {
                                    setting.SettingInfo.SetValue(null, setting.SettingValue, null);
                                }
                                catch(System.Exception)
                                { }
                            }
                        }
                    }

                    var report = BuildPipeline.BuildPlayer(scenes, path, this.BuildTarget, this.BuildOptions);

                    if (cacheInputs != null)
                    {
                        cacheInputs.ApplyToGlobal();
                    }
                    if (cacheSymbols != null)
                    {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildGroup, cacheSymbols);
                    }
                    if(cachePlayerSettings != null)
                    {
                        //loop backwards when resetting from cache
                        for (int i = _playerSettingOverrides.Count - 1; i >= 0; i--)
                        {
                            var setting = _playerSettingOverrides[i];
                            if(setting.SettingInfo != null)
                            {
                                try
                                {
                                    setting.SettingInfo.SetValue(null, cachePlayerSettings[setting], null);
                                }
                                catch(System.Exception)
                                { }
                            }
                        }
                    }

                    EditorUtility.SetDirty(this);
                    AssetDatabase.SaveAssets();

                    if(report != null && report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
                    {
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
                    else
                    {
                        return false;
                    }
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

        [System.Serializable]
        public class PlayerSettingOverride : ISerializationCallbackReceiver
        {

            [System.NonSerialized]
            private PropertyInfo _settingInfo;
            public PropertyInfo SettingInfo
            {
                get { return _settingInfo; }
                set
                {
                    if (value == null || BuildSettings.IsValidPropertySettingInfo(value))
                        _settingInfo = value;
                    else
                        throw new System.ArgumentException("PropertyInfo must be for a static property of the 'PlayerSettings' class.");
                }
            }
            [System.NonSerialized]
            public object SettingValue;
            
            #region Serialization Interface

            [SerializeField]
            private string _propertyName;
            [SerializeField]
            private string _serializedValue;
            [SerializeField]
            private UnityEngine.Object _serializedRef;

            void ISerializationCallbackReceiver.OnAfterDeserialize()
            {
                _settingInfo = null;
                this.SettingValue = null;
                var info = typeof(PlayerSettings).GetProperty(_propertyName, BindingFlags.Static | BindingFlags.Public);
                if (BuildSettings.IsValidPropertySettingInfo(info))
                {
                    var tp = info.PropertyType;
                    if (typeof(UnityEngine.Object).IsAssignableFrom(tp))
                    {
                        _settingInfo = info;
                        this.SettingValue = _serializedRef;
                    }
                    else if (ConvertUtil.IsSupportedType(tp))
                    {
                        _settingInfo = info;
                        this.SettingValue = ConvertUtil.ToPrim(_serializedValue, tp);
                    }
                }
            }

            void ISerializationCallbackReceiver.OnBeforeSerialize()
            {
                if(BuildSettings.IsValidPropertySettingInfo(_settingInfo))
                {
                    _propertyName = _settingInfo.Name;
                    if(typeof(UnityEngine.Object).IsAssignableFrom(_settingInfo.PropertyType))
                    {
                        _serializedRef = this.SettingValue as UnityEngine.Object;
                        _serializedValue = null;
                    }
                    else
                    {
                        _serializedRef = null;
                        _serializedValue = ConvertUtil.Stringify(this.SettingValue);
                    }
                }
                else
                {
                    _propertyName = null;
                    _serializedRef = null;
                    _serializedValue = null;
                }
            }

            #endregion

        }

        public static bool IsValidPropertySettingInfo(PropertyInfo info)
        {
            //is read write
            if (info == null || !info.CanRead || !info.CanWrite) return false;
            //is implemented by PlayerSettings
            if (!info.DeclaringType.IsAssignableFrom(typeof(PlayerSettings))) return false;
            //is supported type
            if (!(typeof(UnityEngine.Object).IsAssignableFrom(info.PropertyType) || ConvertUtil.IsSupportedType(info.PropertyType))) return false;

            //getter is public and static
            var getter = info.GetGetMethod();
            if (!getter.IsStatic || !getter.IsPublic) return false;

            //setter is public and static
            var setter = info.GetSetMethod();
            if (!setter.IsStatic || !setter.IsPublic) return false;

            return true;
        }

        public static IEnumerable<PropertyInfo> GetOverridablePlayerSettings()
        {
            return (from info in typeof(PlayerSettings).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                    where info != null && info.CanRead && info.CanWrite &&
                          (typeof(UnityEngine.Object).IsAssignableFrom(info.PropertyType) || ConvertUtil.IsSupportedType(info.PropertyType)) &&
                          info.GetGetMethod().IsPublic && info.GetSetMethod().IsPublic
                    select info);
        }

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
        public const string PROP_PLAYERSETTINGSOVERRIDES = "_playerSettingOverrides";

        #region Fields

        private com.spacepuppyeditor.Base.ReorderableArrayPropertyDrawer _scenesDrawer = new com.spacepuppyeditor.Base.ReorderableArrayPropertyDrawer(typeof(SceneAsset));

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

            this.DrawPlayerSettingOverrides();

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

        public virtual void DrawPlayerSettingOverrides()
        {
            this.DrawPropertyField(PROP_PLAYERSETTINGSOVERRIDES);
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

    [CustomPropertyDrawer(typeof(BuildSettings.PlayerSettingOverride))]
    internal class PlayerSettingsOverridePropertyDrawer : PropertyDrawer
    {

        public const string PROP_NAME = "_propertyName";
        public const string PROP_VALUE = "_serializedValue";
        public const string PROP_REF = "_serializedRef";

        private static PropertyInfo[] _knownPlayerSettings;
        private static string[] _knownPlayerSettingPropNames;
        private static GUIContent[] _knownPlayerSettingPropNamesPretty;
        static PlayerSettingsOverridePropertyDrawer()
        {
            _knownPlayerSettings = BuildSettings.GetOverridablePlayerSettings().ToArray();
            _knownPlayerSettingPropNames = (from info in _knownPlayerSettings select info.Name).ToArray();
            _knownPlayerSettingPropNamesPretty = (from info in _knownPlayerSettings select new GUIContent(ObjectNames.NicifyVariableName(info.Name))).ToArray();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var r0 = new Rect(position.xMin, position.yMin, position.width / 2f, position.height);
            var r1 = new Rect(r0.xMax, position.yMin, position.width - r0.width, position.height);

            var propName = property.FindPropertyRelative(PROP_NAME);
            var propValue = property.FindPropertyRelative(PROP_VALUE);
            var propRef = property.FindPropertyRelative(PROP_REF);

            int index = System.Array.IndexOf(_knownPlayerSettingPropNames, propName.stringValue);
            EditorGUI.BeginChangeCheck();
            index = EditorGUI.Popup(r0, GUIContent.none, index, _knownPlayerSettingPropNamesPretty);
            if(EditorGUI.EndChangeCheck())
            {
                if (index >= 0 && index < _knownPlayerSettingPropNames.Length)
                    propName.stringValue = _knownPlayerSettingPropNames[index];
                else
                    propName.stringValue = string.Empty;

                propValue.stringValue = string.Empty;
                propRef.objectReferenceValue = null;
            }

            if (index < 0 || index >= _knownPlayerSettings.Length) return;

            var info = _knownPlayerSettings[index];
            if (info.PropertyType.IsEnum)
            {
                int ei = ConvertUtil.ToInt(propValue.stringValue);
                propValue.stringValue = ConvertUtil.ToInt(EditorGUI.EnumPopup(r1, ConvertUtil.ToEnumOfType(info.PropertyType, ei))).ToString();
                propRef.objectReferenceValue = null;
            }
            else
            {
                var etp = VariantReference.GetVariantType(info.PropertyType);
                switch (etp)
                {
                    case VariantType.Null:
                        propValue.stringValue = string.Empty;
                        propRef.objectReferenceValue = null;
                        break;
                    case VariantType.String:
                        propValue.stringValue = EditorGUI.TextField(r1, propValue.stringValue);
                        propRef.objectReferenceValue = null;
                        break;
                    case VariantType.Boolean:
                        propValue.stringValue = ConvertUtil.Stringify(EditorGUI.Toggle(r1, GUIContent.none, ConvertUtil.ToBool(propValue.stringValue)));
                        propRef.objectReferenceValue = null;
                        break;
                    case VariantType.Integer:
                        propValue.stringValue = EditorGUI.IntField(r1, GUIContent.none, ConvertUtil.ToInt(propValue.stringValue)).ToString();
                        propRef.objectReferenceValue = null;
                        break;
                    case VariantType.Float:
                        propValue.stringValue = EditorGUI.FloatField(r1, GUIContent.none, ConvertUtil.ToSingle(propValue.stringValue)).ToString();
                        propRef.objectReferenceValue = null;
                        break;
                    case VariantType.Double:
                        propValue.stringValue = EditorGUI.DoubleField(r1, GUIContent.none, ConvertUtil.ToDouble(propValue.stringValue)).ToString();
                        propRef.objectReferenceValue = null;
                        break;
                    case VariantType.Vector2:
                        propValue.stringValue = ConvertUtil.Stringify(EditorGUI.Vector2Field(r1, GUIContent.none, ConvertUtil.ToVector2(propValue.stringValue)));
                        propRef.objectReferenceValue = null;
                        break;
                    case VariantType.Vector3:
                        propValue.stringValue = ConvertUtil.Stringify(EditorGUI.Vector3Field(r1, GUIContent.none, ConvertUtil.ToVector3(propValue.stringValue)));
                        propRef.objectReferenceValue = null;
                        break;
                    case VariantType.Vector4:
                        propValue.stringValue = ConvertUtil.Stringify(EditorGUI.Vector4Field(r1, (string)null, ConvertUtil.ToVector4(propValue.stringValue)));
                        propRef.objectReferenceValue = null;
                        break;
                    case VariantType.Quaternion:
                        propValue.stringValue = ConvertUtil.Stringify(SPEditorGUI.QuaternionField(r1, GUIContent.none, ConvertUtil.ToQuaternion(propValue.stringValue)));
                        propRef.objectReferenceValue = null;
                        break;
                    case VariantType.Color:
                        propValue.stringValue = ConvertUtil.Stringify(EditorGUI.ColorField(r1, ConvertUtil.ToColor(propValue.stringValue)));
                        propRef.objectReferenceValue = null;
                        break;
                    case VariantType.DateTime:
                        //TODO - should never actually occur
                        propValue.stringValue = string.Empty;
                        propRef.objectReferenceValue = null;
                        break;
                    case VariantType.GameObject:
                    case VariantType.Component:
                    case VariantType.Object:
                        propValue.stringValue = string.Empty;
                        propRef.objectReferenceValue = EditorGUI.ObjectField(r1, GUIContent.none, propValue.objectReferenceValue, info.PropertyType, false);
                        break;
                    case VariantType.LayerMask:
                        propValue.stringValue = SPEditorGUI.LayerMaskField(r1, GUIContent.none, ConvertUtil.ToInt(propValue.stringValue)).ToString();
                        propRef.objectReferenceValue = null;
                        break;
                    case VariantType.Rect:
                        //TODO - should never actually occur
                        propValue.stringValue = string.Empty;
                        propRef.objectReferenceValue = null;
                        break;
                    case VariantType.Numeric:

                        break;
                }
            }
        }

    }

}
