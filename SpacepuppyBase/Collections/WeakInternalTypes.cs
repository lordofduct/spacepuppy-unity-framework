using System;
using System.Collections.Generic;

namespace com.spacepuppy.Collections
{

    internal class WeakReference<T> : WeakReference where T : class
    {
        public static WeakReference<T> Create(T target)
        {
            if (target == null) return WeakNullReference<T>.Singleton;

            return new WeakReference<T>(target);
        }

        protected WeakReference(T target)
            : base(target, false)
        {

        }

        public new T Target
        {
            get { return base.Target as T; }
        }
    }

    internal class WeakNullReference<T> : WeakReference<T> where T : class
    {
        public static WeakNullReference<T> Singleton = new WeakNullReference<T>();

        private WeakNullReference()
            : base(null)
        {

        }

        public override bool IsAlive
        {
            get
            {
                return true;
            }
        }
    }

    internal class WeakKeyReference<T> : WeakReference<T> where T : class
    {
        public readonly int HashCode;

        public WeakKeyReference(T key, WeakKeyComparer<T> comparer)
            : base(key)
        {
            HashCode = comparer.GetHashCode(key);
        }
    }

    internal class WeakKeyComparer<T> : IEqualityComparer<object> where T : class
    {
        private IEqualityComparer<T> _comparer;

        internal WeakKeyComparer(IEqualityComparer<T> comparer = null)
        {
            if (comparer == null) comparer = EqualityComparer<T>.Default;

            _comparer = comparer;
        }

        #region IEqualityComparer Interface

        public int GetHashCode(object obj)
        {
            if (obj is WeakKeyReference<T>) return (obj as WeakKeyReference<T>).HashCode;
            if (obj is WeakReference) return _comparer.GetHashCode((obj as WeakReference<T>).Target);
            return _comparer.GetHashCode(obj as T);
        }

        /// <summary>
        /// Compares two object by this WeakKey rule.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>
        /// There are 9 cases to handle here (3 state, 3^2 permutations, 9).
        /// 
        /// Wa == Alive Weak Reference
        /// Wd == Dead Weak Reference
        /// S  == Strong Reference
        ///   x  |  y  |  Equals(x,y)
        /// ------------------------------------
        ///  Wa  | Wa  |  comparer.Equals(x.Target, y.Target)
        ///  Wa  | Wd  |  False
        ///  Wa  | S   |  comparer.Equals(x.Target, y)
        ///  Wd  | Wa  |  False
        ///  Wd  | Wd  |  x == y
        ///  Wd  | S   |  False
        ///  S   | Wa  |  comparer.Equals(x, y.Target)
        ///  S   | Wd  |  False
        ///  S   | S   |  comparer.Equals(x, y)
        /// </returns>
        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            bool xIsDead = false;
            bool yIsDead = false;
            var a = GetTarget(x, out xIsDead);
            var b = GetTarget(y, out yIsDead);

            if (xIsDead) return (yIsDead) ? (x == y) : false;

            if (yIsDead) return false;

            return _comparer.Equals(a, b);
        }

        #endregion

        private static T GetTarget(object obj, out bool outIsDead)
        {
            var wref = obj as WeakKeyReference<T>;
            T target;

            if (wref != null)
            {
                target = wref.Target;
                outIsDead = !wref.IsAlive;
            }
            else
            {
                target = obj as T;
                outIsDead = false;
            }
            return target;
        }

    }

    internal class WeakReferenceComparer : IEqualityComparer<object>
    {

        private System.Collections.IEqualityComparer _comparer;

        internal WeakReferenceComparer(System.Collections.IEqualityComparer comparer = null)
        {
            if (comparer == null) comparer = EqualityComparer<object>.Default;

            _comparer = comparer;
        }

        #region IEqualityComparer Interface

        public int GetHashCode(object obj)
        {
            if (obj is WeakReference) return _comparer.GetHashCode((obj as WeakReference).Target);
            return _comparer.GetHashCode(obj);
        }

        /// <summary>
        /// Compares two object by this WeakKey rule.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>
        /// There are 9 cases to handle here (3 state, 3^2 permutations, 9).
        /// 
        /// Wa == Alive Weak Reference
        /// Wd == Dead Weak Reference
        /// S  == Strong Reference
        ///   x  |  y  |  Equals(x,y)
        /// ------------------------------------
        ///  Wa  | Wa  |  comparer.Equals(x.Target, y.Target)
        ///  Wa  | Wd  |  False
        ///  Wa  | S   |  comparer.Equals(x.Target, y)
        ///  Wd  | Wa  |  False
        ///  Wd  | Wd  |  x == y
        ///  Wd  | S   |  False
        ///  S   | Wa  |  comparer.Equals(x, y.Target)
        ///  S   | Wd  |  False
        ///  S   | S   |  comparer.Equals(x, y)
        /// </returns>
        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            bool xIsDead = false;
            bool yIsDead = false;
            var a = GetTarget(x, out xIsDead);
            var b = GetTarget(y, out yIsDead);

            if (xIsDead) return (yIsDead) ? (x == y) : false;

            if (yIsDead) return false;

            return _comparer.Equals(a, b);
        }

        #endregion

        private static object GetTarget(object obj, out bool outIsDead)
        {
            var wref = obj as WeakReference;
            object target;

            if (wref != null)
            {
                target = wref.Target;
                outIsDead = !wref.IsAlive;
            }
            else
            {
                target = obj;
                outIsDead = false;
            }
            return target;
        }

    }

}
