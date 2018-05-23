using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy
{

    /// <summary>
    /// Implement on a component that should receive the 'EntityAwake' message. 
    /// This is called after the entire entity hierarchy has had Awake called on it, but before Start is called.
    /// 
    /// This is useful if you want to initialize a script inside an entity after the entire entity has had 
    /// Awake called on it. This is called on all scripts, including those that are inactive. If a script is 
    /// added after the Entity was originaly created (and Awake called on the Entity script), this message will 
    /// not be called.
    /// </summary>
    public interface IEntityAwakeHandler
    {

        void OnEntityAwake(SPEntity entity);

    }

}
