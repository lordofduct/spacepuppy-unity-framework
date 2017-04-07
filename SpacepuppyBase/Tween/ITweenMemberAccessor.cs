
using com.spacepuppy.Dynamic.Accessors;

namespace com.spacepuppy.Tween
{

    /// <summary>
    /// Implement this interface along with the class attribute 'CustomTweenMemberAccessorAttribute' to define a custom accessor 
    /// that will be used to set the value of an object.
    /// 
    /// The CustomTweenMemberAccessorAttribute requires accepts a type to apply this accessor to, as well as the name of the property. 
    /// The property doesn't have to be an actual property on the type, and can be any custom name you define. For example the 
    /// 'GeneralMoveAccessor' is called upon for Rigidbody, Transform, and GameObject for the property "*Move".
    /// </summary>
    public interface ITweenMemberAccessor : IMemberAccessor
    {

        /// <summary>
        /// Return the member type that Get/Set expects.
        /// </summary>
        /// <returns></returns>
        System.Type GetMemberType();

        /// <summary>
        /// Initialize the member accessor, and return the type this accessor is supposed to handle.
        /// </summary>
        /// <param name="propName"></param>
        /// <returns>Return the member type that Get/Set expects.</returns>
        System.Type Init(object target, string propName, string args);

    }

}
