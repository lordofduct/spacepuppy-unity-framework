using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Spawn;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Spawn
{

    [CustomEditor(typeof(ProxySpawnPointManager))]
    public class ProxySpawnPointManagerInspector : SPEditor
    {

        #region Fields

        private bool _expanded = false;

        #endregion

        #region Methods

        protected override void OnSPInspectorGUI()
        {
            this.DrawDefaultInspector();

            var targ = this.target as ProxySpawnPointManager;

            _expanded = EditorGUILayout.Foldout(_expanded, "Proxy Nodes");
            if (_expanded)
            {
                EditorGUI.indentLevel += 1;

                foreach (var node in targ.GetAllProxies())
                {
                    const float btnwidth = 60f;
                    var r = EditorGUILayout.GetControlRect();
                    var r1 = new Rect(r.xMin, r.yMin, r.width - btnwidth, r.height);
                    var r2 = new Rect(r.xMin + r.width - btnwidth, r.yMin, btnwidth, r.height);
                    EditorGUI.LabelField(r1, node.name);
                    if (GUI.Button(r2, "Delete"))
                    {
                        ObjUtil.SmartDestroy(node.gameObject);
                    }
                }
                EditorGUI.indentLevel -= 1;
            }

            if (GUILayout.Button("Add Node"))
            {
                var node = new GameObject("ProxySpawnPoint_" + targ.GetAllProxies().Count().ToString("00"));
                node.AddComponent<ProxySpawnPoint>();
                node.transform.parent = targ.transform;
                node.transform.localPosition = Vector3.zero;
                node.transform.localRotation = Quaternion.identity;
            }
        }

        #endregion

    }

}
