using UnityEngine;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Dynamic.Accessors;
using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// Configure a target property/field of an object.
    /// </summary>
    [System.Serializable()]
    public class VariantMember
    {

        private enum MemberStatus
        {
            Fault = -1,
            Unknown = 0,
            Primitive = 1,
            Complex = 2
        }

        #region Fields

        [SerializeField()]
        [SelectableObject]
        private UnityEngine.Object _target;
        [SerializeField()]
        private string _memberName;

        [System.NonSerialized]
        private IMemberAccessor _accessor;
        [System.NonSerialized]
        private System.Type _memberType;
        [System.NonSerialized]
        private MemberStatus _status;

        #endregion

        #region Properties

        public UnityEngine.Object Target
        {
            get { return _target; }
            set
            {
                if (_target == value) return;
                _target = value;
                this.SetDirty();
            }
        }

        public string MemberName
        {
            get { return _memberName; }
            set
            {
                if (_memberName == value) return;
                _memberName = value;
                this.SetDirty();
            }
        }

        public System.Type MemberType
        {
            get
            {
                if (_status == MemberStatus.Unknown) this.InitAccessor();
                return _memberType;
            }
        }

        #endregion

        #region Methods

        public object GetValue()
        {
            switch(_status)
            {
                case MemberStatus.Unknown:
                    this.InitAccessor();
                    return _accessor != null ? _accessor.Get(_target) : null;
                case MemberStatus.Primitive:
                case MemberStatus.Complex:
                    return _accessor.Get(_target);
                default:
                    return null;
            }

        }

        public void SetValue(object value)
        {
            Restart:
            switch (_status)
            {
                case MemberStatus.Unknown:
                    this.InitAccessor();
                    goto Restart;
                case MemberStatus.Primitive:
                    _accessor.Set(_target, ConvertUtil.ToPrim(value, _memberType));
                    break;
                case MemberStatus.Complex:
                    _accessor.Set(_target, value);
                    break;
            }
        }

        public void SetDirty()
        {
            _accessor = null;
            _memberType = null;
            _status = MemberStatus.Unknown;
        }

        private void InitAccessor()
        {
            _accessor = MemberAccessorPool.GetDynamicAccessor(_target, _memberName, out _memberType);
            if(_accessor == null)
            {
                _status = MemberStatus.Fault;
            }
            else if(ConvertUtil.IsSupportedType(_memberType))
            {
                _status = MemberStatus.Primitive;
            }
            else
            {
                _status = MemberStatus.Complex;
            }
        }

        #endregion

    }

}
