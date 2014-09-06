using UnityEngine;
using UnityEditor;
using System.Collections;

using com.spacepuppy.Spawn;

namespace com.spacepuppyeditor.Inspectors.Spawn
{
    [CustomEditor(typeof(AbstractSpawnPoint))]
    public class SpawnPointEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            var targ = this.target as SpawnPoint;
            if (targ.SpawnPool == null) targ.SpawnPool = null;

            this.DrawDefaultInspector(EditorHelper.PROP_SCRIPT);
            if (targ.UsesDefaultSpawnPool)
            {
                EditorGUILayout.LabelField("Uses default SpawnPool");
            }
            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT);
            
        }

    }
}
