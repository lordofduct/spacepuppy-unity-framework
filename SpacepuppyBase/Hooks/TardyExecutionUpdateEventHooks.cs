using UnityEngine;

namespace com.spacepuppy.Hooks
{

    /// <summary>
    /// This class exists to be added to the Execution Order Manager at an extra early time. Some managers use this to get early update timing.
    /// </summary>
    /// <remarks>
    /// The project must flag this as later execution in Edit->Project Settings->Execution Order
    /// </remarks>
    [AddComponentMenu("SpacePuppy/Hooks/TardyExecutionUpdateEventHooks")]
    public class TardyExecutionUpdateEventHooks : UpdateEventHooks
    {



    }

}
