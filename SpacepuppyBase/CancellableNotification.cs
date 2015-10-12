using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    /// <summary>
    /// Represents a Notification that can be cancelled.
    /// </summary>
    public abstract class CancellableNotification : Notification
    {

        #region Fields

        private bool _cancelled;

        #endregion

        #region CONSTRUCTOR

        public CancellableNotification()
        {

        }

        public CancellableNotification(bool cancelFlagDefault)
        {
            _cancelled = cancelFlagDefault;
        }

        #endregion

        #region Properties

        public bool Cancelled { get { return _cancelled; } set { _cancelled = value; } }

        #endregion

        #region Methods

        public void Reset()
        {
            _cancelled = false;
        }

        #endregion

    }
}
