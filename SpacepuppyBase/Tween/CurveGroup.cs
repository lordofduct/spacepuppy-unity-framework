using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Tween
{

    public class CurveGroup : Curve
    {

        #region Fields

        private CurveGroupCollection _curves;

        #endregion

        #region CONSTRUCTOR

        public CurveGroup()
        {
            _curves = new CurveGroupCollection(this);
        }

        #endregion

        #region Properties

        public ICollection<Curve> Curves { get { return _curves; } }

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

        private class CurveGroupCollection : ICollection<Curve>
        {

            #region Fields

            private CurveGroup _owner;
            internal List<Curve> _lst = new List<Curve>();

            #endregion

            #region CONSTRUCTOR

            internal CurveGroupCollection(CurveGroup owner)
            {
                _owner = owner;
            }

            #endregion

            #region ICollection Interface

            public void Add(Curve item)
            {
                if (_lst.Contains(item)) return;
                _lst.Add(item);
            }

            public void Clear()
            {
                _lst.Clear();
            }

            public bool Contains(Curve item)
            {
                return _lst.Contains(item);
            }

            public void CopyTo(Curve[] array, int arrayIndex)
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

            public bool Remove(Curve item)
            {
                return _lst.Remove(item);
            }

            public IEnumerator<Curve> GetEnumerator()
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
