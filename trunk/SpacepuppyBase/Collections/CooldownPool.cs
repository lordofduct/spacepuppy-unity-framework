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
        private bool _autoUpdate;
        private bool _currentlyAutoUpdating;

        #endregion

        #region CONSTRUCTOR

        public CooldownPool()
        {

        }

        public CooldownPool(bool autoUpdate)
        {
            _autoUpdate = autoUpdate;
        }

        #endregion

        #region Properties

        public int Count { get { return _lst.Count; } }

        public bool AutoUpdate
        {
            get { return _autoUpdate; }
            set
            {
                if (_autoUpdate == value) return;

                _autoUpdate = value;
                if(_autoUpdate && _lst.Count > 0)
                {
                    this.StartAutoUpdate();
                }
                else
                {
                    this.StopAutoUpdate();
                }
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

            var info = new CooldownInfo(obj, duration);
            _lst.Add(info);
            if (_autoUpdate && _lst.Count > 0 && !_currentlyAutoUpdating) this.StartAutoUpdate();
        }

        public bool Contains(T obj)
        {
            for(int i = 0; i < _lst.Count; i++)
            {
                if (_lst[i].Object == obj) return true;
            }
            return false;
        }

        public void Update(float dt)
        {
            for(int i = 0; i < _lst.Count; i++)
            {
                if(_lst[i].Update(dt))
                {
                    _lst.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Clear()
        {
            _lst.Clear();
            this.StopAutoUpdate();
        }



        private void StartAutoUpdate()
        {
            if(_autoUpdate)
            {
                GameLoopEntry.EarlyUpdate -= this.OnEarlyUpdate;
                GameLoopEntry.EarlyUpdate += this.OnEarlyUpdate;
                _currentlyAutoUpdating = true;
            }
        }

        private void StopAutoUpdate()
        {
            GameLoopEntry.EarlyUpdate -= this.OnEarlyUpdate;
            _currentlyAutoUpdating = false;
        }

        private void OnEarlyUpdate(object sender, System.EventArgs e)
        {
            this.Update(GameTime.DeltaTime);
            if (_lst.Count == 0) this.StopAutoUpdate();
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
            private float _dur;
            private float _t;

            public CooldownInfo(T obj, float dur)
            {
                _obj = obj;
                _dur = dur;
                _t = 0f;
            }

            public T Object { get { return _obj; } }
            public float Duration
            {
                get { return _dur; }
                internal set { _dur = value; }
            }
            public float CurrentTime { get { return _t; } }

            internal bool Update(float dt)
            {
                _t += dt;
                return _t >= _dur;
            }
        }

        #endregion


      
    }
}
