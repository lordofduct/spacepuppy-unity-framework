using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.Sensors.Visual
{
    public abstract class VisualSensor : Sensor
    {

        #region Fields

        [SerializeField()]
        [FormerlySerializedAs("SensorColor")]
        private Color _sensorColor = Color.blue;
        [SerializeField()]
        [FormerlySerializedAs("CanDetectSelf")]
        private bool _canDetectSelf;
        [SerializeField()]
        [FormerlySerializedAs("AspectLayerMask")]
        private LayerMask _aspectLayerMask = -1;
        [SerializeField()]
        [FormerlySerializedAs("AspectTagMask")]
        private TagMask _aspectTagMask;

        [SerializeField()]
        [FormerlySerializedAs("RequiresLineOfSight")]
        private bool _requiresLineOfSight;
        [SerializeField()]
        [FormerlySerializedAs("LineOfSightMask")]
        private LayerMask _lineOfSightMask;

        #endregion

        #region Properties

        public Color SensorColor
        {
            get { return _sensorColor; }
            set { _sensorColor = value; }
        }
        public bool CanDetectSelf
        {
            get { return _canDetectSelf; }
            set { _canDetectSelf = value; }
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

        private System.Func<IAspect, bool> _cachedPred;
        private System.Func<IAspect, bool> GetPredicate(System.Func<IAspect, bool> p)
        {
            if(p == null)
            {
                if (_cachedPred == null) _cachedPred = this.Visible;
                return _cachedPred;
            }
            else
            {
                return (a) => this.Visible(a) && p(a);
            }
        }


        public override bool ConcernedWith(UnityEngine.Object obj)
        {
            if(obj is VisualAspect)
            {
                return this.ConcernedWith(obj as VisualAspect);
            }
            else
            {
                var go = GameObjectUtil.GetGameObjectFromSource(obj);
                if (go == null) return false;
                using (var set = com.spacepuppy.Collections.TempCollection.GetSet<VisualAspect>())
                {
                    go.FindComponents<VisualAspect>(set);
                    var e = set.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (this.ConcernedWith(e.Current))
                            return true;
                    }
                    return false;
                }
            }
        }
        private bool ConcernedWith(VisualAspect vaspect)
        {
            if (vaspect == null) return false;
            if (!vaspect.isActiveAndEnabled) return false;
            if (_aspectLayerMask != -1 && !vaspect.gameObject.IntersectsLayerMask(_aspectLayerMask)) return false;
            if (!_aspectTagMask.Intersects(vaspect)) return false;
            if (!_canDetectSelf && vaspect.entityRoot == this.entityRoot) return false;

            return true;
        }

        public override bool SenseAny(System.Func<IAspect, bool> p = null)
        {
            return VisualAspect.Any(this.GetPredicate(p));
        }

        public override IAspect Sense(System.Func<IAspect, bool> p = null)
        {
            p = this.GetPredicate(p);
            return VisualAspect.Pool.Find(p);
        }

        public override IEnumerable<IAspect> SenseAll(System.Func<IAspect, bool> p = null)
        {
            p = this.GetPredicate(p);
            return VisualAspect.Pool.FindAll(p);
        }

        public override int SenseAll(ICollection<IAspect> lst, System.Func<IAspect, bool> p = null)
        {
            p = this.GetPredicate(p);
            return VisualAspect.Pool.FindAll(lst, p);
        }

        public override int SenseAll<T>(ICollection<T> lst, System.Func<T, bool> p = null)
        {
            System.Func<T, bool> p2;
            if (p == null)
                p2 = (a) => this.Visible(a);
            else
                p2 = (a) => this.Visible(a) && p(a);

            return VisualAspect.Pool.FindAll<T>(lst, p2);
        }

        public override bool Visible(IAspect aspect)
        {
            var vaspect = aspect as VisualAspect;

            if (vaspect == null) return false;
            if (!vaspect.isActiveAndEnabled) return false;
            if (_aspectLayerMask != -1 && !aspect.gameObject.IntersectsLayerMask(_aspectLayerMask)) return false;
            if (!_aspectTagMask.Intersects(vaspect)) return false;
            if (!_canDetectSelf && vaspect.entityRoot == this.entityRoot) return false;
            return this.TestVisibility(vaspect);
        }

        protected abstract bool TestVisibility(VisualAspect aspect);

        #endregion
        
    }
}
