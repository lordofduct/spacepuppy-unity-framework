
namespace com.spacepuppy
{

    /// <summary>
    /// Just a name for contract purposes. You should probably inherit from RadicalYieldInstruciton, or composite it, unless 
    /// you know what you're doing.
    /// </summary>
    public interface IRadicalYieldInstruction
    {

        /// <summary>
        /// The instruction completed, but not necessarily successfully.
        /// </summary>
        bool IsComplete { get; }

        ///// <summary>
        ///// This method is called every tick of the RadicalCoroutine that is handling it.
        ///// </summary>
        ///// <returns>Return if the RadicalCoroutine should continue blocking (IsComplete should be false if this returns true)</returns>
        //bool ContinueBlocking();
        //object CurrentYieldObject { get; }

        bool Tick(out object yieldObject);

    }

    /// <summary>
    /// Base abstract class that implements IRadicalYieldInstruction. It implements IRadicalYieldInstruction in the most 
    /// commonly used setup. You should only ever implement IRadicalYieldInstruction directly if you can't inherit from this 
    /// in your inheritance chain, or you want none standard behaviour.
    /// </summary>
    public abstract class RadicalYieldInstruction : IRadicalYieldInstruction
    {

        #region Fields

        private bool _complete;

        #endregion

        #region Properties

        public bool IsComplete { get { return _complete; } }

        #endregion

        #region Methods

        protected virtual void SetSignal()
        {
            _complete = true;
        }

        protected void ResetSignal()
        {
            _complete = false;
        }

        protected virtual bool Tick(out object yieldObject)
        {
            yieldObject = null;
            return !_complete;
        }

        #endregion

        #region IRadicalYieldInstruction Interface

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            if (_complete)
            {
                yieldObject = null;
                return false;
            }

            return this.Tick(out yieldObject);
        }

        #endregion



        #region Static Interface

        public static IProgressingYieldInstruction Null
        {
            get
            {
                return NullYieldInstruction.Null;
            }
        }

        #endregion

    }

}
