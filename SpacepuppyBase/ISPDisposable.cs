using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy
{
    
    /// <summary>
    /// Intended for use with classes that wrap around UnityObjects and need to signal if that 
    /// object has been destroyed (would return true if compared to 'null'). 
    /// 
    /// This is complimented with an 'IsDisposed' extension method so that you can test diposed as 
    /// well as null reference all in one command. See com.spacepuppy.Utils.ObjUtil for more.
    /// 
    /// This is primarily used outside of the spacepuppy base library, and instead the Anim/AI/Movement 
    /// libraries. For an example of its use in the base library see SPComponent or VariantReference.
    /// </summary>
    public interface ISPDisposable : System.IDisposable
    {

        bool IsDisposed { get; }

    }
}
