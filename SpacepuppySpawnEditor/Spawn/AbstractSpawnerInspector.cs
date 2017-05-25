using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Spawn;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Spawn
{

    //[CustomEditor(typeof(AbstractSpawner), true)]
    [System.Obsolete("AbstractSpawner is Obsolete, therefore, so is this.")]
    public class AbstractSpawnerInspector : SPEditor
    {

        public const string BASEPROP_ORDER = "_order";
        public const string BASEPROP_SPAWNPOOL = "_spawnPool";
        public const string BASEPROP_SPAWNASCHILD = "_spawnAsChild";

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawBaseProperties();
            this.DrawOtherProperties();
            this.DrawInformationBox();

            this.serializedObject.ApplyModifiedProperties();
        }


        protected void DrawBaseProperties()
        {
            var targ = this.target as AbstractSpawner;

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);
            this.DrawPropertyField(BASEPROP_ORDER);

            var label = new GUIContent("Spawn Pool");
            if (targ.UsesDefaultSpawnPool)
            {
                label.text += " (default)";
            }
            //this.DrawPropertyField(BASEPROP_SPAWNPOOL, label, false);
            this.DrawPropertyField(BASEPROP_SPAWNASCHILD);
        }

        protected void DrawInformationBox()
        {
            var targ = this.target as AbstractSpawner;

            //if (targ.SupportsSelectionModifier && targ.HasLikeComponent<ISpawnPointPrefabSelector>(true))
            //{
            //    var selector = (from c in targ.GetLikeComponents<ISpawnPointPrefabSelector>() where c.enabled select c).FirstOrDefault();
            //    if (selector != null)
            //    {
            //        EditorGUILayout.HelpBox("This spawnpoint has an active ISpawnPointPrefabSelector of type '" + selector.GetType().Name + "'.", MessageType.Info);
            //    }
            //}

            if(Application.isPlaying)
            {
                EditorGUILayout.BeginVertical("Box");

                var style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.UpperCenter;
                EditorGUILayout.LabelField("Information", style);

                EditorGUILayout.LabelField("Active Spawn Count:", targ.ActiveCount.ToString());

                if (targ.SupportsSelectionModifier && targ.HasComponent<ISpawnPointSelector>(true))
                {
                    EditorGUILayout.LabelField("Selectors:");
                    EditorGUI.indentLevel++;
                    foreach (var s in targ.GetComponents<ISpawnPointSelector>())
                    {
                        EditorGUILayout.LabelField(s.GetType().Name);
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
            }
        }

        protected void DrawOtherProperties(params string[] ignore)
        {
            System.Array.Resize(ref ignore, ignore.Length + 4);
            ignore[ignore.Length - 4] = EditorHelper.PROP_SCRIPT;
            ignore[ignore.Length - 3] = BASEPROP_ORDER;
            ignore[ignore.Length - 2] = BASEPROP_SPAWNPOOL;
            ignore[ignore.Length - 1] = BASEPROP_SPAWNASCHILD;
            this.DrawDefaultInspectorExcept(ignore);
        }


        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }

    }
}
