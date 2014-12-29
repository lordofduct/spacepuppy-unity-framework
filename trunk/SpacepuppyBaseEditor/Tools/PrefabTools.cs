using UnityEngine;
using UnityEditor;
using System.Collections;

namespace com.spacepuppyeditor.Tools
{

    [InitializeOnLoad()]
    public class PrefabTools : SPEditor
    {

        static PrefabTools()
        {
            EditorSceneEvents.OnPrefabAddedToScene -= OnPrefabAddedToScene;
            EditorSceneEvents.OnPrefabAddedToScene += OnPrefabAddedToScene;
        }


        private static void OnPrefabAddedToScene(GameObject go)
        {

            if (Event.current.shift)
            {
                PrefabUtility.DisconnectPrefabInstance(go);
            }

        }


    }

}