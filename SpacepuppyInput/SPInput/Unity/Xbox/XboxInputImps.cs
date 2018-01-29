using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput.Unity.Xbox
{

    public interface IXboxInputProfile : IConfigurableInputProfile<XboxInputId>
    {

    }

    public class XboxInputKeyboardProfile : KeyboardProfile<XboxInputId>, IXboxInputProfile
    {

    }

    public class XboxInputProfileLookupTable : InputProfileLookupTable<IXboxInputProfile, XboxInputId>
    {
    }
    
}
