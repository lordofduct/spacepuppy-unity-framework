using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Spawn;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Inspectors.Spawn
{
    [CustomEditor(typeof(AbstractSpawnPoint), true)]
    public class SpawnPointInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            var targ = this.target as SpawnPoint;
            if (targ.SpawnPool == null) targ.SpawnPool = null;

            this.DrawDefaultInspector(EditorHelper.PROP_SCRIPT);

            var label = new GUIContent("Spawn Pool");
            if (targ.UsesDefaultSpawnPool)
            {
                label.text += " (default)";
            }
            this.DrawDefaultInspector("_spawnPool", label, false);

            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, "_spawnPool");

            if (targ.SupportsSelectionModifier && targ.HasLikeComponent<ISpawnPointPrefabSelector>(true))
            {
                var selector = (from c in targ.GetLikeComponents<ISpawnPointPrefabSelector>() where c.enabled select c).FirstOrDefault();
                if(selector != null)
                {
                EditorGUILayout.HelpBox("This spawnpoint has an active ISpawnPointPrefabSelector of type '" + selector.GetType().Name + "'.", MessageType.Info);
                }
            }
        }

    }
}
