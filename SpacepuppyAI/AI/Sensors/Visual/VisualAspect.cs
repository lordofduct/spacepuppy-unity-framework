using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.AI.Sensors.Visual
{
    public class VisualAspect : SPEntityComponent, IAspect
    {

        #region Static Multiton Interface

        public static readonly MultitonPool<IAspect> Pool = new MultitonPool<IAspect>();
        
        [System.Obsolete("Use VisualAspect.Pool instead.")]
        public static IAspect[] GetAspects()
        {
            return Pool.FindAll(null);
        }

        public static bool Any(System.Func<IAspect, bool> predicate)
        {
            return !object.ReferenceEquals(Pool.Find(predicate), null);
        }

        [System.Obsolete("Use VisualAspect.Pool.Find instead.")]
        public static IAspect GetAspect(System.Func<IAspect, bool> predicate)
        {
            return Pool.Find(predicate);
        }

        [System.Obsolete("Use VisualAspect.Pool.FindAll instead.")]
        public static IList<IAspect> GetAspects(System.Func<IAspect, bool> predicate)
        {
            return Pool.FindAll(predicate);
        }

        [System.Obsolete("Use VisualAspect.Pool.FindAll instead.")]
        public static int GetAspects(ICollection<IAspect> lst, System.Func<IAspect, bool> predicate)
        {
            return Pool.FindAll(lst, predicate);
        }

        [System.Obsolete("Use VisualAspect.Pool.FindAll instead.")]
        public static int GetAspects<T>(ICollection<T> lst, System.Func<T, bool> predicate) where T : class, IAspect
        {
            return Pool.FindAll<T>(lst, predicate);
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
            Pool.AddReference(this);

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Pool.RemoveReference(this);
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
