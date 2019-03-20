using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Modifiers
{

    [CustomPropertyDrawer(typeof(InsertButtonAttribute))]
    public class InsertButtonModifier : PropertyModifier
    {

        protected internal override void OnBeforeGUI(SerializedProperty property, ref bool cancelDraw)
        {
            var attrib = this.attribute as InsertButtonAttribute;
            if(attrib.PrecedeProperty && (!attrib.RuntimeOnly || Application.isPlaying))
            {
                this.DrawButton(property, attrib);
            }
        }

        protected internal override void OnPostGUI(SerializedProperty property)
        {
            var attrib = this.attribute as InsertButtonAttribute;
            if (!attrib.PrecedeProperty && (!attrib.RuntimeOnly || Application.isPlaying))
            {
                this.DrawButton(property, attrib);
            }
        }


        private void DrawButton(SerializedProperty property, InsertButtonAttribute attrib)
        {
            if(GUILayout.Button(attrib.Label))
            {
                var obj = EditorHelper.GetTargetObjectWithProperty(property);
                DynamicUtil.InvokeMethod(obj, attrib.OnClick);
            }
        }

    }

}