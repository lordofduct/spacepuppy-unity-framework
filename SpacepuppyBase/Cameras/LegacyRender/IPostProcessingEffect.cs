using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Cameras.LegacyRender
{

    public interface IPostProcessingEffect
    {

        bool enabled { get; set; }

        void RenderImage(ICamera camera, RenderTexture source, RenderTexture destination);

    }

}
