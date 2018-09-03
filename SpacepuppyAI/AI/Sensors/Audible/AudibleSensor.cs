#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.AI.Sensors.Audible
{

    public class AudibleSensor : ActiveSensor
    {
        
        #region Static Multiton Interface

        public static readonly MultitonPool<AudibleSensor> Pool = new MultitonPool<AudibleSensor>();

        #endregion

        #region Fields

        [SerializeField()]
        private Color _sensorColor = Color.red;

        [SerializeField]
        private float _range;

        [SerializeField]
        [Tooltip("Should we signal the entire entity if one exists?")]
        private bool _signalEntity = true;
        [SerializeField()]
        private bool _canDetectSelf;

        [SerializeField()]
        private LayerMask _aspectLayerMask = -1;
        [SerializeField()]
        private TagMask _aspectTagMask;

        [SerializeField]
        private Trigger _onHeardSound;

        [System.NonSerialized]
        private SPEntity _entity;

        [System.NonSerialized]
        private HashSet<AudibleAspect> _activeSirens;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();
            
            _entity = SPEntity.Pool.GetFromSource(this);
        }

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

        #region Properties

        public Color SensorColor
        {
            get { return _sensorColor; }
            set { _sensorColor = value; }
        }

        public float Range
        {
            get { return _range; }
            set { _range = value; }
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

        public Trigger OnHeardSound
        {
            get { return _onHeardSound; }
        }

        #endregion

        #region Methods

        public bool Ignores(IAspect aspect)
        {
            if (aspect == null) return true;
            if (_aspectLayerMask != -1 && !aspect.gameObject.IntersectsLayerMask(_aspectLayerMask)) return true;
            if (!_aspectTagMask.Intersects(aspect.gameObject)) return true;
            return false;
        }

        public void SignalBlip(IAspect aspect)
        {
            var audible = aspect as AudibleAspect;
            if (object.ReferenceEquals(audible, null) || !this.ConcernedWith(audible)) return;

            this.OnSensedAspect(aspect);
            if (_onHeardSound.Count > 0) _onHeardSound.ActivateTrigger(this, aspect);

            if(_signalEntity && !object.ReferenceEquals(_entity, null))
            {
                Messaging.Broadcast<IAudibleResponder>(_entity.gameObject, (o) => o.OnSound(aspect, false));
            }
            else
            {
                Messaging.Execute<IAudibleResponder>(this.gameObject, (o) => o.OnSound(aspect, false));
            }
        }

        public void SignalEnterSiren(IAspect aspect)
        {
            var audible = aspect as AudibleAspect;
            if (object.ReferenceEquals(audible, null) || !this.ConcernedWith(audible)) return;

            if (_activeSirens == null) _activeSirens = new HashSet<AudibleAspect>();
            bool none = _activeSirens.Count == 0;
            if (!_activeSirens.Add(audible)) return;
            
            this.OnSensedAspect(aspect);
            if (none) this.OnSensorAlert();
            if (_onHeardSound.Count > 0) _onHeardSound.ActivateTrigger(this, aspect);

            if (_signalEntity && !object.ReferenceEquals(_entity, null))
            {
                Messaging.Broadcast<IAudibleResponder>(_entity.gameObject, (o) => o.OnSound(aspect, true));
            }
            else
            {
                Messaging.Execute<IAudibleResponder>(this.gameObject, (o) => o.OnSound(aspect, true));
            }
        }

        public void SignalSirenStay(IAspect aspect)
        {
            var audible = aspect as AudibleAspect;
            if (object.ReferenceEquals(audible, null) || _activeSirens == null || !_activeSirens.Contains(audible)) return;

            if (_signalEntity && !object.ReferenceEquals(_entity, null))
            {
                Messaging.Broadcast<IAudibleSirenResponder>(_entity.gameObject, (o) => o.OnSoundStay(aspect));
            }
            else
            {
                Messaging.Execute<IAudibleSirenResponder>(this.gameObject, (o) => o.OnSoundStay(aspect));
            }
        }

        public void SignalExitSiren(IAspect aspect)
        {
            var audible = aspect as AudibleAspect;
            if (object.ReferenceEquals(audible, null) || _activeSirens == null || !_activeSirens.Contains(audible)) return;
            
            _activeSirens.Remove(audible);

            if(_activeSirens.Count == 0)
            {
                this.OnSensorSleep();
            }

            if (_signalEntity && !object.ReferenceEquals(_entity, null))
            {
                Messaging.Broadcast<IAudibleSirenResponder>(_entity.gameObject, (o) => o.OnSoundExit(aspect));
            }
            else
            {
                Messaging.Execute<IAudibleSirenResponder>(this.gameObject, (o) => o.OnSoundExit(aspect));
            }
        }

        #endregion

        #region Sensor Interface

        public override bool ConcernedWith(UnityEngine.Object obj)
        {
            if (obj is AudibleAspect)
            {
                return this.ConcernedWith(obj as AudibleAspect);
            }
            else
            {
                var go = GameObjectUtil.GetGameObjectFromSource(obj);
                if (go == null) return false;
                using (var set = com.spacepuppy.Collections.TempCollection.GetSet<AudibleAspect>())
                {
                    go.FindComponents<AudibleAspect>(set);
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
        private bool ConcernedWith(AudibleAspect aspect)
        {
            if (aspect == null) return false;
            if (!aspect.isActiveAndEnabled) return false;
            if (_aspectLayerMask != -1 && !aspect.gameObject.IntersectsLayerMask(_aspectLayerMask)) return false;
            if (!_aspectTagMask.Intersects(aspect)) return false;
            if (!_canDetectSelf && aspect.entityRoot == this.entityRoot) return false;

            return true;
        }

        public override bool SenseAny(Func<IAspect, bool> p = null)
        {
            if (_activeSirens == null || _activeSirens.Count == 0) return false;

            if(p != null)
            {
                var e = _activeSirens.GetEnumerator();
                while(e.MoveNext())
                {
                    if (p(e.Current)) return true;
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool Visible(IAspect aspect)
        {
            return _activeSirens != null && aspect is AudibleAspect && _activeSirens.Contains(aspect as AudibleAspect);
        }

        public override IAspect Sense(Func<IAspect, bool> p = null)
        {
            if (_activeSirens == null || _activeSirens.Count == 0) return null;

            var e = _activeSirens.GetEnumerator();
            while (e.MoveNext())
            {
                if (p == null || p(e.Current)) return e.Current;
            }
            return null;
        }

        public override IEnumerable<IAspect> SenseAll(Func<IAspect, bool> p = null)
        {
            if (_activeSirens == null || _activeSirens.Count == 0) yield break;

            var e = _activeSirens.GetEnumerator();
            while (e.MoveNext())
            {
                if (p == null || p(e.Current)) yield return e.Current;
            }
        }

        public override int SenseAll(ICollection<IAspect> lst, Func<IAspect, bool> p = null)
        {
            if (_activeSirens == null || _activeSirens.Count == 0) return 0;

            var e = _activeSirens.GetEnumerator();
            int cnt = 0;
            while (e.MoveNext())
            {
                if (p == null || p(e.Current))
                {
                    cnt++;
                    lst.Add(e.Current);
                }
            }
            return cnt;
        }

        public override int SenseAll<T>(ICollection<T> lst, Func<T, bool> p = null)
        {
            if (_activeSirens == null || _activeSirens.Count == 0) return 0;

            var e = _activeSirens.GetEnumerator();
            int cnt = 0;
            while (e.MoveNext())
            {
                if (e.Current is T && (p == null || p(e.Current as T)))
                {
                    cnt++;
                    lst.Add(e.Current as T);
                }
            }
            return cnt;
        }

        #endregion

    }

}
