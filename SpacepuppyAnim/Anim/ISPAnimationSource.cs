using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Anim
{

    /// <summary>
    /// A source of SPAnim's, like SPAnimationController.
    /// </summary>
    public interface ISPAnimationSource : IComponent
    {

        ISPAnim GetAnim(string name);

    }
}
