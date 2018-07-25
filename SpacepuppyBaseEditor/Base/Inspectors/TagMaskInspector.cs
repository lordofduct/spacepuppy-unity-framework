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


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            com.spacepuppyeditor.Project.TagDataInspector.Touch();
            var targ = EditorHelper.GetTargetObjectOfProperty(property) as TagMask;

            var tags = UnityEditorInternal.InternalEditorUtility.tags;
            int mask = 0;
            if(targ.IntersectAll)
            {
                mask = -1;
            }
            else
            {
                for (int i = 0; i < tags.Length; i++)
                {
                    if (targ != null && targ.Intersects(tags[i]))
                    {
                        mask |= (1 << i);
                    }
                }
            }

            EditorGUI.BeginChangeCheck();
            mask = EditorGUI.MaskField(position, label, mask, tags);

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                if (mask == -1 || mask == (1 << tags.Length) - 1)
                {
                    targ.IntersectAll = true;
                }
                else
                {
                    targ.Clear();
                    for (int i = 0; i < tags.Length; i++)
                    {
                        if((mask & (1 << i)) != 0)
                        {
                            targ.Add(tags[i]);
                        }
                    }
                }
                property.serializedObject.Update();
            }

        }
        
    }

}
