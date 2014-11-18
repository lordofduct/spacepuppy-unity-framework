using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace com.spacepuppy.Collections
{
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
