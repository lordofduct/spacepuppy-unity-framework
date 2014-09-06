using UnityEngine;
using UnityEditor;
using System.Collections;

namespace com.spacepuppyeditor
{

    [InitializeOnLoad()]
    public class EditorSceneEvents : Editor
    {

        #region Static Fields

        //object added to scene vars
        private static bool _dragPerformedLastTime;

        #endregion

        #region STATIC CONSTRUCTOR

        static EditorSceneEvents()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        #endregion

        #region Static Events

        public delegate void OnPrefabAddedToSceneHandler(GameObject objectAdded);
        public static event OnPrefabAddedToSceneHandler OnPrefabAddedToScene;

        #endregion

        #region OnSceneGUI Message

        private static void OnSceneGUI(SceneView scene)
        {
            ProcessObjectAddedToScene();
        }

        private static void ProcessObjectAddedToScene()
        {
            if (_dragPerformedLastTime && OnPrefabAddedToScene != null)
            {
                if (Selection.activeGameObject != null)
                {
                    OnPrefabAddedToScene(Selection.activeGameObject);
                }
            }

            if (Event.current.type == EventType.DragPerform &&
                DragAndDrop.objectReferences.Length > 0 &&
                DragAndDrop.objectReferences[0] is GameObject)
            {
                _dragPerformedLastTime = true;
            }
            else
            {
                _dragPerformedLastTime = false;
            }
        }

        #endregion
        
    }

}