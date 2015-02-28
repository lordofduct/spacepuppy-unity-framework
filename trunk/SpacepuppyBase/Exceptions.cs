using System;
using System.Collections.Generic;

namespace com.spacepuppy
{

    public class AbsentNotificationReceiverException : System.Exception
    {

        private Notification _n;

        public AbsentNotificationReceiverException(Notification n, string msg)
            : base()
        {
            _n = n;
        }

        public Notification Notification { get { return _n; } }

    }

    public class MissingReferenceOfTypeException : UnityEngine.MissingReferenceException
    {

        private Type _requiredType;

        public MissingReferenceOfTypeException()
            : base("Missing reference to object pertinenant to operation.")
        {
            _requiredType = null;
        }

        public MissingReferenceOfTypeException(Type tp)
            : base("Missing reference to object of type '" + tp.Name + "' which is pertinenant to operatoin.")
        {
            _requiredType = tp;
        }

        public MissingReferenceOfTypeException(string msg, Type tp)
            : base(msg)
        {
            _requiredType = tp;
        }

        public Type RequiredType { get { return _requiredType; } }

    }

    public class EntityMissingReferenceException : MissingReferenceOfTypeException
    {

        private UnityEngine.GameObject _root;

        public EntityMissingReferenceException(UnityEngine.GameObject root)
            : base("GameObject entity '" + root.name + "' is missing a reference to an object pertinenant to operation.", null)
        {
            _root = root;
        }

        public EntityMissingReferenceException(UnityEngine.GameObject root, Type tp)
            : base("GameObject entity '" + root.name + "' is missing a reference to an object of type '" + tp.Name + "' which is pertinenant to operation.", tp)
        {
            _root = root;
        }

        public EntityMissingReferenceException(UnityEngine.GameObject root, string msg, Type tp)
            : base(msg, tp)
        {
            _root = root;
        }

        public UnityEngine.GameObject EntityRoot { get { return _root; } }

    }

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
