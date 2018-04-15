
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

        private System.EventHandler _signal;

        protected override void SetSignal()
        {
            base.SetSignal();
            if (_signal != null) _signal(this, System.EventArgs.Empty);
        }

        event System.EventHandler IImmediatelyResumingYieldInstruction.Signal
        {
            add
            {
                if (_signal == null)
                    _signal = value;
                else if (_signal == value)
                    return;
                else
                    _signal += value;
            }
            remove
            {
                if (_signal == null)
                    return;
                else if (_signal == value)
                    _signal = null;
                else
                    _signal -= value;
            }
        }

    }

}
