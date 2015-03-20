using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.spacepuppy.Utils;
using com.spacepuppy.Utils.FastDynamicMemberAccessor;

namespace com.spacepuppy.Tween.Curves
{

    /// <summary>
    /// Base class for curves that reflectively access members of a target object.
    /// If the member curve is a CustomMemberCurve that should be buildable by the MemberCurve.Create factory method, 
    /// it MUST contain a 0 parameter constructor.
    /// </summary>
    public abstract class MemberCurve : ICurve
    {

        #region Fields

        private Ease _ease;
        private float _dur;
        private float _delay;

        private IMemberAccessor _accessor;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// This MUST exist for reflective creation.
        /// </summary>
        public MemberCurve()
        {

        }

        public MemberCurve(float dur)
        {
            _ease = EaseMethods.LinearEaseNone;
            _dur = dur;
            _delay = 0f;
        }

        public MemberCurve(Ease ease, float dur)
        {
            _ease = ease;
            _dur = dur;
            _delay = 0f;
        }

        protected abstract void Init(object start, object end, bool slerp);

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

        public void Update(object targ, float dt, float t)
        {
            if (t < _delay) return;

            var value = GetValue(_ease(t - _delay, 0f, 1f, _dur));
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

        public static MemberCurve Create(object target, string propName, Ease ease, float dur, object start, object end, bool slerp = false)
        {
            if (target == null) throw new System.ArgumentNullException("target");
            return Create(target.GetType(), propName, ease, dur, start, end, slerp);
        }

        public static MemberCurve Create(System.Type targType, string propName, Ease ease, float dur, object start, object end, bool slerp = false)
        {
            MemberInfo memberInfo;
            System.Type memberType;
            MemberAccessorPool.GetMember(targType, propName, out memberInfo, out memberType);

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
                    curve.Init(start, end, slerp);
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

        public static MemberCurve Create(MemberInfo info, Ease ease, float dur, object start, object end, bool slerp = false)
        {
            System.Type memberType;
            if (info is FieldInfo)
                memberType = (info as FieldInfo).FieldType;
            else if (info is PropertyInfo)
                memberType = (info as PropertyInfo).PropertyType;
            else
                throw new System.ArgumentException("MemberInfo must be either a Property or Field.", "info");

            if (_memberTypeToCurveType == null) BuildDictionary();

            if(_memberTypeToCurveType.ContainsKey(memberType))
            {
                try
                {
                    var curve = System.Activator.CreateInstance(_memberTypeToCurveType[memberType], true) as MemberCurve;
                    curve._dur = dur;
                    curve.Ease = ease;
                    curve._accessor = MemberAccessorPool.Get(info);
                    if (curve is NumericMemberCurve && ConvertUtil.IsNumericType(memberType)) (curve as NumericMemberCurve).NumericType = System.Type.GetTypeCode(memberType);
                    curve.Init(start, end, slerp);
                    return curve;
                }
                catch(System.Exception ex)
                {
                    throw new System.InvalidOperationException("Failed to create a MemberCurve for the desired MemberInfo.", ex);
                }
            }
            else
            {
                throw new System.ArgumentException("MemberInfo is for a member type that is not supported.", "info");
            }
        }
        
        internal static void Init(MemberCurve curve, System.Type objType, string propName)
        {
            MemberInfo info;
            System.Type memberType = null;
            MemberAccessorPool.GetMember(objType, propName, out info, out memberType);
            curve._accessor = MemberAccessorPool.Get(info);
            if (memberType != null && curve is NumericMemberCurve && ConvertUtil.IsNumericType(memberType)) (curve as NumericMemberCurve).NumericType = System.Type.GetTypeCode(memberType);
        }

        internal static void Init(MemberCurve curve, MemberInfo info)
        {
            System.Type memberType;
            if (info is FieldInfo)
                memberType = (info as FieldInfo).FieldType;
            else if (info is PropertyInfo)
                memberType = (info as PropertyInfo).PropertyType;
            else
                throw new System.ArgumentException("MemberInfo must be either a Property or Field.", "info");

            curve._accessor = MemberAccessorPool.Get(info);
            if (curve is NumericMemberCurve && ConvertUtil.IsNumericType(memberType)) (curve as NumericMemberCurve).NumericType = System.Type.GetTypeCode(memberType);
        }

        #endregion

    }

}
