using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy
{

    public class TypeArgumentMismatchException : System.ArgumentException
    {

        #region Fields

        private System.Type _type;
        private System.Type _mismatchType;

        #endregion

        #region CONSTRUCTOR

        public TypeArgumentMismatchException(System.Type tp, System.Type mismatchType)
            : this(tp, mismatchType, "Type argument did not match the desired type.", null)
        {

        }

        public TypeArgumentMismatchException(System.Type tp, System.Type mismatchType, string paramName)
            : this(tp, mismatchType, "Type argument did not match the desired type.", paramName)
        {

        }

        public TypeArgumentMismatchException(System.Type tp, System.Type mismatchType, string msg, string paramName)
            : this(tp, mismatchType, msg, paramName, null)
        {
        }

        public TypeArgumentMismatchException(System.Type tp, System.Type mismatchType, string msg, string paramName, System.Exception innerException)
            : base(msg, paramName, innerException)
        {
            _type = tp;
            _mismatchType = mismatchType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The type supplied as an argument.
        /// </summary>
        public System.Type Type { get { return _type; } }

        /// <summary>
        /// The type Type aught to be.
        /// </summary>
        public System.Type MismatchType { get { return _mismatchType; } }

        #endregion

    }

}
