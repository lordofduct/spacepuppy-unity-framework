using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Scenes
{
    public class SceneBehaviourLoadOptionsWithLoadScreen : ISceneBehaviourLoadOptions
    {

        #region Fields

        private System.Type _sceneBehaviourType;

        #endregion


        #region ISceneBehaviourLoadOptions Interface

        System.Type ISceneBehaviourLoadOptions.SceneBehaviourType
        {
            get { return _sceneBehaviourType; }
        }

        void ISceneBehaviourLoadOptions.OnBeforeSceneLoaded(SceneManager manager, ISceneBehaviour sceneBehaviour)
        {
            
        }

        void ISceneBehaviourLoadOptions.OnSceneLoaded(SceneManager manager, ISceneBehaviour sceneBehaviour)
        {
            
        }

        void ISceneBehaviourLoadOptions.OnSceneStarted(SceneManager manager, ISceneBehaviour sceneBehaviour)
        {
            
        }

        #endregion

    }
}
