using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Async
{

    public abstract class RadicalAsyncOperation : IRadicalAsyncOperation, IImmediatelyResumingYieldInstruction
    {

        #region Fields

        private bool _complete;

        #endregion

        #region CONSTRUCTOR

        public RadicalAsyncOperation()
        {
        }

        #endregion

        #region Methods

        protected void SetSignal()
        {
            _complete = true;
            if (_signalHandler != null) _signalHandler(this, System.EventArgs.Empty);
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
            get { return null; }
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

        #region IImmediatelyResumingYieldInstruction Interface

        private System.EventHandler _signalHandler;
        event System.EventHandler IImmediatelyResumingYieldInstruction.Signal
        {
            add { _signalHandler += value; }
            remove { _signalHandler -= value; }
        }

        #endregion

    }

}
