using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Cameras
{
    public sealed class PostProcessorHook : SPComponent
    {

        private void OnRenderImage(UnityEngine.RenderTexture source, UnityEngine.RenderTexture destination)
        {
            
        }

        public static PostProcessorHook GetHook(Camera camera)
        {
            if (camera == null) return null;
            return camera.AddOrGetComponent<PostProcessorHook>();
        }

    }
}
