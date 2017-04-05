using UnityEngine;
using System.Collections.Generic;
using System;

namespace com.spacepuppy.Anim
{
    
    /// <summary>
    /// A custom AnimationClip that isn't a Unity AnimationClip.
    /// 
    /// See 'ScriptedAnimationClip' for more information.
    /// </summary>
    public interface IScriptableAnimationClip
    {

        float Length { get; }

        IScriptableAnimationState GetState(SPAnimationController controller);

    }
    
    public interface IScriptableAnimationState
    {

        float Length { get; }
        int Layer { get; set; }


        void OnStart();

        bool Tick(bool layerIsObscured);

        void OnStop();

    }

}
