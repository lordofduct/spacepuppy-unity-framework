using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace com.spacepuppy.Collections
{

    /// <summary>
    /// Tests for equality based solely on if the references are equal. This is useful for UnityEngine.Objects that overrides the default Equals 
    /// operator returning false if it's been destroyed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectReferenceEqualityComparer<T> : EqualityComparer<T> where T : class
    {
        private static IEqualityComparer<T> _defaultComparer;

        public new static IEqualityComparer<T> Default
        {
            get { return _defaultComparer ?? (_defaultComparer = new ObjectReferenceEqualityComparer<T>()); }
        }

        #region IEqualityComparer<T> Members

        public override bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public override int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }

        #endregion
    }
}
