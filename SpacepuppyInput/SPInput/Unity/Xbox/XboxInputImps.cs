using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox
{

    public interface IXboxInputProfile : IInputProfile<XboxButton, XboxAxis>
    {
        
    }

    public class XboxInputKeyboardProfile : KeyboardProfile<XboxButton, XboxAxis>, IXboxInputProfile
    {

    }

    public class XboxInputProfileLookupTable : InputProfileLookupTable<IXboxInputProfile, XboxButton, XboxAxis>
    {
    }
    
}
