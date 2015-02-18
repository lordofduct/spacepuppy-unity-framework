using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy
{

    /// <summary>
    /// Contract for an object that acts as the token for a coroutine
    /// </summary>
    public interface IRadicalAsyncOperation : IRadicalYieldInstruction
    {

        bool IsComplete { get; }

    }

    public interface IProgressingAsyncOperation : IRadicalAsyncOperation
    {
        float Progress { get; }
    }

}
