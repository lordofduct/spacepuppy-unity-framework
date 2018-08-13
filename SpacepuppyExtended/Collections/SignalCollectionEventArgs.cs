using System;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Collections
{
    public delegate void SignalCollectionEventHandler(object sender, SignalCollectionEventArgs e);

    public class SignalCollectionEventArgs : EventArgs
    {

        private object _object;

        public SignalCollectionEventArgs(object obj)
        {
            _object = obj;
        }

        public object Value { get { return _object; } }

    }

    public interface ISignalingCollection : System.Collections.ICollection
    {

        event EventHandler Clearing;
        event EventHandler Cleared;

        event SignalCollectionEventHandler Added;
        event SignalCollectionEventHandler Removed;

    }
}
