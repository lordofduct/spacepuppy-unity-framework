using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    /// <summary>
    /// A TimeSupplier that allows for individual scaling. You may have to scale the world time to 0 for pause 
    /// or other various effects, but you still want time to tick normally for other aspects like the menu or 
    /// something. BUT, lets say you want to be able to scale that time as well for various effects, independent 
    /// of the world time. By using a custom TimeSupplier you can scale that time independent of the world time. 
    /// 
    /// Furthermore you can stack time scales, just like described in com.spacepuppy.SPTime.
    /// </summary>
    public class CustomTimeSupplier : IScalableTimeSupplier, System.IDisposable
    {

        private const long SECONDS_TO_TICKS = 10000000; //10 million, same as ticks, allows  ~554 554 530 millenia of accuracy
        private const double TICKS_TO_SECONDS = 1E-07;

        #region Fields

        private string _id;
        private double _startTime;
        private double _t;
        private double _ft;
        private double _dt;
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
            get { return _startTime; }
            set
            {
                _startTime = value;
            }
        }

        /// <summary>
        /// The total time passed since the CustomTime was created. Value is relative to the Update sequence.
        /// </summary>
        public double UpdateTotal { get { return _t + _startTime; } }

        /// <summary>
        /// The total time passed since the CustomTime was created. Value is relative to the FixedUpdate sequence;
        /// </summary>
        public double FixedTotal { get { return _ft + _startTime; } }

        /// <summary>
        /// The delta time since the call to standard update. This will always return the delta since last update, regardless of if you call it in update/fixedupdate.
        /// </summary>
        public double UpdateDelta { get { return _dt; } }

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
                _ft += Time.fixedDeltaTime * _scale;
            }
            else
            {
                _dt = Time.unscaledDeltaTime * _scale;
                _t += _dt;
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

        private double GetTimeScale()
        {
            if (_scales.Count == 0) return 1f;

            double result = 1f;
            var e = _scales.Values.GetEnumerator();
            while(e.MoveNext())
            {
                result *= e.Current;
            }
            return result;
        }

        public void Reset()
        {
            _startTime = 0.0;
            _t = 0.0;
            _ft = 0.0;
            _dt = 0.0;
        }

        public void Reset(double startTime)
        {
            _startTime = startTime;
            _t = 0.0;
            _ft = 0.0;
            _dt = 0.0;
        }

        #endregion

        #region ITimeSupplier Interface

        /// <summary>
        /// The total time passed since thie CustomTime was created. Value is dependent on the UpdateSequence being accessed from.
        /// </summary>
        public float Total
        {
            get
            {
                if(GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate)
                {
                    return (float)(_ft + _startTime);
                }
                else
                {
                    return (float)(_t + _startTime);
                }
            }
        }

        public double TotalPrecise
        {
            get
            {
                if (GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate)
                {
                    return (_ft + _startTime);
                }
                else
                {
                    return (_t + _startTime);
                }
            }
        }

        /// <summary>
        /// The delta time since the last call to update/fixedupdate, relative to in which update/fixedupdate you call.
        /// </summary>
        public float Delta { get { return (GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate) ? (float)(_scale) * Time.fixedDeltaTime : (float)_dt; } }

        public bool Paused
        {
            get { return _paused; }
            set { _paused = value; }
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
            _scale = this.GetTimeScale();
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
                _scale = this.GetTimeScale();
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
