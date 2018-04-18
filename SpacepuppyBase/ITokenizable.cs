using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy
{

    /// <summary>
    /// Represents an object that handles its own creation of a token of its state. 
    /// 
    /// A token should be serializable.
    /// 
    /// This contract will be respected by com.spacepuppy.Dynamics when trying to receive a StateToken of an object. 
    /// If the object implements this, the interface will be used, otherwise a <see cref="com.spacepuppy.Dynamic.StateToken"/> will be used.
    /// </summary>
    public interface ITokenizable
    {

        object GetStateToken();
        void RestoreFromStateToken(object token);

    }
}
