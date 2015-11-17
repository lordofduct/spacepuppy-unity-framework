
namespace com.spacepuppy
{

    /// <summary>
    /// A contract joint of IEnumerator and IRadicalYieldInstruction. This can be implemented by an object that can be used in a dual way as either a IEnuemrator or a yield statement. 
    /// This is useful for objects that can be jsut directly tossed into 'MonoBehaviour.StartCoroutine' and do their job, or used as a yield instruction.
    /// </summary>
    public interface IRadicalEnumerator : System.Collections.IEnumerator, IRadicalYieldInstruction
    {
        //just a contract
    }

}
