using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{
    [CustomPropertyDrawer(typeof(TagMask))]
    public class TagMaskInspector : PropertyDrawer
    {

        #region Properties

        private bool _showTags;

        #endregion

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if(_showTags)
            {
                //+1 for the foldout
                return (UnityEditorInternal.InternalEditorUtility.tags.Length + 1) * EditorGUIUtility.singleLineHeight;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var r = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);

            //this may change in later releases...
            _showTags = EditorGUI.Foldout(r, _showTags, label);

            if (_showTags)
            {
                EditorGUI.indentLevel++;

                var targ = EditorHelper.GetTargetObjectOfProperty(property) as TagMask;

                EditorGUI.BeginChangeCheck();
                var tags = (from tag in UnityEditorInternal.InternalEditorUtility.tags select tag).ToArray();
                foreach (var tag in tags)
                {
                    var bSelected = targ.Intersects(tag);
                    r = new Rect(r.xMin, r.yMax, r.width, EditorGUIUtility.singleLineHeight);
                    if (EditorGUI.Toggle(r,tag, bSelected))
                    {
                        targ.Add(tag);
                    }
                    else if(bSelected)
                    {
                        targ.Remove(tag);
                    }
                }

                if (EditorGUI.EndChangeCheck())
                {
                    property.serializedObject.Update();
                }

                EditorGUI.indentLevel--;
            }
        }

    }

}
