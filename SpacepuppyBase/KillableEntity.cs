using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// Implement this interface if you want to write a script that handles how the entity is dealt with when 'Kill' is called on it. 
    /// This overrides the default behaviour of destroying the GameObject and child GameObjects. You will have to destroy the objects 
    /// as you see fit. This is useful for things like returning a GameObject to an object pool to be used later instead of destroying it. 
    /// Multiple IKillableEntity scripts on an entity will cause the kill calls to stack, so any script that contradicts another can result 
    /// in bugs.
    /// </summary>
    public interface IKillableEntity : IComponent
    {

        bool IsDead { get; }
        void Kill();

    }

}
