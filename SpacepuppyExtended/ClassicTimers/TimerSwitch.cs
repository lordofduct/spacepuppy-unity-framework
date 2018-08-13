using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.ClassicTimers
{

    /// <summary>
    /// Turns on for a set amount of time before turning off again. Has programmable delay to turning on.
    /// </summary>
    public class TimerSwitch : Timer
    {

        #region Events

        public event System.EventHandler SwitchedOn;
        private void OnSwitchedOn(System.EventArgs e)
        {
            if (SwitchedOn != null) SwitchedOn(this, e);
        }

        public event System.EventHandler Complete;
        private void OnComplete(System.EventArgs e)
        {
            if (Complete != null) Complete(this, e);
        }

        #endregion

        #region Fields

        private float _t = float.NegativeInfinity;

        private float _delay;
        private float _dur;

        #endregion

        #region CONSTRUCTOR

        public TimerSwitch()
            : this(0, 0)
        {

        }

        public TimerSwitch(float dur)
            : this(dur, 0)
        {
            
        }

        public TimerSwitch(float dur, float delay)
        {
            _delay = delay;
            _dur = dur;
        }

        #endregion

        #region Properties

        public bool Allow
        {
            get { return (_t > 0 && _t < _dur); }
        }

        public bool AutoReset { get; set; }

        public float Duration {
            get { return _dur; }
            set { _dur = value; }
        }

        public float Delay {
            get { return _delay; }
            set { _delay = value; }
        }

        #endregion

        #region Methods

        public void Reset()
        {
            this.Stop();
            _t = float.NegativeInfinity;
        }

        public override void Start()
        {
            _t = -_delay;

            base.Start();
        }

        public void Start(float dur)
        {
            this.Start(dur, 0.0f);
        }

        public void Start(float dur, float delay)
        {
            _delay = delay;
            _dur = dur;

            this.Start();
        }

        protected override void UpdateTimer(float dt)
        {
            if (!this.IsRunning) return;

            if (_t < 0 && _t + dt > 0)
            {
                this.OnSwitchedOn(System.EventArgs.Empty);
            }

            _t += dt;

            if (_t > _dur)
            {
                this.Stop();
                this.OnComplete(System.EventArgs.Empty);

                if (this.AutoReset) this.Reset();
            }
        }

        #endregion

    }
}
