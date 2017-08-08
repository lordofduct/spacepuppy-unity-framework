using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Cameras
{
    public enum CameraCategory
    {
        Other = 31,
        Gameplay = 0,
        UI = 1
    }

    [System.Flags]
    public enum CameraCategoryMask
    {
        Gameplay = 1,
        UI = 2,
        Other = (1 << 31)
    }
    
}
