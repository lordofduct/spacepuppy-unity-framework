
namespace com.spacepuppy
{

    /// <summary>
    /// Implement this if the coroutine should immediately resume on complete instead of waiting to the next frame. This is used 
    /// in situations that are similar to WaitForFixedUpdate, where we want to wait for some section of the update pipeline and 
    /// need to operate immediately.
    /// </summary>
    public interface IImmediatelyResumingYieldInstruction : IRadicalYieldInstruction
    {
        event System.EventHandler Signal;
    }

    /// <summary>
    /// Base abstract class that implement IImmediatelyResumingYieldInstruction. Inherit from this instead of directly implementing 
    /// IImmediatelyResumingYieldInstruction, unless you can't otherwise.
    /// </summary>
    public abstract class ImmediatelyResumingYieldInstruction : RadicalYieldInstruction, IImmediatelyResumingYieldInstruction
    {

        private System.EventHandler _handler;

        protected override void SetSignal()
        {
            base.SetSignal();
            if (_handler != null) _handler(this, System.EventArgs.Empty);
        }

        event System.EventHandler IImmediatelyResumingYieldInstruction.Signal
        {
            add { _handler += value; }
            remove { _handler -= value; }
        }

    }

}
