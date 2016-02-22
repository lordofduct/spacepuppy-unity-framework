using System.Collections.Generic;

namespace com.spacepuppy.Collections
{

    /// <summary>
    /// Used to store temporary references for a duration of time. Call Update to update the pool 
    /// releasing objects that are old enough. Always call Update on the unity main thread!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CooldownPool<T> : IEnumerable<CooldownPool<T>.CooldownInfo> where T : class
    {

        #region Fields

        private Dictionary<T, CooldownInfo> _table = new Dictionary<T, CooldownInfo>();
        private ITimeSupplier _time;

        #endregion

        #region CONSTRUCTOR

        public CooldownPool()
        {
        }

        #endregion

        #region Properties

        public int Count { get { return _table.Count; } }

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
            CooldownInfo info;
            if(_table.TryGetValue(obj, out info))
            {
                info.Duration += duration;
            }
            else
            {
                _table[obj] = new CooldownInfo(obj, this.UpdateTimeSupplier.Total, duration);
            }
        }

        public bool Contains(T obj)
        {
            return _table.ContainsKey(obj);
        }

        public void Update()
        {
            var t = this.UpdateTimeSupplier.Total;
            CooldownInfo info;

            using (var toRemove = TempCollection.GetList<T>())
            {
                var e1 = _table.GetEnumerator();
                while (e1.MoveNext())
                {
                    info = e1.Current.Value;
                    if (info.Object == null || t - info.StartTime > info.Duration)
                    {
                        toRemove.Add(info.Object);
                    }
                }

                if (toRemove.Count > 0)
                {
                    var e2 = toRemove.GetEnumerator();
                    while (e2.MoveNext())
                    {
                        _table.Remove(e2.Current);
                    }
                }
            }
        }

        public void Clear()
        {
            _table.Clear();
        }

        #endregion

        #region IEnumerable Interface

        //TODO - implement propert Enumerator, remember dict.Values allocates mem in mono... ugh

        public IEnumerator<CooldownPool<T>.CooldownInfo> GetEnumerator()
        {
            return _table.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _table.Values.GetEnumerator();
        }

        #endregion

        #region Special Types

        public struct CooldownInfo
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
                internal set { _startTime = value; }
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
