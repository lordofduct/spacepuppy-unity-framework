using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Geom
{
    public interface ICurve
    {

        float GetPosition(float t);
        float Ease(float current, float initialValue, float totalChange, float duration);

    }

    [System.Serializable()]
    public class CubicBezierCurve : ICurve
    {

        #region Fields

        [SerializeField()]
        private float _p0;
        [SerializeField()]
        private float _p1;
        [SerializeField()]
        private float _p2;
        [SerializeField()]
        private float _p3;

        #endregion

        #region CONSTRUCTOR

        public CubicBezierCurve()
        {
            _p0 = 0f;
            _p1 = 0.5f;
            _p2 = 0.5f;
            _p3 = 1f;
        }

        public CubicBezierCurve(float p0, float p1, float p2, float p3)
        {
            _p0 = p0;
            _p1 = p1;
            _p2 = p2;
            _p3 = p3;
        }

        #endregion

        #region Properties

        public float P0
        {
            get { return _p0; }
            set { _p0 = value; }
        }

        public float P1
        {
            get { return _p1; }
            set { _p1 = value; }
        }

        public float P2
        {
            get { return _p2; }
            set { _p2 = value; }
        }

        public float P3
        {
            get { return _p3; }
            set { _p3 = value; }
        }

        #endregion

        #region Methods

        public void Set(float p0, float p1, float p2, float p3)
        {
            _p0 = p0;
            _p1 = p1;
            _p2 = p2;
            _p3 = p3;
        }

        #endregion

        #region ICurve Interface

        public float GetPosition(float t)
        {
            var it = 1f - t;
            return (it * it * it * _p0)
                 + (3f * it * it * t * _p1)
                 + (3f * it * t * t * _p2)
                 + (t * t * t * _p3);
        }

        public float Ease(float c, float s, float e, float d)
        {
            var t = c / d;
            var r = this.GetPosition(t);
            return s + e * r;
        }

        #endregion


    }
}
