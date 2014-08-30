using UnityEngine;

namespace com.spacepuppy
{

    /// <summary>
    /// Just a name for contract purposes.
    /// </summary>
    public interface IRadicalYieldInstruction : System.Collections.IEnumerator
    {

    }

    public abstract class RadicalYieldInstruction : IRadicalYieldInstruction
    {

        #region Fields

        private object _current;

        #endregion

        #region Methods

        protected abstract bool ContinueBlocking(ref object yieldObject);

        #endregion

        #region IEnumerator Interface

        object System.Collections.IEnumerator.Current
        {
            get { return _current; }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            _current = null;
            return this.ContinueBlocking(ref _current);
        }

        void System.Collections.IEnumerator.Reset()
        {
            throw new System.NotSupportedException();
        }

        #endregion

    }
}
