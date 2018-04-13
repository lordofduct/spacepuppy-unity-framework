using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.Sensors.Audible
{

    public class AudibleSensor : SPComponent
    {
        
        #region Static Multiton Interface

        public static readonly MultitonPool<AudibleSensor> Pool = new MultitonPool<AudibleSensor>();

        #endregion

        #region Fields

        [SerializeField()]
        private Color _sensorColor = Color.blue;

        [SerializeField]
        private float _range;

        [SerializeField]
        [Tooltip("Should we signal the entire entity if one exists?")]
        private bool _signalEntity = true;

        [SerializeField()]
        private LayerMask _aspectLayerMask = -1;
        [SerializeField()]
        private TagMask _aspectTagMask;

        [SerializeField]
        private Trigger _onHeardSound;

        [System.NonSerialized]
        private SPEntity _entity;

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

    }

}
