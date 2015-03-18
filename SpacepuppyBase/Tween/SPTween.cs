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
        public static SPTween Instance
        {
            get
            {
                if (_instance == null) _instance = Singleton.CreateSpecialInstance<SPTween>(SPECIAL_NAME);
                return _instance;
            }
        }

        #endregion

        #region Instance Interface

        #region Fields

        private List<Tweener> _runningTweens = new List<Tweener>();
        private List<Tweener> _toAdd = new List<Tweener>();
        private List<Tweener> _toRemove = new List<Tweener>();
        private bool _inUpdate;

        #endregion

        #region Methods

        internal void AddReference(Tweener tween)
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

        internal void RemoveReference(Tweener tween)
        {
            if (_inUpdate)
            {
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
            this.DoUpdate(TweenUpdateType.Update);

            if(Application.isEditor)
            {
                if (this.name.StartsWith(SPECIAL_NAME)) this.name = SPECIAL_NAME + " [ActiveTweens: " + _runningTweens.Count.ToString() + "]";
            }
        }

        private void FixedUpdate()
        {
            this.DoUpdate(TweenUpdateType.FixedUpdate);
        }

        private void LateUpdate()
        {
            this.DoUpdate(TweenUpdateType.LateUpdate);
        }

        private void DoUpdate(TweenUpdateType updateType)
        {
            _inUpdate = true;

            float dt;
            Tweener twn;
            for (int i = 0; i < _runningTweens.Count; i++)
            {
                twn = _runningTweens[i];
                if (twn.UpdateType != updateType) continue;

                switch (twn.DeltaType)
                {
                    case TweenDeltaType.Real:
                        dt = GameTime.RealDeltaTime;
                        break;
                    case TweenDeltaType.Smooth:
                        dt = GameTime.SmoothDeltaTime;
                        break;
                    default:
                        dt = GameTime.DeltaTime;
                        break;
                }
                twn.Update(dt);
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

        public static PropertyHash Props()
        {
            return new PropertyHash();
        }


        public static Tweener Tween(object targ, PropertyHash properties)
        {
            var tween = new ObjectTweener(targ);
            properties.Apply(tween);
            tween.Start();
            return tween;
        }

        public static Tweener Curve(object targ, string propName, ICurve curve)
        {
            var tween = new ObjectTweener(targ);
            tween.Curves.Add(propName, curve);
            tween.Start();
            return tween;
        }

        public static Tweener To(object targ, string propName, Ease ease, object end, float dur)
        {
            var tween = new ObjectTweener(targ);
            SPTween.Props().To(propName, ease, end, dur).Apply(tween);
            tween.Start();
            return tween;
        }

        public static Tweener From(object targ, string propName, Ease ease, object start, float dur)
        {
            var tween = new ObjectTweener(targ);
            SPTween.Props().From(propName, ease, start, dur).Apply(tween);
            tween.Start();
            return tween;
        }

        public static Tweener FromTo(object targ, string propName, Ease ease, object start, object end, float dur)
        {
            var tween = new ObjectTweener(targ);
            SPTween.Props().FromTo(propName, ease, start, end, dur).Apply(tween);
            tween.Start();
            return tween;
        }

        #endregion

    }
}
