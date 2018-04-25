using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween
{

    public class TweenHash : ITweenHash, System.ICloneable
    {

        public enum AnimMode
        {
            AnimCurve = -2,
            Curve = -1,
            To = 0,
            From = 1,
            By = 2,
            FromTo = 3,
            RedirectTo = 4
        }

        #region Fields

        private TweenHash _prevNode;

        private object _id;
        private object _targ;
        private List<PropInfo> _props = new List<PropInfo>();
        private Ease _defaultEase = EaseMethods.LinearEaseNone;
        private float _delay;
        private UpdateSequence _updateType;
        private ITimeSupplier _timeSupplier;
        private TweenWrapMode _wrap;
        private int _wrapCount;
        private bool _reverse;
        private float _speedScale = 1.0f;
        private System.EventHandler _onStep;
        private System.EventHandler _onWrap;
        private System.EventHandler _onFinish;
        private System.EventHandler _onStopped;

        #endregion

        #region CONSTRUCTOR

        public TweenHash(object targ)
        {
            if (targ == null) throw new System.ArgumentNullException("targ");
            _targ = targ;
            _id = null;
        }

        public TweenHash(object targ, object id)
        {
            if (targ == null) throw new System.ArgumentNullException("targ");
            _targ = targ;
            _id = id;
        }

        #endregion

        #region Properties

        #endregion

        #region Config Methods

        /// <summary>
        /// Sets the id for the tween, if a FollowOn sequence, the entire sequence id is updated.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TweenHash SetId(object id)
        {
            _id = id;
            if (_prevNode != null) _prevNode.SetId(id);
            return this;
        }

        public TweenHash Ease(Ease ease)
        {
            _defaultEase = ease ?? EaseMethods.LinearEaseNone;
            return this;
        }

        public TweenHash Delay(float delay)
        {
            _delay = delay;
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

        public TweenHash SpeedScale(float scale)
        {
            _speedScale = scale;
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

        public TweenHash OnStopped(System.EventHandler d)
        {
            if (d == null) return this;
            _onStopped += d;
            return this;
        }

        public TweenHash OnStopped(System.Action<Tweener> d)
        {
            if (d == null) return this;
            _onStopped += (s, e) => d(s as Tweener);
            return this;
        }

        #endregion

        

        #region Curve Methods

        /// <summary>
        /// Follow this tween with another tween.
        /// 
        /// Inherits updateType and timeSupplier, unless otherwise changed.
        /// </summary>
        /// <returns></returns>
        public TweenHash FollowOn()
        {
            var hash = new TweenHash(_targ, _id);
            hash._prevNode = this;
            hash._updateType = _updateType;
            hash._timeSupplier = _timeSupplier;
            return hash;
        }

        public TweenHash Apply(TweenConfigCallback callback, Ease ease, float dur)
        {
            if (callback != null) callback(this, ease, dur);
            return this;
        }

        //#########################
        // CURVES
        //

        public TweenHash UseCurve(TweenCurve curve)
        {
            _props.Add(new PropInfo(AnimMode.Curve, null, null, curve, float.NaN, null));
            return this;
        }

        public TweenHash UseCurve(string memberName, AnimationCurve curve, float dur, object option = null)
        {
            if (curve == null) throw new System.ArgumentNullException("curve");
            _props.Add(new PropInfo(AnimMode.AnimCurve, memberName, EaseMethods.FromAnimationCurve(curve), null, dur, option));
            return this;
        }

        public TweenHash UseCurve(string memberName, AnimationCurve curve, object option = null)
        {
            if (curve == null) throw new System.ArgumentNullException("curve");
            float dur = (curve.keys.Length > 0) ? curve.keys.Last().time : 0f;
            _props.Add(new PropInfo(AnimMode.AnimCurve, memberName, EaseMethods.FromAnimationCurve(curve), null, dur, option));
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

        public TweenHash ByAnimMode(AnimMode mode, string memberName, Ease ease, object value, float dur, object end)
        {
            _props.Add(new PropInfo(mode, memberName, ease, value, dur, null, end));
            return this;
        }

        public TweenHash ByAnimMode(AnimMode mode, string memberName, Ease ease, object value, float dur, object end, object option)
        {
            _props.Add(new PropInfo(mode, memberName, ease, value, dur, option, end));
            return this;
        }

        /// <summary>
        /// Creates a curve that will animate from the current value to the end value, but will rescale the duration from how long it should have 
        /// taken from start to end, but already animated up to current.
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="dur"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public TweenHash RedirectTo(string memberName, object start, object end, float dur, object option = null)
        {
            _props.Add(new PropInfo(AnimMode.RedirectTo, memberName, null, start, dur, option, end));
            return this;
        }

        /// <summary>
        /// Creates a curve that will animate from the current value to the end value, but will rescale the duration from how long it should have 
        /// taken from start to end, but already animated up to current.
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="ease"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="dur"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public TweenHash RedirectTo(string memberName, Ease ease, object start, object end, float dur, object option = null)
        {
            _props.Add(new PropInfo(AnimMode.RedirectTo, memberName, ease, start, dur, option, end));
            return this;
        }

        #endregion
        
        #region Play Methods

        public Tweener Play(bool autoKill, object autoKillToken = null)
        {
            if (_targ == null) return null;

            var tween = this.Create();
            if (autoKill)
            {
                tween.AutoKillToken = autoKillToken;
                tween.Play();
                if(tween.IsPlaying) SPTween.AutoKill(tween);
            }
            else
            {
                tween.Play();
            }
            return tween;
        }

        public Tweener Play(float playHeadPos, bool autoKill, object autoKillToken = null)
        {
            if (_targ == null) return null;

            var tween = this.Create();
            if (autoKill)
            {
                tween.AutoKillToken = autoKillToken;
                tween.Play(playHeadPos);
                SPTween.AutoKill(tween);
            }
            else
            {
                tween.Play(playHeadPos);
            }
            return tween;
        }
        
        public Tweener Create()
        {
            if (_targ == null) return null;

            //set curves
            Tweener tween = null;
            if (_props.Count > 1)
            {
                var grp = new TweenCurveGroup();
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
            else if (_props.Count == 1)
            {
                var curve = this.CreateCurve(_props[0]);
                if (curve == null)
                {
                    Debug.LogWarning("Failed to create tween for property '" + _props[0].name + "' on target.", _targ as UnityEngine.Object);
                    return new ObjectTweener(_targ, TweenCurve.Null);
                }
                else
                    tween = new ObjectTweener(_targ, curve);
            }
            else
            {
                tween = new ObjectTweener(_targ, TweenCurve.Null);
            }

            //set props
            if (_id != null) tween.Id = _id;
            tween.UpdateType = _updateType;
            tween.TimeSupplier = _timeSupplier;
            tween.SpeedScale = _speedScale;
            tween.WrapMode = _wrap;
            tween.WrapCount = _wrapCount;
            tween.Reverse = _reverse;
            tween.Delay = _delay;
            if (_onStep != null) tween.OnStep += _onStep;
            if (_onWrap != null) tween.OnWrap += _onWrap;
            if (_onFinish != null) tween.OnFinish += _onFinish;
            if (_onStopped != null) tween.OnStopped += _onStopped;
            
            if(_prevNode != null)
            {
                var seq = new TweenSequence();
                seq.Id = tween.Id;
                seq.Tweens.Add(tween);

                var node = _prevNode;
                while(node != null)
                {
                    seq.Tweens.Insert(0, node.Create());
                    node = node._prevNode;
                }

                tween = seq;
            }

            return tween;
        }

        private TweenCurve CreateCurve(PropInfo prop)
        {
            try
            {
                Ease ease = (prop.ease == null) ? _defaultEase : prop.ease;
                float dur = prop.dur;
                switch (prop.mode)
                {
                    case AnimMode.AnimCurve:
                        return MemberCurve.CreateFromTo(_targ, prop.name, ease, null, null, dur, prop.option);
                    case AnimMode.Curve:
                        return prop.value as TweenCurve;
                    case AnimMode.To:
                        return MemberCurve.CreateTo(_targ, prop.name, ease, prop.value, dur, prop.option);
                    case AnimMode.From:
                        return MemberCurve.CreateFrom(_targ, prop.name, ease, prop.value, dur, prop.option);
                    case AnimMode.By:
                        return MemberCurve.CreateBy(_targ, prop.name, ease, prop.value, dur, prop.option);
                    case AnimMode.FromTo:
                        return MemberCurve.CreateFromTo(_targ, prop.name, ease, prop.value, prop.altValue, dur, prop.option);
                    case AnimMode.RedirectTo:
                        return MemberCurve.CreateRedirectTo(_targ, prop.name, ease, ConvertUtil.ToSingle(prop.value), ConvertUtil.ToSingle(prop.altValue), dur, prop.option);
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
        
        #endregion

        #region ITweenHash Interface

        ITweenHash ITweenHash.SetId(object id)
        {
            return this.SetId(id);
        }

        ITweenHash ITweenHash.Ease(Ease ease)
        {
            return this.Ease(ease);
        }

        ITweenHash ITweenHash.Delay(float delay)
        {
            return this.Delay(delay);
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

        ITweenHash ITweenHash.SpeedScale(float scale)
        {
            return this.SpeedScale(scale);
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

        ITweenHash ITweenHash.OnStopped(System.EventHandler d)
        {
            return this.OnStopped(d);
        }

        ITweenHash ITweenHash.OnStopped(System.Action<Tweener> d)
        {
            return this.OnFinish(d);
        }

        Tweener ITweenHash.Play(bool autoKill, object autoKillToken)
        {
            return this.Play(autoKill, autoKillToken);
        }

        Tweener ITweenHash.Play(float playHeadPosition, bool autoKill, object autoKillToken)
        {
            return this.Play(playHeadPosition, autoKill, autoKillToken);
        }

        #endregion

        #region ICloneable INterface

        public TweenHash Clone()
        {
            var hash = new TweenHash(_targ, _id);
            hash._props = _props;
            hash._defaultEase = _defaultEase;
            hash._delay = _delay;
            hash._updateType = _updateType;
            hash._timeSupplier = _timeSupplier;
            hash._wrap = _wrap;
            hash._wrapCount = _wrapCount;
            hash._reverse = _reverse;
            hash._speedScale = _speedScale;
            hash._onStep = _onStep;
            hash._onWrap = _onWrap;
            hash._onFinish = _onFinish;
            hash._onStopped = _onStopped;
            return hash;
        }

        object System.ICloneable.Clone()
        {
            return this.Clone();
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

        #endregion

    }

}
