using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

using com.spacepuppy.Scenes;

namespace com.spacepuppy.Scenario
{
    public class i_LoadScene : TriggerableMechanism
    {
        
        #region Fields

        [SerializeField]
        [Tooltip("Prefix with # to load by index.")]
        private string _sceneName;
        [SerializeField]
        private LoadSceneMode _mode;
        [SerializeField]
        private LoadSceneBehaviour _behaviour;
        
        #endregion

        #region Methods

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;
            if (string.IsNullOrEmpty(_sceneName)) return false;
            
            var nm = _sceneName;
            if(nm.StartsWith("#"))
            {
                nm = nm.Substring(1);
                int index;
                if(!int.TryParse(nm, out index))
                    return false;
                if (index < 0 || index >= SceneManager.sceneCountInBuildSettings)
                    return false;

                var manager = Services.Get<ISceneManager>();
                if (manager != null)
                {
                    manager.LoadScene(index, _mode, _behaviour);
                }
                else
                {
                    SceneManagerUtils.LoadScene(index, _mode, _behaviour);
                }
            }
            else
            {
                var manager = Services.Get<ISceneManager>();
                if (manager != null)
                {
                    manager.LoadScene(_sceneName, _mode, _behaviour);
                }
                else
                {
                    SceneManagerUtils.LoadScene(_sceneName, _mode, _behaviour);
                }
            }

            return true;
        }

        #endregion

    }
}
