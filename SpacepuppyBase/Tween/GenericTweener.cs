using UnityEngine;

using com.spacepuppy;

namespace com.spacepuppy.Tween
{
    public class GenericTweener : Tweener
    {

        #region Fields

        private System.Action<Tweener, float, float, object> _callback;
        private object _token;
        private float _dur;

        #endregion

        #region CONSTRUCTOR

        public GenericTweener(System.Action<Tweener, float, float, object> callback, float dur)
        {
            _callback = callback;
            _dur = dur;
            _token = null;
        }

        public GenericTweener(System.Action<Tweener, float, float, object> callback, float dur, object token)
        {
            _callback = callback;
            _dur = dur;
            _token = token;
        }

        #endregion

        #region Methods

        public override object Target
        {
            get { return _callback; }
        }

        public override float TotalDuration
        {
            get { return _dur; }
        }

        protected override void DoUpdate(float dt, float time)
        {
            if (_callback == null) return;
            _callback(this, dt, time, _token);
        }

        #endregion

    }
}
