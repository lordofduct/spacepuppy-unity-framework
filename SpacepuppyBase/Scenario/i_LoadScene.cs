#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

using com.spacepuppy.Scenes;

namespace com.spacepuppy.Scenario
{
    public class i_LoadScene : AutoTriggerableMechanism, ISerializationCallbackReceiver
    {

        #region Fields

        [SerializeField]
        [Tooltip("Prefix with # to load by index.")]
        private SceneRef _scene;

        [SerializeField]
        private LoadSceneMode _mode;
        [SerializeField]
        private LoadSceneBehaviour _behaviour;

        [SerializeField]
        [Tooltip("A token used to persist data across scenes.")]
        VariantReference _persistentToken = new VariantReference();

        [HideInInspector]
        [SerializeField]
        private string _sceneName;

        #endregion

        #region Properties

        public SceneRef Scene
        {
            get { return _scene; }
            set { _scene = value; }
        }

        public LoadSceneMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public LoadSceneBehaviour Behaviour
        {
            get { return _behaviour; }
            set { _behaviour = value; }
        }

        public object PersistentToken
        {
            get { return _persistentToken; }
        }

        #endregion

        #region Methods

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;
            if (string.IsNullOrEmpty(_scene.SceneName)) return false;
            
            var nm = _scene.SceneName;
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
                handle = SceneManagerUtils.LoadScene(nm, _mode, _behaviour);
            }

            if (handle != null)
            {
                handle.PersistentToken = com.spacepuppy.Utils.ObjUtil.ReduceIfProxy(_persistentToken.Value);
            }

            return true;
        }

        #endregion

        #region ISerializationCallback Receiver

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            //exists for backwards compatability
            if(!string.IsNullOrEmpty(_sceneName))
            {
                _scene = new SceneRef(_sceneName);
                _sceneName = null;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {

        }

        #endregion

    }
}
