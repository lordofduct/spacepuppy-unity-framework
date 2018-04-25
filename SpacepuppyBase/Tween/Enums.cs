using System;

namespace com.spacepuppy.Tween
{

    public enum TweenWrapMode
    {
        Once = 0,
        Loop = 1,
        PingPong = 2
    }

    [Flags()]
    public enum StringTweenStyle
    {
        Default = 0,
        LeftToRight = 0,
        RightToLeft = 1,
        Jumble = 2,
        LeftToRightJumble = 2,
        RightToLeftJumble = 3
    }

    public enum QuaternionTweenOption
    {
        Spherical,
        Linear,
        /// <summary>
        /// Rotate full rotation. So if you pass in values 0,0,0 -> 0,720,0 
        /// it will rotate the full 720 degrees.
        /// </summary>
        Long
    }

}
