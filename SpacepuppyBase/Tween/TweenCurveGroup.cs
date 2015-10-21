using System.Collections.Generic;

namespace com.spacepuppy.Tween
{

    public class TweenCurveGroup : TweenCurve
    {

        #region Fields

        private CurveGroupCollection _curves;

        #endregion

        #region CONSTRUCTOR

        public TweenCurveGroup()
        {
            _curves = new CurveGroupCollection(this);
        }

        #endregion

        #region Properties

        public ICollection<TweenCurve> Curves { get { return _curves; } }

        #endregion

        #region Methods

        #endregion

        #region Curve Interface

        protected internal override void Init(Tweener twn)
        {
            base.Init(twn);

            for (int i = 0; i < _curves._lst.Count; i++)
            {
                _curves._lst[i].Init(twn);
            }
        }

        public override float TotalTime
        {
            get
            {
                float dur = 0f;
                float d;
                for (int i = 0; i < _curves._lst.Count; i++)
                {
                    d = _curves._lst[i].TotalTime;
                    if (d > dur) dur = d;
                }
                return dur;
            }
        }

        protected internal override void Update(object targ, float dt, float t)
        {
            for (int i = 0; i < _curves._lst.Count; i++)
            {
                _curves._lst[i].Update(targ, dt, t);
            }
        }

        #endregion

        #region Special Types

        private class CurveGroupCollection : ICollection<TweenCurve>
        {

            #region Fields

            private TweenCurveGroup _owner;
            internal List<TweenCurve> _lst = new List<TweenCurve>();

            #endregion

            #region CONSTRUCTOR

            internal CurveGroupCollection(TweenCurveGroup owner)
            {
                _owner = owner;
            }

            #endregion

            #region ICollection Interface

            public void Add(TweenCurve item)
            {
                if (_lst.Contains(item)) return;
                _lst.Add(item);
            }

            public void Clear()
            {
                _lst.Clear();
            }

            public bool Contains(TweenCurve item)
            {
                return _lst.Contains(item);
            }

            public void CopyTo(TweenCurve[] array, int arrayIndex)
            {
                _lst.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return _lst.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(TweenCurve item)
            {
                return _lst.Remove(item);
            }

            public IEnumerator<TweenCurve> GetEnumerator()
            {
                return _lst.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _lst.GetEnumerator();
            }

            #endregion

        }

        #endregion

    }

}
