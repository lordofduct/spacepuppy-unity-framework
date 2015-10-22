using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Collections
{

    /// <summary>
    /// Allows sampling over a string of chars
    /// </summary>
    public class SamplingCharEnumerator : IEnumerator<char>, ICloneable
    {

        #region Fields

        private string _str;
        private int _index;
        private char _currentElement;

        private int _capturePoint;

        #endregion

        #region CONSTRUCTOR

        public SamplingCharEnumerator()
        {
            _str = string.Empty;
            _index = -1;
        }

        public SamplingCharEnumerator(string str)
        {
            _str = str ?? string.Empty;
            _index = -1;
        }

        #endregion

        #region Properties

        public string String
        {
            get { return _str; }
        }

        public int Position
        {
            get { return _index; }
            set
            {
                _index = value;
                if (_index < -1) _index = -1;
                else if (_index > _str.Length) _index = _str.Length;
            }
        }

        public int CapturePosition
        {
            get { return _capturePoint; }
            set
            {
                _capturePoint = value;
                if (_capturePoint < -1) _capturePoint = -1;
                else if (_capturePoint > _str.Length) _capturePoint = _str.Length;
            }
        }

        #endregion

        #region Methods

        public void Reset(string str)
        {
            _str = str ?? string.Empty;
            _index = -1;
        }

        /// <summary>
        /// Get the next character with out 
        /// </summary>
        /// <returns></returns>
        public char Peek()
        {
            return this.Peek(1);
        }

        /// <summary>
        /// Get a character 'dist' positions ahead, without moving the head.
        /// </summary>
        /// <param name="dist"></param>
        /// <returns></returns>
        public char Peek(int dist)
        {
            int i = _index + dist;
            if (i > 0 && i < _str.Length)
                return _str[i];
            else
                return char.MinValue;
        }

        public char PeekPrevious()
        {
            return this.Peek(-1);
        }

        public char PeekPrevious(int dist)
        {
            return this.Peek(-dist);
        }

        /// <summary>
        /// Move 'dist' chars ahead.
        /// </summary>
        /// <param name="dist"></param>
        /// <returns></returns>
        public bool MoveNext(int dist)
        {
            _index += dist;
            if(_index < 0)
            {
                _index = -1;
                _currentElement = char.MinValue;
                return false;
            }
            else if(_index >= _str.Length)
            {
                _index = _str.Length;
                _currentElement = char.MinValue;
                return false;
            }
            else
            {
                _currentElement = _str[_index];
                return true;
            }
        }

        public bool MovePrevious()
        {
            return this.MoveNext(-1);
        }

        public bool MovePrevious(int dist)
        {
            return this.MoveNext(-dist);
        }

        /// <summary>
        /// Saves the current position so that you can return a substring in the range of the current index and the index the enumerator is on when 'EndCapture' is called.
        /// </summary>
        public void StartCapture()
        {
            if (_index < 0)
                throw new InvalidOperationException();

            _capturePoint = _index;
        }

        public string EndCapture()
        {
            int start;
            int end;
            if(_index < _capturePoint)
            {
                start = _index;
                end = _capturePoint;
            }
            else
            {
                start = _capturePoint;
                end = _index;
            }

            if (start < 0) start = 0;
            if (end < 0) end = 0;

            if (start == 0 && end == _str.Length)
                return _str;
            else
                return _str.Substring(start, end - start);
        }

        public int CaptureCompare(string str, bool ignoreCase)
        {
            int start;
            int end;
            if (_index < _capturePoint)
            {
                start = _index;
                end = _capturePoint;
            }
            else
            {
                start = _capturePoint;
                end = _index;
            }

            if (start < 0) start = 0;
            if (end < 0) end = 0;

            int len = end - start;
            if (len == 0 && str.Length == 0) return 0;

            return string.Compare(_str, start, str, 0, len, ignoreCase);
        }

        public bool CaptureEquals(string str, bool ignoreCase)
        {
            return this.CaptureCompare(str, ignoreCase) == 0;
        }

        #endregion

        #region IEnumerator Interface

        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }
        
        public char Current
        {
            get
            {
                if (_index < 0)
                    throw new InvalidOperationException();
                if (_index >= _str.Length)
                    throw new InvalidOperationException();
                else
                    return _currentElement;
            }
        }
        
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        
        public bool MoveNext()
        {
            if (_index < _str.Length - 1)
            {
                _index = _index + 1;
                _currentElement = _str[_index];
                return true;
            }
            else
            {
                _index = _str.Length;
                _currentElement = char.MinValue;
                return false;
            }
        }
        
        public void Dispose()
        {
            _str = string.Empty;
            _index = 0;
            _currentElement = char.MinValue;
        }
        
        public void Reset()
        {
            _currentElement = char.MinValue;
            _index = -1;
        }

        #endregion

    }
}
