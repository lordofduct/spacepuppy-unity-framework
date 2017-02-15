
namespace com.spacepuppy
{

    /// <summary>
    /// By implementing this interface, the RadicalCoroutine knows to call Reset any time it receives the instruction 
    /// 'before' operating the instruction. This way an instruction can clean itself up for reuse if yielded more than once.
    /// </summary>
    public interface IResettingYieldInstruction : IRadicalYieldInstruction
    {

        void Reset();

    }

}
