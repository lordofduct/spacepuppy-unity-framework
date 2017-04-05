using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.UserInput
{

    /// <summary>
    /// Represents a sequence that can be registered with the GameInputManager.
    /// </summary>
    public interface ISequence
    {

        /// <summary>
        /// Called by the GameInputManager when the sequence is first registered.
        /// </summary>
        void OnStart();

        /// <summary>
        /// Sequence polling Update
        /// </summary>
        /// <returns>Returns true when sequence is complete</returns>
        bool Update();

    }
}
