namespace com.spacepuppy
{

    /// <summary>
    /// Represents a notification that should act as a signal for some other code to perform an action. For instance if a weapon strikes an enemy, it may want to signal 
    /// that a knockback should occur. The enemy then has a component that listens for this signal, and performs the knockback if it so desires. The Signal has a 'Handled' 
    /// property so that the receiver can denote if the action has been taken care of. This way if more than one component serves to handle the signal, they can test this 
    /// and make sure they don't compound the action.
    /// </summary>
    public abstract class Signal : Notification
    {

        #region Fields

        private bool _handled;

        #endregion

        #region CONSTRUCTOR

        public Signal()
        {

        }

        #endregion

        #region Properties

        public bool Handled
        {
            get { return _handled; }
            set { _handled = value; }
        }

        #endregion

    }
}
