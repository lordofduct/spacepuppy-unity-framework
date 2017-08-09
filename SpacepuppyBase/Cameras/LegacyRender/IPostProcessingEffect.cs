using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Cameras.LegacyRender
{
    public interface IPostProcessingEffect
    {

        void RenderImage(RenderTexture source, RenderTexture destination);

    }
}
