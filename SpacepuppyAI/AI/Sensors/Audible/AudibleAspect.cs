using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.AI.Sensors.Audible
{

    public class AudibleAspect : SPComponent, IAspect, IUpdateable
    {

        #region Static Multiton Interface

        public static readonly MultitonPool<IAspect> Pool = new MultitonPool<IAspect>();

        #endregion

        #region Fields

        [SerializeField()]
        private float _precedence;

        [SerializeField]
        private float _range;

        [SerializeField()]
        private Color _aspectColor = Color.blue;

        [System.NonSerialized]
        private SirenToken _currentToken;
        [System.NonSerialized]
        private HashSet<AudibleSensor> _activeSensors;

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

            if (_currentToken != null) this.EndSiren();
            Pool.RemoveReference(this);
        }

        #endregion

        #region Properties

        public float Range
        {
            get { return _range; }
            set { _range = value; }
        }

        public bool SirenIsActive
        {
            get { return _currentToken != null; }
        }

        #endregion

        #region Methods

        public void SignalBlip()
        {
            if (_currentToken != null) return;

            var pos = this.transform.position;
            float d, r;

            var e = AudibleSensor.Pool.GetEnumerator();
            while (e.MoveNext())
            {
                var sensor = e.Current;
                d = (sensor.transform.position - pos).sqrMagnitude;
                r = (sensor.Range + _range);
                if (d < r * r)
                {
                    sensor.SignalBlip(this);
                }
            }
        }

        public SirenToken BeginSignalSiren()
        {
            if (_currentToken != null) return _currentToken;
            if (_activeSensors == null) _activeSensors = new HashSet<AudibleSensor>();

            _currentToken = new SirenToken(this);
            (this as IUpdateable).Update();
            GameLoopEntry.UpdatePump.Add(this);
            return _currentToken;
        }

        public void EndSiren()
        {
            GameLoopEntry.UpdatePump.Remove(this);
            var token = _currentToken;
            _currentToken = null;

            if(_activeSensors != null && _activeSensors.Count > 0)
            {
                using (var lst = TempCollection.GetList(_activeSensors))
                {
                    _activeSensors.Clear();
                    for(int i = 0; i < lst.Count; i++)
                    {
                        lst[i].SignalExitSiren(this);
                    }
                }
            }

            if (token != null && !token.IsComplete) token.SignalComplete();
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

        #region IUpdateable Interface

        void IUpdateable.Update()
        {
            if (_currentToken == null)
            {
                GameLoopEntry.UpdatePump.Remove(this);
                return;
            }

            TempHashSet<AudibleSensor> activeSet = null;
            if (_activeSensors.Count > 0) activeSet = TempCollection.GetSet(_activeSensors);

            var pos = this.transform.position;
            float d, r;

            var e = AudibleSensor.Pool.GetEnumerator();
            while(e.MoveNext())
            {
                var sensor = e.Current;
                if (_activeSensors.Contains(sensor) || sensor.Ignores(this)) continue;

                d = (sensor.transform.position - pos).sqrMagnitude;
                r = (sensor.Range + _range);
                if (d < r * r)
                {
                    _activeSensors.Add(sensor);
                    sensor.SignalEnterSiren(this);
                }
            }

            if(activeSet != null)
            {
                var e2 = activeSet.GetEnumerator();
                while(e2.MoveNext())
                {
                    var sensor = e.Current;

                    d = (sensor.transform.position - pos).sqrMagnitude;
                    r = (sensor.Range + _range);
                    if (d < r * r)
                    {
                        sensor.SignalSirenStay(this);
                    }
                    else
                    {
                        _activeSensors.Remove(sensor);
                        sensor.SignalExitSiren(this);
                    }
                }

                activeSet.Dispose();
            }
        }

        #endregion

    }

}
