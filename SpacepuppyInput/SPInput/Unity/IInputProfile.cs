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

        bool Contains(TInputId id);
        InputToken GetMapping(TInputId id);

    }

    public interface IConfigurableInputProfile<TInputId> : IInputProfile<TInputId> where TInputId : struct, System.IConvertible
    {
        void SetButtonMapping(TInputId id, InputToken token);
        void SetAxisMapping(TInputId id, InputToken token);
        void Reset();
    }

}
