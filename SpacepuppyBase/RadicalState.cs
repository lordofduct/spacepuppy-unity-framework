using UnityEngine;
using System.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy
{
    public class RadicalState
    {

        #region Fields

        private MonoBehaviour _handle;
        private RadicalCoroutineDisableMode _disableMode;

        private RadicalCoroutine _routine;
        private System.Action _onCancel;
        private System.Action _onComplete;

        #endregion

        #region CONSTRUCTOR

        public RadicalState(MonoBehaviour handle)
        {
            if (object.ReferenceEquals(handle, null)) throw new System.ArgumentNullException("handle");
            _handle = handle;
        }

        #endregion

        #region Properties

        public RadicalCoroutine CurrentState
        {
            get { return _routine; }
        }

        #endregion

        #region Methods

        public RadicalCoroutine ChangeState(IEnumerator routine, System.Action onCancel = null, System.Action onComplete = null)
        {
            if (routine == null) throw new System.ArgumentNullException("routine");

            if (_routine != null) _routine.Cancel();

            _onCancel = onCancel;
            _onComplete = onComplete;
            _routine = _handle.StartRadicalCoroutine(routine, _disableMode);
            _routine.OnCancelling += this.OnCancelling;
            _routine.OnFinished += this.OnFinished;
            return _routine;
        }

        public void ExitState()
        {
            if (_routine != null) _routine.Cancel();
        }

        private void OnCancelling(object sender, System.EventArgs e)
        {
            if (_routine == null) return;
            if (_onCancel != null && _routine.Cancelled) _onCancel();
            _routine = null;
        }

        private void OnFinished(object sender, System.EventArgs e)
        {
            if (_routine == null) return;
            if (_onComplete != null && _routine.Complete) _onComplete();
            _routine = null;
        }

        #endregion

    }
}
