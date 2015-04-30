using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base.Commands
{
    public class ComponentSearchWindow : EditorWindow
    {

        public const string MENU_NAME = SPMenu.MENU_NAME_ROOT + "/Find References To Scripts";
        public const int MENU_PRIORITY = SPMenu.MENU_PRIORITY_GROUP2;

        #region Singleton

        private static ComponentSearchWindow _window;

        private void OnEnable()
        {
            if (_window == null) _window = this;
            else Object.DestroyImmediate(this);
        }

        private void OnDisable()
        {
            if (_window == this) _window = null;
        }

        #endregion

        #region Menu Entries

        [MenuItem(MENU_NAME, priority = MENU_PRIORITY)]
        private static void OpenFromMenu()
        {
            if(_window == null)
            {
                _window = EditorWindow.GetWindow<ComponentSearchWindow>();
                _window.Show();
                _window.position = new Rect(20, 80, 500, 300);
            }
            else
            {
                _window.Focus();
            }
        }

        [MenuItem("Assets/Find References In Prefabs")]
        private static void DoFind(MenuCommand data)
        {
            var script = Selection.activeObject as MonoScript;
            if (script == null) return;

            var klass = script.GetClass();
            if (typeof(Component).IsAssignableFrom(klass))
            {
                if (_window == null)
                {
                    _window = EditorWindow.GetWindow<ComponentSearchWindow>();
                    _window._mode = 0;
                    _window._targetScript = script;
                    _window._forceRefresh = true;
                    _window.Show();
                    _window.position = new Rect(20, 80, 500, 300);
                }
                else
                {
                    _window._mode = 0;
                    _window._targetScript = script;
                    _window._forceRefresh = true;
                    _window.Focus();
                }
            }
        }

        [MenuItem("Assets/Find References In Prefabs", validate = true)]
        private static bool ValidateDoFind(MenuCommand data)
        {
            return Selection.activeObject is MonoScript;
        }

        #endregion

        #region Fields

        private string[] _modes = new string[] { "Search for component usage in Prefabs", "Search for component usage in Scene", "Search for missing components in Prefabs", "Search for missing components in Scene" };
        private int _mode;

        private List<string> _prefabResults = new List<string>();
        private List<GameObject> _sceneResults = new List<GameObject>();
        private bool _forceRefresh;

        private MonoScript _targetScript;

        private Vector2 _scrollPos;

        #endregion

        #region Methods

        private void OnGUI()
        {
            //draw header
            GUILayout.Space(3);
            EditorGUI.BeginChangeCheck();
            _mode = SPEditorGUILayout.SelectionTabs(_mode, _modes, 2);
            if(EditorGUI.EndChangeCheck())
            {
                _prefabResults.Clear();
                _sceneResults.Clear();
                _forceRefresh = true;
            }
            GUILayout.Space(3);

            switch(_mode)
            {
                case 0:
                    EditorGUI.BeginChangeCheck();
                    _targetScript = EditorGUILayout.ObjectField(_targetScript, typeof(MonoScript), false) as MonoScript;
                    if (EditorGUI.EndChangeCheck() || _forceRefresh)
                    {
                        this.FillBySearchForComponentUsage(false);
                    }
                    if (_prefabResults.Count == 0)
                    {
                        var msgStyle = new GUIStyle(GUI.skin.label);
                        msgStyle.alignment = TextAnchor.MiddleCenter;
                        msgStyle.fontStyle = FontStyle.Bold;
                        string msg = (_targetScript == null) ? "Choose a script file." : "No prefabs use component " + _targetScript.name;
                        EditorGUI.LabelField(new Rect(0f, this.position.height / 2f, this.position.width, EditorGUIUtility.singleLineHeight), msg, msgStyle);
                    }
                    else
                    {
                        this.DoDrawPrefabList();
                    }
                    break;
                case 1:
                    EditorGUI.BeginChangeCheck();
                    _targetScript = EditorGUILayout.ObjectField(_targetScript, typeof(MonoScript), false) as MonoScript;
                    if (EditorGUI.EndChangeCheck() || _forceRefresh)
                    {
                        this.FillBySearchForComponentUsage(true);
                    }
                    if (_prefabResults.Count == 0)
                    {
                        var msgStyle = new GUIStyle(GUI.skin.label);
                        msgStyle.alignment = TextAnchor.MiddleCenter;
                        msgStyle.fontStyle = FontStyle.Bold;
                        string msg = (_targetScript == null) ? "Choose a script file." : "No scene objects use component " + _targetScript.name;
                        EditorGUI.LabelField(new Rect(0f, this.position.height / 2f, this.position.width, EditorGUIUtility.singleLineHeight), msg, msgStyle);
                    }
                    else
                    {
                        this.DoDrawSceneList();
                    }
                    break;
                case 2:
                    if(GUILayout.Button("Search!"))
                    {
                        this.FillBySearchForMissingComponents(false);
                    }
                    if (_prefabResults.Count == 0)
                    {
                        var msgStyle = new GUIStyle(GUI.skin.label);
                        msgStyle.alignment = TextAnchor.MiddleCenter;
                        msgStyle.fontStyle = FontStyle.Bold;
                        EditorGUI.LabelField(new Rect(0f, this.position.height / 2f, this.position.width, EditorGUIUtility.singleLineHeight * 2f), "No prefabs are missing any script references!\nClick Search to check again.", msgStyle);
                    }
                    else
                    {
                        this.DoDrawPrefabList();
                    }
                    break;
                case 3:
                    if(GUILayout.Button("Search!"))
                    {
                        this.FillBySearchForMissingComponents(true);
                    }
                    if (_prefabResults.Count == 0)
                    {
                        var msgStyle = new GUIStyle(GUI.skin.label);
                        msgStyle.alignment = TextAnchor.MiddleCenter;
                        msgStyle.fontStyle = FontStyle.Bold;
                        EditorGUI.LabelField(new Rect(0f, this.position.height / 2f, this.position.width, EditorGUIUtility.singleLineHeight * 2f), "No scene objects are missing any script references!\nClick Search to check again.", msgStyle);
                    }
                    else
                    {
                        this.DoDrawSceneList();
                    }
                    break;
            }

            _forceRefresh = false;
        }

        private void DoDrawPrefabList()
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            float w = this.position.width - 10f;
            float lw = w * 0.8f;
            float bw = w * 0.2f;
            foreach(var p in _prefabResults)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(p, GUILayout.Width(lw));
                if(GUILayout.Button("Select", GUILayout.Width(bw)))
                {
                    EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(p));
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }

        private void DoDrawSceneList()
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            float w = this.position.width - 10f;
            float lw = w * 0.8f;
            float bw = w * 0.2f;
            foreach (var go in _sceneResults)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(go.transform.GetHiearchyPathName(), GUILayout.Width(lw));
                if (GUILayout.Button("Select", GUILayout.Width(bw)))
                {
                    Selection.activeObject = go;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }




        private void FillBySearchForComponentUsage(bool testScene)
        {
            _prefabResults.Clear();
            _sceneResults.Clear();

            if (_targetScript != null)
            {
                if(testScene)
                {
                    foreach (var go in GameObject.FindObjectsOfType<GameObject>())
                    {
                        if (go.HasComponent(_targetScript.GetClass())) _sceneResults.Add(go);
                    }
                }
                else
                {
                    _prefabResults.AddRange(PrefabHelper.GetAllPrefabAssetPathsDependentOn(_targetScript));
                }
            }
        }
        
        private void FillBySearchForMissingComponents(bool testScene)
        {
            _prefabResults.Clear();
            _sceneResults.Clear();

            if (_targetScript != null)
            {
                if (testScene)
                {
                    foreach(var go in GameObject.FindObjectsOfType<GameObject>())
                    {
                        foreach(var c in go.GetComponents<Component>())
                        {
                            if(c == null)
                            {
                                _sceneResults.Add(go);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    GameObject go;
                    foreach(var p in PrefabHelper.GetAllPrefabAssetPaths())
                    {
                        go = AssetDatabase.LoadMainAssetAtPath(p) as GameObject;
                        if(go != null && PrefabUtility.GetPrefabType(go) == PrefabType.Prefab)
                        {
                            foreach(var c in go.GetComponentsInChildren<Component>(true))
                            {
                                if (c == null) _prefabResults.Add(p);
                            }
                        }
                    }
                }
            }
        }

        #endregion

    }
}
