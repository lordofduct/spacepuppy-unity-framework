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

        #region Properties

        public new MultiTag target { get { return base.target as MultiTag; } }

        private bool _showTags;

        #endregion

        protected override void OnSPInspectorGUI()
        {
            //this may change in later releases...
            _showTags = EditorGUILayout.Foldout(_showTags, "Tags");

            if (_showTags)
            {
                EditorGUI.indentLevel++;

                var currentTags = this.target.GetTags().ToArray();
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
                    this.target.UpdateTags(selectedTags.ToArray());
                    this.serializedObject.Update();
                }

                EditorGUI.indentLevel--;
            }

        }

    }

}