using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Cameras
{

    public interface ICameraMovementControllerState
    {

        /// <summary>
        /// Entering this state.
        /// </summary>
        /// <param name="controller"></param>
        void Activate(CameraMovementController controller);

        /// <summary>
        /// Exiting this state.
        /// </summary>
        void Deactivate();

        /// <summary>
        /// The controller has been paused, react accordingly if necessary.
        /// </summary>
        void OnPaused();

        /// <summary>
        /// The controller resumed after being paused, react accordingly.
        /// </summary>
        void OnResumed();

        /// <summary>
        /// Update the movement per frame.
        /// </summary>
        void UpdateMovement();

        /// <summary>
        /// If the movement style follows some target (like the TetherTarget), snap to it immediately.
        /// </summary>
        void SnapToTarget();

    }

}
