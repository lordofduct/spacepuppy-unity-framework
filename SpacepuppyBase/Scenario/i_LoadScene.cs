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
        private bool _async;
        [SerializeField]
        private LoadSceneMode _mode;

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
                    if (_async)
                        manager.LoadSceneAsync(index, _mode);
                    else
                        manager.LoadScene(index, _mode);
                }
                else
                {
                    if (_async)
                        SceneManager.LoadSceneAsync(index, _mode);
                    else
                        SceneManager.LoadScene(index, _mode);
                }
            }
            else
            {
                var manager = Services.Get<ISceneManager>();
                if (manager != null)
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
            }
            
            return true;
        }

        #endregion

    }
}
