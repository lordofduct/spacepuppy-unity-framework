using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.Sensors
{

    public abstract class Sensor : SPEntityComponent
    {

        #region Fields
        
        [System.NonSerialized()]
        private CompositeSensor _compositeSensorParent;
        
        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();
            
            this.SyncCompositeSensorParent();
        }

        protected override void Start()
        {
            base.Start();

            if (_compositeSensorParent != null && !_compositeSensorParent.Contains(this)) _compositeSensorParent.SyncChildSensors();
        }

        private void SyncCompositeSensorParent()
        {
            if (this is CompositeSensor)
            {
                _compositeSensorParent = null;
                return;
            }

            _compositeSensorParent = this.GetComponent<CompositeSensor>();
            if (_compositeSensorParent == null) _compositeSensorParent = this.GetComponentInParent<CompositeSensor>();
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        /// <summary>
        /// Since Sensors may filter out objects based on masks, this returns true if the sensor would even bother attempting to see a GameObject.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract bool ConcernedWith(UnityEngine.Object obj);

        public abstract bool SenseAny(System.Func<IAspect, bool> p = null);

        public abstract bool Visible(IAspect aspect);

        public abstract IAspect Sense(System.Func<IAspect, bool> p = null);

        public abstract IEnumerable<IAspect> SenseAll(System.Func<IAspect, bool> p = null);

        public abstract int SenseAll(ICollection<IAspect> lst, System.Func<IAspect, bool> p = null);

        public abstract int SenseAll<T>(ICollection<T> lst, System.Func<T, bool> p = null) where T : class, IAspect;

        #endregion
        
    }

    public abstract class ActiveSensor : Sensor
    {

        /// <summary>
        /// Occurs if the sensor actively picks something up.
        /// </summary>
        public event System.EventHandler<TempEventArgs> SensedAspect;
        /// <summary>
        /// Occurs when a long term sensor event begins (i.e. a collider entered the sensor and is staying)
        /// </summary>
        public event System.EventHandler SensorAlert;
        /// <summary>
        /// Occurs when a long term sensor event exits (i.e. all colliders have exited the sensor)
        /// </summary>
        public event System.EventHandler SensorSleep;

        protected bool HasSensedAspectListeners
        {
            get { return SensedAspect != null; }
        }

        protected bool HasSensorAlertListeners
        {
            get { return SensorSleep != null; }
        }

        protected bool HasSensorSleepListeners
        {
            get { return SensorSleep != null; }
        }

        protected void OnSensedAspect(IAspect aspect)
        {
            var d = this.SensedAspect;
            if(d != null)
            {
                var ev = TempEventArgs.Create(aspect);
                d(this, ev);
                TempEventArgs.Release(ev);
            }
        }

        protected void OnSensorAlert()
        {
            var d = this.SensorAlert;
            if (d != null)
            {
                d(this, System.EventArgs.Empty);
            }
        }

        protected void OnSensorSleep()
        {
            var d = this.SensorSleep;
            if (d != null)
            {
                d(this, System.EventArgs.Empty);
            }
        }

    }

}
