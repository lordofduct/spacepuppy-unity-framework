using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Anim
{

    public interface IAnimControllerMask
    {

        bool CanPlay(ISPAnim anim);
        bool CanPlay(SPAnimClip anim);

    }

}
