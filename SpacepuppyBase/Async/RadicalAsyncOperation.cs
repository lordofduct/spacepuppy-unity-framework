using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Async
{

    public abstract class RadicalAsyncOperation : IRadicalAsyncOperation
    {

        #region Fields

        private bool _complete;

        #endregion

        #region CONSTRUCTOR

        public RadicalAsyncOperation()
        {
        }

        #endregion

        #region Properties

        protected virtual object CurrentYieldObject
        {
            get { return null; }
        }

        #endregion

        #region Methods

        protected void SetSignal()
        {
            _complete = true;
        }

        #endregion

        #region IPsuedoAsyncResult

        public bool IsComplete
        {
            get { return _complete; }
        }

        #endregion

        #region IRadicalYieldInstruction Interface

        object System.Collections.IEnumerator.Current
        {
            get { return this.CurrentYieldObject; }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            return !_complete;
        }

        void System.Collections.IEnumerator.Reset()
        {
            throw new System.NotSupportedException();
        }

        #endregion

    }

}
