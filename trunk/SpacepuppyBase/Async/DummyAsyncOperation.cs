using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Async
{
    public class DummyAsyncOperation : IProgressingAsyncOperation
    {

        #region Static Interface

        private static DummyAsyncOperation _instance = new DummyAsyncOperation();
        public static DummyAsyncOperation Instance { get { return _instance; } }

        private DummyAsyncOperation()
        {
            //block constructor
        }

        #endregion

        #region IProgressingAsyncOperation Interface

        public float Progress
        {
            get { return 1f; }
        }

        public bool IsComplete
        {
            get { return true; }
        }

        public object Current
        {
            get { return null; }
        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {

        }

        #endregion

    }
}
