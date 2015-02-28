using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Scenes
{
    public class ModifiedLoadedSceneContainerException : InvalidOperationException
    {

        public ModifiedLoadedSceneContainerException()
            : base("Attempted to modify the contents of a SceneContainer that is already loaded.")
        {

        }

        public ModifiedLoadedSceneContainerException(System.Exception innerException)
            : base("Attempted to modify the contents of a SceneContainer that is already loaded.", innerException)
        {

        }

        public ModifiedLoadedSceneContainerException(string msg)
            : base(msg)
        {

        }

        public ModifiedLoadedSceneContainerException(string msg, System.Exception innerException)
            : base(msg, innerException)
        {

        }

    }
}
