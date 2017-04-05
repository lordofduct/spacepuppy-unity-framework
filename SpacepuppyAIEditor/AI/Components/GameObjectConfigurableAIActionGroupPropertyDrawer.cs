using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.AI;
using com.spacepuppy.Utils;

using com.spacepuppy.AI.Components;

namespace com.spacepuppyeditor.AI.Components
{

    [CustomPropertyDrawer(typeof(GameObjectConfigurableAIActionGroup), true)]
    public class GameObjectConfigurableAIActionGroupPropertyDrawer : ConfigurableAIActionGroupPropertyDrawer
    {


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var h = base.GetPropertyHeight(property, label);
            if(this.DrawFlat || property.isExpanded)
            {
                h += EditorGUIUtility.singleLineHeight;
            }
            return h;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            this.Init();

            if (!this.DrawFlat)
            {
                var r0 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
                position = new Rect(position.xMin, r0.yMax, position.width, position.height - r0.height);

                property.isExpanded = EditorGUI.Foldout(r0, property.isExpanded, label);
                if (!property.isExpanded) return;

                EditorGUI.indentLevel++;
            }

            position = this.DrawPrimaryPortionOfInspector(position, property);

            var goProp = property.FindPropertyRelative("_actionSequenceContainer");
            EditorGUI.PropertyField(position, goProp);

            if (!this.DrawFlat)
            {
                EditorGUI.indentLevel--;
            }
        }


    }
}
