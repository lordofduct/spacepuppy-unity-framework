using System;
using System.Collections.Generic;

namespace com.spacepuppy.Collections
{
    public class ReusableStringReader : System.IO.TextReader
    {
        
        #region Fields

        private string _source;
        private int _nextChar;
        private int _sourceLength;

        #endregion

        #region CONSTRUCTOR

        public ReusableStringReader()
        {
            this.Reset(string.Empty);
        }

        public ReusableStringReader(string s)
        {
            this.Reset(s);
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Reset(string str)
        {
            _source = str ?? string.Empty;
            _nextChar = 0;
            _sourceLength = _source.Length;
        }

        private void CheckObjectDisposedException()
        {
            if (_source == null)
                throw new ObjectDisposedException("StringReader", "Cannot read from a closed StringReader");
        }

        #endregion

        #region TextReader Interface

        public override void Close()
        {
            this.Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            _source = (string)null;
        }

        public override int Peek()
        {
            this.CheckObjectDisposedException();
            if (_nextChar >= _sourceLength)
                return -1;
            return (int)_source[_nextChar];
        }

        public override int Read()
        {
            this.CheckObjectDisposedException();
            if (_nextChar >= _sourceLength)
                return -1;
            return (int)_source[_nextChar++];
        }

        public override int Read(char[] buffer, int index, int count)
        {
            this.CheckObjectDisposedException();
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (buffer.Length - index < count)
                throw new ArgumentException();
            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException();
            int count1 = _nextChar <= _sourceLength - count ? count : _sourceLength - _nextChar;
            _source.CopyTo(_nextChar, buffer, index, count1);
            _nextChar += count1;
            return count1;
        }

        public override string ReadLine()
        {
            this.CheckObjectDisposedException();
            if (_nextChar >= _source.Length)
                return (string)null;
            int num1 = _source.IndexOf('\r', _nextChar);
            int num2 = _source.IndexOf('\n', _nextChar);
            bool flag = false;
            int num3;
            if (num1 == -1)
            {
                if (num2 == -1)
                    return this.ReadToEnd();
                num3 = num2;
            }
            else if (num2 == -1)
            {
                num3 = num1;
            }
            else
            {
                num3 = num1 <= num2 ? num1 : num2;
                flag = num1 + 1 == num2;
            }
            string str = _source.Substring(_nextChar, num3 - _nextChar);
            _nextChar = num3 + (!flag ? 1 : 2);
            return str;
        }

        public override string ReadToEnd()
        {
            this.CheckObjectDisposedException();
            string str = _source.Substring(_nextChar, _sourceLength - _nextChar);
            _nextChar = _sourceLength;
            return str;
        }

        #endregion
        
    }
}
