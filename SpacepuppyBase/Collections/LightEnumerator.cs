using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Collections
{

    public struct LightEnumerator : IEnumerator
    {

        private object _e;
        private object _current;
        private int _index;

        #region CONSTRUCTOR

        public LightEnumerator(IEnumerable e)
        {
            if (e is IList)
            {
                _e = e;
                _index = 0;
            }
            else
            {
                _e = e.GetEnumerator();
                _index = -1;
            }
            _current = null;
        }

        #endregion


        #region IEnumerator Interface

        public object Current
        {
            get
            {
                return _current;
            }
        }
        
        public void Dispose()
        {
            _e = null;
            _current = null;
            _index = -1;
        }

        public bool MoveNext()
        {
            if (_e == null) return false;

            if (_index < 0)
            {
                var e = _e as IEnumerator;
                if (e.MoveNext())
                {
                    _current = e.Current;
                    return true;
                }
                else
                {
                    _current = null;
                    return false;
                }
            }
            else
            {
                var lst = _e as IList;
                if (_index < lst.Count)
                {
                    _current = lst[_index];
                    _index++;
                    return true;
                }
                else
                {
                    _current = null;
                    return false;
                }
            }
        }

        void IEnumerator.Reset()
        {
            throw new NotImplementedException();
        }

        #endregion



        public static LightEnumerator Create(IEnumerable e)
        {
            return new LightEnumerator(e);
        }

        public static LightEnumerator<T> Create<T>(IEnumerable<T> e)
        {
            return new LightEnumerator<T>(e);
        }

    }

    public struct LightEnumerator<T> : IEnumerator<T>
    {

        private object _e;
        private T _current;
        private int _index;

        #region CONSTRUCTOR

        public LightEnumerator(IEnumerable<T> e)
        {
            if(e is IList<T>)
            {
                _e = e;
                _index = 0;
            }
            else
            {
                _e = e.GetEnumerator();
                _index = -1;
            }
            _current = default(T);
        }

        #endregion


        #region IEnumerator Interface

        public T Current
        {
            get
            {
                return _current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return _current;
            }
        }
        
        public void Dispose()
        {
            _e = null;
            _current = default(T);
            _index = -1;
        }

        public bool MoveNext()
        {
            if (_e == null) return false;

            if (_index < 0)
            {
                var e = _e as IEnumerator<T>;
                if (e.MoveNext())
                {
                    _current = e.Current;
                    return true;
                }
                else
                {
                    _current = default(T);
                    return false;
                }
            }
            else
            {
                var lst = _e as IList<T>;
                if (_index < lst.Count)
                {
                    _current = lst[_index];
                    _index++;
                    return true;
                }
                else
                {
                    _current = default(T);
                    return false;
                }
            }
        }
        
        void IEnumerator.Reset()
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}
