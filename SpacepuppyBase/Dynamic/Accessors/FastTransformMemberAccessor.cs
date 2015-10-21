using UnityEngine;
using System.Reflection;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Dynamic.Accessors
{
    internal class FastTransformMemberAccessor : IMemberAccessor
    {

        #region Fields

        private System.Action<Transform, object> _setter;
        private System.Func<Transform, object> _getter;
        private IMemberAccessor _alternate;

        #endregion

        #region CONSTRUCTOR

        private FastTransformMemberAccessor()
        {

        }

        #endregion

        #region Methods

        private void SetPosition(Transform targ, object value)
        {
            targ.position = ConvertUtil.ToVector3(value);
        }

        private object GetPosition(Transform targ)
        {
            return targ.position;
        }

        private void SetLocalPosition(Transform targ, object value)
        {
            targ.localPosition = ConvertUtil.ToVector3(value);
        }

        private object GetLocalPosition(Transform targ)
        {
            return targ.localPosition;
        }

        private void SetLocalScale(Transform targ, object value)
        {
            targ.localScale = ConvertUtil.ToVector3(value);
        }

        private object GetLocalScale(Transform targ)
        {
            return targ.localScale;
        }

        private void SetEulerAngles(Transform targ, object value)
        {
            targ.eulerAngles = ConvertUtil.ToVector3(value);
        }

        private object GetEulerAngles(Transform targ)
        {
            return targ.eulerAngles;
        }

        private void SetLocalEulerAngles(Transform targ, object value)
        {
            targ.localEulerAngles = ConvertUtil.ToVector3(value);
        }

        private object GetLocalEulerAngles(Transform targ)
        {
            return targ.localEulerAngles;
        }

        private void SetRotation(Transform targ, object value)
        {
            targ.rotation = ConvertUtil.ToQuaternion(value);
        }

        private object GetRotation(Transform targ)
        {
            return targ.rotation;
        }

        private void SetLocalRotation(Transform targ, object value)
        {
            targ.localRotation = ConvertUtil.ToQuaternion(value);
        }

        private object GetLocalRotation(Transform targ)
        {
            return targ.localRotation;
        }

        #endregion

        #region IMemberAccessor Interface

        public object Get(object target)
        {
            return (_alternate != null) ? _alternate.Get(target) : _getter(target as Transform);
        }

        public void Set(object target, object value)
        {
            if (_alternate != null)
                _alternate.Set(target, value);
            else
                _setter(target as Transform, value);
        }

        #endregion

        #region Factory

        public static FastTransformMemberAccessor Create(MemberInfo info, bool useBasicMemberAccessor = false)
        {
            if(info == null) throw new System.ArgumentNullException("info");
            if(info.MemberType != MemberTypes.Property && info.MemberType != MemberTypes.Field) throw new System.ArgumentException("MemberInfo must be of type FieldInfo or PropertyInfo", "info");

            FastTransformMemberAccessor obj = null;
            switch(info.Name)
            {
                case "position" :
                    obj = new FastTransformMemberAccessor();
                    obj._setter = obj.SetPosition;
                    obj._getter = obj.GetPosition;
                    break;
                case "localPosition":
                    obj = new FastTransformMemberAccessor();
                    obj._setter = obj.SetLocalPosition;
                    obj._getter = obj.GetLocalPosition;
                    break;
                case "localScale":
                    obj = new FastTransformMemberAccessor();
                    obj._setter = obj.SetLocalScale;
                    obj._getter = obj.GetLocalScale;
                    break;
                case "eulerAngles":
                    obj = new FastTransformMemberAccessor();
                    obj._setter = obj.SetEulerAngles;
                    obj._getter = obj.GetEulerAngles;
                    break;
                case "localEulerAngles":
                    obj = new FastTransformMemberAccessor();
                    obj._setter = obj.SetLocalEulerAngles;
                    obj._getter = obj.GetLocalEulerAngles;
                    break;
                case "rotation":
                    obj = new FastTransformMemberAccessor();
                    obj._setter = obj.SetRotation;
                    obj._getter = obj.GetRotation;
                    break;
                case "localRotation":
                    obj = new FastTransformMemberAccessor();
                    obj._setter = obj.SetLocalRotation;
                    obj._getter = obj.GetLocalRotation;
                    break;
                default:
                    obj = new FastTransformMemberAccessor();
                    if (useBasicMemberAccessor)
                    {
                        obj._alternate = new BasicMemberAccessor(info);
                    }
                    else
                    {
                        if (info.MemberType == MemberTypes.Field)
                            obj._alternate = new FieldAccessor(info as FieldInfo);
                        else
                            obj._alternate = new PropertyAccessor(info as PropertyInfo);
                    }
                    break;
            }

            return obj;
        }

        #endregion

    }
}
