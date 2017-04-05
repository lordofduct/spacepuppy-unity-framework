using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.AI;
using com.spacepuppy.AI.Components;
using com.spacepuppy.StateMachine;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.AI.Components
{
    [CustomPropertyDrawer(typeof(ListAIStatesAttribute))]
    public class ListAIStatesPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var cache = SPGUI.DisableIfPlaying();

            /*
            Component[] states = null;
            var src = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
            if (src != null) states = ParentComponentStateSupplier<IAIState>.GetComponentsOnTarg(src, false).Cast<Component>().ToArray();
            else states = new Component[] { };

            var componentLabels = (from c in states select EditorHelper.TempContent(string.Format("{0} ({1})", c.name, c.GetType().Name))).ToArray();
            property.objectReferenceValue = SPEditorGUI.SelectComponentField(position, label, states, componentLabels, property.objectReferenceValue as Component);

            cache.Reset();
            */

            Component[] states = null;
            var src = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
            if (src != null) states = ParentComponentStateSupplier<IAIState>.GetComponentsOnTarg(src, false).Cast<Component>().Prepend(null).ToArray();
            else states = new Component[] { };

            var componentLabels = (from c in states select (c != null) ? EditorHelper.TempContent(string.Format("{0} ({1})", c.name, c.GetType().Name)) : EditorHelper.TempContent("...Nothing")).ToArray();
            property.objectReferenceValue = SPEditorGUI.SelectComponentField(position, label, states, componentLabels, property.objectReferenceValue as Component);

            cache.Reset();
        }

    }
}
