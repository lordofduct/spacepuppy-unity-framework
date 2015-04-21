using System;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween
{

    public abstract class ImplicitCurve : Curve
    {

        #region Fields

        private Ease _ease;
        private float _dur;
        private float _delay;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// This MUST exist for reflective creation.
        /// </summary>
        protected ImplicitCurve()
        {
        }

        public ImplicitCurve(float dur)
        {
            _ease = EaseMethods.LinearEaseNone;
            _dur = dur;
            _delay = 0f;
        }

        public ImplicitCurve(Ease ease, float dur)
        {
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

        protected abstract object GetCurrentValue(object targ);

        /// <summary>
        /// Returns the appropriate value of the member on the curve at t, where t is a scalar (t would be 1.0, not 100, for 100%).
        /// </summary>
        /// <param name="t">The percentage of completion across the curve that the member is at.</param>
        /// <returns></returns>
        protected abstract void UpdateValue(object targ, float dt, float t, float easedT);

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
            this.UpdateValue(targ, dt, t, (_dur > 0) ? _ease(t - _delay, 0f, 1f, _dur) : 1f);
        }

        #endregion

        #region Factory

        private class CurveData
        {
            public int priority;
            public System.Type TargetType;
            public System.Type CurveType;
        }

        private static ListDictionary<string, CurveData> _targetToCurveType;
        private static void BuildDictionary()
        {
            _targetToCurveType = new ListDictionary<string, CurveData>();
            foreach (var tp in TypeUtil.GetTypesAssignableFrom(typeof(ImplicitCurve)))
            {
                var attribs = tp.GetCustomAttributes(typeof(CustomImplicitCurveAttribute), false).Cast<CustomImplicitCurveAttribute>().ToArray();
                foreach (var attrib in attribs)
                {
                    var data = new CurveData()
                    {
                        priority = attrib.priority,
                        TargetType = attrib.HandledTargetType,
                        CurveType = tp
                    };
                    _targetToCurveType.Add(attrib.HandledPropName, data);
                }
            }

            foreach(var lst in _targetToCurveType.Lists)
            {
                (lst as List<CurveData>).Sort((a, b) => b.priority.CompareTo(a.priority));
            }
        }
        private static System.Type GetCurveType(object target, string propName)
        {
            if (_targetToCurveType == null) ImplicitCurve.BuildDictionary();

            IList<CurveData> lst;
            if(_targetToCurveType.Lists.TryGetList(propName, out lst))
            {
                var tp = target.GetType();
                CurveData data;
                int cnt = lst.Count;
                for (int i = 0; i < cnt; i++)
                {
                    data = lst[i];
                    if (data.TargetType.IsAssignableFrom(tp)) return data.CurveType;
                }
            }
            return null;
        }


        public new static ImplicitCurve CreateFromTo(object target, string propName, Ease ease, object start, object end, float dur, object option = null)
        {
            if (target == null) throw new System.ArgumentNullException("target");

            var tp = GetCurveType(target, propName);
            if(tp != null)
            {
                try
                {
                    var curve = System.Activator.CreateInstance(tp, true) as ImplicitCurve;
                    curve._dur = dur;
                    curve._ease = ease;
                    curve.ReflectiveInit(start, end, option);
                    return curve;
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception("Failed to reflect custom curve of type '" + tp.FullName + "'.", ex);
                }
            }

            return null;
        }

        public new static ImplicitCurve CreateTo(object target, string propName, Ease ease, object end, float dur, object option = null)
        {
            if (target == null) throw new System.ArgumentNullException("target");

            var tp = GetCurveType(target, propName);
            if (tp != null)
            {
                try
                {
                    var curve = System.Activator.CreateInstance(tp, true) as ImplicitCurve;
                    curve._dur = dur;
                    curve._ease = ease;
                    var start = curve.GetCurrentValue(target);
                    curve.ReflectiveInit(start, end, option);
                    return curve;
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception("Failed to reflect custom curve of type '" + tp.FullName + "'.", ex);
                }
            }

            return null;
        }

        public new static ImplicitCurve CreateFrom(object target, string propName, Ease ease, object start, float dur, object option = null)
        {
            if (target == null) throw new System.ArgumentNullException("target");

            var tp = GetCurveType(target, propName);
            if (tp != null)
            {
                try
                {
                    var curve = System.Activator.CreateInstance(tp, true) as ImplicitCurve;
                    curve._dur = dur;
                    curve._ease = ease;
                    var end = curve.GetCurrentValue(target);
                    curve.ReflectiveInit(start, end, option);
                    return curve;
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception("Failed to reflect custom curve of type '" + tp.FullName + "'.", ex);
                }
            }

            return null;
        }

        public new static ImplicitCurve CreateBy(object target, string propName, Ease ease, object amt, float dur, object option = null)
        {
            if (target == null) throw new System.ArgumentNullException("target");

            var tp = GetCurveType(target, propName);
            if (tp != null)
            {
                try
                {
                    var curve = System.Activator.CreateInstance(tp, true) as ImplicitCurve;
                    curve._dur = dur;
                    curve._ease = ease;
                    var start = curve.GetCurrentValue(target);
                    var propTp = (start != null) ? start.GetType() : typeof(float);
                    var end = Curve.TrySum(propTp, start, amt);
                    curve.ReflectiveInit(start, end, option);
                    return curve;
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception("Failed to reflect custom curve of type '" + tp.FullName + "'.", ex);
                }
            }

            return null;
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
        public new static ImplicitCurve CreateRedirectTo(object target, string propName, Ease ease, float start, float end, float dur, object option = null)
        {
            if (target == null) throw new System.ArgumentNullException("target");

            var tp = GetCurveType(target, propName);
            if (tp != null)
            {
                try
                {
                    var curve = System.Activator.CreateInstance(tp, true) as ImplicitCurve;
                    var current = curve.GetCurrentValue(target);
                    dur = MathUtil.PercentageOffMinMax(ConvertUtil.ToSingle(current), end, start) * dur;

                    curve._dur = dur;
                    curve._ease = ease;
                    curve.ReflectiveInit(start, end, option);
                    return curve;
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception("Failed to reflect custom curve of type '" + tp.FullName + "'.", ex);
                }
            }

            return null;
        }


        #endregion

    }

}
