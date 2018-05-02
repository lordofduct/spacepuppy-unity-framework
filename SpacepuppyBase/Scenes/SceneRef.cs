#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Scenes
{

    [System.Serializable]
    public struct SceneRef
    {

        #region Fields

        [SerializeField]
        private UnityEngine.Object _sceneAsset;

        [SerializeField]
        private string _sceneName;

        #endregion

        #region CONSTRUCTOR

        public SceneRef(string sceneName)
        {
            _sceneAsset = null;
            _sceneName = sceneName;
        }

        #endregion

        #region Properties

        public UnityEngine.Object SceneAsset
        {
            get { return _sceneAsset; }
        }

        public string SceneName
        {
            get { return _sceneName; }
            set
            {
                if(!string.Equals(_sceneName, value))
                {
                    _sceneAsset = null;
                    _sceneName = value;
                }
            }
        }

        #endregion

        #region Implicit Conversion

        public static implicit operator string(SceneRef sceneRef)
        {
            if (sceneRef == null) return null;
            return sceneRef._sceneName;
        }

        public static implicit operator SceneRef(string sceneName)
        {
            return new SceneRef(sceneName);
        }

        #endregion

    }
}
