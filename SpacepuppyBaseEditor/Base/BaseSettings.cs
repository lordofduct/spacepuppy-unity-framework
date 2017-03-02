using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using com.spacepuppy.Project;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{
    public class BaseSettings : EditorWindow
    {

        #region Consts

        private const float BTN_WIDTH = 275f;

        public const string MENU_NAME = SPMenu.MENU_NAME_SETTINGS + "/Base Settings";
        public const int MENU_PRIORITY = SPMenu.MENU_PRIORITY_SETTINGS;
        public const string SETTING_SPEDITOR_ISDEFAULT_ACTIVE = "UseSPEditor.IsDefault.Active";
        public const string SETTING_ADVANCEDANIMINSPECTOR_ACTIVE = "AdvancedAnimationInspector.Active";
        public const string SETTING_HIERARCHYDRAWER_ACTIVE = "EditorHierarchyEvents.Active";
        public const string SETTING_HIEARCHYALTERNATECONTEXTMENU_ACTIVE = "EditorHierarchyAlternateContextMenu.Active";

        #endregion

        #region Menu Entries

        [MenuItem(MENU_NAME, priority = MENU_PRIORITY)]
        public static void OpenBaseSettings()
        {
            if (_openWindow == null)
            {
                EditorWindow.GetWindow<BaseSettings>();
            }
            else
            {
                GUI.BringWindowToFront(_openWindow.GetInstanceID());
            }
        }

        #endregion

        #region Window

        private static BaseSettings _openWindow;


        private GameSettingsBase _gameSettings;
        private CustomTimeLayersData _timeLayersData;

        private Vector2 _scenesScrollBarPosition;

        private void OnEnable()
        {
            if (_openWindow == null)
                _openWindow = this;
            else
                Object.DestroyImmediate(this);

            this.titleContent = new GUIContent("Base Settings");

            _gameSettings = AssetDatabase.LoadAssetAtPath(GameSettingsBase.PATH_DEFAULTSETTINGS_FULL, typeof(GameSettingsBase)) as GameSettingsBase;
            _timeLayersData = AssetDatabase.LoadAssetAtPath(CustomTimeLayersData.PATH_DEFAULTSETTINGS_FULL, typeof(CustomTimeLayersData)) as CustomTimeLayersData;
        }

        private void OnDisable()
        {
            if(_openWindow == this) _openWindow = null;
        }

        private void OnGUI()
        {
            Rect rect;

            //var labelWidthCache = EditorGUIUtility.labelWidth;
            //EditorGUIUtility.labelWidth = Mathf.Min(this.position.width - 20f, 300f);

            EditorGUI.BeginChangeCheck();
            bool useSPEditor = EditorGUILayout.ToggleLeft("Use SPEditor as default editor for MonoBehaviour", EditorProjectPrefs.Local.GetBool(BaseSettings.SETTING_SPEDITOR_ISDEFAULT_ACTIVE, true));
            if (EditorGUI.EndChangeCheck()) EditorProjectPrefs.Local.SetBool(BaseSettings.SETTING_SPEDITOR_ISDEFAULT_ACTIVE, useSPEditor);

            EditorGUI.BeginChangeCheck();
            bool useAdvancedAnimInspector = EditorGUILayout.ToggleLeft("Use Advanced Animation Inspector", EditorProjectPrefs.Local.GetBool(BaseSettings.SETTING_ADVANCEDANIMINSPECTOR_ACTIVE, true));
            if (EditorGUI.EndChangeCheck()) EditorProjectPrefs.Local.SetBool(BaseSettings.SETTING_ADVANCEDANIMINSPECTOR_ACTIVE, useAdvancedAnimInspector);

            EditorGUI.BeginChangeCheck();
            bool hierarchyDrawerActive = EditorGUILayout.ToggleLeft("Use Hierarchy Drawers", EditorProjectPrefs.Local.GetBool(BaseSettings.SETTING_HIERARCHYDRAWER_ACTIVE, true));
            if (EditorGUI.EndChangeCheck())
            {
                EditorProjectPrefs.Local.SetBool(BaseSettings.SETTING_HIERARCHYDRAWER_ACTIVE, hierarchyDrawerActive);
                EditorHierarchyDrawerEvents.SetActive(hierarchyDrawerActive);
            }

            EditorGUI.BeginChangeCheck();
            bool hierarchCustomContextMenu = EditorGUILayout.ToggleLeft("Use Alternate Hierarchy Context Menu", EditorProjectPrefs.Local.GetBool(BaseSettings.SETTING_HIEARCHYALTERNATECONTEXTMENU_ACTIVE, true));
            if(EditorGUI.EndChangeCheck())
            {
                EditorProjectPrefs.Local.SetBool(BaseSettings.SETTING_HIEARCHYALTERNATECONTEXTMENU_ACTIVE, hierarchCustomContextMenu);
                EditorHierarchyAlternateContextMenuEvents.SetActive(hierarchCustomContextMenu);
            }

            if (_gameSettings == null)
            {
                rect = EditorGUILayout.GetControlRect();
                rect.width = Mathf.Min(rect.width, BTN_WIDTH);

                if (GUI.Button(rect, "Create Default GameSettings Data Resource"))
                {
                    var tps = (from t in TypeUtil.GetTypesAssignableFrom(typeof(GameSettingsBase)) where !t.IsAbstract && !t.IsInterface select t).ToArray();
                    
                    var menu = new GenericMenu();
                    foreach(var tp in tps)
                    {
                        menu.AddItem(EditorHelper.TempContent(tp.Name), false, () =>
                        {
                            _gameSettings = ScriptableObjectHelper.CreateAsset(tp, GameSettingsBase.PATH_DEFAULTSETTINGS_FULL) as GameSettingsBase;
                        });
                    }
                    menu.ShowAsContext();
                }
            }
            else
            {
                EditorGUILayout.ObjectField("Game Settings", _gameSettings, typeof(GameSettingsBase), false);
            }

            if (_timeLayersData == null)
            {
                rect = EditorGUILayout.GetControlRect();
                rect.width = Mathf.Min(rect.width, BTN_WIDTH);
                if(GUI.Button(rect, "Create Custom Time Layers Data Resource"))
                {
                    _timeLayersData = ScriptableObjectHelper.CreateAsset<CustomTimeLayersData>(CustomTimeLayersData.PATH_DEFAULTSETTINGS_FULL);
                }
            }
            else
            {
                EditorGUILayout.ObjectField("Custom Time Layers Data", _timeLayersData, typeof(CustomTimeLayersData), false);
            }




            EditorGUILayout.Space();
            EditorGUILayout.Space();

            this.DrawScenes();


            //EditorGUIUtility.labelWidth = labelWidthCache;
        }


        #endregion

        #region Debug Build Scenes

        private void DrawScenes()
        {
            EditorGUILayout.LabelField("Scenes in Build", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            _scenesScrollBarPosition = EditorGUILayout.BeginScrollView(_scenesScrollBarPosition, GUI.skin.box, GUILayout.Height(200f));
            
            EditorGUI.BeginChangeCheck();
            var scenes = EditorBuildSettings.scenes;
            foreach (var scene in scenes)
            {
                //EditorGUILayout.LabelField(scene.path);
                scene.enabled = EditorGUILayout.ToggleLeft(EditorHelper.TempContent(scene.path), scene.enabled);
            }
            if (EditorGUI.EndChangeCheck()) EditorBuildSettings.scenes = scenes;

            EditorGUILayout.EndScrollView();

            //DRAG & DROP ON SCROLLVIEW
            var dropArea = GUILayoutUtility.GetLastRect();

            var ev = Event.current;
            switch (ev.type)
            {
                case EventType.DragUpdated:
                    if (dropArea.Contains(ev.mousePosition) && (from o in DragAndDrop.objectReferences where o is SceneAsset select o).Any())
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    break;
                case EventType.DragPerform:
                    if (dropArea.Contains(ev.mousePosition) && (from o in DragAndDrop.objectReferences where o is SceneAsset select o).Any())
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        DragAndDrop.AcceptDrag();

                        var lst = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                        foreach (var o in DragAndDrop.objectReferences)
                        {
                            var scene = o as SceneAsset;
                            if (scene == null) continue;

                            var p = AssetDatabase.GetAssetPath(scene);
                            if (!(from s in lst where s.path == p select s).Any())
                            {
                                lst.Add(new EditorBuildSettingsScene(p, true));
                            }
                        }
                        EditorBuildSettings.scenes = lst.ToArray();
                    }
                    break;

            }



            ////////////////
            //BUTTONS!
            var rect = EditorGUILayout.GetControlRect();

            //DESELECT
            var selectAllPosition = new Rect(rect.xMin, rect.yMin, 100f, rect.height);
            if (GUI.Button(selectAllPosition, new GUIContent("Select All")))
            {
                var arr = EditorBuildSettings.scenes;
                foreach (var s in arr)
                {
                    s.enabled = true;
                }
                EditorBuildSettings.scenes = arr;
            }

            //DESELECT
            var deselectPosition = new Rect(rect.xMin + 105f, rect.yMin, 100f, rect.height);
            if(GUI.Button(deselectPosition, new GUIContent("Deselect All")))
            {
                var arr = EditorBuildSettings.scenes;
                foreach (var s in arr)
                {
                    s.enabled = false;
                }
                EditorBuildSettings.scenes = arr;
            }

            //CLEAR
            var cancelPosition = new Rect(rect.xMax - 110f, rect.yMin, 50f, rect.height);
            if (GUI.Button(cancelPosition, new GUIContent("Clear")))
            {
                EditorBuildSettings.scenes = new EditorBuildSettingsScene[] { };
            }
            //SYNC
            var applyPosition = new Rect(rect.xMax - 55f, rect.yMin, 50f, rect.height);
            var oldScenes = EditorBuildSettings.scenes;
            if (GUI.Button(applyPosition, new GUIContent("Sync")))
            {
                var lst = new List<EditorBuildSettingsScene>();
                var mainFolder = Application.dataPath.EnsureNotEndsWith("Assets");
                foreach (var file in Directory.GetFiles(Application.dataPath + "/Scenes", "*.unity", SearchOption.AllDirectories))
                {
                    var normalizedFile = file.EnsureNotStartWith(mainFolder);
                    bool enabled = (from s in oldScenes where s.enabled && s.path == normalizedFile select s).Any();
                    lst.Add(new EditorBuildSettingsScene(normalizedFile, enabled));
                }
                EditorBuildSettings.scenes = lst.ToArray();
            }
            EditorGUI.indentLevel--;
        }

        #endregion

    }
}
