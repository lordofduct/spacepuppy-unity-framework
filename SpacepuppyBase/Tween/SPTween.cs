using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Tween
{
    public class SPTween : Singleton
    {

        #region Singleton Entrance

        private const string SPECIAL_NAME = "Spacepuppy.SPTween";

        private static SPTween _instance;

        #endregion

        #region Instance Interface

        #region Fields

        private List<Tweener> _runningTweens = new List<Tweener>();
        private List<Tweener> _toAdd = new List<Tweener>();
        private List<Tweener> _toRemove = new List<Tweener>();
        private bool _inUpdate;

        #endregion

        #region Methods

        internal static bool IsRunning(Tweener tween)
        {
            if (GameLoopEntry.ApplicationClosing) return false;
            if (_instance == null) return false;
            return _instance._runningTweens.Contains(tween) || _instance._toAdd.Contains(tween);
        }

        internal static void AddReference(Tweener tween)
        {
            if (GameLoopEntry.ApplicationClosing) return;
            if (_instance == null) _instance = Singleton.CreateSpecialInstance<SPTween>(SPECIAL_NAME);
            _instance.AddReference_Imp(tween);
        }
        private void AddReference_Imp(Tweener tween)
        {
            if(_inUpdate)
            {
                if (_runningTweens.Contains(tween) || _toAdd.Contains(tween)) return;
                _toAdd.Add(tween);
            }
            else
            {
                if (_runningTweens.Contains(tween)) return;
                _runningTweens.Add(tween);
            }
        }

        internal static void RemoveReference(Tweener tween)
        {
            if (GameLoopEntry.ApplicationClosing) return;
            if (_instance == null) return;
            _instance.RemoveReference_Imp(tween);
        }
        private void RemoveReference_Imp(Tweener tween)
        {
            if (!_runningTweens.Contains(tween)) return;
            if (_inUpdate)
            {
                if (_toRemove.Contains(tween)) return;
                _toRemove.Add(tween);
            }
            else
            {
                _runningTweens.Remove(tween);
            }
        }

        #endregion

        #region Update Methods

        private void Update()
        {
            this.DoUpdate(UpdateSequence.Update);

            if(Application.isEditor)
            {
                if (this.name.StartsWith(SPECIAL_NAME)) this.name = SPECIAL_NAME + " [ActiveTweens: " + _runningTweens.Count.ToString() + "]";
            }
        }

        private void FixedUpdate()
        {
            this.DoUpdate(UpdateSequence.FixedUpdate);
        }

        private void LateUpdate()
        {
            this.DoUpdate(UpdateSequence.LateUpdate);
        }

        private void DoUpdate(UpdateSequence updateType)
        {
            _inUpdate = true;

            float dt;
            Tweener twn;
            for (int i = 0; i < _runningTweens.Count; i++)
            {
                twn = _runningTweens[i];
                if (twn.UpdateType != updateType) continue;

                twn.Update();
            }

            _inUpdate = false;

            for(int i = 0; i < _toRemove.Count; i++)
            {
                _runningTweens.Remove(_toRemove[i]);
            }
            _toRemove.Clear();
            for(int i = 0; i < _toAdd.Count; i++)
            {
                _runningTweens.Add(_toAdd[i]);
            }
            _toAdd.Clear();
        }

        #endregion

        #endregion

        #region Static Interface

        public static TweenHash Tween(object targ)
        {
            return new TweenHash(targ);
        }

        public static ITweenHash Tween(TweenerUpdateCallback callback, float dur)
        {
            return new CallbackTweener(callback, dur);
        }

        public static Tweener Curve(object targ, Curve curve)
        {
            var tween = new ObjectTweener(targ, curve);
            tween.Play();
            return tween;
        }

        public static Tweener To(object targ, string propName, object end, float dur)
        {
            var tween = new ObjectTweener(targ, MemberCurve.CreateTo(targ, propName, EaseMethods.LinearEaseNone, end, dur));
            tween.Play();
            return tween;
        }

        public static Tweener To(object targ, string propName, Ease ease, object end, float dur)
        {
            var tween = new ObjectTweener(targ, MemberCurve.CreateTo(targ, propName, ease, end, dur));
            tween.Play();
            return tween;
        }

        public static Tweener From(object targ, string propName, object start, float dur)
        {
            var tween = new ObjectTweener(targ, MemberCurve.CreateFrom(targ, propName, EaseMethods.LinearEaseNone, start, dur));
            tween.Play();
            return tween;
        }

        public static Tweener From(object targ, string propName, Ease ease, object start, float dur)
        {
            var tween = new ObjectTweener(targ, MemberCurve.CreateFrom(targ, propName, ease, start, dur));
            tween.Play();
            return tween;
        }

        public static Tweener By(object targ, string propName, object amt, float dur)
        {
            var tween = new ObjectTweener(targ, MemberCurve.CreateBy(targ, propName, EaseMethods.LinearEaseNone, amt, dur));
            tween.Play();
            return tween;
        }

        public static Tweener By(object targ, string propName, Ease ease, object amt, float dur)
        {
            var tween = new ObjectTweener(targ, MemberCurve.CreateBy(targ, propName, ease, amt, dur));
            tween.Play();
            return tween;
        }

        public static Tweener FromTo(object targ, string propName, object start, object end, float dur)
        {
            var tween = new ObjectTweener(targ, MemberCurve.CreateFromTo(targ, propName, EaseMethods.LinearEaseNone, start, end, dur));
            tween.Play();
            return tween;
        }

        public static Tweener FromTo(object targ, string propName, Ease ease, object start, object end, float dur)
        {
            var tween = new ObjectTweener(targ, MemberCurve.CreateFromTo(targ, propName, ease, start, end, dur));
            tween.Play();
            return tween;
        }

        #endregion

    }
}
