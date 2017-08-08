using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Anim
{

    /// <summary>
    /// A source of SPAnim's, like SPAnimationController.
    /// </summary>
    public interface ISPAnimationSource
    {

        /// <summary>
        /// If the source can return valid animations that are playable. 
        /// Can be false if for instance the component is disabled, or being destroyed (if it's a component).
        /// </summary>
        bool CanPlayAnim { get; }

        ISPAnim GetAnim(string name);

    }

    //public interface ISPAnimClipSource
    //{

    //    SPAnimClip this[string name] { get; }

    //    SPAnimClip GetClip(string name);

    //}

}
