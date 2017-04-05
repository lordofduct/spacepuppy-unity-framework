using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Spawn;

namespace com.spacepuppyeditor.Spawn
{

    [CanEditMultipleObjects()]
    [CustomEditor(typeof(ProxySpawnPoint))]
    public class ProxySpawnPointInspector : SPEditor
    {

        protected override void OnSPInspectorGUI()
        {
            this.DrawDefaultInspector();

            if (!this.serializedObject.isEditingMultipleObjects && this.target is ProxySpawnPoint && (this.target as ProxySpawnPoint).Busy)
            {
                EditorGUILayout.HelpBox("SpawnPoint Is Busy.", MessageType.Warning);
            }

        }

        public override bool RequiresConstantRepaint()
        {
            return !this.serializedObject.isEditingMultipleObjects;
        }

    }

}
