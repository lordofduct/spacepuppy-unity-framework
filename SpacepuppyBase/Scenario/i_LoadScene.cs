using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;
            if (string.IsNullOrEmpty(_sceneName)) return false;

            if (_async)
                SceneManager.LoadSceneAsync(_sceneName, _mode);
            else
                SceneManager.LoadScene(_sceneName, _mode);

            return true;
        }

        #endregion

    }
}
