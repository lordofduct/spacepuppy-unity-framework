using UnityEngine;

namespace com.spacepuppy
{

    /// <summary>
    /// Just a name for contract purposes.
    /// </summary>
    public interface IRadicalYieldInstruction : System.Collections.IEnumerator
    {
        void Init(RadicalCoroutine routine);
    }

    public abstract class RadicalYieldInstruction : IRadicalYieldInstruction
    {

        #region Fields

        private object _current;
        private RadicalCoroutine _routine;

        #endregion

        #region Properties

        protected RadicalCoroutine Routine { get { return _routine; } }

        #endregion

        #region Methods

        protected virtual void Init()
        {

        }

        protected virtual void DeInit()
        {

        }

        protected abstract bool ContinueBlocking(ref object yieldObject);

        #endregion

        #region IRadicalYieldInstruction Interface

        void IRadicalYieldInstruction.Init(RadicalCoroutine routine)
        {
            _routine = routine;
            this.Init();
        }

        #endregion

        #region IEnumerator Interface

        object System.Collections.IEnumerator.Current
        {
            get { return _current; }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            _current = null;
            if (this.ContinueBlocking(ref _current))
            {
                return true;
            }
            else
            {
                this.DeInit();
                _routine = null;
                return false;
            }
        }

        void System.Collections.IEnumerator.Reset()
        {
            throw new System.NotSupportedException();
        }

        #endregion

    }

}
