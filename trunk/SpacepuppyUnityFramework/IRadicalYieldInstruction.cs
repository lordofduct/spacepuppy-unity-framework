using UnityEngine;

namespace com.spacepuppy
{
    public interface IRadicalYieldInstruction
    {
        object CurrentYieldObject { get; }

        bool ContinueBlocking(RadicalCoroutine routine);
    }
}
