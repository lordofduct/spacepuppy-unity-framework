using UnityEngine;
using System.Collections;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy
{
    public class RadicalState : System.IDisposable
    {

        #region Fields

        private MonoBehaviour _handle;
        private RadicalCoroutineDisableMode _disableMode;

        private RadicalCoroutine _routine;
        private System.Action _onCancel;
        private System.Action _onComplete;

        #endregion

        #region CONSTRUCTOR

        public RadicalState(MonoBehaviour handle, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (object.ReferenceEquals(handle, null)) throw new System.ArgumentNullException("handle");
            _handle = handle;
            _disableMode = disableMode;
        }

        #endregion

        #region Properties

        public RadicalCoroutine CurrentState
        {
            get { return _routine; }
        }

        public RadicalCoroutineDisableMode DisableMode
        {
            get { return _disableMode; }
            set { _disableMode = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Change to a new state, cancelling any existing running state, cancel callback for previous state will be called.
        /// </summary>
        /// <param name="routine">Routine to run.</param>
        /// <param name="onCancel">Cancel callback when state changes.</param>
        /// <param name="onComplete">Complete callback when state completes.</param>
        /// <returns></returns>
        public RadicalCoroutine ChangeState(IEnumerator routine, System.Action onCancel = null, System.Action onComplete = null)
        {
            if (_disposed) throw new System.InvalidOperationException("Object is disposed.");
            if (routine == null) throw new System.ArgumentNullException("routine");

            if (_routine != null && _routine.Active) _routine.Cancel();

            _onCancel = onCancel;
            _onComplete = onComplete;
            _routine = _handle.StartRadicalCoroutine(routine, _disableMode);
            _routine.OnCancelling += this.OnCancelling;
            _routine.OnFinished += this.OnFinished;
            return _routine;
        }

        /// <summary>
        /// Exit the current state, cancelling it, and firing the cancel callback.
        /// </summary>
        public void ExitState()
        {
            if (_disposed) throw new System.InvalidOperationException("Object is disposed.");
            if (_routine != null && _routine.Active)
            {
                _routine.Cancel();
                _routine = null;
            }
        }

        /// <summary>
        /// Removes event listeners before cancelling the current state so that no callback signals occur.
        /// </summary>
        public void SilentlyExitState()
        {
            if (_disposed) throw new System.InvalidOperationException("Object is disposed.");
            if (_routine != null && _routine.Active)
            {
                var rot = _routine;
                _routine = null;
                rot.OnCancelling -= this.OnCancelling;
                rot.OnFinished -= this.OnFinished;
                rot.Cancel();
            }
        }

        /// <summary>
        /// Removes event listeners before cancelling the routine so that no callback signals occur.
        /// </summary>
        /// <param name="routine">Supposed state to cancel, this may be a routine that was previously started on this but exited.</param>
        /// <returns>Returns true if was the running routine.</returns>
        public bool SilentlyExitState(RadicalCoroutine routine)
        {
            if (_disposed) throw new System.InvalidOperationException("Object is disposed.");
            if (routine == null) return false;
            
            if (_routine == routine)
            {
                _routine = null;
                routine.OnCancelling -= this.OnCancelling;
                routine.OnFinished -= this.OnFinished;
                routine.Cancel();
                return true;
            }
            else
            {
                routine.Cancel();
                return false;
            }
        }

        private void OnCancelling(object sender, System.EventArgs e)
        {
            if (_disposed || _routine == null) return;
            if (sender != _routine) return;
            if (_onCancel != null && GameLoopEntry.QuitState != QuitState.Quit && _routine.Cancelled) _onCancel();
            _routine = null;
        }

        private void OnFinished(object sender, System.EventArgs e)
        {
            if (_disposed || _routine == null) return;
            if (sender != _routine) return;
            if (_onComplete != null && _routine.Complete) _onComplete();
            _routine = null;
        }

        #endregion

        #region IDisposable Interface

        private bool _disposed;
        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            if(_routine != null)
            {
                _routine.Cancel();
                _routine = null;
            }
        }

        #endregion

    }
}
