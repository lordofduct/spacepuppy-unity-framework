using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.AI;
using com.spacepuppy.AI.Components;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.AI
{

    public class AITreeDebugWindow : EditorWindow
    {

        public const string MENU_NAME = SPMenu.MENU_NAME_TOOLS + "/AI Tree Debug Window";
        public const int MENU_PRIORITY = SPMenu.MENU_PRIORITY_TOOLS;

        #region Menu

        private static AITreeDebugWindow _window;

        [MenuItem(MENU_NAME, priority = MENU_PRIORITY)]
        private static void OpenWindow()
        {
            if (_window == null)
            {
                _window = EditorWindow.GetWindow<AITreeDebugWindow>();
                _window.Show();
                _window.position = new Rect(20, 80, 500, 300);
            }
            else
            {
                _window.Focus();
            }
        }

        #endregion

        #region Fields

        private AITreeController _ai;

        private Color _failedColor = Color.red.SetAlpha(0.1f);
        private Color _waitingColor = Color.yellow.SetAlpha(0.1f);
        private Color _successColor = Color.green.SetAlpha(0.1f);
        private Color _activeStateColor = Color.blue.SetAlpha(0.1f);
        private Color _selectedColor = Color.grey.SetAlpha(0.5f);

        private Dictionary<int, AITreeExpandedLookupTable> _allTables = new Dictionary<int, AITreeExpandedLookupTable>();
        private AITreeExpandedLookupTable _currentTable;
        private int _currentSelectionId;

        private Vector2 _scrollPosition;

        #endregion

        #region CONSTRUCTOR

        private void OnEnable()
        {
            this.titleContent = new GUIContent("AI Tree Debug Window");
            this.autoRepaintOnSceneChange = true;
            this.UpdateAIFromSelection();
        }

        #endregion

        #region OnGUI

        private void OnGUI()
        {
            //draw header
            if (Application.isPlaying)
            {
                var cache = SPGUI.Disable();
                EditorGUILayout.ObjectField("Current Target: ", _ai, typeof(AITreeController), true);
                cache.Reset();
            }
            else
            {
                var rect = EditorGUILayout.GetControlRect();
                Rect r1, r2;
                const float BTN_WIDTH = 150f;
                if (rect.width > BTN_WIDTH * 2f)
                {
                    r1 = new Rect(rect.xMin, rect.yMin, rect.width - BTN_WIDTH, rect.height);
                    r2 = new Rect(r1.xMax, rect.yMin, BTN_WIDTH, rect.height);
                }
                else
                {
                    var w = rect.width / 2f;
                    r1 = new Rect(rect.xMin, rect.yMin, w, rect.height);
                    r2 = new Rect(r1.xMax, rect.yMin, w, rect.height);
                }

                var cache = SPGUI.Disable();
                EditorGUI.ObjectField(r1, "Current Target: ", _ai, typeof(AITreeController), true);
                cache.Reset();

                if (GUI.Button(r2, "Sync Actions"))
                {
                    this.SyncActions();
                }
            }

            //if we're not configured correctly, stop
            if (_ai == null) return;
            if (_currentTable == null) this.CleanCurrentState();

            //draw actions
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            try
            {
                this.DrawNode(_ai);
                //this.DrawActionGroup(_ai);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
            EditorGUILayout.EndScrollView();

        }

        private void OnSelectionChange()
        {
            this.UpdateAIFromSelection();
        }

        private void OnFocus()
        {
            this.UpdateAIFromSelection();
        }

        #endregion

        #region Commands

        private void UpdateAIFromSelection()
        {
            if (Selection.activeGameObject == null) return;
            
            var c = Selection.activeGameObject.GetComponentInParent<AITreeController>();
            if (c != null)
            {
                _ai = c;
                this.SyncActions();
                this.CleanCurrentState();
                this.Repaint();
                return;
            }
        }

        private void SyncActions()
        {
            if (Application.isPlaying) return;
            if (_ai == null) return;

            _ai.SyncActions();

            foreach(var n in AINodeUtil.GetAllNodes(_ai.ActionLoop, false))
            {
                if (n is IAIEditorSyncActionsCallbackReceiver) (n as IAIEditorSyncActionsCallbackReceiver).SyncActions();
            }
        }

        private void CleanCurrentState()
        {
            //var deadTreeIds = (from pair in _allTables where !pair.Value.GetIsAlive() select pair.Key).ToArray();
            //foreach (var id in deadTreeIds)
            //{
            //    _allTables.Remove(id);
            //}

            if(_ai != null)
            {
                int id = _ai.GetInstanceID();
                AITreeExpandedLookupTable table;
                if(!_allTables.TryGetValue(id, out table))
                {
                    table = new AITreeExpandedLookupTable(id);
                    _allTables.Add(id, table);
                }
                _currentTable = table;
            }
            else
            {
                _currentTable = null;
            }

            //_currentSelectionId = 0;
        }

        #endregion

        #region Draw Methods

        private void DrawNode(IAINode node)
        {
            var rect = EditorGUILayout.GetControlRect();
            bool hasChildren = (node is IAIActionGroup || node is IAIStateMachine);

            //first click region
            if (MouseUtil.GuiClicked(Event.current, MouseUtil.BTN_LEFT, rect))
            {
                _currentSelectionId = AITreeDebugWindow.GetNodeHash(node);
                if (node is Component)
                {
                    Selection.activeGameObject = (node as Component).gameObject;
                    EditorGUIUtility.PingObject(node as Component);
                }
                this.Repaint();
            }
            else if (MouseUtil.GuiClicked(Event.current, MouseUtil.BTN_RIGHT, rect))
            {
                this.DrawPopup(node);
            }

            //draw selection rect
            if (_currentSelectionId == AITreeDebugWindow.GetNodeHash(node))
            {
                EditorGUI.DrawRect(rect, _selectedColor);
            }

            //draw label
            if (hasChildren)
            {
                _currentTable[node] = EditorGUI.Foldout(rect, _currentTable[node], node.DisplayName);
            }
            else
            {
                EditorGUI.LabelField(rect, node.DisplayName);
            }

            //highlight if running
            if(node is IAIAction)
            {
                var act = node as IAIAction;
                switch (act.ActionState)
                {
                    case ActionResult.Waiting:
                        EditorGUI.DrawRect(rect, _waitingColor);
                        break;
                    case ActionResult.Success:
                        EditorGUI.DrawRect(rect, _successColor);
                        break;
                    case ActionResult.Failed:
                        EditorGUI.DrawRect(rect, _failedColor);
                        break;
                }
            }
            else if(node is IAIState)
            {
                var state = node as IAIState;
                if(state.IsActive)
                {
                    EditorGUI.DrawRect(rect, _activeStateColor);
                }
            }


            if (hasChildren && _currentTable[node])
            {
                EditorGUI.indentLevel++;
                if(node is IAIActionGroup)
                {
                    foreach(var child in (node as IAIActionGroup))
                    {
                        this.DrawNode(child);
                    }
                }
                else if(node is IAIStateMachine)
                {
                    foreach (var child in (node as IAIStateMachine))
                    {
                        this.DrawNode(child);
                    }
                }
                EditorGUI.indentLevel--;
            }
        }
        
        private void DrawPopup(IAINode node)
        {
            
        }

        #endregion



        #region Static Utils

        private static int GetNodeHash(IAINode node)
        {
            if (node is Component)
            {
                return (node as Component).GetInstanceID();
            }
            else
            {
                //TODO - need better method of doing this
                return node.GetHashCode();
            }
        }

        #endregion

        #region Special Types

        private class AITreeExpandedLookupTable
        {

            #region Fields

            private int _ownerInstanceId;
            private Dictionary<int, bool> _table = new Dictionary<int, bool>();

            #endregion

            #region CONSTRUCTOR

            public AITreeExpandedLookupTable(AITreeController controller)
            {
                _ownerInstanceId = controller.GetInstanceID();
            }

            public AITreeExpandedLookupTable(int id)
            {
                _ownerInstanceId = id;
            }

            #endregion

            #region Properties

            public bool this[IAINode node]
            {
                get
                {
                    if (node.IsNullOrDestroyed()) return true;

                    bool result;
                    if (_table.TryGetValue(AITreeDebugWindow.GetNodeHash(node), out result))
                    {
                        return result;
                    }
                    return true;
                }
                set
                {
                    if (node.IsNullOrDestroyed()) return;

                    _table[AITreeDebugWindow.GetNodeHash(node)] = value;
                }
            }

            #endregion

            #region Methods

            public bool GetIsAlive()
            {
                return !EditorUtility.InstanceIDToObject(_ownerInstanceId).IsNullOrDestroyed();
            }

            public void Clean()
            {

            }

            #endregion

        }

        #endregion


    }
}
