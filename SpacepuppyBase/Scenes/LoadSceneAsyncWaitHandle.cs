using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace com.spacepuppy.Scenes
{

    public class LoadSceneWaitHandle : System.EventArgs, IProgressingYieldInstruction
    {

        #region Fields

        private string _sceneName;
        private LoadSceneMode _mode;
        private LoadSceneBehaviour _behaviour;
        private Scene _scene;
        private AsyncOperation _op;

        private bool _initialized;

        #endregion

        #region CONSTRUCTOR

        public LoadSceneWaitHandle(string sceneName, LoadSceneMode mode, LoadSceneBehaviour behaviour)
        {
            _sceneName = sceneName;
            _mode = mode;
            _behaviour = behaviour;
            _op = null;
            _scene = default(Scene);
            _initialized = false;
        }

        ~LoadSceneWaitHandle()
        {
            //make sure we clean ourselves up
            SceneManager.sceneLoaded -= this.OnSceneLoaded;
        }

        public void Init(Scene sc, AsyncOperation op)
        {
            if (_initialized) throw new System.InvalidOperationException("LoadSceneAsyncWaitHandle has already been initialized.");
            _initialized = true;
            _scene = sc;
            _op = op;
            if(_behaviour == LoadSceneBehaviour.AsyncAndWait && _op != null && !_op.isDone)
            {
                _op.allowSceneActivation = false;
            }
            SceneManager.sceneLoaded += this.OnSceneLoaded;
        }

        #endregion

        #region Properties

        public string SceneName
        {
            get { return _sceneName; }
        }

        public LoadSceneMode Mode
        {
            get { return _mode; }
        }

        public LoadSceneBehaviour Behaviour
        {
            get { return _behaviour; }
        }

        public Scene Scene
        {
            get { return _scene; }
        }

        public bool ReadyAndWaitingToActivate
        {
            get { return !object.ReferenceEquals(_op, null) && _op.progress >= 0.9f && !_op.isDone; }
        }

        /// <summary>
        /// Use this to pass some token between scenes. 
        /// Anything that handles the ISceneLoadedMessageReceiver will receiver a reference to this handle, and therefore this token.
        /// The token should not be something that is destroyed by the load process.
        /// </summary>
        public object PersistentToken
        {
            get;
            set;
        }

        #endregion

        #region Methods
        
        public void WaitToActivate()
        {
            if(_behaviour >= LoadSceneBehaviour.Async)
            {
                _behaviour = LoadSceneBehaviour.AsyncAndWait;
                if(_op != null && !_op.isDone)
                {
                    _op.allowSceneActivation = false;
                }
            }
        }

        public void ActivateScene()
        {
            if (_op != null) _op.allowSceneActivation = true;
        }


        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //note Scene == Scene compares the handle and works just fine
            if (_scene == scene)
            {
                SceneManager.sceneLoaded -= this.OnSceneLoaded;
                com.spacepuppy.Utils.Messaging.FindAndBroadcast<ISceneLoadedMessageReceiver>((o) => o.OnSceneLoaded(this));
            }
        }

        #endregion

        #region IProgressingAsyncOperation Interface

        public float Progress
        {
            get
            {
                switch(_behaviour)
                {
                    case LoadSceneBehaviour.Standard:
                        return _initialized ? 0f : 1f;
                    case LoadSceneBehaviour.Async:
                    case LoadSceneBehaviour.AsyncAndWait:
                        if (object.ReferenceEquals(_op, null))
                            return 0f;
                        else
                            return _op.progress;
                    default:
                        return 1f;
                }
            }
        }

        public bool IsComplete
        {
            get
            {
                switch (_behaviour)
                {
                    case LoadSceneBehaviour.Standard:
                        return _initialized;
                    case LoadSceneBehaviour.Async:
                    case LoadSceneBehaviour.AsyncAndWait:
                        if (object.ReferenceEquals(_op, null))
                            return false;
                        else
                            return _op.isDone;
                    default:
                        return true;
                }
            }
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            if(!_initialized)
            {
                yieldObject = null;
                return false;
            }

            switch(_behaviour)
            {
                case LoadSceneBehaviour.Standard:
                    if(_initialized)
                    {
                        yieldObject = null;
                        return false;
                    }
                    else
                    {
                        yieldObject = null;
                        return true;
                    }
                case LoadSceneBehaviour.Async:
                    if (!object.ReferenceEquals(_op, null) && _op.isDone)
                    {
                        yieldObject = null;
                        return false;
                    }
                    else
                    {
                        yieldObject = _op;
                        return true;
                    }
                case LoadSceneBehaviour.AsyncAndWait:
                    if(object.ReferenceEquals(_op, null))
                    {
                        yieldObject = null;
                        return false;
                    }
                    else if(_op.isDone || this.ReadyAndWaitingToActivate)
                    {
                        yieldObject = null;
                        return false;
                    }
                    else
                    {
                        yieldObject = _op;
                        return true;
                    }
                default:
                    yieldObject = null;
                    return false;
            }
        }

        #endregion

    }
    
}
