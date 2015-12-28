using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Tween;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Cameras
{
    public abstract class CameraFade : SPComponent
    {

        #region Static Interface

        private static HashSet<CameraFade> _fades;

        static CameraFade()
        {
            _fades = new HashSet<CameraFade>(com.spacepuppy.Collections.ObjectReferenceEqualityComparer<CameraFade>.Default);
        }

        public static int ActiveFadeCount { get { return _fades.Count; } }

        public static CameraFade[] GetActiveFades()
        {
            return _fades.ToArray();
        }

        public static void FadeInActiveFades(float dur, EaseStyle ease = EaseStyle.LinearEaseIn)
        {
            foreach(var f in _fades)
            {
                f.FadeIn(dur, ease);
            }
        }

        #endregion

        #region Events

        public event System.EventHandler FadeOutComplete;
        public event System.EventHandler FadeInComplete;

        #endregion

        #region Fields

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("DestroyOnComplete")]
        private bool _destroyOnComplete;

        private ITimeSupplier _timeSupplier;

        private bool _active;
        private Tweener _tween;
        private float _totalDuration;
        private int _direction;

        #endregion

        #region Properties

        public bool DestroyOnComplete
        {
            get { return _destroyOnComplete; }
            set { _destroyOnComplete = value; }
        }
        
        public ITimeSupplier TimeSupplier
        {
            get { return _timeSupplier; }
            set { _timeSupplier = value ?? SPTime.Real; }
        }

        public bool IsActiveFade { get { return _active; } }

        #endregion

        #region Methods

        public void FadeOut(float dur, EaseStyle ease = EaseStyle.LinearEaseOut)
        {
            _active = true;
            _fades.Add(this);

            if (_tween != null)
            {
                _tween.Stop();
                _tween = null;
            }
            _totalDuration = dur;
            _direction = 1;
            if(dur <= 0f)
            {
                this.OnFadeOutComplete(null, null);
            }
            else
            {
                _tween = SPTween.Tween(this.OnTweenStep, dur)
                                .Use(_timeSupplier ?? SPTime.Real)
                                .OnFinish(this.OnFadeOutComplete)
                                .Play();
            }
        }

        public void FadeIn(float dur, EaseStyle ease = EaseStyle.LinearEaseIn, bool destroyOnComplete = true)
        {
            if (!_active) throw new System.InvalidOperationException("Cannot FadeIn a CameraFade that isnot currently faded out.");

            if (_tween != null)
            {
                _tween.Stop();
                _tween = null;
            }
            _totalDuration = dur;
            _direction = -1;

            _tween = SPTween.Tween(this.OnTweenStep, dur)
                            .Use(_timeSupplier ?? SPTime.Real)
                            .OnFinish(this.OnFadeInComplete)
                            .Play();
        }

        private void OnTweenStep(Tweener twn, float dt, float t)
        {
            if(_direction > 0)
            {
                this.UpdateFade(Mathf.Clamp01(t / _totalDuration));
            }
            else
            {
                this.UpdateFade(Mathf.Clamp01((_totalDuration - t) / _totalDuration));
            }
        }

        private void OnFadeOutComplete(object sender, System.EventArgs e)
        {
            this.UpdateFade(1.0f);
            if (this.FadeOutComplete != null) this.FadeOutComplete(this, System.EventArgs.Empty);
        }

        private void OnFadeInComplete(object sender, System.EventArgs e)
        {
            _active = false;
            _fades.Remove(this);
            if (this.FadeInComplete != null) this.FadeInComplete(this, System.EventArgs.Empty);
            if (this.DestroyOnComplete && !this.IsNullOrDestroyed()) this.gameObject.Kill();
        }

        protected abstract void UpdateFade(float percentage);

        #endregion

    }
}
