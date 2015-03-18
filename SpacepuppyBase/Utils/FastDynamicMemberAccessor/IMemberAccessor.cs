//
// IMemberAccessor.cs
//
// Author: James Nies
// Licensed under The Code Project Open License (CPOL): http://www.codeproject.com/info/cpol10.aspx

namespace com.spacepuppy.Utils.FastDynamicMemberAccessor
{
    /// <summary>
    /// The IMemberAccessor interface defines a member
    /// accessor.
    /// </summary>
    public interface IMemberAccessor
    {
        /// <summary>
        /// Gets the value stored in the member for
        /// the specified target.
        /// </summary>
        /// <param name="target">Object to retrieve
        /// the member from.</param>
        /// <returns>Member value.</returns>
        object Get(object target);

        /// <summary>
        /// Sets the value for the member of
        /// the specified target.
        /// </summary>
        /// <param name="target">Object to set the
        /// member on.</param>
        /// <param name="value">Member value.</param>
        void Set(object target, object value);
    }
}