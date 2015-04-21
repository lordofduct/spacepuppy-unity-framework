using System;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    [CustomImplicitCurve(typeof(IScalableTimeSupplier), "Scale")]
    public class TimeScaleCurve : ImplicitCurve
    {

        public const string DEFAULT_TIMESCALE_ID = "SPTween.TimeScale";

        #region Fields

        private string _id;
        private float _start;
        private float _end;

        #endregion

        #region CONSTRUCTOR

        protected TimeScaleCurve()
        {
            //required for reflection
        }

        public TimeScaleCurve(float dur, float start, float end, string id) 
            : base(dur)
        {
            _id = id;
            _start = start;
            _end = end;
        }

        public TimeScaleCurve(Ease ease, float dur, float start, float end, string id) 
            : base(ease, dur)
        {
            _id = id;
            _start = start;
            _end = end;
        }

        #endregion


        #region ImplicitCurve Interface

        protected override void ReflectiveInit(object start, object end, object option)
        {
            _id = (option != null) ? Convert.ToString(option) : DEFAULT_TIMESCALE_ID;
            _start = ConvertUtil.ToSingle(start);
            _end = ConvertUtil.ToSingle(end);
        }

        protected override object GetCurrentValue(object targ)
        {
            var supplier = targ as IScalableTimeSupplier;
            if (supplier != null && supplier.HasScale(_id))
            {
                return supplier.GetScale(_id);
            }
            return 1f;
        }

        protected override void UpdateValue(object targ, float dt, float t, float easedT)
        {
            var supplier = targ as IScalableTimeSupplier;
            if(supplier != null)
            {
                var value = (_end - _start) * easedT + _start;
                supplier.SetScale(_id, value);
            }
        }

        #endregion

    }
}
