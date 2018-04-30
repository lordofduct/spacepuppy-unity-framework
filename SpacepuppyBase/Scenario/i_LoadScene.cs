#pragma warning disable 0649 // variable declared but not used.
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

        [SerializeField]
        [Tooltip("A token used to persist data across scenes.")]
        VariantReference _persistentToken;
        
        #endregion

        #region Methods

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;
            if (string.IsNullOrEmpty(_sceneName)) return false;
            
            var nm = _sceneName;
            LoadSceneWaitHandle handle;
            if (nm.StartsWith("#"))
            {
                nm = nm.Substring(1);
                int index;
                if(!int.TryParse(nm, out index))
                    return false;
                if (index < 0 || index >= SceneManager.sceneCountInBuildSettings)
                    return false;

                handle = SceneManagerUtils.LoadScene(index, _mode, _behaviour);
            }
            else
            {
                handle = SceneManagerUtils.LoadScene(_sceneName, _mode, _behaviour);
            }

            if (handle != null)
            {
                handle.PersistentToken = com.spacepuppy.Utils.ObjUtil.ReduceIfProxy(_persistentToken.Value);
            }

            return true;
        }

        #endregion

    }
}
