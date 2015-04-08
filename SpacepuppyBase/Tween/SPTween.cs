using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

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

        private static BiDictionary<object, IAutoKillableTweener> _autoKillDict = new BiDictionary<object, IAutoKillableTweener>();

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
                if(tween is IAutoKillableTweener)
                {
                    var auto = tween as IAutoKillableTweener;
                    var targ = auto.Token;
                    IAutoKillableTweener old;
                    if(_autoKillDict.TryGetValue(targ, out old))
                    {
                        old.Kill();
                    }
                }
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
            if (_inUpdate)
            {
                if (!_runningTweens.Contains(tween)) return;
                if (_toRemove.Contains(tween)) return;
                _toRemove.Add(tween);
            }
            else
            {
                _runningTweens.Remove(tween);
                if(tween is IAutoKillableTweener && tween.IsComplete)
                {
                    var auto = tween as IAutoKillableTweener;
                    if(_autoKillDict.Reverse.ContainsKey(auto))
                    {
                        _autoKillDict.Reverse.Remove(auto);
                    }
                }
            }
        }

        /// <summary>
        /// Flag a Tweener that implements IAutoKillableTweener to be auto killed if another tween targeting the same object is played. 
        /// Until the Tweener is either killed, or finished, it will be eligible for being automatically 
        /// killed if another Tweener starts playing that tweens the same target object. Note that other tweener 
        /// must implement IAutoKillableTweener as well (though doesn't have to be flagged to AutoKill).
        /// </summary>
        /// <param name="tween"></param>
        public static void AutoKill(Tweener tween)
        {
            if (tween == null || !(tween is IAutoKillableTweener)) return;
            if (GameLoopEntry.ApplicationClosing) return;
            if (_instance == null) _instance = Singleton.CreateSpecialInstance<SPTween>(SPECIAL_NAME);
            _instance.AutoKill_Imp(tween as IAutoKillableTweener);
        }
        private void AutoKill_Imp(IAutoKillableTweener tween)
        {
            var targ = tween.Token;
            IAutoKillableTweener old;
            if(_autoKillDict.TryGetValue(targ, out old))
            {
                old.Kill();
            }
            _autoKillDict[targ] = tween;
        }


        public static void KillAll(object id)
        {
            if (GameLoopEntry.ApplicationClosing) return;
            if (_instance == null) return;

            var arr = (from t in _instance._runningTweens where t.Id == id select t).ToArray();
            foreach (var t in arr)
            {
                t.Kill();
            }
        }
        public static void KillAll()
        {
            if (GameLoopEntry.ApplicationClosing) return;
            if (_instance == null) return;

            var arr = _instance._runningTweens.ToArray();
            foreach(var t in arr)
            {
                t.Kill();
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
                this.RemoveReference_Imp(_toRemove[i]);
            }
            _toRemove.Clear();
            for(int i = 0; i < _toAdd.Count; i++)
            {
                this.AddReference_Imp(_toAdd[i]);
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

        public static Tweener PlayCurve(object targ, Curve curve)
        {
            var tween = new ObjectTweener(targ, curve);
            tween.Play();
            return tween;
        }

        public static Tweener PlayTo(object targ, string propName, object end, float dur, object option = null)
        {
            var tween = new ObjectTweener(targ, MemberCurve.CreateTo(targ, propName, EaseMethods.LinearEaseNone, end, dur, option));
            tween.Play();
            return tween;
        }

        public static Tweener PlayTo(object targ, string propName, Ease ease, object end, float dur, object option = null)
        {
            var tween = new ObjectTweener(targ, MemberCurve.CreateTo(targ, propName, ease, end, dur, option));
            tween.Play();
            return tween;
        }

        public static Tweener PlayFrom(object targ, string propName, object start, float dur, object option = null)
        {
            var tween = new ObjectTweener(targ, MemberCurve.CreateFrom(targ, propName, EaseMethods.LinearEaseNone, start, dur, option));
            tween.Play();
            return tween;
        }

        public static Tweener PlayFrom(object targ, string propName, Ease ease, object start, float dur, object option = null)
        {
            var tween = new ObjectTweener(targ, MemberCurve.CreateFrom(targ, propName, ease, start, dur, option));
            tween.Play();
            return tween;
        }

        public static Tweener PlayBy(object targ, string propName, object amt, float dur, object option = null)
        {
            var tween = new ObjectTweener(targ, MemberCurve.CreateBy(targ, propName, EaseMethods.LinearEaseNone, amt, dur, option));
            tween.Play();
            return tween;
        }

        public static Tweener PlayBy(object targ, string propName, Ease ease, object amt, float dur, object option = null)
        {
            var tween = new ObjectTweener(targ, MemberCurve.CreateBy(targ, propName, ease, amt, dur, option));
            tween.Play();
            return tween;
        }

        public static Tweener PlayFromTo(object targ, string propName, object start, object end, float dur, object option = null)
        {
            var tween = new ObjectTweener(targ, MemberCurve.CreateFromTo(targ, propName, EaseMethods.LinearEaseNone, start, end, dur, option));
            tween.Play();
            return tween;
        }

        public static Tweener PlayFromTo(object targ, string propName, Ease ease, object start, object end, float dur, object option = null)
        {
            var tween = new ObjectTweener(targ, MemberCurve.CreateFromTo(targ, propName, ease, start, end, dur, option));
            tween.Play();
            return tween;
        }

        #endregion

    }
}
