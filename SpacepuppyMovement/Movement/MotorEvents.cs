using UnityEngine;

namespace com.spacepuppy.Movement
{

    public delegate void StyleChangedEventHandler(object sender, StyleChangedEventArgs e);

    public class StyleChangedEventArgs : System.EventArgs
    {

        #region Fields

        private IMovementStyle _oldStyle;
        private IMovementStyle _newStyle;
        private bool _currentStateIsStacking;

        #endregion

        #region CONSTRUCTOR

        public StyleChangedEventArgs(IMovementStyle oldStyle, IMovementStyle newStyle, bool currentStateIsStacking)
        {
            _oldStyle = oldStyle;
            _newStyle = newStyle;
            _currentStateIsStacking = currentStateIsStacking;
        }

        #endregion

        #region Properties

        public IMovementStyle LastMovementStyle { get { return _oldStyle; } }
        public IMovementStyle CurrentMovementStyle { get { return _newStyle; } }
        public bool CurrentStyleIsStacking { get { return _currentStateIsStacking; } }

        #endregion

    }

}
