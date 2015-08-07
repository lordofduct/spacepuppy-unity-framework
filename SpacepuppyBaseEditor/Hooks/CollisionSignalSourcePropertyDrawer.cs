using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Hooks;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Hooks
{
    [CustomPropertyDrawer(typeof(CollisionSignalSource))]
    public class CollisionSignalSourcePropertyDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var signalSourceProp = property.FindPropertyRelative("_signalSource");
            return SPEditorGUI.GetPropertyHeight(signalSourceProp, label, false);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var signalSourceProp = property.FindPropertyRelative("_signalSource");
            SPEditorGUI.PropertyField(position, signalSourceProp, label);

            if (signalSourceProp.objectReferenceValue == null)
            {
                var go = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
                if(go != null && (go.HasComponent<Rigidbody>() || go.HasComponent<Collider>()))
                    signalSourceProp.objectReferenceValue = go;
                else
                {
                    var root = go.FindTrueRoot();
                    if (root != null && (root.HasComponent<Rigidbody>() || root.HasComponent<Collider>()))
                        signalSourceProp.objectReferenceValue = root;
                }
            }
        }

    }
}
