using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Collections
{
    public class CooldownPool<T> : IEnumerable<CooldownPool<T>.CooldownInfo> where T : class
    {

        #region Fields

        private List<CooldownInfo> _lst = new List<CooldownInfo>();
        private ITimeSupplier _time;

        #endregion

        #region CONSTRUCTOR

        public CooldownPool()
        {
        }

        #endregion

        #region Properties

        public int Count { get { return _lst.Count; } }

        public ITimeSupplier UpdateTimeSupplier
        {
            get
            {
                if (_time == null) _time = SPTime.Normal;
                return _time;
            }
            set
            {
                if (value == null) throw new System.ArgumentNullException("value");
                _time = value;
            }
        }

        #endregion

        #region Methods

        public void Add(T obj, float duration)
        {
            for(int i = 0; i < _lst.Count; i++)
            {
                if(_lst[i].Object == obj)
                {
                    _lst[i].Duration += duration;
                    return;
                }
            }

            var info = new CooldownInfo(obj, this.UpdateTimeSupplier.Total, duration);
            _lst.Add(info);
        }

        public bool Contains(T obj)
        {
            for(int i = 0; i < _lst.Count; i++)
            {
                if (_lst[i].Object == obj) return true;
            }
            return false;
        }

        public void Update()
        {
            var t = this.UpdateTimeSupplier.Total;
            CooldownInfo info;
            for(int i = 0; i < _lst.Count; i++)
            {
                info = _lst[i];
                if(info.Object == null || t - info.StartTime > info.Duration)
                {
                    _lst.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Clear()
        {
            _lst.Clear();
        }

        #endregion

        #region IEnumerable Interface
        public IEnumerator<CooldownPool<T>.CooldownInfo> GetEnumerator()
        {
            return _lst.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _lst.GetEnumerator();
        }

        #endregion

        #region Special Types

        public class CooldownInfo
        {
            private T _obj;
            private float _startTime;
            private float _dur;

            public CooldownInfo(T obj, float startTime, float dur)
            {
                _obj = obj;
                _startTime = startTime;
                _dur = dur;
            }

            public T Object { get { return _obj; } }
            public float StartTime
            {
                get { return _startTime; }
                set { _startTime = value; }
            }
            public float Duration
            {
                get { return _dur; }
                internal set { _dur = value; }
            }

        }

        #endregion

    }
}
