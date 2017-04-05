using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.Sensors.Collision
{
    public class RaycastSensor : Sensor
    {

        #region Fields

        [SerializeField()]
        private CartesianAxis _axis;
        [SerializeField()]
        [MinRange(0f)]
        private float _distance = 1f;
        [SerializeField()]
        [MinRange(0f)]
        [Tooltip("A radius greater than 0 will cast as a sphere. Note that testing visibility with a sphere cast can be very expensive, especially if inifinite distance.")]
        private float _radius;
        [SerializeField()]
        [Tooltip("When Sense is called should all overlaps be returned. Note this is MUCH slower, especially for rays of infinite distance.")]
        private bool _getAll;
        [SerializeField()]
        private LayerMask _layers = -1;

        #endregion

        #region Properties

        public CartesianAxis Axis
        {
            get { return _axis; }
            set { _axis = value; }
        }

        public float Distance
        {
            get { return _distance; }
            set { _distance = Mathf.Clamp(value, 0f, float.PositiveInfinity); }
        }

        public float Radius
        {
            get { return _radius; }
            set { _radius = Mathf.Clamp(value, 0f, float.PositiveInfinity); }
        }

        public bool GetAll
        {
            get { return _getAll; }
            set { _getAll = value; }
        }

        public LayerMask LayerMask
        {
            get { return _layers; }
            set { _layers = value; }
        }

        #endregion

        #region Methods

        private Collider GetFirstHit()
        {
            var t = this.transform;
            var ray = new Ray(t.position, t.GetAxis(_axis));
            RaycastHit hit;
            if (_radius > 0f)
            {
                if (Physics.SphereCast(ray, _radius, out hit, _distance, _layers))
                {
                    return hit.collider;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (Physics.Raycast(ray, out hit, _distance, _layers))
                {
                    return hit.collider;
                }
                else
                {
                    return null;
                }
            }
        }

        private RaycastHit[] GetAllHits()
        {
            var t = this.transform;
            var ray = new Ray(t.position, t.GetAxis(_axis));
            if (_radius > 0f)
            {
                return Physics.SphereCastAll(ray, _radius, _distance, _layers);
            }
            else
            {
                return Physics.RaycastAll(ray, _distance, _layers);
            }
        }

        #endregion

        #region Sensor Interface

        public override bool ConcernedWith(UnityEngine.Object obj)
        {
            var go = GameObjectUtil.GetGameObjectFromSource(obj);
            if (go == null) return false;
            if (!go.IntersectsLayerMask(_layers)) return false;

            return true;
        }

        public override bool SenseAny(System.Func<IAspect, bool> p = null)
        {
            var t = this.transform;
            var ray = new Ray(t.position, t.GetAxis(_axis));

            if(p == null)
            {
                if (_radius > 0f)
                {
                    return Physics.SphereCast(ray, _radius, _distance, _layers);
                }
                else
                {
                    return Physics.Raycast(ray, _distance, _layers);
                }
            }
            else
            {
                RaycastHit[] arr;
                if (_radius > 0f)
                {
                    arr = Physics.SphereCastAll(ray, _radius, _distance, _layers);
                }
                else
                {
                    arr = Physics.SphereCastAll(ray, _distance, _layers);
                }

                for(int i = 0; i < arr.Length; i++)
                {
                    var a = ColliderAspect.GetAspect(arr[i].collider);
                    if (p(a)) return true;
                }
            }

            return false;
        }

        public override IAspect Sense(System.Func<IAspect, bool> p = null)
        {
            if(p == null)
            {
                var hit = this.GetFirstHit();
                return (hit != null) ? ColliderAspect.GetAspect(hit) : null;
            }
            else
            {
                return this.SenseAll(p).FirstOrDefault();
            }
        }

        public override IEnumerable<IAspect> SenseAll(System.Func<IAspect, bool> p = null)
        {
            if(_getAll)
            {
                var hits = this.GetAllHits();
                if(p == null)
                {
                    return (from h in hits select ColliderAspect.GetAspect(h.collider) as IAspect);
                }
                else
                {
                    return (from h in hits let a = ColliderAspect.GetAspect(h.collider) as IAspect where p(a) select a);
                }
            }
            else if(p == null)
            {
                var hit = this.GetFirstHit();
                return (hit != null) ? new IAspect[] { ColliderAspect.GetAspect(hit) } : Enumerable.Empty<IAspect>();
            }
            else
            {
                var hits = this.GetAllHits();
                var hit = (from h in hits let a = ColliderAspect.GetAspect(h.collider) as IAspect where p(a) select a).FirstOrDefault();
                return (hit != null) ? new IAspect[] { hit } : Enumerable.Empty<IAspect>();
            }
        }

        public override int SenseAll(ICollection<IAspect> lst, System.Func<IAspect, bool> p = null)
        {
            if (lst == null) throw new System.ArgumentNullException("lst");
            if (lst.IsReadOnly) throw new System.ArgumentException("List to fill can not be read-only.", "lst");

            if(_getAll)
            {
                var hits = this.GetAllHits();
                if (p == null)
                {
                    foreach(var h in hits)
                    {
                        lst.Add(ColliderAspect.GetAspect(h.collider));
                    }
                    return hits.Length;
                }
                else
                {
                    int cnt = 0;
                    IAspect a;
                    foreach(var h in hits)
                    {
                        a = ColliderAspect.GetAspect(h.collider);
                        if(p(a))
                        {
                            lst.Add(a);
                            cnt++;
                        }
                    }
                    return cnt;
                }
            }
            else if(p == null)
            {
                var hit = this.GetFirstHit();
                if(hit != null)
                {
                    lst.Add(ColliderAspect.GetAspect(hit));
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                var hits = this.GetAllHits();
                var hit = (from h in hits let a = ColliderAspect.GetAspect(h.collider) as IAspect where p(a) select a).FirstOrDefault();
                if (hit != null)
                {
                    lst.Add(hit);
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public override int SenseAll<T>(ICollection<T> lst, System.Func<T, bool> p = null)
        {
            if (lst == null) throw new System.ArgumentNullException("lst");
            if (lst.IsReadOnly) throw new System.ArgumentException("List to fill can not be read-only.", "lst");
            
            int cnt = lst.Count;
            using (var tc = com.spacepuppy.Collections.TempCollection.GetList<IAspect>())
            {

                if (this.SenseAll(tc, null) > 0)
                {
                    var e = tc.GetEnumerator();
                    if (p == null)
                    {
                        while (e.MoveNext())
                        {
                            if (e.Current is T) lst.Add(e.Current as T);
                        }
                    }
                    else
                    {
                        while (e.MoveNext())
                        {
                            if (e.Current is T && p(e.Current as T)) lst.Add(e.Current as T);
                        }
                    }
                }
            }
            return lst.Count - cnt;
        }

        public override bool Visible(IAspect aspect)
        {
            var colAspect = aspect as ColliderAspect;
            if (colAspect == null || colAspect.Collider == null) return false;

            if(_radius > 0f)
            {
                var c = colAspect.Collider;
                var hits = this.GetAllHits();
                foreach(var h in hits)
                {
                    if (h.collider == c) return true;
                }
                return false;
            }
            else
            {
                var t = this.transform;
                var ray = new Ray(t.position, t.GetAxis(_axis));
                RaycastHit hit;
                return colAspect.Collider.Raycast(ray, out hit, _distance);
            }

        }

        #endregion

    }
}
