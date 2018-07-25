using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using com.spacepuppy.Project;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    public class SpacepuppySettingsWindow : EditorWindow
    {

        #region Consts

        private const float BTN_WIDTH = 275f;

        public const string MENU_NAME = SPMenu.MENU_NAME_SETTINGS + "/Spacepuppy Settings";
        public const int MENU_PRIORITY = SPMenu.MENU_PRIORITY_SETTINGS;
        
        #endregion

        #region Menu Entries

        [MenuItem(MENU_NAME, priority = MENU_PRIORITY)]
        public static void OpenWindow()
        {
            if (_openWindow == null)
            {
                EditorWindow.GetWindow<SpacepuppySettingsWindow>();
            }
            else
            {
                GUI.BringWindowToFront(_openWindow.GetInstanceID());
            }
        }

        #endregion
        
        #region Window

        private static SpacepuppySettingsWindow _openWindow;


        private GameSettings _gameSettings;
        private CustomTimeLayersData _timeLayersData;

        private Vector2 _scenesScrollBarPosition;

        private void OnEnable()
        {
            if (_openWindow == null)
                _openWindow = this;
            else
                Object.DestroyImmediate(this);

            this.titleContent = new GUIContent("SP Settings");

            _gameSettings = AssetDatabase.LoadAssetAtPath(GameSettings.PATH_DEFAULTSETTINGS_FULL, typeof(GameSettings)) as GameSettings;
            _timeLayersData = AssetDatabase.LoadAssetAtPath(CustomTimeLayersData.PATH_DEFAULTSETTINGS_FULL, typeof(CustomTimeLayersData)) as CustomTimeLayersData;
        }

        private void OnDisable()
        {
            if(_openWindow == this) _openWindow = null;
        }

        private void OnGUI()
        {
            Rect rect;

            var boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.stretchHeight = false;

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            bool storeLocal = EditorGUILayout.ToggleLeft("Store Settings Local", SpacepuppySettings.StoreSettingsLocal);
            if (EditorGUI.EndChangeCheck()) SpacepuppySettings.StoreSettingsLocal = storeLocal;

            /*
             * Editor Use
             */

            EditorGUILayout.Space();
            GUILayout.BeginVertical("Editor Settings", boxStyle);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            bool useSPEditor = EditorGUILayout.ToggleLeft("Use SPEditor as default editor for MonoBehaviour", SpacepuppySettings.UseSPEditorAsDefaultEditor);
            if (EditorGUI.EndChangeCheck()) SpacepuppySettings.UseSPEditorAsDefaultEditor = useSPEditor;

            EditorGUI.BeginChangeCheck();
            bool useAdvancedAnimInspector = EditorGUILayout.ToggleLeft("Use Advanced Animation Inspector", SpacepuppySettings.UseAdvancedAnimationInspector);
            if (EditorGUI.EndChangeCheck()) SpacepuppySettings.UseAdvancedAnimationInspector = useAdvancedAnimInspector;

            EditorGUI.BeginChangeCheck();
            bool hierarchyDrawerActive = EditorGUILayout.ToggleLeft("Use Hierarchy Drawers", SpacepuppySettings.UseHierarchDrawer);
            if (EditorGUI.EndChangeCheck())
            {
                SpacepuppySettings.UseHierarchDrawer = hierarchyDrawerActive;
                EditorHierarchyDrawerEvents.SetActive(hierarchyDrawerActive);
            }

            EditorGUI.BeginChangeCheck();
            bool hierarchCustomContextMenu = EditorGUILayout.ToggleLeft("Use Alternate Hierarchy Context Menu", SpacepuppySettings.UseHierarchyAlternateContextMenu);
            if(EditorGUI.EndChangeCheck())
            {
                SpacepuppySettings.UseHierarchyAlternateContextMenu = hierarchCustomContextMenu;
                EditorHierarchyAlternateContextMenuEvents.SetActive(hierarchCustomContextMenu);
            }

            GUILayout.EndVertical();

            /*
             * Material Search Settings
             */

            GUILayout.BeginVertical("Material Settings", boxStyle);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            bool setMaterialSearch = EditorGUILayout.ToggleLeft("Configure Material Settings On Import", SpacepuppySettings.SetMaterialSearchOnImport);
            if (EditorGUI.EndChangeCheck()) SpacepuppySettings.SetMaterialSearchOnImport = setMaterialSearch;

            EditorGUI.BeginChangeCheck();
            var materialSearch = (ModelImporterMaterialSearch)EditorGUILayout.EnumPopup("Material Search", SpacepuppySettings.MaterialSearch);
            if (EditorGUI.EndChangeCheck()) SpacepuppySettings.MaterialSearch = materialSearch;

            GUILayout.EndVertical();

            /*
             * Animation Settings
             */

            GUILayout.BeginVertical("Animation Settings", boxStyle);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            bool setAnimSettings = EditorGUILayout.ToggleLeft("Configure Animation Settings On Import", SpacepuppySettings.SetAnimationSettingsOnImport);
            if (EditorGUI.EndChangeCheck()) SpacepuppySettings.SetAnimationSettingsOnImport = setAnimSettings;

            EditorGUI.BeginChangeCheck();
            var animRigType = (ModelImporterAnimationType)EditorGUILayout.EnumPopup("Aimation Rig Type", SpacepuppySettings.ImportAnimRigType);
            if (EditorGUI.EndChangeCheck()) SpacepuppySettings.ImportAnimRigType = animRigType;

            GUILayout.EndVertical();

            /*
             * Game Settings
             */

            GUILayout.BeginVertical("Game Settings", boxStyle);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (_gameSettings == null)
            {
                rect = EditorGUILayout.GetControlRect();
                rect.width = Mathf.Min(rect.width, BTN_WIDTH);

                if (GUI.Button(rect, "Create Default GameSettings Data Resource"))
                {
                    var tps = (from t in TypeUtil.GetTypesAssignableFrom(typeof(GameSettings)) where !t.IsAbstract && !t.IsInterface select t).ToArray();
                    
                    var menu = new GenericMenu();
                    foreach(var tp in tps)
                    {
                        menu.AddItem(EditorHelper.TempContent(tp.Name), false, () =>
                        {
                            _gameSettings = ScriptableObjectHelper.CreateAsset(tp, GameSettings.PATH_DEFAULTSETTINGS_FULL) as GameSettings;
                        });
                    }
                    menu.ShowAsContext();
                }
            }
            else
            {
                EditorGUILayout.ObjectField("Game Settings", _gameSettings, typeof(GameSettings), false);
            }

            EditorGUILayout.Space();

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

            GUILayout.EndVertical();




            EditorGUILayout.Space();
            EditorGUILayout.Space();
            this.DrawScenes();
        }

        #endregion













        #region Debug Build Scenes - OBSOLETE

        private void DrawScenes()
        {
            EditorGUILayout.LabelField("Scenes in Build", EditorStyles.boldLabel);

            //EditorGUI.indentLevel++;
            
            var style = new GUIStyle(GUI.skin.box);
            style.stretchHeight = true;
            _scenesScrollBarPosition = EditorGUILayout.BeginScrollView(_scenesScrollBarPosition, style);

            bool changed = false;
            var scenes = EditorBuildSettings.scenes;
            for (int i = 0; i < scenes.Length; i++)
            {
                var r = EditorGUILayout.GetControlRect();
                var r0 = new Rect(r.xMin, r.yMin, 25f, r.height);
                var r1 = new Rect(r0.xMax, r.yMin, (r.xMax - r0.xMax) * 0.66f, r.height);
                var r2 = new Rect(r1.xMax, r.yMin, r.xMax - r1.xMax, r.height);

                EditorGUI.BeginChangeCheck();
                scenes[i].enabled = EditorGUI.Toggle(r0, scenes[i].enabled);
                if (EditorGUI.EndChangeCheck()) changed = true;

                EditorGUI.LabelField(r1, EditorHelper.TempContent(scenes[i].path));
                var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenes[i].path);
                EditorGUI.BeginChangeCheck();
                scene = EditorGUI.ObjectField(r2, GUIContent.none, scene, typeof(SceneAsset), false) as SceneAsset;
                if (EditorGUI.EndChangeCheck())
                {
                    changed = true;
                    scenes[i] = new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(scene), scenes[i].enabled);
                }
            }
            if (changed) EditorBuildSettings.scenes = scenes;

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
            if (GUI.Button(deselectPosition, new GUIContent("Deselect All")))
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
            if (GUI.Button(applyPosition, new GUIContent("Add All")))
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



            //EditorGUI.indentLevel--;

        }

        /*
        private void DrawScenes()
        {
            EditorGUILayout.LabelField("Scenes in Build", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            //var h = (this.position.height - GUILayoutUtility.GetLastRect().yMax);
            //if (h < 0f) h = 0f;
            //_scenesScrollBarPosition = EditorGUILayout.BeginScrollView(_scenesScrollBarPosition, GUI.skin.box, GUILayout.Height(h));

            var style = new GUIStyle(GUI.skin.box);
            style.stretchHeight = true;
            _scenesScrollBarPosition = EditorGUILayout.BeginScrollView(_scenesScrollBarPosition, style);

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
        */
    
        #endregion

    }

}
