using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Render;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Render
{

    [CustomEditor(typeof(MaterialSource))]
    public class MaterialSourceInspector : SPEditor
    {

        public const string PROP_RENDERER = "_renderer";
        public const string PROP_MAKEUNIQUEONSTART = "_makeUniqueOnStart";

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);

            this.DrawDefaultMaterialSourceInspector();

            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_RENDERER, PROP_MAKEUNIQUEONSTART);

            this.serializedObject.ApplyModifiedProperties();
        }

        protected void DrawDefaultMaterialSourceInspector()
        {
            var prop = this.serializedObject.FindProperty(PROP_RENDERER);
            var source = this.target as MaterialSource;
            var go = GameObjectUtil.GetGameObjectFromSource(source);
            if(go == null)
            {
                EditorGUILayout.HelpBox("MaterialSource can not find target GameObject it's attached to.", MessageType.Error);
                return;
            }

            if(Application.isPlaying)
            {
                GUI.enabled = false;
                this.DrawPropertyField(PROP_RENDERER);
                this.DrawPropertyField(PROP_MAKEUNIQUEONSTART);
                GUI.enabled = true;
            }
            else
            {
                if (prop.objectReferenceValue != null && prop.objectReferenceValue is Renderer)
                {
                    var renderer = prop.objectReferenceValue as Renderer;
                    if (renderer.gameObject != go)
                    {
                        prop.objectReferenceValue = null;
                    }


                }

                var renderers = go.GetComponents<Renderer>();
                if (renderers.Length == 0)
                {
                    EditorGUILayout.HelpBox("MaterialSource can not find a Renderer on that GameObject it's attached to.", MessageType.Error);
                    return;
                }
                else
                {
                    var sources = go.GetComponents<MaterialSource>();
                    if (sources.Length > renderers.Length)
                    {
                        Debug.LogWarning("There are too many MaterialSources attached to this GameObject. Removing extra.", go);
                        UnityEngine.Object.DestroyImmediate(this.target);
                        return;
                    }

                    renderers = renderers.Except((from s in sources where s.Renderer != null && s.Renderer != source.Renderer select s.Renderer)).ToArray();
                    var names = (from r in renderers select EditorHelper.TempContent( r.GetType().Name)).ToArray();
                    int index = renderers.IndexOf(source.Renderer);

                    index = EditorGUILayout.Popup(EditorHelper.TempContent("Renderer"), index, names);
                    if (index >= 0)
                        prop.objectReferenceValue = renderers[index];
                    else
                        prop.objectReferenceValue = null;
                }

                this.DrawPropertyField(PROP_MAKEUNIQUEONSTART);
            }
        }

    }
}
