using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor.Inspectors
{

    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public class TagSelectorPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.BeginProperty(position, label, property);

                var attrib = this.attribute as TagSelectorAttribute;

                if (attrib.AllowUntagged)
                {
                    property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
                }
                else
                {
                    var tags = (from s in UnityEditorInternal.InternalEditorUtility.tags where s != SPConstants.TAG_UNTAGGED select new GUIContent(s)).ToArray();
                    var stag = property.stringValue;
                    int index = -1;
                    for (int i = 0; i < tags.Length; i++)
                    {
                        if (tags[i].text == stag)
                        {
                            index = i;
                            break;
                        }
                    }
                    index = EditorGUI.Popup(position, label, index, tags);
                    if (index >= 0)
                    {
                        property.stringValue = tags[index].text;
                    }
                    else
                    {
                        property.stringValue = null;
                    }
                }

                EditorGUI.EndProperty();
                if (EditorGUI.EndChangeCheck()) property.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }

        }

    }

}
