using System;
using System.Collections.Generic;

namespace com.spacepuppy
{

    /// <summary>
    /// These are temporary event args that avoid gc when calling an event that needs to include a single value with it, 
    /// while still conforming to the traditional Microsoft (sender,eventArgs) format.
    /// </summary>
    public sealed class TempEventArgs : EventArgs
    {

        #region Fields

        private object _value;

        #endregion

        #region CONSTRUCTOR

        private TempEventArgs(object value)
        {
            _value = value;
        }

        #endregion

        #region Properties

        public object Value
        {
            get { return _value; }
        }

        #endregion

        #region Multiton Reference

        private static int _cacheLimit = 100;
        public static int CacheLimit
        {
            get { return _cacheLimit; }
            set { _cacheLimit = value; }
        }

        private static Stack<TempEventArgs> _args = new Stack<TempEventArgs>();

        public static TempEventArgs Create(object value)
        {
            lock(_args)
            {
                if(_args.Count > 0)
                {
                    var e = _args.Pop();
                    e._value = value;
                    return e;
                }
            }

            return new TempEventArgs(value);
        }

        public static void Release(TempEventArgs e)
        {
            lock(_args)
            {
                if(_args.Count < _cacheLimit)
                {
                    e._value = null;
                    _args.Push(e);
                }
            }
        }

        #endregion

    }

}
