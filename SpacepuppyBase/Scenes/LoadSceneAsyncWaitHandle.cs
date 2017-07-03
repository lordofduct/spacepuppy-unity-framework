using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace com.spacepuppy.Scenes
{
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
            if (op == null) throw new System.ArgumentNullException("op");
            _op = op;
            _scene = scene; 
        }

        internal LoadSceneAsyncWaitHandle()
        {
            _waitingForHandle = true;
        }
        internal void Init(AsyncOperation op, Scene scene)
        {
            if (op == null) throw new System.ArgumentNullException("op");
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
