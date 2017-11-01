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

        public void Init(Scene sc, AsyncOperation op)
        {
            _initialized = true;
            _scene = sc;
            _op = op;
            if(_behaviour == LoadSceneBehaviour.AsyncAndWait && _op != null && !_op.isDone)
            {
                _op.allowSceneActivation = false;
            }
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


    public class LoadSceneAsyncWaitHandle : IProgressingYieldInstruction
    {

        #region Fields

        private AsyncOperation _op;
        private Scene _scene;
        private bool _waitingForHandle;

        #endregion

        #region CONSTRUCTOR

        public LoadSceneAsyncWaitHandle(AsyncOperation op, Scene scene)
        {
            //if (op == null) throw new System.ArgumentNullException("op");
            _op = op;
            _scene = scene; 
        }

        internal LoadSceneAsyncWaitHandle()
        {
            _waitingForHandle = true;
        }
        internal void Init(AsyncOperation op, Scene scene)
        {
            //if (op == null) throw new System.ArgumentNullException("op");
            _op = op;
            _scene = scene;
            _waitingForHandle = false;
        }

        #endregion

        #region Properties

        public Scene Scene
        {
            get { return _scene; }
        }

        #endregion

        #region IProgressingAsyncOperation Interface

        public float Progress
        {
            get
            {
                if (_waitingForHandle)
                    return 0f;
                else
                    return _op.progress;
            }
        }

        public bool IsComplete
        {
            get
            {
                if (_waitingForHandle)
                    return false;
                else
                    return _op.isDone;
            }
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            if (!_waitingForHandle && _op.isDone)
            {
                yieldObject = null;
                return false;
            }
            else
            {
                yieldObject = _op;
                return true;
            }
        }

        #endregion

    }
}
