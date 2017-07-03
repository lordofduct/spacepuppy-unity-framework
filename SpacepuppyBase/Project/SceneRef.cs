using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Project
{

    [System.Serializable]
    public class SceneRef
    {

        #region Fields

        [SerializeField]
        private UnityEngine.Object _sceneAsset;

        [SerializeField]
        private string _sceneName;

        #endregion

        #region Properties

        public UnityEngine.Object SceneAsset
        {
            get { return _sceneAsset; }
        }

        public string SceneName
        {
            get { return _sceneName; }
        }

        #endregion

        #region Implicit Conversion

        public static implicit operator string(SceneRef sceneRef)
        {
            if (sceneRef == null) return null;
            return sceneRef._sceneName;
        }

        #endregion

    }
}
