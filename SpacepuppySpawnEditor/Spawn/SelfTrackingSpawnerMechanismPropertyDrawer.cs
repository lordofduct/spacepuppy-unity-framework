using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Spawn;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Spawn
{

    [CustomPropertyDrawer(typeof(SelfTrackingSpawnerMechanism))]
    public class SelfTrackingSpawnerMechanismPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var spawnPoolProp = property.FindPropertyRelative("_spawnPool");
            var attrib = this.fieldInfo.GetCustomAttributes(typeof(SelfTrackingSpawnerMechanism.ConfigAttribute), false).FirstOrDefault() as SelfTrackingSpawnerMechanism.ConfigAttribute;
            
            const string TOOLTIP = "If left empty the default SpawnPool will be used instead.";
            string slbl;
            if (attrib != null)
                slbl = attrib.Label;
            else if (spawnPoolProp.objectReferenceValue == null)
                slbl = "Spawn Pool (default)";
            else
                slbl = "Spawn Pool";

            spawnPoolProp.objectReferenceValue = EditorGUI.ObjectField(position, EditorHelper.TempContent(slbl, TOOLTIP), spawnPoolProp.objectReferenceValue, typeof(SpawnPool), true);
        }

    }
}
