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
        
        IButtonInputSignature CreateButtonSignature(string id, TButton button, Joystick joystick = Joystick.All);

        IButtonInputSignature CreateButtonSignature(string id, TAxis axis, AxleValueConsideration consideration = AxleValueConsideration.Positive, Joystick joystick = Joystick.All, float axleButtonDeadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE);

        IAxleInputSignature CreateAxisSignature(string id, TAxis axis, Joystick joystick = Joystick.All);

        IDualAxleInputSignature CreateDualAxisSignature(string id, TAxis axisX, TAxis axisY, Joystick joystick = Joystick.All);

    }

}
