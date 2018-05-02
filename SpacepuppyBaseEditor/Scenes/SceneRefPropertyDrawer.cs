using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Scenes;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor.Scenes
{

    [CustomPropertyDrawer(typeof(SceneRef))]
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
            var totalPos = position;
            position = EditorGUI.PrefixLabel(position, label);

            var assetProp = property.FindPropertyRelative(PROP_SCENEASSET);
            var nameProp = property.FindPropertyRelative(PROP_SCENENAME);

            const float TOGGLE_WIDTH = 30f;
            Rect rObjField = new Rect(position.xMin, position.yMin, Mathf.Max(position.width - TOGGLE_WIDTH, 0f), EditorGUIUtility.singleLineHeight);
            if (assetProp.objectReferenceValue != null)
            {
                EditorGUI.BeginChangeCheck();
                assetProp.objectReferenceValue = EditorGUI.ObjectField(rObjField, GUIContent.none, assetProp.objectReferenceValue, typeof(SceneAsset), false);
                if (EditorGUI.EndChangeCheck())
                {
                    var scene = assetProp.objectReferenceValue as SceneAsset;
                    nameProp.stringValue = (scene != null) ? scene.name : string.Empty;
                }
            }
            else
            {
                var rText = new Rect(rObjField.xMin, rObjField.yMin, Mathf.Max(rObjField.width - EditorHelper.OBJFIELD_DOT_WIDTH, 0f), rObjField.height);
                var rDot = new Rect(rText.xMax, rObjField.yMin, Mathf.Min(rObjField.width - rText.width, EditorHelper.OBJFIELD_DOT_WIDTH), rObjField.height);
                EditorGUI.BeginChangeCheck();
                assetProp.objectReferenceValue = EditorGUI.ObjectField(rDot, GUIContent.none, assetProp.objectReferenceValue, typeof(SceneAsset), false);
                nameProp.stringValue = EditorGUI.TextField(rText, nameProp.stringValue);


                var ev = Event.current;
                switch (ev.type)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        if (totalPos.Contains(ev.mousePosition))
                        {
                            var scene = DragAndDrop.objectReferences.FirstOrDefault((o) => o is SceneAsset) as SceneAsset;
                            DragAndDrop.visualMode = scene != null ? DragAndDropVisualMode.Link : DragAndDropVisualMode.Rejected;

                            if (scene != null && ev.type == EventType.DragPerform)
                            {
                                assetProp.objectReferenceValue = scene;
                                nameProp.stringValue = scene.name;
                            }
                        }
                        break;
                }
            }

            var rBtn = new Rect(rObjField.xMax, position.yMin, Mathf.Min(TOGGLE_WIDTH, position.width - rObjField.width), EditorGUIUtility.singleLineHeight);
            if(GUI.Button(rBtn, "X"))
            {
                assetProp.objectReferenceValue = null;
                nameProp.stringValue = string.Empty;
            }
            
            EditorGUI.EndProperty();
        }

    }

}
