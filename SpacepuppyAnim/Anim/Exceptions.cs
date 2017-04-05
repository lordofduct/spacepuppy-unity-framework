using System;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Anim
{

    public class UnknownStateException : System.ArgumentException
    {

        #region Fields

        private string _stateName;

        #endregion

        #region CONSTRUCTOR

        public UnknownStateException(string stateName) :
            base("Unknown state of name '" + stateName + "'.")
        {
            _stateName = stateName;
        }

        #endregion

        #region Properties

        public string StateName { get { return _stateName; } }

        #endregion

    }

    public class AnimationInvalidAccessException : System.InvalidOperationException
    {

        public AnimationInvalidAccessException()
            : base("Can not access SPAnimationController until it has been initialized.")
        {

        }

    }

}
