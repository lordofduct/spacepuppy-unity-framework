
namespace com.spacepuppy
{

    /// <summary>
    /// By implementing this interface, the RadicalCoroutine knows to call Dispose on the instruction when it is 
    /// done yielding for its duration. The instruction can than return itself to a object cache pool during Dispose 
    /// for reuse later.
    /// </summary>
    public interface IPooledYieldInstruction : IRadicalYieldInstruction, System.IDisposable
    {

    }

}
