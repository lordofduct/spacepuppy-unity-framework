using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.spacepuppy.Tween.Curves;
using com.spacepuppy.Utils;
using com.spacepuppy.Utils.FastDynamicMemberAccessor;

namespace com.spacepuppy.Tween
{
    public class ObjectTweener : Tweener
    {

        #region Fields

        private object _target;
        private CurveCollection _curves;
        private float _totalDur = float.NaN;

        #endregion

        #region CONSTRUCTOR

        public ObjectTweener(object targ)
        {
            _target = targ;
            _curves = new CurveCollection(this);
            this.OnPlay += this.OnPlayHandler;
        }

        #endregion

        #region Properties

        public CurveCollection Curves { get { return _curves; } }

        #endregion

        #region Methods

        private float CalcTotalDur()
        {
            float l = 0f;
            var lst = _curves._lst;
            var cnt = lst.Count;

            for (int i = 0; i < cnt; i++)
            {
                if (lst[i].TotalDuration > l) l = lst[i].TotalDuration;
            }
            return l;
        }

        private void OnPlayHandler(object sender, System.EventArgs e)
        {
            _totalDur = this.CalcTotalDur();
        }

        #endregion

        #region Tweener Interface

        public override object Target
        {
            get { return _target; }
        }

        public override float TotalDuration
        {
            get
            {
                if(float.IsNaN(_totalDur))
                {
                    return this.CalcTotalDur();
                }
                else
                {
                    return _totalDur;
                }
            }
        }

        public override void Stop()
        {
            base.Stop();

            _totalDur = float.NaN;
        }

        protected override void DoUpdate(float dt, float time)
        {
            if(_target is Transform)
            {
                var trans = _target as Transform;
            }

            var lst = _curves._lst;
            var cnt = lst.Count;
            for(int i = 0; i < cnt; i++)
            {
                lst[i].Update(_target, dt, time);
            }
        }

        #endregion

        #region Special Types

        public class CurveCollection : ICollection<ICurve>
        {

            #region Fields

            private ObjectTweener _owner;
            internal List<ICurve> _lst = new List<ICurve>();

            #endregion

            #region CONSTRUCTOR

            public CurveCollection(ObjectTweener owner)
            {
                _owner = owner;
            }

            #endregion

            #region Methods

            public void Add(string propName, ICurve curve)
            {
                if (_owner.IsRunning) throw new System.NotSupportedException("Running Tweener cannot have its curve collection modified.");
                if (_lst.Contains(curve)) _lst.Remove(curve);

                if (curve is MemberCurve) MemberCurve.Init(curve as MemberCurve, _owner._target.GetType(), propName);
                _lst.Add(curve);
            }

            public ICurve AddTo(string propName, Ease ease, object end, float dur)
            {
                if (_owner.IsRunning) throw new System.NotSupportedException("Running Tweener cannot have its curve collection modified.");

                var memberInfo = MemberAccessorPool.GetMember(_owner._target.GetType(), propName);

                object start = MemberAccessorPool.Get(memberInfo).Get(_owner._target);

                var curve = MemberCurve.Create(memberInfo, ease, dur, start, end);
                _lst.Add(curve);
                return curve;
            }

            public ICurve AddFrom(string propName, Ease ease, object start, float dur)
            {
                if (_owner.IsRunning) throw new System.NotSupportedException("Running Tweener cannot have its curve collection modified.");

                var memberInfo = MemberAccessorPool.GetMember(_owner._target.GetType(), propName);

                object end = MemberAccessorPool.Get(memberInfo).Get(_owner._target);

                var curve = MemberCurve.Create(memberInfo, ease, dur, start, end);
                _lst.Add(curve);
                return curve;
            }

            public ICurve AddBy(string propName, Ease ease, object amt, float dur)
            {
                if (_owner.IsRunning) throw new System.NotSupportedException("Running Tweener cannot have its curve collection modified.");

                var memberInfo = MemberAccessorPool.GetMember(_owner._target.GetType(), propName);

                object start = MemberAccessorPool.Get(memberInfo).Get(_owner._target);
                object end = CurveCollection.TrySum(memberInfo, start, amt);

                var curve = MemberCurve.Create(memberInfo, ease, dur, start, end);
                _lst.Add(curve);
                return curve;
            }

            public ICurve AddFromTo(string propName, Ease ease, object start, object end, float dur)
            {
                if (_owner.IsRunning) throw new System.NotSupportedException("Running Tweener cannot have its curve collection modified.");

                var curve = MemberCurve.Create(_owner._target, propName, ease, dur, start, end);
                _lst.Add(curve);
                return curve;
            }

            #endregion

            #region ICollection Interface

            void ICollection<ICurve>.Add(ICurve item)
            {
                if (_owner.IsRunning) throw new System.NotSupportedException("Running Tweener cannot have its curve collection modified.");
                if (!_lst.Contains(item)) _lst.Add(item);
            }

            public void Clear()
            {
                if (_owner.IsRunning) throw new System.NotSupportedException("Running Tweener cannot have its curve collection modified.");
                _lst.Clear();
            }

            public bool Contains(ICurve item)
            {
                return _lst.Contains(item);
            }

            public void CopyTo(ICurve[] array, int arrayIndex)
            {
                _lst.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return _lst.Count; }
            }

            public bool IsReadOnly
            {
                get { return _owner.IsRunning; }
            }

            public bool Remove(ICurve item)
            {
                if (_owner.IsRunning) throw new System.NotSupportedException("Running Tweener cannot have its curve collection modified.");
                return _lst.Remove(item);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _lst.GetEnumerator();
            }

            public IEnumerator<ICurve> GetEnumerator()
            {
                return _lst.GetEnumerator();
            }

            #endregion



            private static object TrySum(System.Reflection.MemberInfo info, object a, object b)
            {
                System.Type tp;
                if (info is System.Reflection.FieldInfo)
                    tp = (info as System.Reflection.FieldInfo).FieldType;
                else if (info is System.Reflection.PropertyInfo)
                    tp = (info as System.Reflection.PropertyInfo).PropertyType;
                else
                    return b;

                if (ConvertUtil.IsNumericType(tp))
                {
                    return ConvertUtil.ToPrim(ConvertUtil.ToDouble(a) + ConvertUtil.ToDouble(b), tp);
                }
                else if (tp == typeof(Vector2))
                {
                    return ConvertUtil.ToVector2(a) + ConvertUtil.ToVector2(b);
                }
                else if (tp == typeof(Vector3))
                {
                    return ConvertUtil.ToVector3(a) + ConvertUtil.ToVector3(b);
                }
                else if (tp == typeof(Vector4))
                {
                    return ConvertUtil.ToVector4(a) + ConvertUtil.ToVector4(b);
                }
                else if (tp == typeof(Quaternion))
                {
                    return ConvertUtil.ToQuaternion(a) * ConvertUtil.ToQuaternion(b);
                }
                else if (tp == typeof(Color))
                {
                    return ConvertUtil.ToColor(a) + ConvertUtil.ToColor(b);
                }
                else if (tp == typeof(Color32))
                {
                    return ConvertUtil.ToColor32(ConvertUtil.ToColor(a) + ConvertUtil.ToColor(b));
                }

                return b;
            }

        }

        #endregion

    }
}
