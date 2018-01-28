using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity
{

    public interface IInputProfile<TInputId> where TInputId : struct, System.IConvertible
    {

        bool TryPollButton(out TInputId button, Joystick joystick = Joystick.All);

        bool TryPollAxis(out TInputId axis, out float value, Joystick joystick = Joystick.All, float deadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE);
        
        InputToken GetMapping(TInputId id);

    }

}
