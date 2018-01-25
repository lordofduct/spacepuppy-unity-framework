using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox
{

    public interface IXboxInputProfile : IInputProfile<XboxButton, XboxAxis>
    {

        /*
        IButtonInputSignature CreateButtonSignature(string id, XboxButton button, SPJoystick joystick = SPJoystick.All);

        IButtonInputSignature CreateButtonSignature(string id, XboxAxis axis, AxleValueConsideration consideration = AxleValueConsideration.Positive, SPJoystick joystick = SPJoystick.All, float axleButtonDeadZone = AxleButtonInputSignature.DEFAULT_BTNDEADZONE);

        IAxleInputSignature CreateAxisSignature(string id, XboxAxis axis, SPJoystick joystick = SPJoystick.All);

        IDualAxleInputSignature CreateDualAxisSignature(string id, XboxAxis axisX, XboxAxis axisY, SPJoystick joystick = SPJoystick.All);
        */

    }

}
