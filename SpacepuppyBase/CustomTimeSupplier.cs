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

        #region Fields

        private string _id;
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

        /// <summary>
        /// The total time passed since the CustomTime was created. Value is relative to the Update sequence.
        /// </summary>
        public float UpdateTotal { get { return (float)_t; } }

        /// <summary>
        /// The total time passed since the CustomTime was created. Value is relative to the FixedUpdate sequence;
        /// </summary>
        public float FixedTotal { get { return (float)_ft; } }

        /// <summary>
        /// The delta time since the call to standard update. This will always return the delta since last update, regardless of if you call it in update/fixedupdate.
        /// </summary>
        public float UpdateDelta { get { return (float)_dt; } }

        /// <summary>
        /// The delta time since the call to fixed update. This will always return the delta since last fixedupdate, regardless of if you call it in update/fixedupdate.
        /// </summary>
        public float FixedDelta { get { return Time.fixedDeltaTime * (float)_scale; } }

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
            return SPTime.RemoveCustomTime(this);
        }

        private double GetTimeScale()
        {
            if (_scales.Count == 0) return 1f;

            double result = 1f;
            foreach(var value in _scales.Values)
            {
                result *= value;
            }
            return result;
        }

        #endregion

        #region ITimeSupplier Interface

        /// <summary>
        /// The total time passed since thie CustomTime was created. Value is dependent on the UpdateSequence being accessed from.
        /// </summary>
        public float Total { get { return (GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate) ? (float)_ft : (float)_t; } }

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
            if(_scales.Remove(id))
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
