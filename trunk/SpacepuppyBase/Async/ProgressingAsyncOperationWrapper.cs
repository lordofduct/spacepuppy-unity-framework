using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Async
{

    public class ProgressingAsyncOperationWrapper : IProgressingAsyncOperation
    {

        #region Fields

        private IProgressingAsyncOperation[] _operations;

        #endregion

        #region CONSTRUCTOR

        public ProgressingAsyncOperationWrapper(params IProgressingAsyncOperation[] operations)
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
                else if (_operations.Length == 1) return _operations[0].Progress;
                else
                {
                    float p = 0f;
                    for (int i = 0; i < _operations.Length; i++)
                    {
                        p += _operations[i].Progress;
                    }
                    return p / _operations.Length;
                }
            }
        }

        public bool IsComplete
        {
            get
            {
                for (int i = 0; i < _operations.Length; i++)
                {
                    if (!_operations[i].IsComplete) return false;
                }
                return true;
            }
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                for (int i = 0; i < _operations.Length; i++)
                {
                    if (!_operations[i].IsComplete) return _operations[i];
                }
                return null;
            }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            return !this.IsComplete;
        }

        void System.Collections.IEnumerator.Reset()
        {
            throw new System.NotSupportedException();
        }

        #endregion

    }

}
