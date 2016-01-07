

namespace com.spacepuppy.Dynamic
{

    /// <summary>
    /// Define a component that can be used as a state modifier with its own ruleset. 
    /// 
    /// For example it may store some state information for a Camera, but the target may be a GameObject. 
    /// The IStateModifier can get the Camera from the GameObject and update it accordingly.
    /// </summary>
    public interface IStateModifier
    {

        /// <summary>
        /// Copy the current state of the IModifier to the StateToken. 
        /// Used to allow callers to gather the state and modify it further.
        /// </summary>
        /// <param name="token"></param>
        void CopyTo(StateToken token);
        /// <summary>
        /// Lerp a StateToken to the current state of the IModifier, just pass along what is need to 'StateToken.LerpTo'. 
        /// Used to allow callers to gather the state and modify it further.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="t"></param>
        void LerpTo(StateToken token, float t);
        /// <summary>
        /// Set the state of some target to that of the IModifier using the IModifier rules.
        /// </summary>
        /// <param name="targ"></param>
        void Modify(object targ);
        /// <summary>
        /// Set the state of some target to that of the StateToken using the IModifier rules.
        /// </summary>
        /// <param name="targ"></param>
        /// <param name="token"></param>
        void ModifyWith(object targ, StateToken token);

    }

}
