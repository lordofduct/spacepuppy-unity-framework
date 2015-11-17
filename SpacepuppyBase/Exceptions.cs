using System;

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

}
