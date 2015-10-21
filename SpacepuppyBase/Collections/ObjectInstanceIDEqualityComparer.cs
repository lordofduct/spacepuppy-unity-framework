using System.Collections.Generic;

namespace com.spacepuppy.Collections
{

    /// <summary>
    /// Tests for equality based solely on if the references are equal. This is useful for UnityEngine.Objects that overrides the default Equals 
    /// operator returning false if it's been destroyed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectInstanceIDEqualityComparer<T> : EqualityComparer<T> where T : UnityEngine.Object
    {
        private static IEqualityComparer<T> _defaultComparer;

        public new static IEqualityComparer<T> Default
        {
            get { return _defaultComparer ?? (_defaultComparer = new ObjectInstanceIDEqualityComparer<T>()); }
        }

        #region IEqualityComparer<T> Members

        public override bool Equals(T x, T y)
        {
            return GetHashCode(x) == GetHashCode(y);
        }

        public override int GetHashCode(T obj)
        {
            if (object.ReferenceEquals(obj, null)) return 0;
            return obj.GetInstanceID();
        }

        #endregion
    }
}
