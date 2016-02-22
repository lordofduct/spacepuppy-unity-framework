using UnityEngine;
using System;
using System.Collections.Generic;

namespace com.spacepuppy
{

    /// <summary>
    /// A TimeSupplier that allows for individual scaling. You may have to scale the world time to 0 for pause 
    /// or other various effects, but you still want time to tick normally for other aspects like the menu or 
    /// something. BUT, lets say you want to be able to scale that time as well for various effects, independent 
    /// of the world time. By using a custom TimeSupplier you can scale that time independent of the world time. 
    /// 
    /// Furthermore you can stack time scales, just like described in com.spacepuppy.SPTime.
    /// 
    /// Allows for approximately 29,247 years of simulation, twice that if you include negative time, while 
    /// maintaining at minimum 3 digits of fractional precision for seconds (millisecond precision) when at the 
    /// extents of its range.
    /// </summary>
    public class CustomTimeSupplier : IScalableTimeSupplier, System.IDisposable
    {

        private const long SECONDS_TO_TICKS = 10000000L;
        private const double TICKS_TO_SECONDS = 1E-07d;

        private static long GetTicksSafe(double value)
        {
            if (double.IsNaN(value))
                return 0;

            value *= (double)SECONDS_TO_TICKS;
            if (value <= (double)long.MinValue)
                return long.MinValue;
            else if (value >= (double)long.MaxValue)
                return long.MaxValue;
            else
            {
                try
                {
                    return (long)value;
                }
                catch
                {
                    return 0;
                }
            }
        }

        #region Fields

        private string _id;
        private long _startTime;
        private long _t;
        private long _ft;
        private double _scale = 1.0;
        private bool _paused;
        private Dictionary<string, double> _scales = new Dictionary<string, double>();

        #endregion

        #region CONSTRUCTOR

        internal CustomTimeSupplier(string id)
        {
            _id = id;
        }

        #endregion

        #region Properties

        public string Id { get { return _id; } }

        public bool Valid { get { return _id != null; } }

        public double StartTime
        {
            get { return _startTime * TICKS_TO_SECONDS; }
            set
            {
                _startTime = GetTicksSafe(value);
            }
        }

        public TimeSpan TotalSpan
        {
            get
            {
                if (GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate)
                {
                    return new TimeSpan(_ft + _startTime);
                }
                else
                {
                    return new TimeSpan(_t + _startTime);
                }
            }
        }




        /// <summary>
        /// The total time passed since the CustomTime was created. Value is relative to the Update sequence.
        /// </summary>
        public double UpdateTotal { get { return (_t + _startTime) * TICKS_TO_SECONDS; } }

        public TimeSpan UpdateTotalSpan { get { return new TimeSpan(_t + _startTime); } }

        /// <summary>
        /// The delta time since the call to standard update. This will always return the delta since last update, regardless of if you call it in update/fixedupdate.
        /// </summary>
        public double UpdateDelta { get { return Time.unscaledDeltaTime * _scale; } }




        /// <summary>
        /// The total time passed since the CustomTime was created. Value is relative to the FixedUpdate sequence;
        /// </summary>
        public double FixedTotal { get { return (_ft + _startTime) * TICKS_TO_SECONDS; } }
        
        public TimeSpan FixedTotalSpan { get { return new TimeSpan(_ft + _startTime); } }

        /// <summary>
        /// The delta time since the call to fixed update. This will always return the delta since last fixedupdate, regardless of if you call it in update/fixedupdate.
        /// </summary>
        public double FixedDelta { get { return Time.fixedDeltaTime * _scale; } }

        #endregion

        #region Methods

        internal void Update(bool isFixed)
        {
            if (_paused) return;

            if (isFixed)
            {
                _ft += (long)(Time.fixedDeltaTime * _scale * SECONDS_TO_TICKS);
            }
            else
            {
                _t += (long)(Time.unscaledDeltaTime * _scale * SECONDS_TO_TICKS);
            }
        }

        public bool Destroy()
        {
            if( SPTime.RemoveCustomTime(this))
            {
                _id = null;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SyncTimeScale()
        {
            double result = 1d;
            if (_scales.Count > 0)
            {
                var e = _scales.GetEnumerator();
                while (e.MoveNext())
                {
                    result *= e.Current.Value;
                }
            }
            if(System.Math.Abs(result - _scale) > 0.0000001d)
            {
                _scale = result;
                if (this.TimeScaleChanged != null) this.TimeScaleChanged(this, System.EventArgs.Empty);
            }
            else
            {
                _scale = result;
            }
        }

        public void Reset()
        {
            _startTime = 0;
            _t = 0;
            _ft = 0;
        }

        public void Reset(double startTime)
        {
            _startTime = GetTicksSafe(startTime);
            _t = 0;
            _ft = 0;
        }


        /// <summary>
        /// Adjust the current 'TotalTime' by some amount. 
        /// WARNING - delta is not effected
        /// WARNING - time based event systems might be adversely impacted
        /// Especially if the value is negative.
        /// USE AT OWN RISK!
        /// </summary>
        /// <param name="value"></param>
        public void AdjustTime(double value)
        {
            _startTime += GetTicksSafe(value);
        }

        #endregion

        #region ITimeSupplier Interface

        public event System.EventHandler TimeScaleChanged;

        /// <summary>
        /// The total time passed since thie CustomTime was created. Value is dependent on the UpdateSequence being accessed from.
        /// </summary>
        public float Total
        {
            get
            {
                if(GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate)
                {
                    return (float)((_ft + _startTime) * TICKS_TO_SECONDS);
                }
                else
                {
                    return (float)((_t + _startTime) * TICKS_TO_SECONDS);
                }
            }
        }

        public double TotalPrecise
        {
            get
            {
                if (GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate)
                {
                    return (_ft + _startTime) * TICKS_TO_SECONDS;
                }
                else
                {
                    return (_t + _startTime) * TICKS_TO_SECONDS;
                }
            }
        }

        /// <summary>
        /// The delta time since the last call to update/fixedupdate, relative to in which update/fixedupdate you call.
        /// </summary>
        public float Delta
        {
            get
            {
                if (_paused)
                    return 0f;
                else
                    return (GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate) ? (float)(Time.fixedDeltaTime * _scale) : (float)(Time.unscaledDeltaTime * _scale);
            }
        }

        public bool Paused
        {
            get { return _paused; }
            set
            {
                if (_paused == value) return;
                _paused = value;
                if (this.TimeScaleChanged != null) this.TimeScaleChanged(this, System.EventArgs.Empty);
            }
        }

        public float Scale
        {
            get { return (float)_scale; }
        }

        public IEnumerable<string> ScaleIds
        {
            get { return _scales.Keys; }
        }

        public void SetScale(string id, float scale)
        {
            _scales[id] = (double)scale;
            this.SyncTimeScale();
        }

        public float GetScale(string id)
        {
            double result;
            if (_scales.TryGetValue(id, out result))
            {
                return (float)result;
            }
            else
            {
                return float.NaN;
            }
        }

        public bool RemoveScale(string id)
        {
            if (_scales.Remove(id))
            {
                this.SyncTimeScale();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasScale(string id)
        {
            return _scales.ContainsKey(id);
        }

        #endregion

        #region IDisposable Interface

        void System.IDisposable.Dispose()
        {
            this.Destroy();
        }

        #endregion

    }
    
}
