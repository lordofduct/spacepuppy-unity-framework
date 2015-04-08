using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    public class CustomTimeSupplier : ITimeSupplier, System.IDisposable
    {

        #region Fields

        private string _id;
        private double _t;
        private double _ft;
        private double _dt;
        private double _scale;
        private bool _paused;

        #endregion

        #region CONSTRUCTOR

        internal CustomTimeSupplier(string id, float scale)
        {
            _id = id;
            _scale = (double)scale;
        }

        #endregion

        #region Properties

        public string Id { get { return _id; } }

        /// <summary>
        /// The total time passed since thie CustomTime was created. Value is dependent on the UpdateSequence being accessed from.
        /// </summary>
        public float Total { get { return (GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate) ? (float)_ft : (float)_t; } }

        /// <summary>
        /// The total time passed since the CustomTime was created. Value is relative to the Update sequence.
        /// </summary>
        public float UpdateTotal { get { return (float)_t; } }

        /// <summary>
        /// The total time passed since the CustomTime was created. Value is relative to the FixedUpdate sequence;
        /// </summary>
        public float FixedTotal { get { return (float)_ft; } }

        /// <summary>
        /// The delta time since the last call to update/fixedupdate, relative to in which update/fixedupdate you call.
        /// </summary>
        public float Delta { get { return (GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate) ? (float)(_scale) * Time.fixedDeltaTime : (float)_dt; } }

        /// <summary>
        /// The delta time since the call to standard update. This will always return the delta since last update, regardless of if you call it in update/fixedupdate.
        /// </summary>
        public float UpdateDelta { get { return (float)_dt; } }

        /// <summary>
        /// The delta time since the call to fixed update. This will always return the delta since last fixedupdate, regardless of if you call it in update/fixedupdate.
        /// </summary>
        public float FixedDelta { get { return Time.fixedDeltaTime * (float)_scale; } }

        public float Scale
        {
            get { return (float)_scale; }
            set { _scale = (double)_scale; }
        }

        public bool Paused
        {
            get { return _paused; }
            set { _paused = value; }
        }

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

        #endregion

        #region IDisposable Interface

        void System.IDisposable.Dispose()
        {
            this.Destroy();
        }

        #endregion

    }

}
