using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Scenes
{
    public enum LoadSceneBehaviour
    {
        /// <summary>
        /// like unity's SceneManager.LoadScene. Due to the way Unity implements this, it is considered glitchy by the SP team. Thusly it is set to -1.
        /// </summary>
        Standard = -1,
        /// <summary>
        /// The preferred default method.
        /// </summary>
        Async = 0,
        /// <summary>
        /// Loads async, but does not activate scene.
        /// </summary>
        AsyncAndWait = 1
    }
}
