using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.AI.Sensors;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.Scenario
{

    [Infobox("Setting 'UseProximityTrigger' false will make this tick constantly, multiple t_OnSense configured this way can be very expensive.", MessageType =InfoBoxMessageType.Warning)]
    public class t_OnSense : TriggerComponent
    {

        #region Fields

        [SerializeField]
        [DefaultFromSelf]
        private Sensor _sensor;
        [SerializeField]
        private float _interval = 1f;
        [SerializeField]
        private bool _useProximityTrigger = true;


        [System.NonSerialized]
        private RadicalCoroutine _routine;
        [System.NonSerialized]
        private HashSet<Collider> _nearColliders;

        #endregion

        #region CONSTRUCTOR

        protected override void Start()
        {
            base.Start();

            this.TryStartRoutine();
        }

        #endregion

        #region Properties

        public Sensor Sensor
        {
            get { return _sensor; }
            set { _sensor = value; }
        }

        public float Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }

        public bool UseProximityTrigger
        {
            get { return _useProximityTrigger; }
            set { _useProximityTrigger = value; }
        }

        #endregion

        #region Methods

        protected void OnTriggerEnter(Collider other)
        {
            if (!_useProximityTrigger) return;
            if (_sensor == null) return;
            if (!_sensor.ConcernedWith(other.gameObject)) return;

            if (_nearColliders == null) _nearColliders = new HashSet<Collider>();
            _nearColliders.Add(other);

            this.TryStartRoutine();
        }

        protected void OnTriggerExit(Collider other)
        {
            if (!_useProximityTrigger || _nearColliders == null) return;
            if (_sensor == null) return;

            _nearColliders.Remove(other);

            if (_nearColliders.Count == 0 && _routine.Active)
            {
                _routine.Stop();
            }
        }




        private void TryStartRoutine()
        {
            if (!this.isActiveAndEnabled) return;

            if (!_useProximityTrigger || (_nearColliders != null && _nearColliders.Count > 0))
            {
                if (_routine == null || _routine.Finished)
                {
                    _routine = this.StartRadicalCoroutine(this.SenseRoutine(), RadicalCoroutineDisableMode.Pauses);
                }
                else if (!_routine.Active)
                {
                    _routine.Start(this, RadicalCoroutineDisableMode.Pauses);
                }
            }
        }

        private System.Collections.IEnumerator SenseRoutine()
        {
            while (true)
            {
                if (_sensor == null) yield break;

                if (_sensor.SenseAny())
                {
                    this.ActivateTrigger();
                }

                yield return WaitForDuration.Seconds(_interval);
            }
        }

        #endregion

    }
}
