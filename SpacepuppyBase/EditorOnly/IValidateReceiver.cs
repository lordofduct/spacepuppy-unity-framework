using System;

namespace com.spacepuppy.EditorOnly
{
    /// <summary>
    /// Receives a signal that the inspector validated for a component/scriptableobject that this object is a member of.
    /// Only sent to objects that aren't serialized by reference (custom classes/structs marked serializable). 
    /// Make sure your SPSettings has UseSPEditorAsDefault and SignalValidateReceiver enabled.
    /// </summary>
    public interface IValidateReceiver
    {

        void OnValidate();

    }
}
