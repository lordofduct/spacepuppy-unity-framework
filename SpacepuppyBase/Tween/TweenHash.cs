using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.spacepuppy.Tween.Curves;
using com.spacepuppy.Utils;
using com.spacepuppy.Utils.FastDynamicMemberAccessor;

namespace com.spacepuppy.Tween
{

    public class TweenHash
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
        private DeltaTimeType _deltaType;
        private TweenWrapMode _wrap;
        private int _wrapCount;
        private bool _reverse;
        private System.EventHandler _onPlay;
        private System.EventHandler _onStep;
        private System.EventHandler _onWrap;
        private System.EventHandler _onFinish;

        #endregion

        #region CONSTRUCTOR

        public TweenHash()
        {

        }

        public TweenHash(object targ)
        {
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
            _deltaType = DeltaTimeType.Normal;
            return this;
        }

        public TweenHash UseRealTime()
        {
            _deltaType = DeltaTimeType.Real;
            return this;
        }

        public TweenHash UseSmoothTime()
        {
            _deltaType = DeltaTimeType.Smooth;
            return this;
        }

        public TweenHash Use(DeltaTimeType type)
        {
            _deltaType = type;
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

        public TweenHash OnPlay(System.EventHandler d)
        {
            if (d == null) return this;
            _onPlay += d;
            return this;
        }

        public TweenHash OnPlay(System.Action<Tweener> d)
        {
            if (d == null) return this;
            _onPlay += (s, e) => d(s as Tweener);
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

        public TweenHash Curve(string memberName, ICurve curve)
        {
            _props.Add(new PropInfo(AnimMode.Curve, memberName, null, curve, float.NaN));
            return this;
        }

        public TweenHash To(string memberName, object end, float dur)
        {
            _props.Add(new PropInfo(AnimMode.To, memberName, null, end, dur));
            return this;
        }

        public TweenHash To(string memberName, Ease ease, object end, float dur)
        {
            _props.Add(new PropInfo(AnimMode.To, memberName, ease, end, dur));
            return this;
        }

        public TweenHash From(string memberName, object start, float dur)
        {
            _props.Add(new PropInfo(AnimMode.From, memberName, null, start, dur));
            return this;
        }

        public TweenHash From(string memberName, Ease ease, object start, float dur)
        {
            _props.Add(new PropInfo(AnimMode.From, memberName, ease, start, dur));
            return this;
        }

        public TweenHash By(string memberName, object amt, float dur)
        {
            _props.Add(new PropInfo(AnimMode.By, memberName, null, amt, dur));
            return this;
        }

        public TweenHash By(string memberName, Ease ease, object amt, float dur)
        {
            _props.Add(new PropInfo(AnimMode.By, memberName, ease, amt, dur));
            return this;
        }

        public TweenHash FromTo(string memberName, object start, object end, float dur)
        {
            _props.Add(new PropInfo(AnimMode.FromTo, memberName, null, start, dur, end));
            return this;
        }

        public TweenHash FromTo(string memberName, Ease ease, object start, object end, float dur)
        {
            _props.Add(new PropInfo(AnimMode.FromTo, memberName, ease, start, dur, end));
            return this;
        }

        #endregion

        #region Play Methods

        public Tweener Create()
        {
            if (_targ == null) return null;

            var tween = new ObjectTweener(_targ);
            this.Apply(tween);
            return tween;
        }

        public Tweener[] Create(params object[] targets)
        {
            if (targets == null || targets.Length == 0)
            {
                return (_targ != null) ? new Tweener[] { this.Create() } : new Tweener[] { };
            }

            Tweener[] tweens;
            if (_targ != null)
            {
                tweens = new Tweener[targets.Length + 1];
                tweens[tweens.Length - 1] = this.Create();
            }
            else
            {
                tweens = new Tweener[targets.Length];
            }
            for (int i = 0; i < targets.Length; i++)
            {
                var tw = new ObjectTweener(targets[i]);
                tweens[i] = tw;
                this.Apply(tw);
            }
            return tweens;
        }

        public Tweener Play()
        {
            if (_targ == null) return null;

            var tween = this.Create();
            tween.Play();
            return tween;
        }

        public Tweener[] Play(params object[] targets)
        {
            var tweens = this.Create(targets);
            for(int i = 0; i < tweens.Length; i++)
            {
                tweens[i].Play();
            }
            return tweens;
        }

        public Tweener Play(float playHeadPos)
        {
            if (_targ == null) return null;

            var tween = this.Create();
            tween.Play(playHeadPos);
            return tween;
        }

        public Tweener[] Play(float playHeadPos, params object[] targets)
        {
            var tweens = this.Create(targets);
            for (int i = 0; i < tweens.Length; i++)
            {
                tweens[i].Play(playHeadPos);
            }
            return tweens;
        }

        public void Apply(ObjectTweener tween)
        {
            //set curves
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

            //set props
            tween.UpdateType = _updateType;
            tween.DeltaType = _deltaType;
            tween.WrapMode = _wrap;
            tween.WrapCount = _wrapCount;
            tween.Reverse = _reverse;
            if (_onPlay != null) tween.OnPlay += _onPlay;
            if (_onStep != null) tween.OnStep += _onStep;
            if (_onWrap != null) tween.OnWrap += _onWrap;
            if (_onFinish != null) tween.OnFinish += _onFinish;
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
