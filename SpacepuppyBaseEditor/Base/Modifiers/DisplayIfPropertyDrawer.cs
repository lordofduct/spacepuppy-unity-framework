using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Modifiers
{

    [CustomPropertyDrawer(typeof(DisplayIfAttribute))]
    public class DisplayIfPropertyDrawer : PropertyModifier
    {

        protected internal override void OnBeforeGUI(SerializedProperty property, ref bool cancelDraw)
        {
            var attrib = this.attribute as DisplayIfAttribute;
            var targ = EditorHelper.GetTargetObjectWithProperty(property);
            
            if (property.serializedObject.isEditingMultipleObjects)
            {
                cancelDraw = true;
            }
            else if (attrib != null && targ != null)
            {
                cancelDraw = !ConvertUtil.ToBool(DynamicUtil.GetValue(targ, attrib.MemberName));
                if (attrib.DisplayIfNot) cancelDraw = !cancelDraw;
            }
        }

    }

}
