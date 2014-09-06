using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Spawn;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Inspectors.Spawn
{

    [InitializeOnLoad()]
    [CustomEditor(typeof(PlaceAtRandomNodeOnSpawn))]
    public class PlaceAtRandomNodeOnSpawnInspector : Editor
    {

        #region Fields

        private bool _expanded = false;

        #endregion

        #region Draw Inspector

        public override void OnInspectorGUI()
        {
            this.DrawDefaultInspector();

            var targ = this.target as PlaceAtRandomNodeOnSpawn;

            _expanded = EditorGUILayout.Foldout(_expanded, "Nodes");
            if(_expanded)
            {
                EditorGUI.indentLevel += 1;

                EditorGUILayout.HelpBox("Node names must begin with the word 'node' (case-insenstive).", MessageType.Info);
                
                foreach (var node in targ.GetAllNodes())
                {
                    const float btnwidth = 60f;
                    var r = EditorGUILayout.GetControlRect();
                    var r1 = new Rect(r.xMin, r.yMin, r.width - btnwidth, r.height);
                    var r2 = new Rect(r.xMin + r.width - btnwidth, r.yMin, btnwidth, r.height);
                    EditorGUI.LabelField(r1, node.name);
                    if(GUI.Button(r2, "Delete"))
                    {
                        GameObject.DestroyImmediate(node.gameObject);
                    }
                }
                EditorGUI.indentLevel -= 1;
            }

            if(GUILayout.Button("Add Node"))
            {
                var node = new GameObject("Node" + targ.GetAllNodes().Length.ToString("00"));
                node.transform.parent = targ.transform;
                node.transform.localPosition = Vector3.zero;
                node.transform.localRotation = Quaternion.identity;
            }

        }

        #endregion

        #region Static OnScene

        static PlaceAtRandomNodeOnSpawnInspector()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView scene)
        {
            if (!GameObjectUtil.IsGameObjectSource(Selection.activeObject)) return;

            var selectedGo = GameObjectUtil.GetGameObjectFromSource(Selection.activeObject);
            var targ = GetTarget(selectedGo);
            if (targ == null) return;

            var c = Color.cyan;
            c.a = 0.5f;
            Handles.color = c;
            foreach(var node in targ.GetAllNodes())
            {
                HandlesHelper.DrawSphere(node.position, node.rotation, 0.2f);
            }
        }

        private static PlaceAtRandomNodeOnSpawn GetTarget(GameObject go)
        {
            PlaceAtRandomNodeOnSpawn result;
            result = go.GetComponent<PlaceAtRandomNodeOnSpawn>();
            if (result != null) return result;

            foreach(var p in go.GetParents())
            {
                result = p.GetComponent<PlaceAtRandomNodeOnSpawn>();
                if (result != null) return result;
            }

            return null;
        }

        #endregion

    }

}
