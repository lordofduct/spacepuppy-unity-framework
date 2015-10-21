using System.Collections.Generic;

namespace com.spacepuppy.Tween
{

    public class TweenSequence : Tweener
    {

        #region Fields

        private object _id;
        private TweenSequenceCollection _sequence;
        private Tweener _current;
        private int _currentIndex = -1;
        private float _nextTweenTime;

        #endregion
        
        #region CONSTRUCTOR

        public TweenSequence()
        {
            _sequence = new TweenSequenceCollection(this);
        }

        #endregion

        #region Properties

        public override object Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public IList<Tweener> Tweens { get { return _sequence; } }

        #endregion

        #region Methods

        #endregion

        #region Tweener Interface

        protected internal override float GetPlayHeadLength()
        {
            throw new System.NotImplementedException();
        }

        public override void Play(float playHeadPosition)
        {
            base.Play(playHeadPosition);


            float t = 0f;
            float nt = 0f;
            Tweener twn = null;
            int twnIndex = -1;
            for (int i = 0; i < _sequence.Count; i++)
            {
                nt = t + _sequence[i].TotalTime;
                if (playHeadPosition < nt)
                {
                    twnIndex = i;
                    twn = _sequence[i];
                    break;
                }
                else
                {
                    t = nt;
                }
            }

            if (_current != null && _current != twn)
            {
                _current.Stop();
                _current = null;
            }

            if (twn != null)
            {
                _current = twn;
                _currentIndex = twnIndex;
                _nextTweenTime = nt;
                _current.Play(playHeadPosition - t);
            }
            else
            {
                _nextTweenTime = float.PositiveInfinity;
                _currentIndex = -1;
            }

            //for (int i = 0; i < _sequence.Count; i++)
            //{
            //    _sequence[i].SetIsInPlayingGroupOrSequence(true);
            //}
        }

        public override void Stop()
        {
            base.Stop();

            //for (int i = 0; i < _sequence.Count; i++)
            //{
            //    _sequence[i].SetIsInPlayingGroupOrSequence(false);
            //}
        }

        protected internal override void DoUpdate(float dt, float t)
        {
            if (_currentIndex < 0) return;

            if (t > _nextTweenTime)
            {
                if (_current != null) _current.Stop(); //make sure that the current is stopped
                if (_currentIndex == _sequence.Count - 1) return;
                _currentIndex++;
                _current = _sequence[_currentIndex];
                _current.Play(t - _nextTweenTime);
                _nextTweenTime += _current.TotalTime;
            }
        }

        //internal override void SetIsInPlayingGroupOrSequence(bool value)
        //{
        //    base.SetIsInPlayingGroupOrSequence(value);

        //    for (int i = 0; i < _sequence.Count; i++)
        //    {
        //        _sequence[i].SetIsInPlayingGroupOrSequence(value);
        //    }
        //}

        #endregion

        #region Special Types

        private class TweenSequenceCollection : IList<Tweener>
        {
            private TweenSequence _owner;
            private List<Tweener> _lst = new List<Tweener>();

            internal TweenSequenceCollection(TweenSequence owner)
            {
                _owner = owner;
            }

            #region IList Interface

            public int IndexOf(Tweener item)
            {
                return _lst.IndexOf(item);
            }

            public void Insert(int index, Tweener item)
            {
                _lst.Insert(index, item);
            }

            public void RemoveAt(int index)
            {
                _lst.RemoveAt(index);
            }

            public Tweener this[int index]
            {
                get
                {
                    return _lst[index];
                }
                set
                {
                    _lst[index] = value;
                }
            }

            public void Add(Tweener item)
            {
                _lst.Add(item);
            }

            public void Clear()
            {
                _lst.Clear();
            }

            public bool Contains(Tweener item)
            {
                return _lst.Contains(item);
            }

            public void CopyTo(Tweener[] array, int arrayIndex)
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

            public bool Remove(Tweener item)
            {
                return _lst.Remove(item);
            }

            public IEnumerator<Tweener> GetEnumerator()
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
