using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity
{

    public interface IInputProfile<TButton, TAxis> where TButton : struct, System.IConvertible where TAxis : struct, System.IConvertible
    {

        IButtonInputSignature CreateButtonSignature(string id, TButton button, SPJoystick joystick = SPJoystick.All);

        IButtonInputSignature CreateButtonSignature(string id, TAxis axis, AxleValueConsideration consideration = AxleValueConsideration.Positive, SPJoystick joystick = SPJoystick.All, float axleButtonDeadZone = AxleButtonInputSignature.DEFAULT_BTNDEADZONE);

        IAxleInputSignature CreateAxisSignature(string id, TAxis axis, SPJoystick joystick = SPJoystick.All);

        IDualAxleInputSignature CreateDualAxisSignature(string id, TAxis axisX, TAxis axisY, SPJoystick joystick = SPJoystick.All);

    }

}
