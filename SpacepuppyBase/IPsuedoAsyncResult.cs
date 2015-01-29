using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    public delegate void PsuedoAsyncCallback(IPsuedoAsyncResult asyncResult);

    /// <summary>
    /// Contract for an object that acts as the token for a coroutine
    /// </summary>
    public interface IPsuedoAsyncResult : IRadicalYieldInstruction
    {

        /// <summary>
        /// An token to identify this state on complete.
        /// </summary>
        object AsyncState { get; }
        bool IsComplete { get; }

    }

    public abstract class PsuedoAsyncResult : IPsuedoAsyncResult, IImmediatelyResumingYieldInstruction
    {

        #region Fields

        private object _asyncState;
        private bool _complete;

        #endregion

        #region CONSTRUCTOR

        public PsuedoAsyncResult(object asyncState) : base()
        {
            _asyncState = asyncState;
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

        public object AsyncState
        {
            get { return _asyncState; }
        }

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
