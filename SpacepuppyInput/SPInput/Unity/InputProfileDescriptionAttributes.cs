using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity
{


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class InputProfileDescriptionAttribute : System.Attribute
    {
        public string DisplayName;
        public TargetPlatform Platform;
        public string Description;

        public InputProfileDescriptionAttribute(string displayName, TargetPlatform platform)
        {
            this.DisplayName = displayName;
            this.Platform = platform;
        }

    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class InputProfileJoystickNameAttribute : System.Attribute
    {
        public string JoystickName;

        public InputProfileJoystickNameAttribute(string joystickName)
        {
            this.JoystickName = joystickName;
        }

    }
    
}
