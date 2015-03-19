using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.spacepuppy.Tween.Curves;
using com.spacepuppy.Utils;
using com.spacepuppy.Utils.FastDynamicMemberAccessor;

namespace com.spacepuppy.Tween
{

    public class PropertyHash
    {

        private enum AnimMode
        {
            Curve = -1,
            To = 0,
            From = 1,
            By = 2,
            FromTo = 3
        }

        #region Fields

        private List<PropInfo> _props = new List<PropInfo>();
        private Ease _defaultEase = EaseMethods.LinearEaseNone;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        #endregion

        #region Prop Methods

        public PropertyHash Curve(string memberName, ICurve curve)
        {
            _props.Add(new PropInfo(AnimMode.Curve, memberName, null, curve, float.NaN));
            return this;
        }

        public PropertyHash To(string memberName, object end, float dur)
        {
            _props.Add(new PropInfo(AnimMode.To, memberName, null, end, dur));
            return this;
        }

        public PropertyHash To(string memberName, Ease ease, object end, float dur)
        {
            _props.Add(new PropInfo(AnimMode.To, memberName, ease, end, dur));
            return this;
        }

        public PropertyHash From(string memberName, object start, float dur)
        {
            _props.Add(new PropInfo(AnimMode.From, memberName, null, start, dur));
            return this;
        }

        public PropertyHash From(string memberName, Ease ease, object start, float dur)
        {
            _props.Add(new PropInfo(AnimMode.From, memberName, ease, start, dur));
            return this;
        }

        public PropertyHash By(string memberName, object amt, float dur)
        {
            _props.Add(new PropInfo(AnimMode.By, memberName, null, amt, dur));
            return this;
        }

        public PropertyHash By(string memberName, Ease ease, object amt, float dur)
        {
            _props.Add(new PropInfo(AnimMode.By, memberName, ease, amt, dur));
            return this;
        }

        public PropertyHash FromTo(string memberName, object start, object end, float dur)
        {
            _props.Add(new PropInfo(AnimMode.FromTo, memberName, null, start, dur, end));
            return this;
        }

        public PropertyHash FromTo(string memberName, Ease ease, object start, object end, float dur)
        {
            _props.Add(new PropInfo(AnimMode.FromTo, memberName, ease, start, dur, end));
            return this;
        }

        #endregion

        #region Apply Method

        public void Apply(ObjectTweener tween)
        {
            var targ = tween.Target;
            var targTp = targ.GetType();

            for (int i = 0; i < _props.Count; i++)
            {
                var prop = _props[i];
                try
                {
                    Ease ease = (prop.ease == null) ? _defaultEase : prop.ease;
                    float dur = prop.dur;
                    switch (prop.mode)
                    {
                        case AnimMode.Curve:
                            var curve = prop.value as ICurve;
                            if(curve != null)
                            {
                                tween.Curves.Add(prop.name, curve);
                            }
                            break;
                        case AnimMode.To:
                            tween.Curves.AddTo(prop.name, ease, prop.value, dur);
                            break;
                        case AnimMode.From:
                            tween.Curves.AddFrom(prop.name, ease, prop.value, dur);
                            break;
                        case AnimMode.By:
                            tween.Curves.AddBy(prop.name, ease, prop.value, dur);
                            break;
                        case AnimMode.FromTo:
                            tween.Curves.AddFromTo(prop.name, ease, prop.value, prop.altValue, dur);
                            break;
                    }
                }
                catch
                {
                    Debug.LogWarning("Failed to tween property '" + prop.name + "' on target.", targ as Object);
                }
            }
        }

        #endregion

        #region Special Types

        private struct PropInfo
        {
            public AnimMode mode;
            public string name;
            public Ease ease;
            public object value;
            public float dur;
            public object altValue;
            public bool slerp;

            public PropInfo(AnimMode mode, string nm, Ease e, object v, float d)
            {
                this.mode = mode;
                this.name = nm;
                this.ease = e;
                this.value = v;
                this.dur = d;
                this.altValue = null;
                this.slerp = false;
            }

            public PropInfo(AnimMode mode, string nm, Ease e, object v, float d, object altV)
            {
                this.mode = mode;
                this.name = nm;
                this.ease = e;
                this.value = v;
                this.dur = d;
                this.altValue = altV;
                this.slerp = false;
            }
        }

        #endregion

    }

}
