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

        private static readonly PointOctree<IAspect> _staticOctree = new PointOctree<IAspect>(10, (a) => a != null ? a.transform.position : Vector3.zero);
        private static readonly PointOctree<IAspect> _dynamicOctree = new PointOctree<IAspect>(10, (a) => a != null ? a.transform.position : Vector3.zero);
        private static readonly HashSet<VisualAspect> _omniAspects = new HashSet<VisualAspect>();

        public static int GetNearby(ICollection<IAspect> coll, Vector3 pos, float radius)
        {
            //TODO - replace this with the octree implementation once bug fixed

            int cnt = 0;

            var e = Pool.GetEnumerator();
            float r2 = radius * radius;
            while(e.MoveNext())
            {
                if(e.Current.OmniPresent || (e.Current.transform.position - pos).sqrMagnitude <= r2)
                {
                    cnt++;
                    coll.Add(e.Current);
                }
            }

            return cnt;
        }

        /*
         * TODO - must first bug-fix Octree to use this.
         * 
        
        public static int GetNearby(ICollection<IAspect> coll, Vector3 pos, float radius)
        {
            int cnt = 0;

            if(_omniAspects.Count > 0)
            {
                var e = _omniAspects.GetEnumerator();
                while(e.MoveNext())
                {
                    coll.Add(e.Current);
                    cnt++;
                }
            }

            if(_staticOctree.Count > 0)
            {
                cnt += _staticOctree.GetNearby(coll, pos, radius);
            }

            if (_dynamicOctree.Count > 0)
            {
                cnt += _dynamicOctree.GetNearby(coll, pos, radius);
            }

            return cnt;
        }

        static VisualAspect()
        {
            GameLoopEntry.EarlyUpdate += OnUpdate;
        }

        private static float _timer;
        private static void OnUpdate(object sender, System.EventArgs ev)
        {
            _timer += Time.unscaledDeltaTime;
            if(_timer > 0.1f)
            {
                _timer = 0f;
                _dynamicOctree.Resync();
            }
        }
         */

        private void CategorizeAspect()
        {
            Pool.AddReference(this);
            //if (_omniPresent)
            //{
            //    _omniAspects.Add(this);
            //}
            //else if (_dynamic)
            //{
            //    _dynamicOctree.Add(this);
            //}
            //else
            //{
            //    _staticOctree.Add(this);
            //}
        }

        private void DecategorizeAspect()
        {
            Pool.RemoveReference(this);
            //if (_omniPresent)
            //{
            //    _omniAspects.Remove(this);
            //}
            //else if (_dynamic)
            //{
            //    _dynamicOctree.Remove(this);
            //}
            //else
            //{
            //    _staticOctree.Remove(this);
            //}
        }

        #endregion

        #region Fields

        [SerializeField()]
        private float _precedence;

        [SerializeField]
        [MinRange(0f)]
        private float _radius;

        [SerializeField()]
        private Color _aspectColor = Color.blue;

        [SerializeField]
        [Tooltip("This Aspect is always visible regardless.")]
        private bool _omniPresent;

        [SerializeField]
        [Tooltip("This aspect can move and therefore should have the octree updated.")]
        private bool _dynamic;

        #endregion

        #region CONSTRUCTOR

        protected override void OnEnable()
        {
            this.CategorizeAspect();

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            this.DecategorizeAspect();
        }

        #endregion

        #region Properties
        
        public float Radius
        {
            get { return _radius; }
            set { _radius = value = Mathf.Max(0f, value); }
        }
        
        public bool OmniPresent
        {
            get { return _omniPresent; }
            set
            {
                if (_omniPresent == value) return;

                if (this.isActiveAndEnabled)
                {
                    this.DecategorizeAspect();
                    _omniPresent = value;
                    this.CategorizeAspect();
                }
                else
                {
                    _omniPresent = value;
                }
            }
        }

        public bool Dynamic
        {
            get { return _dynamic; }
            set
            {
                if (_dynamic == value) return;

                if (this.isActiveAndEnabled)
                {
                    this.DecategorizeAspect();
                    _dynamic = value;
                    if (this.isActiveAndEnabled) this.CategorizeAspect();
                }
                else
                {
                    _dynamic = value;
                }
            }
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
