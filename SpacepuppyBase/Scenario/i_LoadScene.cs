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
        private string _sceneName;
        [SerializeField]
        private bool _async;
        [SerializeField]
        private LoadSceneMode _mode;

        #endregion

        #region Methods

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;
            if (string.IsNullOrEmpty(_sceneName)) return false;

            var manager = Services.Get<ISceneManager>();
            if(manager != null)
            {
                if (_async)
                    manager.LoadSceneAsync(_sceneName, _mode);
                else
                    manager.LoadScene(_sceneName, _mode);
            }
            else
            {
                if (_async)
                    SceneManager.LoadSceneAsync(_sceneName, _mode);
                else
                    SceneManager.LoadScene(_sceneName, _mode);
            }

            return true;
        }

        #endregion

    }
}
