using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.AI.Sensors.Visual
{
    public class VisualAspect : SPComponent, IAspect
    {

        #region Static Multiton Interface

        private static MultitonPool<IAspect> _pool = new MultitonPool<IAspect>();

        public static MultitonPool<IAspect> Pool { get { return _pool; } }

        [System.Obsolete("Use VisualAspect.Pool instead.")]
        public static IAspect[] GetAspects()
        {
            return _pool.FindAll(null);
        }

        public static bool Any(System.Func<IAspect, bool> predicate)
        {
            return !object.ReferenceEquals(_pool.Find(predicate), null);
        }

        [System.Obsolete("Use VisualAspect.Pool.Find instead.")]
        public static IAspect GetAspect(System.Func<IAspect, bool> predicate)
        {
            return _pool.Find(predicate);
        }

        [System.Obsolete("Use VisualAspect.Pool.FindAll instead.")]
        public static IList<IAspect> GetAspects(System.Func<IAspect, bool> predicate)
        {
            return _pool.FindAll(predicate);
        }

        [System.Obsolete("Use VisualAspect.Pool.FindAll instead.")]
        public static int GetAspects(ICollection<IAspect> lst, System.Func<IAspect, bool> predicate)
        {
            return _pool.FindAll(lst, predicate);
        }

        [System.Obsolete("Use VisualAspect.Pool.FindAll instead.")]
        public static int GetAspects<T>(ICollection<T> lst, System.Func<T, bool> predicate) where T : class, IAspect
        {
            return _pool.FindAll<T>(lst, predicate);
        }

        #endregion




        #region Fields

        [SerializeField()]
        private float _precedence;

        [SerializeField()]
        private Color _aspectColor = Color.blue;

        #endregion

        #region CONSTRUCTOR

        protected override void OnEnable()
        {
            _pool.AddReference(this);

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _pool.RemoveReference(this);
        }

        #endregion

        #region IAspect Interface

        bool IAspect.IsActive
        {
            get { return this.isActiveAndEnabled; }
        }

        public float Precedence
        {
            get { return _precedence; }
            set { _precedence = value; }
        }

        public Color AspectColor
        {
            get { return _aspectColor; }
            set { _aspectColor = value; }
        }

        #endregion

    }
}
