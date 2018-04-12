using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.AI.Sensors.Audible
{

    public interface IAudibleResponder
    {

        /// <summary>
        /// Occurs when a blip occurs, or when entering a siren.
        /// </summary>
        /// <param name="aspect">The sound source.</param>
        /// <param name="isSiren">If the sound is the the beginning of a siren.</param>
        void OnSound(IAspect aspect, bool isSiren);

    }

    public interface IAudibleSirenResponder : IAudibleResponder
    {

        /// <summary>
        /// Stayed in range of a siren.
        /// </summary>
        /// <param name="aspect">The sound source.</param>
        void OnSoundStay(IAspect aspect);

        /// <summary>
        /// Exited the range of a siren.
        /// </summary>
        /// <param name="aspect">The sound source.</param>
        void OnSoundExit(IAspect aspect);

    }

}
