using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Async
{
    public class AsyncOperationWrapper : IProgressingYieldInstruction
    {

        #region Fields

        private AsyncOperation[] _operations;
        private AsyncOperation _recentOperation;
        private bool _complete;

        #endregion

        #region CONSTRUCTOR

        public AsyncOperationWrapper(params AsyncOperation[] operations)
        {
            if (operations == null) throw new System.ArgumentNullException("operations");
            _operations = operations;
        }

        #endregion

        #region IProgressingAsyncOperation Interface

        public float Progress
        {
            get
            {
                if (_operations.Length == 0) return 1f;
                else if (_operations.Length == 1) return _operations[0].progress;
                else
                {
                    float p = 0f;
                    for (int i = 0; i < _operations.Length; i++)
                    {
                        p += _operations[i].progress;
                    }
                    return p / _operations.Length;
                }
            }
        }

        public bool IsComplete
        {
            get
            {
                if (_complete) return true;
                else
                {
                    for (int i = 0; i < _operations.Length; i++)
                    {
                        if (!_operations[i].isDone) return false;
                    }
                    _complete = true;
                    return true;
                }
            }
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            if (_recentOperation == null || _recentOperation.isDone)
            {
                for (int i = 0; i < _operations.Length; i++)
                {
                    if (!_operations[i].isDone)
                    {
                        _recentOperation = _operations[i];
                        yieldObject = _recentOperation;
                        return true;
                    }
                }
            }

            yieldObject = null;
            return false;
        }

        #endregion

    }
}
