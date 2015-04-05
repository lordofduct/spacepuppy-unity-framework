using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.spacepuppy.Tween.Curves;
using com.spacepuppy.Utils;
using com.spacepuppy.Utils.FastDynamicMemberAccessor;

namespace com.spacepuppy.Tween
{

    /// <summary>
    /// Base class for curves that reflectively access members of a target object.
    /// If the member curve is a CustomMemberCurve that should be buildable by the MemberCurve.Create factory method, 
    /// it MUST contain a 0 parameter constructor.
    /// </summary>
    public abstract class MemberCurve : Curve
    {

        #region Fields

        private Ease _ease;
        private float _dur;
        private float _delay;

        private string _memberName;
        private IMemberAccessor _accessor;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// This MUST exist for reflective creation.
        /// </summary>
        protected MemberCurve()
        {
        }

        public MemberCurve(string propName, float dur)
        {
            _memberName = propName;
            _ease = EaseMethods.LinearEaseNone;
            _dur = dur;
            _delay = 0f;
        }

        public MemberCurve(string propName, Ease ease, float dur)
        {
            _memberName = propName;
            _ease = ease;
            _dur = dur;
            _delay = 0f;
        }

        /// <summary>
        /// Override this method to handle the reflective creation of this object.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="slerp"></param>
        protected abstract void ReflectiveInit(object start, object end, object option);

        #endregion

        #region Properties

        public Ease Ease
        {
            get { return _ease; }
            set
            {
                if (value == null) throw new System.ArgumentNullException("value");
                _ease = value;
            }
        }

        public float Duration
        {
            get { return _dur; }
            protected set { _dur = value; }
        }

        public float Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the appropriate value of the member on the curve at t, where t is a scalar (t would be 1.0, not 100, for 100%).
        /// </summary>
        /// <param name="t">The percentage of completion across the curve that the member is at.</param>
        /// <returns></returns>
        protected abstract object GetValue(float t);

        #endregion

        #region ICurve Interface

        public float TotalDuration
        {
            get { return _delay + _dur; }
        }

        public override float TotalTime
        {
            get { return _delay + _dur; }
        }

        protected internal override void Update(object targ, float dt, float t)
        {
            if (t < _delay) return;

            var value = GetValue(_ease(t - _delay, 0f, 1f, _dur));
            if (_accessor == null) _accessor = MemberAccessorPool.Get(targ, _memberName);
            _accessor.Set(targ, value);
        }

        #endregion

        #region Static Factory

        private static Dictionary<System.Type, System.Type> _memberTypeToCurveType;

        private static void BuildDictionary()
        {
            _memberTypeToCurveType = new Dictionary<System.Type, System.Type>();

            var priorities = new Dictionary<System.Type, int>();
            foreach(var tp in TypeUtil.GetTypesAssignableFrom(typeof(MemberCurve)))
            {
                var attribs = tp.GetCustomAttributes(typeof(CustomMemberCurveAttribute), false).Cast<CustomMemberCurveAttribute>().ToArray();
                foreach(var attrib in attribs)
                {
                    if (!priorities.ContainsKey(attrib.HandledMemberType) || priorities[attrib.HandledMemberType] > attrib.priority)
                    {
                        priorities[attrib.HandledMemberType] = attrib.priority;
                        _memberTypeToCurveType[attrib.HandledMemberType] = tp;
                    }
                }
            }
        }

        private static MemberCurve Create(MemberInfo memberInfo, System.Type memberType, Ease ease, float dur, object start, object end, object option)
        {
            if (_memberTypeToCurveType == null) BuildDictionary();

            if (_memberTypeToCurveType.ContainsKey(memberType))
            {
                try
                {
                    var curve = System.Activator.CreateInstance(_memberTypeToCurveType[memberType], true) as MemberCurve;
                    curve._dur = dur;
                    curve.Ease = ease;
                    curve._accessor = MemberAccessorPool.Get(memberInfo);
                    if (curve is NumericMemberCurve && ConvertUtil.IsNumericType(memberType)) (curve as NumericMemberCurve).NumericType = System.Type.GetTypeCode(memberType);
                    curve.ReflectiveInit(start, end, option);
                    return curve;
                }
                catch (System.Exception ex)
                {
                    throw new System.InvalidOperationException("Failed to create a MemberCurve for the desired MemberInfo.", ex);
                }
            }
            else
            {
                throw new System.ArgumentException("MemberInfo is for a member type that is not supported.", "info");
            }
        }

        public static Curve CreateTo(object target, string propName, Ease ease, object end, float dur, object option = null)
        {
            if (target == null) throw new System.ArgumentNullException("target");
            MemberInfo memberInfo;
            System.Type memberType;
            MemberAccessorPool.GetMember(target.GetType(), propName, out memberInfo, out memberType);

            object start = MemberAccessorPool.Get(memberInfo).Get(target);

            return MemberCurve.Create(memberInfo, memberType, ease, dur, start, end, option);
        }

        public static Curve CreateFrom(object target, string propName, Ease ease, object start, float dur, object option = null)
        {
            if (target == null) throw new System.ArgumentNullException("target");
            MemberInfo memberInfo;
            System.Type memberType;
            MemberAccessorPool.GetMember(target.GetType(), propName, out memberInfo, out memberType);

            object end = MemberAccessorPool.Get(memberInfo).Get(target);

            return MemberCurve.Create(memberInfo, memberType, ease, dur, start, end, option);
        }

        public static Curve CreateBy(object target, string propName, Ease ease, object amt, float dur, object option = null)
        {
            if (target == null) throw new System.ArgumentNullException("target");
            MemberInfo memberInfo;
            System.Type memberType;
            MemberAccessorPool.GetMember(target.GetType(), propName, out memberInfo, out memberType);

            object start = MemberAccessorPool.Get(memberInfo).Get(target);
            object end = MemberCurve.TrySum(memberInfo, start, amt);

            return MemberCurve.Create(memberInfo, memberType, ease, dur, start, end, option);
        }

        public static Curve CreateFromTo(object target, string propName, Ease ease, object start, object end, float dur, object option = null)
        {
            if (target == null) throw new System.ArgumentNullException("target");
            MemberInfo memberInfo;
            System.Type memberType;
            MemberAccessorPool.GetMember(target.GetType(), propName, out memberInfo, out memberType);

            return MemberCurve.Create(memberInfo, memberType, ease, dur, start, end, option);
        }


        private static object TrySum(System.Reflection.MemberInfo info, object a, object b)
        {
            System.Type tp;
            if (info is System.Reflection.FieldInfo)
                tp = (info as System.Reflection.FieldInfo).FieldType;
            else if (info is System.Reflection.PropertyInfo)
                tp = (info as System.Reflection.PropertyInfo).PropertyType;
            else
                return b;

            if (ConvertUtil.IsNumericType(tp))
            {
                return ConvertUtil.ToPrim(ConvertUtil.ToDouble(a) + ConvertUtil.ToDouble(b), tp);
            }
            else if (tp == typeof(Vector2))
            {
                return ConvertUtil.ToVector2(a) + ConvertUtil.ToVector2(b);
            }
            else if (tp == typeof(Vector3))
            {
                return ConvertUtil.ToVector3(a) + ConvertUtil.ToVector3(b);
            }
            else if (tp == typeof(Vector4))
            {
                return ConvertUtil.ToVector4(a) + ConvertUtil.ToVector4(b);
            }
            else if (tp == typeof(Quaternion))
            {
                return ConvertUtil.ToQuaternion(a) * ConvertUtil.ToQuaternion(b);
            }
            else if (tp == typeof(Color))
            {
                return ConvertUtil.ToColor(a) + ConvertUtil.ToColor(b);
            }
            else if (tp == typeof(Color32))
            {
                return ConvertUtil.ToColor32(ConvertUtil.ToColor(a) + ConvertUtil.ToColor(b));
            }

            return b;
        }

        #endregion

    }

}
