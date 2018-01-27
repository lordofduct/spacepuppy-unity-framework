using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity
{

    public interface IInputProfile<TButton, TAxis> where TButton : struct, System.IConvertible where TAxis : struct, System.IConvertible
    {

        bool TryPollButton(out TButton button, Joystick joystick = Joystick.All);

        bool TryPollAxis(out TAxis axis, out float value, Joystick joystick = Joystick.All, float deadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE);

        ButtonDelegate CreateButtonDelegate(TButton button, Joystick joystick = Joystick.All);
        
        AxisDelegate CreateAxisDelegate(TAxis axis, Joystick joystick = Joystick.All);
        
    }

}
