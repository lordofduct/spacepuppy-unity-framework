using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy
{
    public interface IUpdateable
    {

        void Update();

    }

    public class UpdatePump
    {

        #region Fields

        private HashSet<IUpdateable> _set = new HashSet<IUpdateable>();
        private HashSet<IUpdateable> _toAdd = new HashSet<IUpdateable>();
        private HashSet<IUpdateable> _toRemove = new HashSet<IUpdateable>();
        private bool _inUpdate;

        #endregion

        #region Methods

        public bool Contains(IUpdateable obj)
        {
            return _set.Contains(obj);
        }

        public void Add(IUpdateable obj)
        {
            if (_inUpdate)
            {
                _toAdd.Add(obj);
            }
            else
            {
                _set.Add(obj);
            }
        }

        public void Remove(IUpdateable obj)
        {
            if (_inUpdate)
            {
                if (_set.Contains(obj))
                    _toRemove.Add(obj);
            }
            else
            {
                _set.Remove(obj);
            }
        }

        public void Update()
        {
            _inUpdate = true;
            var e = _set.GetEnumerator();
            while(e.MoveNext())
            {
                e.Current.Update();
            }
            _inUpdate = false;

            if (_toAdd.Count > 0)
            {
                e = _toAdd.GetEnumerator();
                while (e.MoveNext())
                {
                    _set.Add(e.Current);
                }
            }

            if (_toRemove.Count > 0)
            {
                e = _toRemove.GetEnumerator();
                while (e.MoveNext())
                {
                    _set.Remove(e.Current);
                }
                _toRemove.Clear();
            }
        }

        #endregion

    }

}
