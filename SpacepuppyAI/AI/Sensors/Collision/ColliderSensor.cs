#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Geom;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.AI.Sensors.Collision
{
    public class ColliderSensor : Sensor
    {

        [System.Flags()]
        public enum AllowedColliderMode
        {
            Solid = 1,
            Trigger = 2,
            Both = 3
        }

        #region Fields

        [SerializeField()]
        private bool _canDetectSelf;
        [SerializeField()]
        [EnumFlags()]
        private AllowedColliderMode _allowedColliders = AllowedColliderMode.Both;
        [SerializeField()]
        private LayerMask _aspectLayerMask = -1;
        [SerializeField()]
        private TagMask _aspectTagMask;

        [SerializeField()]
        [Tooltip("The line of sight is naive and works as from the position of this to the center of the bounds of the target collider.")]
        private bool _requiresLineOfSight;
        [SerializeField()]
        private LayerMask _lineOfSightMask;


        [System.NonSerialized()]
        private HashSet<Collider> _intersectingColliders = new HashSet<Collider>();

        #endregion

        #region CONSTRUCTOR

        protected override void OnDisable()
        {
            base.OnDisable();

            _intersectingColliders.Clear();
        }

        #endregion

        #region Properties

        public bool CanDetectSelf
        {
            get { return _canDetectSelf; }
            set { _canDetectSelf = value; }
        }
        public AllowedColliderMode AllowedColliders
        {
            get { return _allowedColliders; }
            set { _allowedColliders = value; }
        }
        public LayerMask AspectLayerMask
        {
            get { return _aspectLayerMask; }
            set { _aspectLayerMask = value; }
        }
        public TagMask AspectTagMask
        {
            get { return _aspectTagMask; }
        }

        public bool RequiresLineOfSight
        {
            get { return _requiresLineOfSight; }
            set { _requiresLineOfSight = value; }
        }

        public LayerMask LineOfSightMask
        {
            get { return _lineOfSightMask; }
            set { _lineOfSightMask = value; }
        }

        #endregion

        #region Methods

        protected void OnTriggerEnter(Collider coll)
        {
            if (!this.ConcernedWith(coll)) return;

            _intersectingColliders.Add(coll);
        }

        protected void OnTriggerExit(Collider coll)
        {
            _intersectingColliders.Remove(coll);
        }

        private bool ConcernedWith(Collider coll)
        {
            if (coll == null) return false;
            var mode = (coll.isTrigger) ? AllowedColliderMode.Trigger : AllowedColliderMode.Solid;
            if ((_allowedColliders & mode) == 0) return false;
            if (_aspectLayerMask != -1 && !coll.gameObject.IntersectsLayerMask(_aspectLayerMask)) return false;
            if (!_aspectTagMask.Intersects(coll)) return false;

            if (!_canDetectSelf)
            {
                var root = coll.FindRoot();
                if (root == this.entityRoot) return false;
            }

            return true;
        }

        protected bool IsLineOfSight(Collider coll)
        {
            var v = coll.GetCenter() - this.transform.position;
            //RaycastHit hit;
            //if(Physics.Raycast(this.transform.position, v, out hit, v.magnitude, _lineOfSightMask))
            //{
            //    return (hit.collider == coll);
            //}
            using (var lst = com.spacepuppy.Collections.TempCollection.GetList<RaycastHit>())
            {
                int cnt = PhysicsUtil.RaycastAll(this.transform.position, v, lst, v.magnitude, _lineOfSightMask);
                if(cnt > 0)
                {
                    var otherRoot = coll.FindRoot();
                    for (int i = 0; i < cnt; i++)
                    {
                        //we ignore ourself
                        var r = lst[i].collider.FindRoot();
                        if (r != otherRoot && r != this.entityRoot) return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region Sensor Interface

        public override bool ConcernedWith(UnityEngine.Object obj)
        {
            if(obj is Collider)
            {
                return this.ConcernedWith(obj as Collider);
            }
            else
            {
                var go = GameObjectUtil.GetGameObjectFromSource(obj);
                if (go == null) return false;
                using (var set = com.spacepuppy.Collections.TempCollection.GetSet<Collider>())
                {
                    go.FindComponents<Collider>(set);
                    var e = set.GetEnumerator();
                    while(e.MoveNext())
                    {
                        if (this.ConcernedWith(e.Current))
                            return true;
                    }
                    return false;
                }
            }
        }

        public override bool SenseAny(System.Func<IAspect, bool> p = null)
        {
            if(p == null && !_requiresLineOfSight)
            {
                return _intersectingColliders.Count > 0;
            }
            else
            {
                var e = _intersectingColliders.GetEnumerator();
                while(e.MoveNext())
                {
                    var a = ColliderAspect.GetAspect(e.Current);
                    if ((p == null || p(a)) && (!_requiresLineOfSight || this.IsLineOfSight(e.Current))) return true;
                }
            }

            return false;
        }

        public override bool Visible(IAspect aspect)
        {
            var colAspect = aspect as ColliderAspect;
            if (colAspect == null) return false;

            return _intersectingColliders.Contains(colAspect.Collider);
        }

        public override IAspect Sense(System.Func<IAspect, bool> p = null)
        {
            if (_intersectingColliders.Count == 0) return null;

            if (p == null && !_requiresLineOfSight)
                return ColliderAspect.GetAspect(_intersectingColliders.First());
            else
            {
                var e = _intersectingColliders.GetEnumerator();
                while (e.MoveNext())
                {
                    var a = ColliderAspect.GetAspect(e.Current);
                    if ((p == null || p(a)) && (!_requiresLineOfSight || this.IsLineOfSight(e.Current))) return a;
                }
                return null;
            }
        }

        public override IEnumerable<IAspect> SenseAll(System.Func<IAspect, bool> p = null)
        {
            if (p == null && !_requiresLineOfSight)
            {
                return (from c in _intersectingColliders select ColliderAspect.GetAspect(c)).ToArray();
            }
            else
            {
                return (from c in _intersectingColliders let a = ColliderAspect.GetAspect(c) where (p == null || p(a)) && (!_requiresLineOfSight || this.IsLineOfSight(c)) select a).ToArray();
            }
        }

        public override int SenseAll(ICollection<IAspect> lst, System.Func<IAspect, bool> p = null)
        {
            if (lst == null) throw new System.ArgumentNullException("lst");
            if (lst.IsReadOnly) throw new System.ArgumentException("List to fill can not be read-only.", "lst");
            if (_intersectingColliders.Count == 0) return 0;

            if(p == null && !_requiresLineOfSight)
            {
                var e = _intersectingColliders.GetEnumerator();
                while(e.MoveNext())
                {
                    lst.Add(ColliderAspect.GetAspect(e.Current));
                }
                return _intersectingColliders.Count;
            }
            else
            {
                var e = _intersectingColliders.GetEnumerator();
                int cnt = 0;
                while(e.MoveNext())
                {
                    var a = ColliderAspect.GetAspect(e.Current);
                    if((p == null || p(a)) && (!_requiresLineOfSight || this.IsLineOfSight(e.Current)))
                    {
                        lst.Add(a);
                        cnt++;
                    }
                }
                return cnt;
            }
        }

        public override int SenseAll<T>(ICollection<T> lst, System.Func<T, bool> p = null)
        {
            if (lst == null) throw new System.ArgumentNullException("lst");
            if (lst.IsReadOnly) throw new System.ArgumentException("List to fill can not be read-only.", "lst");
            if (_intersectingColliders.Count == 0) return 0;

            if (p == null && !_requiresLineOfSight)
            {
                int cnt = 0;
                var e = _intersectingColliders.GetEnumerator();
                while(e.MoveNext())
                {
                    var a = ColliderAspect.GetAspect(e.Current);
                    var o = ObjUtil.GetAsFromSource<T>(a);
                    if(o != null)
                    {
                        lst.Add(o);
                        cnt++;
                    }
                }
                return cnt;
            }
            else
            {
                var e = _intersectingColliders.GetEnumerator();
                int cnt = 0;
                while (e.MoveNext())
                {
                    var a = ColliderAspect.GetAspect(e.Current);
                    var o = ObjUtil.GetAsFromSource<T>(a);
                    if ((p == null || p(o)) && (!_requiresLineOfSight || this.IsLineOfSight(e.Current)))
                    {
                        lst.Add(e.Current as T);
                        cnt++;
                    }
                }
                return cnt;
            }
        }

        #endregion

    }
}
