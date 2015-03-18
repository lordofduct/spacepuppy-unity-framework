using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Tween
{

    public enum TweenUpdateType
    {

        Update = 0,
        FixedUpdate = 1,
        LateUpdate = 2

    }

    public enum TweenDeltaType
    {
        Normal = 0,
        Real = 1,
        Smooth = 2
    }

    public enum TweenWrapMode
    {
        Once = 0,
        Loop = 1,
        PingPong = 2
    }

}
