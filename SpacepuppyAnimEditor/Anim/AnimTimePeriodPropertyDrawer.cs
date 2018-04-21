using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Anim;

using com.spacepuppyeditor.Base;

namespace com.spacepuppyeditor.Anim
{

    [CustomPropertyDrawer(typeof(AnimTimePeriodAttribute))]
    public class AnimTimePeriodPropertyDrawer : SPTimePeriodPropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            var secondsProp = property.FindPropertyRelative(PROP_SECONDS);
            if(secondsProp != null && secondsProp.propertyType == SerializedPropertyType.Float)
            {
                var choices = new GUIContent[] { EditorHelper.TempContent("To Complete"), EditorHelper.TempContent("For Duration") };
                int index = (secondsProp.floatValue == float.PositiveInfinity) ? 0 : 1;
                if(index == 0)
                {
                    index = EditorGUI.Popup(position, index, choices);
                    if (index == 1) secondsProp.floatValue = 0f;
                }
                else
                {
                    var r = new Rect(position.xMin, position.yMin, Mathf.Min(position.width, 80f), EditorGUIUtility.singleLineHeight);
                    index = EditorGUI.Popup(r, index, choices);

                    if(index == 0)
                    {
                        secondsProp.floatValue = float.PositiveInfinity;
                        property.FindPropertyRelative(SPTimePropertyDrawer.PROP_TIMESUPPLIERNAME).stringValue = null;
                    }
                    else
                    {
                        position = new Rect(r.xMax, position.yMin, position.width - r.width, position.height);
                        this.DrawTimePeriodSansLabel(position, property);
                    }
                }
            }
            else
            {
                this.DrawTimePeriodSansLabel(position, property);
            }

        }

    }

}
