using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.spacepuppy.Collections;
using com.spacepuppy.Tween.Curves;
using com.spacepuppy.Utils;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Dynamic.Accessors;

namespace com.spacepuppy.Tween
{

    /// <summary>
    /// Base class for curves that reflectively access members of a target object.
    /// If the member curve is a CustomMemberCurve that should be buildable by the MemberCurve.Create factory method, 
    /// it MUST contain a 0 parameter constructor.
    /// </summary>
    public abstract class MemberCurve : TweenCurve
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
        /// <param name="option"></param>
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
        protected abstract object GetValueAt(float dt, float t);

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

        protected internal sealed override void Update(object targ, float dt, float t)
        {
            if (t < _delay) return;

            var value = GetValueAt(dt, t - _delay);
            if (_accessor == null)
            {
                System.Type memberType;
                _accessor = MemberCurve.GetAccessor(targ, _memberName, out memberType);
            }
            _accessor.Set(targ, value);
        }

        #endregion

        #region Static Factory

        private class CustomAccessorData
        {
            public int priority;
            public System.Type TargetType;
            public System.Type AccessorType;
        }

        private static Dictionary<System.Type, System.Type> _memberTypeToCurveType;
        private static ListDictionary<string, CustomAccessorData> _targetToCustomAccessor;

        private static void BuildCurveTypeDictionary()
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

        private static void BuildAccessorDictionary()
        {
            _targetToCustomAccessor = new ListDictionary<string, CustomAccessorData>();
            foreach (var tp in TypeUtil.GetTypesAssignableFrom(typeof(ITweenMemberAccessor)))
            {
                var attribs = tp.GetCustomAttributes(typeof(CustomTweenMemberAccessorAttribute), false).Cast<CustomTweenMemberAccessorAttribute>().ToArray();
                foreach (var attrib in attribs)
                {
                    var data = new CustomAccessorData()
                    {
                        priority = attrib.priority,
                        TargetType = attrib.HandledTargetType,
                        AccessorType = tp
                    };
                    _targetToCustomAccessor.Add(attrib.HandledPropName, data);
                }
            }

            foreach (var lst in _targetToCustomAccessor.Lists)
            {
                (lst as List<CustomAccessorData>).Sort((a, b) => b.priority.CompareTo(a.priority)); //sort descending
            }
        }

        private static IMemberAccessor GetAccessor(object target, string propName, out System.Type memberType)
        {
            if (_targetToCustomAccessor == null) BuildAccessorDictionary();

            string args = null;
            if(propName != null)
            {
                int fi = propName.IndexOf("(");
                if (fi >= 0)
                {
                    int li = propName.LastIndexOf(")");
                    if (li < fi) li = propName.Length;
                    args = propName.Substring(fi + 1, li - fi - 1);
                    propName = propName.Substring(0, fi);
                }
            }

            IList<CustomAccessorData> lst;
            if (_targetToCustomAccessor.Lists.TryGetList(propName, out lst))
            {
                var tp = target.GetType();
                CustomAccessorData data;
                int cnt = lst.Count;
                for (int i = 0; i < cnt; i++)
                {
                    data = lst[i];
                    if (data.TargetType.IsAssignableFrom(tp))
                    {
                        try
                        {
                            var acc = System.Activator.CreateInstance(data.AccessorType) as ITweenMemberAccessor;
                            memberType = acc.Init(propName, args);
                            return acc;
                        }
                        catch
                        {
                            Debug.LogWarning("Failed to create Custom MemberAccessor of type '" + tp.FullName + "'.");
                            break;
                        }
                    }
                }
            }
            return MemberAccessorPool.GetAccessor(target.GetType(), propName, out memberType);
        }

        private static MemberCurve Create(System.Type memberType, IMemberAccessor accessor, Ease ease, float dur, object start, object end, object option)
        {
            if (_memberTypeToCurveType == null) BuildCurveTypeDictionary();

            if (_memberTypeToCurveType.ContainsKey(memberType))
            {
                try
                {
                    var curve = System.Activator.CreateInstance(_memberTypeToCurveType[memberType], true) as MemberCurve;
                    curve._dur = dur;
                    curve.Ease = ease;
                    curve._accessor = accessor;
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

        public new static MemberCurve CreateTo(object target, string propName, Ease ease, object end, float dur, object option = null)
        {
            if (target == null) throw new System.ArgumentNullException("target");
            System.Type memberType;
            var accessor = MemberCurve.GetAccessor(target, propName, out memberType);

            object start = accessor.Get(target);

            return MemberCurve.Create(memberType, accessor, ease, dur, start, end, option);
        }

        public new static MemberCurve CreateFrom(object target, string propName, Ease ease, object start, float dur, object option = null)
        {
            if (target == null) throw new System.ArgumentNullException("target");
            System.Type memberType;
            var accessor = MemberCurve.GetAccessor(target, propName, out memberType);

            object end = accessor.Get(target);

            return MemberCurve.Create(memberType, accessor, ease, dur, start, end, option);
        }

        public new static MemberCurve CreateBy(object target, string propName, Ease ease, object amt, float dur, object option = null)
        {
            if (target == null) throw new System.ArgumentNullException("target");
            System.Type memberType;
            var accessor = MemberCurve.GetAccessor(target, propName, out memberType);

            object start = accessor.Get(target);
            object end = TweenCurve.TrySum(memberType, start, amt);

            return MemberCurve.Create(memberType, accessor, ease, dur, start, end, option);
        }

        public new static MemberCurve CreateFromTo(object target, string propName, Ease ease, object start, object end, float dur, object option = null)
        {
            if (target == null) throw new System.ArgumentNullException("target");
            System.Type memberType;
            var accessor = MemberCurve.GetAccessor(target, propName, out memberType);

            return MemberCurve.Create(memberType, accessor, ease, dur, start, end, option);
        }

        /// <summary>
        /// Creates a curve that will animate from the current value to the end value, but will rescale the duration from how long it should have 
        /// taken from start to end, but already animated up to current.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propName"></param>
        /// <param name="ease"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="dur"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public new static MemberCurve CreateRedirectTo(object target, string propName, Ease ease, float start, float end, float dur, object option = null)
        {
            if (target == null) throw new System.ArgumentNullException("target");
            System.Type memberType;
            var accessor = MemberCurve.GetAccessor(target, propName, out memberType);

            var current = accessor.Get(target);
            dur = MathUtil.PercentageOffMinMax(ConvertUtil.ToSingle(current), end, start) * dur;

            return MemberCurve.Create(memberType, accessor, ease, dur, current, ConvertUtil.ToPrim(end, memberType), option);
        }

        #endregion

    }

}
