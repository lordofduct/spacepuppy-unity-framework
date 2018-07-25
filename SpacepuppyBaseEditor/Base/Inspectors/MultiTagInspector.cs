using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor.Base
{

    [CustomEditor(typeof(MultiTag))]
    public class MultiTagInspector : SPEditor
    {

        public const string PROP_SEARCHABLEIFINACTIVE = "_searchableIfInactive";
        public const string PROP_TAGS = "_tags";

        #region Properties
        
        private bool _showTags;

        #endregion

        protected override void OnBeforeSPInspectorGUI()
        {
            var go = com.spacepuppy.Utils.GameObjectUtil.GetGameObjectFromSource(this.target);

            if (go != null && !go.CompareTag(SPConstants.TAG_MULTITAG))
            {
                go.tag = SPConstants.TAG_MULTITAG;
                EditorUtility.SetDirty(go);
            }
        }

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            //this.DrawPropertyField(PROP_SEARCHABLEIFINACTIVE);
            var searchableProp = this.serializedObject.FindProperty(PROP_SEARCHABLEIFINACTIVE);
            searchableProp.boolValue = EditorGUILayout.ToggleLeft(searchableProp.displayName, searchableProp.boolValue);

            //this may change in later releases...
            _showTags = EditorGUILayout.Foldout(_showTags, "Tags");

            if (_showTags)
            {
                EditorGUI.indentLevel++;

                var tagsProp = this.serializedObject.FindProperty(PROP_TAGS);
                var currentTags = this.GetTags(tagsProp);
                var selectedTags = new List<string>();

                EditorGUI.BeginChangeCheck();
                var tags = from tag in UnityEditorInternal.InternalEditorUtility.tags where (tag != SPConstants.TAG_UNTAGGED && tag != SPConstants.TAG_MULTITAG && tag != SPConstants.TAG_EDITORONLY) select tag;
                foreach (var tag in tags)
                {
                    var bSelected = currentTags.Contains(tag);
                    if (EditorGUILayout.Toggle(tag, bSelected))
                    {
                        selectedTags.Add(tag);
                    }
                }

                if (EditorGUI.EndChangeCheck())
                {
                    this.SetTags(tagsProp, selectedTags.ToArray());
                }

                EditorGUI.indentLevel--;
            }

            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_SEARCHABLEIFINACTIVE, PROP_TAGS);

            this.serializedObject.ApplyModifiedProperties();
        }




        private string[] GetTags(SerializedProperty prop)
        {
            string[] tags = new string[prop.arraySize];
            for (int i = 0; i < prop.arraySize; i++)
            {
                tags[i] = prop.GetArrayElementAtIndex(i).stringValue;
            }
            return tags;
        }

        private void SetTags(SerializedProperty prop, string[] tags)
        {
            prop.arraySize = tags.Length;
            for(int i = 0; i < tags.Length; i++)
            {
                prop.GetArrayElementAtIndex(i).stringValue = tags[i];
            }
        }

    }

}