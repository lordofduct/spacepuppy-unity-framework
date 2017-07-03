using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Project;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Project
{

    [CustomPropertyDrawer(typeof(SceneRef), true)]
    public class SceneRefPropertyDrawer : PropertyDrawer
    {

        public const string PROP_SCENEASSET = "_sceneAsset";
        public const string PROP_SCENENAME = "_sceneName";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var assetProp = property.FindPropertyRelative(PROP_SCENEASSET);
            var nameProp = property.FindPropertyRelative(PROP_SCENENAME);

            EditorGUI.BeginChangeCheck();
            assetProp.objectReferenceValue = EditorGUI.ObjectField(position, label, assetProp.objectReferenceValue, typeof(SceneAsset), false);
            if(EditorGUI.EndChangeCheck())
            {
                var scene = assetProp.objectReferenceValue as SceneAsset;
                nameProp.stringValue = (scene != null) ? scene.name : string.Empty;
            }

            EditorGUI.EndProperty();
        }


    }
}
