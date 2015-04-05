using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.spacepuppy.Utils;
using com.spacepuppy.Utils.FastDynamicMemberAccessor;

namespace com.spacepuppy.Tween
{

    public class TweenHash : ITweenHash
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

        private object _targ;
        private List<PropInfo> _props = new List<PropInfo>();
        private Ease _defaultEase = EaseMethods.LinearEaseNone;
        private UpdateSequence _updateType;
        private ITimeSupplier _timeSupplier;
        private TweenWrapMode _wrap;
        private int _wrapCount;
        private bool _reverse;
        private System.EventHandler _onStep;
        private System.EventHandler _onWrap;
        private System.EventHandler _onFinish;

        #endregion

        #region CONSTRUCTOR

        public TweenHash(object targ)
        {
            if (targ == null) throw new System.ArgumentNullException("targ");
            _targ = targ;
        }

        #endregion

        #region Properties

        #endregion

        #region Config Methods

        public TweenHash Ease(Ease ease)
        {
            _defaultEase = ease ?? EaseMethods.LinearEaseNone;
            return this;
        }

        public TweenHash UseUpdate()
        {
            _updateType = UpdateSequence.Update;
            return this;
        }

        public TweenHash UseFixedUpdate()
        {
            _updateType = UpdateSequence.FixedUpdate;
            return this;
        }

        public TweenHash UseLateUpdate()
        {
            _updateType = UpdateSequence.LateUpdate;
            return this;
        }

        public TweenHash Use(UpdateSequence type)
        {
            _updateType = type;
            return this;
        }

        public TweenHash UseNormalTime()
        {
            _timeSupplier = SPTime.Normal;
            return this;
        }

        public TweenHash UseRealTime()
        {
            _timeSupplier = SPTime.Real;
            return this;
        }

        public TweenHash UseSmoothTime()
        {
            _timeSupplier = SPTime.Smooth;
            return this;
        }

        public TweenHash Use(ITimeSupplier time)
        {
            _timeSupplier = time ?? SPTime.Normal;
            return this;
        }

        public TweenHash PlayOnce()
        {
            _wrap = TweenWrapMode.Once;
            return this;
        }

        public TweenHash Loop(int count = -1)
        {
            _wrap = TweenWrapMode.Loop;
            _wrapCount = count;
            return this;
        }

        public TweenHash PingPong(int count = -1)
        {
            _wrap = TweenWrapMode.PingPong;
            _wrapCount = count;
            return this;
        }

        public TweenHash Wrap(TweenWrapMode wrap, int count = -1)
        {
            _wrap = wrap;
            _wrapCount = count;
            return this;
        }

        public TweenHash Reverse()
        {
            _reverse = true;
            return this;
        }

        public TweenHash Reverse(bool reverse)
        {
            _reverse = reverse;
            return this;
        }

        public TweenHash OnStep(System.EventHandler d)
        {
            if (d == null) return this;
            _onStep += d;
            return this;
        }

        public TweenHash OnStep(System.Action<Tweener> d)
        {
            if (d == null) return this;
            _onStep += (s, e) => d(s as Tweener);
            return this;
        }

        public TweenHash OnWrap(System.EventHandler d)
        {
            if (d == null) return this;
            _onWrap += d;
            return this;
        }

        public TweenHash OnWrap(System.Action<Tweener> d)
        {
            if (d == null) return this;
            _onWrap += (s, e) => d(s as Tweener);
            return this;
        }

        public TweenHash OnFinish(System.EventHandler d)
        {
            if (d == null) return this;
            _onFinish += d;
            return this;
        }

        public TweenHash OnFinish(System.Action<Tweener> d)
        {
            if (d == null) return this;
            _onFinish += (s, e) => d(s as Tweener);
            return this;
        }

        #endregion

        #region Curve Methods

        //#########################
        // CURVES
        //

        public TweenHash Curve(Curve curve)
        {
            _props.Add(new PropInfo(AnimMode.Curve, null, null, curve, float.NaN, null));
            return this;
        }

        public TweenHash To(string memberName, object end, float dur, object option = null)
        {
            _props.Add(new PropInfo(AnimMode.To, memberName, null, end, dur, option));
            return this;
        }

        public TweenHash To(string memberName, Ease ease, object end, float dur, object option = null)
        {
            _props.Add(new PropInfo(AnimMode.To, memberName, ease, end, dur, option));
            return this;
        }

        public TweenHash From(string memberName, object start, float dur, object option = null)
        {
            _props.Add(new PropInfo(AnimMode.From, memberName, null, start, dur, option));
            return this;
        }

        public TweenHash From(string memberName, Ease ease, object start, float dur, object option = null)
        {
            _props.Add(new PropInfo(AnimMode.From, memberName, ease, start, dur, option));
            return this;
        }

        public TweenHash By(string memberName, object amt, float dur, object option = null)
        {
            _props.Add(new PropInfo(AnimMode.By, memberName, null, amt, dur, option));
            return this;
        }

        public TweenHash By(string memberName, Ease ease, object amt, float dur, object option = null)
        {
            _props.Add(new PropInfo(AnimMode.By, memberName, ease, amt, dur, option));
            return this;
        }

        public TweenHash FromTo(string memberName, object start, object end, float dur, object option = null)
        {
            _props.Add(new PropInfo(AnimMode.FromTo, memberName, null, start, dur, option, end));
            return this;
        }

        public TweenHash FromTo(string memberName, Ease ease, object start, object end, float dur, object option = null)
        {
            _props.Add(new PropInfo(AnimMode.FromTo, memberName, ease, start, dur, option, end));
            return this;
        }

        #endregion

        #region Play Methods

        public Tweener Play()
        {
            if (_targ == null) return null;

            var tween = this.Create();
            tween.Play();
            return tween;
        }

        public Tweener Play(float playHeadPos)
        {
            if (_targ == null) return null;

            var tween = this.Create();
            tween.Play(playHeadPos);
            return tween;
        }
        public Tweener Create()
        {
            if (_targ == null) return null;

            //set curves
            Tweener tween = null;
            if (_props.Count == 0)
            {
                return new ObjectTweener(_targ, new NullCurve());
            }
            else if (_props.Count == 1)
            {
                var curve = this.CreateCurve(_props[0]);
                if (curve == null)
                    Debug.LogWarning("Failed to create tween for property '" + _props[0].name + "' on target.", _targ as Object);
                else
                    tween = new ObjectTweener(_targ, curve);
            }
            else
            {
                var grp = new CurveGroup();
                for (int i = 0; i < _props.Count; i++)
                {
                    var curve = this.CreateCurve(_props[i]);
                    if (curve == null)
                        Debug.LogWarning("Failed to create tween for property '" + _props[i].name + "' on target.", _targ as Object);
                    else
                        grp.Curves.Add(curve);
                }
                tween = new ObjectTweener(_targ, grp);
            }

            //set props
            tween.UpdateType = _updateType;
            tween.TimeSupplier = _timeSupplier;
            tween.WrapMode = _wrap;
            tween.WrapCount = _wrapCount;
            tween.Reverse = _reverse;
            if (_onStep != null) tween.OnStep += _onStep;
            if (_onWrap != null) tween.OnWrap += _onWrap;
            if (_onFinish != null) tween.OnFinish += _onFinish;

            return tween;
        }
        private Curve CreateCurve(PropInfo prop)
        {
            try
            {
                Ease ease = (prop.ease == null) ? _defaultEase : prop.ease;
                float dur = prop.dur;
                switch (prop.mode)
                {
                    case AnimMode.Curve:
                        return prop.value as Curve;
                    case AnimMode.To:
                        return MemberCurve.CreateTo(_targ, prop.name, ease, prop.value, dur, prop.option);
                    case AnimMode.From:
                        return MemberCurve.CreateFrom(_targ, prop.name, ease, prop.value, dur, prop.option);
                    case AnimMode.By:
                        return MemberCurve.CreateBy(_targ, prop.name, ease, prop.value, dur, prop.option);
                    case AnimMode.FromTo:
                        return MemberCurve.CreateFromTo(_targ, prop.name, ease, prop.value, prop.altValue, dur, prop.option);
                }
            }
            catch
            {
                return null;
            }

            return null;
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
            public object option;

            public PropInfo(AnimMode mode, string nm, Ease e, object v, float d, object option)
            {
                this.mode = mode;
                this.name = nm;
                this.ease = e;
                this.value = v;
                this.dur = d;
                this.altValue = null;
                this.option = option;
            }

            public PropInfo(AnimMode mode, string nm, Ease e, object v, float d, object option, object altV)
            {
                this.mode = mode;
                this.name = nm;
                this.ease = e;
                this.value = v;
                this.dur = d;
                this.altValue = altV;
                this.option = option;
            }

        }

        private class NullCurve : Curve
        {

            public override float TotalTime
            {
                get { return 0f; }
            }

            protected internal override void Update(object targ, float dt, float t)
            {
                //do nothing
            }
        }

        #endregion

        #region ITweenHash Interface

        ITweenHash ITweenHash.Ease(Ease ease)
        {
            return this.Ease(ease);
        }

        ITweenHash ITweenHash.UseUpdate()
        {
            return this.UseUpdate();
        }

        ITweenHash ITweenHash.UseFixedUpdate()
        {
            return this.UseFixedUpdate();
        }

        ITweenHash ITweenHash.UseLateUpdate()
        {
            return this.UseLateUpdate();
        }

        ITweenHash ITweenHash.Use(UpdateSequence type)
        {
            return this.Use(type);
        }

        ITweenHash ITweenHash.UseNormalTime()
        {
            return this.UseNormalTime();
        }

        ITweenHash ITweenHash.UseRealTime()
        {
            return this.UseRealTime();
        }

        ITweenHash ITweenHash.UseSmoothTime()
        {
            return this.UseSmoothTime();
        }

        ITweenHash ITweenHash.Use(ITimeSupplier time)
        {
            return this.Use(time);
        }

        ITweenHash ITweenHash.PlayOnce()
        {
            return this.PlayOnce();
        }

        ITweenHash ITweenHash.Loop(int count)
        {
            return this.Loop(count);
        }

        ITweenHash ITweenHash.PingPong(int count)
        {
            return this.PingPong(count);
        }

        ITweenHash ITweenHash.Wrap(TweenWrapMode wrap, int count)
        {
            return this.Wrap(wrap, count);
        }

        ITweenHash ITweenHash.Reverse()
        {
            return this.Reverse();
        }

        ITweenHash ITweenHash.Reverse(bool reverse)
        {
            return this.Reverse(reverse);
        }

        ITweenHash ITweenHash.OnStep(System.EventHandler d)
        {
            return this.OnStep(d);
        }

        ITweenHash ITweenHash.OnStep(System.Action<Tweener> d)
        {
            return this.OnStep(d);
        }

        ITweenHash ITweenHash.OnWrap(System.EventHandler d)
        {
            return this.OnWrap(d);
        }

        ITweenHash ITweenHash.OnWrap(System.Action<Tweener> d)
        {
            return this.OnWrap(d);
        }

        ITweenHash ITweenHash.OnFinish(System.EventHandler d)
        {
            return this.OnFinish(d);
        }

        ITweenHash ITweenHash.OnFinish(System.Action<Tweener> d)
        {
            return this.OnFinish(d);
        }

        Tweener ITweenHash.Play()
        {
            return this.Play();
        }

        Tweener ITweenHash.Play(float playHeadPosition)
        {
            return this.Play(playHeadPosition);
        }

        #endregion

    }

}
