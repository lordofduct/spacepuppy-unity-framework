using System;
using System.Collections;
using System.Collections.Generic;

namespace com.spacepuppy.Tween
{

    public class TweenSequence : Tweener
    {

        #region Fields

        private object _id;
        private TweenSequenceCollection _sequence;

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

        public TweenSequenceCollection Tweens { get { return _sequence; } }

        #endregion
        
        #region Tweener Interface

        protected internal override float GetPlayHeadLength()
        {
            float dur = 0f;
            var e = _sequence.GetEnumerator();
            while(e.MoveNext())
            {
                dur += e.Current.GetPlayHeadLength();
            }
            return dur;
        }

        protected internal override void DoUpdate(float dt, float t)
        {

            float _lastT = 0f;
            float totalTime = 0f;
            var e = _sequence.GetEnumerator();
            while(e.MoveNext())
            {
                totalTime = e.Current.TotalTime;
                if (t < _lastT + totalTime)
                {
                    e.Current.DoUpdate(dt, t - _lastT);
                    break;
                }
                else
                {
                    _lastT += totalTime;
                }
            }

        }

        #endregion

        #region Special Types

        public class TweenSequenceCollection : IList<Tweener>
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
                if (item == null) throw new System.ArgumentNullException("item");


                _lst.Insert(index, item);
            }

            public void RemoveAt(int index)
            {
                if (_owner.IsPlaying) throw new System.InvalidOperationException("Cannot modify TweenSequence while it is playing.");

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
                    if (_lst[index] == value) return;

                    if (_owner.IsPlaying) throw new System.InvalidOperationException("Cannot modify TweenSequence while it is playing.");
                    _lst[index] = value;
                }
            }

            public void Add(Tweener item)
            {
                if (_owner.IsPlaying) throw new System.InvalidOperationException("Cannot modify TweenSequence while it is playing.");

                _lst.Add(item);
            }

            public void Clear()
            {
                if (_owner.IsPlaying) throw new System.InvalidOperationException("Cannot modify TweenSequence while it is playing.");

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
                get { return _owner.IsPlaying; }
            }

            public bool Remove(Tweener item)
            {
                if (_owner.IsPlaying) throw new System.InvalidOperationException("Cannot modify TweenSequence while it is playing.");

                return _lst.Remove(item);
            }

            #endregion

            #region IEnumerable Interface

            public Enumerator GetEnumerator()
            {
                return new Enumerator(this);
            }

            IEnumerator<Tweener> IEnumerable<Tweener>.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion

            #region Special Types

            public struct Enumerator : IEnumerator<Tweener>
            {

                #region Fields

                private List<Tweener>.Enumerator _e;

                #endregion

                #region CONSTRUCTOR

                public Enumerator(TweenSequenceCollection coll)
                {
                    if (coll == null) throw new System.ArgumentNullException("coll");
                    _e = coll._lst.GetEnumerator();
                }

                #endregion

                #region IEnumerator Interface

                public Tweener Current
                {
                    get
                    {
                        return _e.Current;
                    }
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return _e.Current;
                    }
                }

                public void Dispose()
                {
                    _e.Dispose();
                }

                public bool MoveNext()
                {
                    return _e.MoveNext();
                }

                void IEnumerator.Reset()
                {
                    (_e as IEnumerator).Reset();
                }

                #endregion

            }

            #endregion

        }

        #endregion

    }

}
