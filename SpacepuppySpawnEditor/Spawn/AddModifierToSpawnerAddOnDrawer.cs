using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Spawn;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Base;

namespace com.spacepuppyeditor.Spawn
{

    [CustomAddonDrawer(typeof(ISpawner), displayAsFooter = true)]
    public class AddModifierToSpawnerAddOnDrawer : SPEditorAddonDrawer
    {

        public override void OnInspectorGUI()
        {
            var rect = EditorGUILayout.GetControlRect(false, 35f);
            rect = new Rect(rect.xMin + 10f, rect.yMin + 5f, rect.width - 20f, rect.height - 10f);
            
            if(GUI.Button(rect, "Add Spawn Modifier"))
            {
                TypeSelectionDropDownWindow.ShowAndCallbackOnSelect(rect, typeof(ISpawnerModifier), (tp) =>
                {
                    if(tp != null && TypeUtil.IsType(tp, typeof(Component)))
                    {
                        var go = GameObjectUtil.GetGameObjectFromSource(this.SerializedObject.targetObject);
                        if (go != null) go.AddComponent(tp);
                    }
                }, false, false);
            }
        }

    }

}
