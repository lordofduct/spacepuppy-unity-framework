using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

using com.spacepuppy.Waypoints;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Waypoints
{

    [CustomEditor(typeof(WaypointPathComponent))]
    public class WaypointPathComponentInspector : SPEditor
    {

        #region Fields

        private const float SEG_LENGTH = 0.2f;

        private const string PROP_WAYPOINTS = "_waypoints";
        private const string PROP_NODE_TRANSFORM = "_transform";
        private const float ARG_BTN_WIDTH = 18f;

        private WaypointPathComponent _targ;

        private SerializedProperty _nodesProp;
        private ReorderableList _nodeList;
        private GUIContent _argBtnLabel = new GUIContent("||", "Toggle allowing to manually configure which Transform is this waypoint.");

        private List<Transform> _lastNodeCache = new List<Transform>();
        private IEnumerable<Transform> _currentNodes;

        #endregion

        #region CONSTRUCTOR

        protected override void OnEnable()
        {
            base.OnEnable();

            _targ = this.target as WaypointPathComponent;

            _nodesProp = this.serializedObject.FindProperty(PROP_WAYPOINTS);
            _nodeList = new ReorderableList(this.serializedObject, _nodesProp);
            _nodeList.elementHeight = EditorGUIUtility.singleLineHeight;
            _nodeList.drawHeaderCallback = _nodeList_DrawHeader;
            _nodeList.drawElementCallback = _nodeList_DrawElement;
            _nodeList.onAddCallback = _nodeList_OnAdded;

            _currentNodes = this.GetCurrentNodes();
            _lastNodeCache.Clear();
            _lastNodeCache.AddRange(_currentNodes);
        }

        #endregion

        #region Methods

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawDefaultInspectorExcept(PROP_WAYPOINTS);

            //clean nodes
            for(int i = 0; i < _nodesProp.arraySize; i++)
            {
                if(_nodesProp.GetArrayElementAtIndex(i).FindPropertyRelative(PROP_NODE_TRANSFORM).objectReferenceValue == null)
                {
                    _nodesProp.DeleteArrayElementAtIndex(i);
                    i--;
                }
            }
            //_currentNodes = this.GetCurrentNodes();

            if (_nodesProp.arraySize != _lastNodeCache.Count || !_lastNodeCache.SequenceEqual(_currentNodes))
            {
                //delete any nodes that are no longer in the collection
                for (int i = 0; i < _lastNodeCache.Count; i++ )
                {
                    if(_lastNodeCache[i] != null && !_currentNodes.Contains(_lastNodeCache[i]) && _lastNodeCache[i].parent == _targ.transform)
                    {
                        ObjUtil.SmartDestroy(_lastNodeCache[i].gameObject);
                    }
                }

                //update cache
                _lastNodeCache.Clear();
                _lastNodeCache.AddRange(_currentNodes);

                //update names
                var rx = new Regex(@"^Node\d+$");
                for(int i = 0; i < _lastNodeCache.Count; i++)
                {
                    var node = _lastNodeCache[i];
                    var nm = "Node" + i.ToString("000");
                    if (node != null && node.parent == _targ.transform && node.name != nm && rx.IsMatch(node.name))
                    {
                        node.name = nm;
                    }
                }
            }


            _nodeList.DoLayoutList();

            this.serializedObject.ApplyModifiedProperties();
        }

        private IEnumerable<Transform> GetCurrentNodes()
        {
            for(int i = 0; i < _nodesProp.arraySize; i++)
            {
                yield return _nodesProp.GetArrayElementAtIndex(i).FindPropertyRelative(PROP_NODE_TRANSFORM).objectReferenceValue as Transform;
            }
        }

        #endregion

        #region NodeList Handlers

        private void _nodeList_DrawHeader(Rect area)
        {
            EditorGUI.LabelField(area, "Waypoints");
        }

        private void _nodeList_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            if (area.width < ARG_BTN_WIDTH)
            {
                return;
            }

            var elementProp = _nodesProp.GetArrayElementAtIndex(index);
            var transformProp = elementProp.FindPropertyRelative(PROP_NODE_TRANSFORM);
            var btnRect = new Rect(area.xMax - ARG_BTN_WIDTH, area.yMin, ARG_BTN_WIDTH, area.height);
            var propRect = new Rect(area.xMin, area.yMin, area.width - ARG_BTN_WIDTH, area.height);

            if(transformProp.objectReferenceValue == null || elementProp.isExpanded)
            {
                EditorGUI.PropertyField(propRect, transformProp, GUIContent.none);
            }
            else
            {
                var t = transformProp.objectReferenceValue as Transform;
                EditorGUI.LabelField(propRect, EditorHelper.TempContent(t.name), EditorHelper.TempContent("{ strength: " + t.localScale.z.ToString("0.00") + " }"));

                if(ReorderableListHelper.IsClickingArea(area))
                {
                    EditorGUIUtility.PingObject(t);
                }
            }

            if(GUI.Button(btnRect, _argBtnLabel))
            {
                elementProp.isExpanded = !elementProp.isExpanded;
            }

            if (GUI.enabled) ReorderableListHelper.DrawDraggableElementDeleteContextMenu(_nodeList, area, index, isActive, isFocused);
        }

        private void _nodeList_OnAdded(ReorderableList lst)
        {
            lst.serializedProperty.arraySize++;
            lst.index = lst.serializedProperty.arraySize - 1;

            var elementProp = lst.serializedProperty.GetArrayElementAtIndex(lst.index);
            var transProp = elementProp.FindPropertyRelative(PROP_NODE_TRANSFORM);
            var lastNode = (lst.index > 1) ? lst.serializedProperty.GetArrayElementAtIndex(lst.index - 1).FindPropertyRelative(PROP_NODE_TRANSFORM).objectReferenceValue as Transform : null;
            
            var go = new GameObject("Node" + lst.index.ToString("000"));
            IconHelper.SetIconForObject(go, IconHelper.Icon.DiamondPurple);
            go.transform.parent = _targ.transform;
            if (lastNode != null)
            {
                go.transform.position = lastNode.position;
                go.transform.rotation = lastNode.rotation;
                go.transform.localScale = lastNode.localScale;
            }
            else
            {
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.LookRotation(Vector3.right);
                go.transform.localScale = Vector3.one * 0.5f;
            }
            transProp.objectReferenceValue = go.transform;
        }

        #endregion


        #region Gizmos

        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
        private static void OnDrawGizmos(WaypointPathComponent c, GizmoType gizmoType)
        {
            if (gizmoType.HasFlag(GizmoType.NotInSelectionHierarchy) && !c.transform.IsParentOf(Selection.activeTransform)) return;

            var path = WaypointPathComponent.GetPath(c, false);
            if (path == null || path.Count == 0) return;
            var matrix = (c.started && c.TransformRelativeTo != null) ? Matrix4x4.TRS(c.TransformRelativeTo.position, c.TransformRelativeTo.rotation, Vector3.one) : Matrix4x4.identity;

            Gizmos.color = Color.red;
            var pnts = path.GetDetailedPositions(SEG_LENGTH);
            for (int i = 1; i < pnts.Length; i++)
            {
                Gizmos.DrawLine(matrix.MultiplyPoint3x4(pnts[i - 1]), matrix.MultiplyPoint3x4(pnts[i]));
            }

            IWaypoint pnt;

            Gizmos.color = Color.green;
            pnt = path.ControlPoint(0);
            Gizmos.DrawWireCube(matrix.MultiplyPoint3x4(pnt.Position), Vector3.one * 0.5f);

            if (path.Count > 1)
            {
                Gizmos.color = Color.yellow;
                for (int i = 1; i < path.Count - 1; i++)
                {
                    pnt = path.ControlPoint(i);
                    Gizmos.DrawWireSphere(matrix.MultiplyPoint3x4(pnt.Position), 0.25f);
                }

                Gizmos.color = Color.red;
                pnt = path.ControlPoint(path.Count - 1);
                Gizmos.DrawWireCube(matrix.MultiplyPoint3x4(pnt.Position), Vector3.one * 0.5f);
            }
        }

        #endregion

    }

}
