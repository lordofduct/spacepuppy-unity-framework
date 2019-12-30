using System;
using System.Text;

using Vector4 = UnityEngine.Vector4;
using Quaternion = UnityEngine.Quaternion;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Dynamic
{

    /// <summary>
    /// ##Statements
    /// (...)
    /// "..."
    /// ##Operators
    /// +
    /// *
    /// -
    /// /
    /// %
    /// ^
    /// >
    /// <
    /// >=
    /// <=
    /// ==
    /// !=
    /// ||
    /// &&
    /// 
    /// Does not respect order of operations, use parens to define order.
    /// 
    /// ##Functions
    /// Str(x)
    /// Abs(x)
    /// Sqrt(x)
    /// Cos(x)
    /// Sin(x)
    /// Tan(x)
    /// Acos(x)
    /// Asin(x)
    /// Atan(x)
    /// Atan2(y, x)
    /// Rand(max)
    /// Rand(min, max)
    /// RandInt(max)
    /// RandInt(min, max)
    /// vec(x)
    /// vec(x,y)
    /// vec(x,y,z)
    /// vec(x,y,z,w)
    /// rot(x,y,z)
    /// 
    /// These function names are not case sensitive
    /// 
    /// #Constants
    /// $null
    /// $true
    /// $false
    /// $pi
    /// $2pi
    /// $pi_2
    /// $rad2deg
    /// $deg2rad
    /// $secsInMin
    /// $secsInHour
    /// $secsInDay
    /// $secsInWeek
    /// $secsInYear
    /// $infinity
    /// $inf
    /// $-infinity
    /// $-inf
    /// $time
    /// $unscaledtime
    /// $fixedtime
    /// $deltatime
    /// 
    /// These are global values, they are not case sensitive
    /// 
    /// #Special Case - local variable
    /// $.nameOfProperty
    /// 
    /// In the case of the object referenced on a VariantReference, you can access its properties by typing $ followed by a dot (.) 
    /// and the name of the property. The property is case sensitive and must be spelled as it appears in code.
    /// 
    /// $.CurrentTime / $secsInHour % 24
    /// 
    /// #Special Case - cast local variable
    /// $(Type).nameOfProperty
    /// 
    /// You can grab a component from the local variable before evaluating a property with the parens following $.
    /// 
    /// rotation summation requires using the * operator instead of +
    /// </summary>
    public class Evaluator
    {

        #region Static Interface

        private static com.spacepuppy.Collections.ObjectCachePool<Evaluator> _pool = new ObjectCachePool<Evaluator>(64);
        private static com.spacepuppy.Collections.ObjectCachePool<VariantReference> _variantPool = new ObjectCachePool<VariantReference>(128, () => new VariantReference(), (v) => v.Value = null);
        private static com.spacepuppy.Collections.ObjectCachePool<ReusableStringReader> _readerPool = new ObjectCachePool<ReusableStringReader>(64);

        public static object EvalValue(string command, object x)
        {
            var obj = _pool.GetInstance();
            try
            {
                return obj.EvalStatement(command, x);
            }
            finally
            {
                _pool.Release(obj);
            }
        }

        public static float EvalNumber(string command, object x)
        {
            var obj = _pool.GetInstance();
            var temp = _variantPool.GetInstance();
            try
            {
                var r = _readerPool.GetInstance();
                r.Reset(command);

                obj._reader = r;
                obj._x = x;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;

                bool t2 = obj.EvalStatement(temp);
                var result = temp.FloatValue;

                obj._reader.Dispose();
                _readerPool.Release(obj._reader as ReusableStringReader);
                obj._x = null;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;

                return result;
            }
            finally
            {
                _pool.Release(obj);
                _variantPool.Release(temp);
            }
        }

        public static UnityEngine.Vector2 EvalVector2(string command, object x)
        {
            var obj = _pool.GetInstance();
            var temp = _variantPool.GetInstance();
            try
            {
                var r = _readerPool.GetInstance();
                r.Reset(command);

                obj._reader = r;
                obj._x = x;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;

                bool t2 = obj.EvalStatement(temp);
                var result = temp.Vector2Value;

                obj._reader.Dispose();
                _readerPool.Release(obj._reader as ReusableStringReader);
                obj._x = null;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;

                return result;
            }
            finally
            {
                _pool.Release(obj);
                _variantPool.Release(temp);
            }
        }

        public static UnityEngine.Vector3 EvalVector3(string command, object x)
        {
            var obj = _pool.GetInstance();
            var temp = _variantPool.GetInstance();
            try
            {
                var r = _readerPool.GetInstance();
                r.Reset(command);

                obj._reader = r;
                obj._x = x;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;

                bool t2 = obj.EvalStatement(temp);
                var result = temp.Vector3Value;

                obj._reader.Dispose();
                _readerPool.Release(obj._reader as ReusableStringReader);
                obj._x = null;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;

                return result;
            }
            finally
            {
                _pool.Release(obj);
                _variantPool.Release(temp);
            }
        }

        public static Vector4 EvalVector4(string command, object x)
        {
            var obj = _pool.GetInstance();
            var temp = _variantPool.GetInstance();
            try
            {
                var r = _readerPool.GetInstance();
                r.Reset(command);

                obj._reader = r;
                obj._x = x;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;

                bool t2 = obj.EvalStatement(temp);
                var result = temp.Vector4Value;

                obj._reader.Dispose();
                _readerPool.Release(obj._reader as ReusableStringReader);
                obj._x = null;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;

                return result;
            }
            finally
            {
                _pool.Release(obj);
                _variantPool.Release(temp);
            }
        }

        public static Quaternion EvalQuaternion(string command, object x)
        {
            var obj = _pool.GetInstance();
            var temp = _variantPool.GetInstance();
            try
            {
                var r = _readerPool.GetInstance();
                r.Reset(command);

                obj._reader = r;
                obj._x = x;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;

                bool t2 = obj.EvalStatement(temp);
                var result = temp.QuaternionValue;

                obj._reader.Dispose();
                _readerPool.Release(obj._reader as ReusableStringReader);
                obj._x = null;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;

                return result;
            }
            finally
            {
                _pool.Release(obj);
                _variantPool.Release(temp);
            }
        }

        public static UnityEngine.Color EvalColor(string command, object x)
        {
            var obj = _pool.GetInstance();
            var temp = _variantPool.GetInstance();
            try
            {
                var r = _readerPool.GetInstance();
                r.Reset(command);

                obj._reader = r;
                obj._x = x;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;

                bool t2 = obj.EvalStatement(temp);
                var result = temp.ColorValue;

                obj._reader.Dispose();
                _readerPool.Release(obj._reader as ReusableStringReader);
                obj._x = null;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;

                return result;
            }
            finally
            {
                _pool.Release(obj);
                _variantPool.Release(temp);
            }
        }

        public static bool EvalBool(string command, object x)
        {
            var obj = _pool.GetInstance();
            var temp = _variantPool.GetInstance();
            try
            {
                var r = _readerPool.GetInstance();
                r.Reset(command);

                obj._reader = r;
                obj._x = x;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;

                bool t2 = obj.EvalStatement(temp);
                var result = temp.BoolValue;

                obj._reader.Dispose();
                _readerPool.Release(obj._reader as ReusableStringReader);
                obj._x = null;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;

                return result;
            }
            finally
            {
                _pool.Release(obj);
                _variantPool.Release(temp);
            }
        }

        public static UnityEngine.Rect EvalRect(string command, object x)
        {
            var obj = _pool.GetInstance();
            var temp = _variantPool.GetInstance();
            try
            {
                var r = _readerPool.GetInstance();
                r.Reset(command);

                obj._reader = r;
                obj._x = x;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;

                bool t2 = obj.EvalStatement(temp);
                var result = temp.RectValue;

                obj._reader.Dispose();
                _readerPool.Release(obj._reader as ReusableStringReader);
                obj._x = null;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;

                return result;
            }
            finally
            {
                _pool.Release(obj);
                _variantPool.Release(temp);
            }
        }

        public static string EvalString(string command, object x)
        {
            var obj = _pool.GetInstance();
            var temp = _variantPool.GetInstance();
            try
            {
                var r = _readerPool.GetInstance();
                r.Reset(command);

                obj._reader = r;
                obj._x = x;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;

                bool t2 = obj.EvalStatement(temp);
                var result = temp.StringValue;

                obj._reader.Dispose();
                _readerPool.Release(obj._reader as ReusableStringReader);
                obj._x = null;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;

                return result;
            }
            finally
            {
                _pool.Release(obj);
                _variantPool.Release(temp);
            }
        }

        #endregion


        #region Fields

        private System.IO.TextReader _reader;
        private object _x;
        private StringBuilder _strBuilder = new StringBuilder();
        private int _parenCount;
        private char _current;
        
        #endregion

        #region Methods

        public object EvalStatement(string command, object x)
        {
            var r = _readerPool.GetInstance();
            r.Reset(command);

            _reader = r;
            _x = x;
            _strBuilder.Length = 0;
            _parenCount = 0;
            _current = (char)0;

            var temp = _variantPool.GetInstance();
            bool t2 = this.EvalStatement(temp);
            var result = temp.Value;
            _variantPool.Release(temp);

            _reader.Dispose();
            _readerPool.Release(_reader as ReusableStringReader);
            _x = null;
            _strBuilder.Length = 0;
            _parenCount = 0;
            _current = (char)0;

            return result;
        }

        public object EvalStatement(System.IO.TextReader command, object x)
        {
            if (command == null) throw new System.ArgumentNullException("command");

            _reader = command;
            _x = x;
            _strBuilder.Length = 0;
            _parenCount = 0;
            _current = (char)0;

            var temp = _variantPool.GetInstance();
            bool t2 = this.EvalStatement(temp);
            var result = temp.Value;
            _variantPool.Release(temp);

            _reader.Dispose();
            _x = null;
            _strBuilder.Length = 0;
            _parenCount = 0;
            _current = (char)0;

            return result;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="requireClosingParen"></param>
        /// <returns>Returns true if successfully evaluated the entire statement</returns>
        private bool EvalStatement(VariantReference state, bool requireClosingParen = false)
        {
            this.EvalNextValue(state);

            if (_current == ')')
            {
                int c = _reader.Read();
                if (c >= 0) _current = (char)c;
                _parenCount--;
                return true;
            }

            VariantReference temp = _variantPool.GetInstance();
            try
            {
                for (int i = _current; i >= 0; i = _reader.Read())
                {
                    _current = (char)i;

                    if (char.IsWhiteSpace(_current)) continue;

                    switch (_current)
                    {
                        case '+':
                            this.EvalNextValue(temp);
                            DoSum(state, temp);
                            break;
                        case '-':
                            this.EvalNextValue(temp);
                            DoMinus(state, temp);
                            break;
                        case '*':
                            this.EvalNextValue(temp);
                            DoProduct(state, temp);
                            break;
                        case '/':
                            this.EvalNextValue(temp);
                            DoDivide(state, temp);
                            break;
                        case '^':
                            this.EvalNextValue(temp);
                            state.DoubleValue = Math.Pow(state.DoubleValue, temp.DoubleValue);
                            break;
                        case '%':
                            this.EvalNextValue(temp);
                            state.DoubleValue = state.DoubleValue % temp.DoubleValue;
                            break;
                        case '=':
                            {
                                if (_reader.Peek() == '=')
                                {
                                    _reader.Read();
                                    this.EvalNextValue(temp);
                                    state.BoolValue = TryEquals(state, temp);
                                }
                                else
                                {
                                    this.EvalNextValue(temp);
                                    state.BoolValue = TryEquals(state, temp);
                                }
                            }
                            break;
                        case '!':
                            {
                                if (_reader.Peek() == '=')
                                {
                                    _reader.Read();
                                    this.EvalNextValue(temp);
                                    state.BoolValue = !TryEquals(state, temp);
                                }
                                else
                                {
                                    throw new System.InvalidOperationException("Failed to parse the command.");
                                }
                            }
                            break;
                        case '<':
                            {
                                if (_reader.Peek() == '=')
                                {
                                    _reader.Read();
                                    this.EvalNextValue(temp);
                                    state.BoolValue = (state.DoubleValue <= temp.DoubleValue);
                                }
                                else
                                {
                                    this.EvalNextValue(temp);
                                    state.BoolValue = (state.DoubleValue < temp.DoubleValue);
                                }
                            }
                            break;
                        case '>':
                            {
                                if (_reader.Peek() == '=')
                                {
                                    _reader.Read();
                                    this.EvalNextValue(temp);
                                    state.BoolValue = (state.DoubleValue >= temp.DoubleValue);
                                }
                                else
                                {
                                    this.EvalNextValue(temp);
                                    state.BoolValue = (state.DoubleValue > temp.DoubleValue);
                                }
                            }
                            break;
                        case '|':
                            {
                                if (_reader.Peek() == '|')
                                {
                                    _reader.Read();
                                    this.EvalNextValue(temp);
                                    state.BoolValue = state.BoolValue || temp.BoolValue;
                                }
                                else
                                {
                                    this.EvalNextValue(temp);
                                    state.IntValue = state.IntValue | temp.IntValue;
                                }
                            }
                            break;
                        case '&':
                            {
                                if (_reader.Peek() == '&')
                                {
                                    _reader.Read();
                                    this.EvalNextValue(temp);
                                    state.BoolValue = state.BoolValue && temp.BoolValue;
                                }
                                else
                                {
                                    this.EvalNextValue(temp);
                                    state.IntValue = state.IntValue & temp.IntValue;
                                }
                            }
                            break;
                        case ',':
                            //reached the end of the first parameter
                            return false;
                        case ')':
                            //reached the end of the statement
                            int c = _reader.Read();
                            if (c >= 0) _current = (char)c;
                            _parenCount--;
                            return true;
                    }

                    if (_current == ',')
                    {
                        return false;
                    }
                    if (_current == ')')
                    {
                        int c = _reader.Read();
                        if (c >= 0) _current = (char)c;
                        _parenCount--;
                        return true;
                    }
                }
            }
            finally
            {
                _variantPool.Release(temp);
            }

            //ran out of statement with no errors, must be the end
            if (requireClosingParen)
                throw new System.InvalidOperationException("Failed to parse the command.");

            return true;
        }

        private void EvalNextValue(VariantReference state)
        {
            int i = _reader.Read();
            for (; i >= 0 && char.IsWhiteSpace((char)i); i = _reader.Read())
            {
            }
            if (i < 0)
            {
                state.Value = null;
                return;
            }

            _current = (char)i;
            if (!IsValidWordPrefix(_current)) throw new System.InvalidOperationException("Failed to parse the command.");

            if (char.IsDigit(_current))
            {
                state.DoubleValue = EvalNumber();
                return;
            }
            if (char.IsLetter(_current))
            {
                EvalFunc(state);
                return;
            }

            switch (_current)
            {
                case '$':
                    this.EvalVariable(state);
                    return;
                case '(':
                    _parenCount++;
                    bool temp = this.EvalStatement(state);
                    return;
                case '+':
                    this.EvalNextValue(state);
                    return;
                case '-':
                    this.EvalNextValue(state);
                    DoNegate(state);
                    return;
                case '\"':
                    EvalString(state);
                    return;
                case ')':
                    return;
                default:
                    throw new System.InvalidOperationException("Failed to parse the command.");
            }
        }

        private double EvalNumber()
        {
            const int CHAR_0 = (int)'0';
            long high = ((int)_current - CHAR_0);
            long low = 0;
            int lowLen = -1;

            for (int i = _reader.Read(); i >= 0; i = _reader.Read())
            {
                _current = (char)i;

                if (char.IsDigit(_current))
                {
                    if (lowLen < 0)
                        high = (high * 10) + ((int)_current - CHAR_0);
                    else
                    {
                        low = (low * 10) + ((int)_current - CHAR_0);
                        lowLen++;
                    }
                }
                else if (_current == '.')
                {
                    if (lowLen < 0)
                        lowLen = 0;
                    else
                        throw new System.InvalidOperationException("Failed to parse the command.");
                }
                else if (char.IsWhiteSpace(_current))
                {
                    break;
                }
                else if (_current == ')' || _current == ',')
                {
                    break;
                }
                else if (IsArithmeticSymbol(_current))
                {
                    break;
                }
                else
                    throw new System.InvalidOperationException("Failed to parse the command.");

            }

            if (low != 0)
                return ((double)high + ((double)low / Math.Pow(10, lowLen)));
            else
                return (double)high;
        }

        private void EvalVariable(VariantReference state)
        {
            _strBuilder.Length = 0;
            int i = _reader.Read();
            if (i < 0)
            {
                state.Value = _x;
                return;
            }

            _current = (char)i;
            if (_current == '.')
            {
                var target = _x;
                string sprop;

                //access x
                for (i = _reader.Read(); i >= 0; i = _reader.Read())
                {
                    _current = (char)i;

                    if (char.IsLetterOrDigit(_current) || _current == '_')
                    {
                        _strBuilder.Append(_current);
                    }
                    else if (_current == '.')
                    {
                        sprop = _strBuilder.ToString();
                        _strBuilder.Length = 0;
                        target = DynamicUtil.GetValue(target, sprop);
                    }
                    else if(_current == '(')
                    {
                        //it's a function!
                        while (_reader.Peek() != 0 && char.IsWhiteSpace((char)_reader.Peek())) _reader.Read(); //consume whitespace

                        if((char)_reader.Peek() == ')')
                        {
                            _reader.Read();
                            sprop = _strBuilder.ToString();
                            _strBuilder.Length = 0;
                            state.Value = DynamicUtil.GetValue(target, sprop);
                            return;
                        }
                        else
                        {
                            using (var lst = TempCollection.GetList<object>())
                            {
                                VariantReference temp = _variantPool.GetInstance();
                                try
                                {
                                    bool complete = false;
                                    while (!complete)
                                    {
                                        complete = this.EvalStatement(temp, true);
                                        lst.Add(temp.Value);
                                    }
                                }
                                finally
                                {
                                    _variantPool.Release(temp);
                                }

                                sprop = _strBuilder.ToString();
                                _strBuilder.Length = 0;
                                state.Value = DynamicUtil.GetValue(target, sprop, lst.ToArray());
                                return;
                            }
                        }
                    }
                    else if (char.IsWhiteSpace(_current) || IsArithmeticSymbol(_current) || _current == ')' || _current == ',' || _current == ']')
                        break;
                    else
                        throw new System.InvalidOperationException("Failed to parse the command.");
                }

                sprop = _strBuilder.ToString();
                _strBuilder.Length = 0;
                state.Value = DynamicUtil.GetValue(target, sprop);
                return;
            }
            else if(_current == '(')
            {
                for (i = _reader.Read(); i >= 0; i = _reader.Read())
                {
                    _current = (char)i;

                    if (char.IsLetterOrDigit(_current) || _current == '_')
                    {
                        _strBuilder.Append(_current);
                    }
                    else if(_current == ')')
                    {
                        break;
                    }
                    else
                        throw new System.InvalidOperationException("Failed to parse the command.");
                }

                i = _reader.Read();
                if(i < 0 || (char)i != '.')
                    throw new System.InvalidOperationException("Failed to parse the command.");

                string stp = _strBuilder.ToString();
                _strBuilder.Length = 0;

                var go = GameObjectUtil.GetGameObjectFromSource(_x);
                object target = go != null ? go.GetComponent(stp) : null;
                string sprop;

                //access target
                for (i = _reader.Read(); i >= 0; i = _reader.Read())
                {
                    _current = (char)i;

                    if (char.IsLetterOrDigit(_current) || _current == '_')
                    {
                        _strBuilder.Append(_current);
                    }
                    else if (_current == '.')
                    {
                        sprop = _strBuilder.ToString();
                        _strBuilder.Length = 0;
                        target = DynamicUtil.GetValue(target, sprop);
                    }
                    else if (_current == '(')
                    {
                        //it's a function!
                        while (_reader.Peek() != 0 && char.IsWhiteSpace((char)_reader.Peek())) _reader.Read(); //consume whitespace

                        if ((char)_reader.Peek() == ')')
                        {
                            _reader.Read();
                            sprop = _strBuilder.ToString();
                            _strBuilder.Length = 0;
                            state.Value = DynamicUtil.GetValue(target, sprop);
                            return;
                        }
                        else
                        {
                            using (var lst = TempCollection.GetList<object>())
                            {
                                VariantReference temp = _variantPool.GetInstance();
                                try
                                {
                                    bool complete = false;
                                    while (!complete)
                                    {
                                        complete = this.EvalStatement(temp, true);
                                        lst.Add(temp.Value);
                                    }
                                }
                                finally
                                {
                                    _variantPool.Release(temp);
                                }

                                sprop = _strBuilder.ToString();
                                _strBuilder.Length = 0;
                                state.Value = DynamicUtil.GetValue(target, sprop, lst.ToArray());
                                return;
                            }
                        }
                    }
                    else if (char.IsWhiteSpace(_current) || IsArithmeticSymbol(_current) || _current == ')' || _current == ',' || _current == ']')
                        break;
                    else
                        throw new System.InvalidOperationException("Failed to parse the command.");
                }

                sprop = _strBuilder.ToString();
                _strBuilder.Length = 0;
                state.Value = DynamicUtil.GetValue(target, sprop);
                return;
            }
            else if (char.IsLetterOrDigit(_current) || _current == '_' || _current == '-')
            {
                //global
                _strBuilder.Append(char.ToLower(_current));

                for (i = _reader.Read(); i >= 0; i = _reader.Read())
                {
                    _current = (char)i;

                    if (char.IsLetterOrDigit(_current) || _current == '_')
                    {
                        _strBuilder.Append(char.ToLower(_current));
                    }
                    else if (char.IsWhiteSpace(_current) || IsArithmeticSymbol(_current) || _current == ')' || _current == ',' || _current == ']')
                        break;
                    else
                        throw new System.InvalidOperationException("Failed to parse the command.");
                }


                var str = _strBuilder.ToString();
                _strBuilder.Length = 0;

                switch (str)
                {
                    case "true":
                        state.BoolValue = true;
                        return;
                    case "false":
                        state.BoolValue = false;
                        return;
                    case "null":
                        state.Value = null;
                        return;
                    case "pi":
                        state.DoubleValue = Math.PI;
                        return;
                    case "2pi":
                        const double TWO_PI = (System.Math.PI * 2d);
                        state.DoubleValue = TWO_PI;
                        return;
                    case "pi_2":
                        const double PI_TWO = (System.Math.PI / 2d);
                        state.DoubleValue = PI_TWO;
                        return;
                    case "rad2deg":
                        const double RAD2DEG = (180d / System.Math.PI);
                        state.DoubleValue = RAD2DEG;
                        return;
                    case "deg2rad":
                        const double DEG2RAD = (System.Math.PI / 180d);
                        state.DoubleValue = DEG2RAD;
                        return;
                    case "secsinmin":
                        state.DoubleValue = 60d;
                        return;
                    case "secsinhour":
                        state.DoubleValue = 3600d;
                        return;
                    case "secsinday":
                        state.DoubleValue = 86400d;
                        return;
                    case "secsinweek":
                        state.DoubleValue = 604800d;
                        return;
                    case "secsinyear":
                        state.DoubleValue = 31536000d;
                        return;
                    case "infinity":
                    case "inf":
                        state.DoubleValue = double.PositiveInfinity;
                        return;
                    case "-infinity":
                    case "-inf":
                        state.DoubleValue = double.NegativeInfinity;
                        return;
                    case "time":
                        state.DoubleValue = UnityEngine.Time.time;
                        return;
                    case "unscaledtime":
                        state.DoubleValue = UnityEngine.Time.unscaledTime;
                        return;
                    case "fixedtime":
                        state.DoubleValue = UnityEngine.Time.fixedTime;
                        return;
                    case "deltatime":
                        state.DoubleValue = UnityEngine.Time.deltaTime;
                        return;
                    case "fixeddeltatime":
                        state.DoubleValue = UnityEngine.Time.fixedDeltaTime;
                        return;
                    default:
                        state.Value = null;
                        return;
                }
            }
            else if (char.IsWhiteSpace(_current) || IsArithmeticSymbol(_current) || _current == ')' || _current == ',' || _current == ']')
            {
                state.Value = _x;
                return;
            }
            else
            {
                throw new System.InvalidOperationException("Failed to parse the command.");
            }

        }

        private void EvalFunc(VariantReference state)
        {
            _strBuilder.Length = 0;
            _strBuilder.Append(char.ToLower(_current));

            for (int i = _reader.Read(); i >= 0; i = _reader.Read())
            {
                _current = (char)i;

                if (char.IsLetterOrDigit(_current))
                {
                    _strBuilder.Append(char.ToLower(_current));
                }
                else if (_current == '(')
                    break;
                else
                    throw new System.InvalidOperationException("Failed to parse the command.");
            }

            var name = _strBuilder.ToString();
            _strBuilder.Length = 0;

            _parenCount++;

            VariantReference temp = _variantPool.GetInstance();
            try
            {
                bool reachedEnd;
                switch (name)
                {
                    case "str":
                        {
                            reachedEnd = EvalStatement(temp, true);
                            if (!reachedEnd)
                                throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                            state.StringValue = temp.StringValue;
                            return;
                        }
                    case "abs":
                        {
                            reachedEnd = EvalStatement(temp, true);
                            if (!reachedEnd)
                                throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                            state.DoubleValue = Math.Abs(temp.DoubleValue);
                            return;
                        }
                    case "sqrt":
                        {
                            reachedEnd = EvalStatement(temp, true);
                            if (!reachedEnd)
                                throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                            state.DoubleValue = Math.Sqrt(temp.DoubleValue);
                            return;
                        }
                    case "cos":
                        {
                            reachedEnd = EvalStatement(temp, true);
                            if (!reachedEnd)
                                throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                            state.DoubleValue = Math.Cos(temp.DoubleValue);
                            return;
                        }
                    case "sin":
                        {
                            reachedEnd = EvalStatement(temp, true);
                            if (!reachedEnd)
                                throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                            state.DoubleValue = Math.Sin(temp.DoubleValue);
                            return;
                        }
                    case "tan":
                        {
                            reachedEnd = EvalStatement(temp, true);
                            if (!reachedEnd)
                                throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                            state.DoubleValue = Math.Tan(temp.DoubleValue);
                            return;
                        }
                    case "acos":
                        {
                            reachedEnd = EvalStatement(temp, true);
                            if (!reachedEnd)
                                throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                            state.DoubleValue = Math.Acos(temp.DoubleValue);
                            return;
                        }
                    case "asin":
                        {
                            reachedEnd = EvalStatement(temp, true);
                            if (!reachedEnd)
                                throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                            state.DoubleValue = Math.Asin(temp.DoubleValue);
                            return;
                        }
                    case "atan":
                        {
                            reachedEnd = EvalStatement(temp, true);
                            if (!reachedEnd)
                                throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                            state.DoubleValue = Math.Atan(temp.DoubleValue);
                            return;
                        }
                    case "atan2":
                        {
                            reachedEnd = this.EvalStatement(temp, true);
                            if (reachedEnd)
                                throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                            double y = temp.DoubleValue;

                            reachedEnd = EvalStatement(temp, true);
                            if (!reachedEnd)
                                throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                            double x = temp.DoubleValue;

                            state.DoubleValue = Math.Atan2(y, x);
                            return;
                        }
                    case "rand":
                        {
                            reachedEnd = this.EvalStatement(temp, true);
                            if (temp.ValueType == VariantType.Quaternion)
                            {
                                if (reachedEnd)
                                {
                                    state.QuaternionValue = Quaternion.Slerp(Quaternion.identity, temp.QuaternionValue, RandomUtil.Standard.Next());
                                    return;
                                }

                                var x = temp.QuaternionValue;
                                reachedEnd = this.EvalStatement(temp, true);
                                if (reachedEnd)
                                {
                                    state.QuaternionValue = Quaternion.Slerp(x, temp.QuaternionValue, RandomUtil.Standard.Next());
                                    return;
                                }
                                
                                throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                            }
                            else
                            {
                                if(reachedEnd)
                                {
                                    DoProductInterlerp(temp, RandomUtil.Standard.NextDouble());
                                    state.CopyValue(temp);
                                    return;
                                }

                                var x = temp.Value;
                                reachedEnd = this.EvalStatement(temp, true);
                                if(reachedEnd)
                                {
                                    state.Value = TryLerp(x, temp.Value, RandomUtil.Standard.Next());
                                    return;
                                }

                                throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                            }
                        }
                    case "randint":
                        {
                            reachedEnd = this.EvalStatement(temp, true);
                            if(reachedEnd)
                            {
                                state.IntValue = RandomUtil.Standard.Next(temp.IntValue);
                                return;
                            }

                            int x = temp.IntValue;
                            reachedEnd = this.EvalStatement(temp, true);
                            if(!reachedEnd)
                                throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");

                            state.IntValue = RandomUtil.Standard.Next(x, temp.IntValue);
                            return;
                        }
                    case "vec":
                        {
                            reachedEnd = this.EvalStatement(temp, true);
                            if(reachedEnd)
                            {
                                switch(temp.ValueType)
                                {
                                    case VariantType.Vector2:
                                    case VariantType.Vector3:
                                    case VariantType.Vector4:
                                    case VariantType.Quaternion:
                                        state.CopyValue(temp);
                                        break;
                                    default:
                                        state.Vector2Value = new UnityEngine.Vector2(temp.FloatValue, 0f);
                                        break;
                                }
                                return;
                            }

                            float x = temp.FloatValue;
                            reachedEnd = this.EvalStatement(temp, true);
                            if(reachedEnd)
                            {
                                switch(temp.ValueType)
                                {
                                    case VariantType.Vector2:
                                        state.Vector3Value = new UnityEngine.Vector3(x, temp.Vector2Value.x, temp.Vector2Value.y);
                                        break;
                                    case VariantType.Vector3:
                                        state.Vector4Value = new UnityEngine.Vector4(x, temp.Vector3Value.x, temp.Vector3Value.y, temp.Vector3Value.z);
                                        break;
                                    case VariantType.Vector4:
                                        state.Vector4Value = new UnityEngine.Vector4(x, temp.Vector4Value.x, temp.Vector4Value.y, temp.Vector4Value.z);
                                        break;
                                    default:
                                        state.Vector2Value = new UnityEngine.Vector2(x, temp.FloatValue);
                                        break;
                                }
                                return;
                            }

                            float y = temp.FloatValue;
                            reachedEnd = this.EvalStatement(temp, true);
                            if(reachedEnd)
                            {
                                switch (temp.ValueType)
                                {
                                    case VariantType.Vector2:
                                        state.Vector4Value = new UnityEngine.Vector4(x, y, temp.Vector2Value.x, temp.Vector2Value.y);
                                        break;
                                    case VariantType.Vector3:
                                        state.Vector4Value = new UnityEngine.Vector4(x, y, temp.Vector3Value.x, temp.Vector3Value.y);
                                        break;
                                    case VariantType.Vector4:
                                        state.Vector4Value = new UnityEngine.Vector4(x, y, temp.Vector4Value.x, temp.Vector4Value.y);
                                        break;
                                    default:
                                        state.Vector3Value = new UnityEngine.Vector3(x, y, temp.FloatValue);
                                        break;
                                }
                                return;
                            }

                            float z = temp.FloatValue;
                            reachedEnd = this.EvalStatement(temp, true);
                            if (reachedEnd)
                            {
                                state.Vector4Value = new UnityEngine.Vector4(x, y, z, temp.FloatValue);
                                return;
                            }

                            throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                        }
                    case "rot":
                        {
                            reachedEnd = this.EvalStatement(temp, true);
                            if (reachedEnd)
                            {
                                switch (temp.ValueType)
                                {
                                    case VariantType.Vector2:
                                    case VariantType.Vector3:
                                    case VariantType.Vector4:
                                        state.QuaternionValue = Quaternion.Euler(temp.Vector3Value);
                                        break;
                                    case VariantType.Quaternion:
                                        state.QuaternionValue = temp.QuaternionValue;
                                        break;
                                    default:
                                        state.QuaternionValue = Quaternion.Euler(temp.FloatValue, 0f, 0f);
                                        break;
                                }
                                return;
                            }

                            float x = temp.FloatValue;
                            reachedEnd = this.EvalStatement(temp, true);
                            if(reachedEnd)
                            {
                                switch (temp.ValueType)
                                {
                                    case VariantType.Vector2:
                                    case VariantType.Vector3:
                                    case VariantType.Vector4:
                                    case VariantType.Quaternion:
                                        state.QuaternionValue = Quaternion.Euler(x, temp.Vector2Value.x, temp.Vector2Value.y);
                                        break;
                                    default:
                                        state.QuaternionValue = Quaternion.Euler(x, temp.FloatValue, 0f);
                                        break;
                                }
                                return;
                            }

                            float y = temp.FloatValue;
                            reachedEnd = this.EvalStatement(temp, true);
                            if (reachedEnd)
                            {
                                state.QuaternionValue = Quaternion.Euler(x, y, temp.FloatValue);
                                return;
                            }

                            throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                        }
                    default:
                        throw new System.InvalidOperationException("Failed to parse the command: Unknown Function");
                }
            }
            finally
            {
                _variantPool.Release(temp);
            }
        }
        
        private void EvalString(VariantReference state)
        {
            _strBuilder.Length = 0;

            bool successfulClose = false;
            for (int i = _reader.Read(); i >= 0; i = _reader.Read())
            {
                _current = (char)i;
                
                if(_current == '\"')
                {
                    successfulClose = true;
                    break;
                }
                else if (_current == '\\')
                {
                    i = _reader.Read();
                    if(i < 0)
                    {
                        throw new System.InvalidOperationException("Failed to parse the command: string statement syntax error.");
                    }

                    _current = (char)i;
                    _strBuilder.Append(_current);
                }
                else
                {
                    _strBuilder.Append(_current);
                }
            }

            if(!successfulClose)
            {
                throw new System.InvalidOperationException("Failed to parse the command: string statement syntax error.");
            }

            state.StringValue = _strBuilder.ToString();
            _strBuilder.Length = 0;
        }

        #endregion

        #region Utils
        
        private static bool IsArithmeticSymbol(char c)
        {
            switch (c)
            {
                case '+':
                case '*':
                case '-':
                case '/':
                case '%':
                case '^':
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsValidWordPrefix(char c)
        {
            return char.IsLetterOrDigit(c) || c == '$' || c == '_' || c == '+' || c == '-' || c == '(' || c == '\"' || c == ')';
        }


        //these operations modify the state of 'left'

        private static void DoNegate(VariantReference value)
        {
            switch (value.ValueType)
            {
                case VariantType.Boolean:
                    value.BoolValue = !value.BoolValue;
                    break;
                case VariantType.Integer:
                    value.IntValue = -value.IntValue;
                    break;
                case VariantType.Float:
                    value.FloatValue = -value.FloatValue;
                    break;
                case VariantType.Double:
                    value.DoubleValue = -value.DoubleValue;
                    break;
                case VariantType.Vector2:
                    value.Vector2Value = -value.Vector2Value;
                    break;
                case VariantType.Vector3:
                    value.Vector3Value = -value.Vector3Value;
                    break;
                case VariantType.Vector4:
                    value.Vector4Value = -value.Vector4Value;
                    break;
                case VariantType.Quaternion:
                    value.QuaternionValue = Quaternion.Inverse(value.QuaternionValue);
                    break;
                case VariantType.Color:
                    value.ColorValue = UnityEngine.Color.white - value.ColorValue;
                    break;
                case VariantType.LayerMask:
                    value.LayerMaskValue = -value.LayerMaskValue;
                    break;
                case VariantType.Numeric:
                    value.DoubleValue = -value.DoubleValue;
                    break;
            }
        }

        private static void DoSum(VariantReference left, VariantReference right)
        {
            if(left.ValueType == VariantType.String || right.ValueType == VariantType.String)
            {
                left.StringValue += right.StringValue;
                return;
            }

            switch (left.ValueType)
            {
                case VariantType.Integer:
                    if (right.ValueType == VariantType.Integer)
                        left.IntValue += right.IntValue;
                    else
                        left.DoubleValue += right.DoubleValue;
                    break;
                case VariantType.Float:
                    left.FloatValue += right.FloatValue;
                    break;
                case VariantType.Double:
                    left.DoubleValue += right.DoubleValue;
                    break;
                case VariantType.Vector2:
                    left.Vector2Value += right.Vector2Value;
                    break;
                case VariantType.Vector3:
                    left.Vector3Value += right.Vector3Value;
                    break;
                case VariantType.Vector4:
                    left.Vector4Value += right.Vector4Value;
                    break;
                case VariantType.Quaternion:
                    left.Vector4Value += right.Vector4Value;
                    break;
                case VariantType.Color:
                    left.ColorValue += right.ColorValue;
                    break;
                case VariantType.LayerMask:
                    left.LayerMaskValue += right.LayerMaskValue;
                    break;
                case VariantType.Numeric:
                    left.DoubleValue += right.DoubleValue;
                    break;
                default:
                    left.DoubleValue = double.NaN;
                    break;
            }
        }

        private static void DoMinus(VariantReference left, VariantReference right)
        {
            switch (left.ValueType)
            {
                case VariantType.Integer:
                    if (right.ValueType == VariantType.Integer)
                        left.IntValue -= right.IntValue;
                    else
                        left.DoubleValue -= right.DoubleValue;
                    break;
                case VariantType.Float:
                    left.FloatValue -= right.FloatValue;
                    break;
                case VariantType.Double:
                    left.DoubleValue -= right.DoubleValue;
                    break;
                case VariantType.Vector2:
                    left.Vector2Value -= right.Vector2Value;
                    break;
                case VariantType.Vector3:
                    left.Vector3Value -= right.Vector3Value;
                    break;
                case VariantType.Vector4:
                    left.Vector4Value -= right.Vector4Value;
                    break;
                case VariantType.Quaternion:
                    left.Vector4Value -= right.Vector4Value;
                    break;
                case VariantType.Color:
                    left.ColorValue -= right.ColorValue;
                    break;
                case VariantType.LayerMask:
                    left.LayerMaskValue -= right.LayerMaskValue;
                    break;
                case VariantType.Numeric:
                    left.DoubleValue -= right.DoubleValue;
                    break;
                default:
                    left.DoubleValue = double.NaN;
                    break;
            }
        }

        private static void DoProductInterlerp(VariantReference left, double right)
        {
            Vector4 vl;
            switch (left.ValueType)
            {
                case VariantType.Integer:
                case VariantType.Float:
                case VariantType.Double:
                    left.DoubleValue *= right;
                    break;
                case VariantType.Vector2:
                    vl = left.Vector2Value;
                    vl.x = vl.x * (float)right;
                    vl.y = vl.y * (float)right;
                    left.Vector4Value = vl;
                    break;
                case VariantType.Vector3:
                    vl = left.Vector3Value;
                    vl.x = vl.x * (float)right;
                    vl.y = vl.y * (float)right;
                    vl.z = vl.z * (float)right;
                    left.Vector4Value = vl;
                    break;
                case VariantType.Vector4:
                    vl = left.Vector4Value;
                    vl.x = vl.x * (float)right;
                    vl.y = vl.y * (float)right;
                    vl.z = vl.z * (float)right;
                    vl.w = vl.w * (float)right;
                    left.Vector4Value = vl;
                    break;
                case VariantType.Quaternion:
                    left.Vector4Value = left.Vector4Value * (float)right;
                    break;
                case VariantType.Color:
                    left.ColorValue = ColorUtil.Lerp(UnityEngine.Color.black, left.ColorValue, (float)right);
                    break;
                case VariantType.LayerMask:
                case VariantType.Numeric:
                    left.DoubleValue *= right;
                    break;
                default:
                    left.DoubleValue = left.DoubleValue * right;
                    break;
            }
        }

        private static void DoProduct(VariantReference left, VariantReference right)
        {
            Vector4 vl;
            Vector4 vr;
            switch(left.ValueType)
            {
                case VariantType.Integer:
                case VariantType.Float:
                case VariantType.Double:
                    left.DoubleValue *= right.DoubleValue;
                    break;
                case VariantType.Vector2:
                    vl = left.Vector2Value;
                    switch(right.ValueType)
                    {
                        case VariantType.Integer:
                        case VariantType.Float:
                        case VariantType.Double:
                            vl.x *= right.FloatValue;
                            vl.y *= right.FloatValue;
                            break;
                        default:
                            vr = right.Vector2Value;
                            vl.x = vl.x * vr.x;
                            vl.y = vl.y * vr.y;
                            break;
                    }
                    left.Vector2Value = vl;
                    break;
                case VariantType.Vector3:
                    vl = left.Vector3Value;
                    vr = right.Vector3Value;
                    switch (right.ValueType)
                    {
                        case VariantType.Integer:
                        case VariantType.Float:
                        case VariantType.Double:
                            vl.x *= right.FloatValue;
                            vl.y *= right.FloatValue;
                            vl.z *= right.FloatValue;
                            break;
                        default:
                            vr = right.Vector3Value;
                            vl.x = vl.x * vr.x;
                            vl.y = vl.y * vr.y;
                            vl.z = vl.z * vr.z;
                            break;
                    }
                    left.Vector3Value = vl;
                    break;
                case VariantType.Vector4:
                    vl = left.Vector4Value;
                    vr = right.Vector4Value;
                    switch (right.ValueType)
                    {
                        case VariantType.Integer:
                        case VariantType.Float:
                        case VariantType.Double:
                            vl.x *= right.FloatValue;
                            vl.y *= right.FloatValue;
                            vl.z *= right.FloatValue;
                            vl.w *= right.FloatValue;
                            break;
                        default:
                            vr = right.Vector4Value;
                            vl.x = vl.x * vr.x;
                            vl.y = vl.y * vr.y;
                            vl.z = vl.z * vr.z;
                            vl.w = vl.w * vr.w;
                            break;
                    }
                    left.Vector4Value = vl;
                    break;
                case VariantType.Quaternion:
                    switch (right.ValueType)
                    {
                        case VariantType.Vector2:
                        case VariantType.Vector3:
                            left.Vector3Value = left.QuaternionValue * right.Vector3Value;
                            break;
                        default:
                            left.QuaternionValue = left.QuaternionValue * right.QuaternionValue;
                            break;
                    }
                    break;
                case VariantType.LayerMask:
                    left.LayerMaskValue *= right.LayerMaskValue;
                    break;
                case VariantType.Numeric:
                    left.DoubleValue *= right.DoubleValue;
                    break;
                default:
                    left.DoubleValue = double.NaN;
                    break;
            }
        }

        private static void DoDivide(VariantReference left, VariantReference right)
        {
            Vector4 vl;
            Vector4 vr;

            switch (left.ValueType)
            {
                case VariantType.Integer:
                case VariantType.Float:
                case VariantType.Double:
                    left.DoubleValue /= right.DoubleValue;
                    break;
                case VariantType.Vector2:
                    vl = left.Vector2Value;
                    switch (right.ValueType)
                    {
                        case VariantType.Integer:
                        case VariantType.Float:
                        case VariantType.Double:
                            vl.x /= right.FloatValue;
                            vl.y /= right.FloatValue;
                            break;
                        default:
                            vr = right.Vector2Value;
                            vl.x = vl.x / vr.x;
                            vl.y = vl.y / vr.y;
                            break;
                    }
                    left.Vector2Value = vl;
                    break;
                case VariantType.Vector3:
                    vl = left.Vector3Value;
                    switch (right.ValueType)
                    {
                        case VariantType.Integer:
                        case VariantType.Float:
                        case VariantType.Double:
                            vl.x /= right.FloatValue;
                            vl.y /= right.FloatValue;
                            vl.z /= right.FloatValue;
                            break;
                        default:
                            vr = right.Vector3Value;
                            vl.x = vl.x / vr.x;
                            vl.y = vl.y / vr.y;
                            vl.z = vl.z / vr.z;
                            break;
                    }
                    left.Vector3Value = vl;
                    break;
                case VariantType.Vector4:
                    vl = left.Vector4Value;
                    switch (right.ValueType)
                    {
                        case VariantType.Integer:
                        case VariantType.Float:
                        case VariantType.Double:
                            vl.x /= right.FloatValue;
                            vl.y /= right.FloatValue;
                            vl.z /= right.FloatValue;
                            vl.w /= right.FloatValue;
                            break;
                        default:
                            vr = right.Vector4Value;
                            vl.x = vl.x / vr.x;
                            vl.y = vl.y / vr.y;
                            vl.z = vl.z / vr.z;
                            vl.w = vl.w / vr.w;
                            break;
                    }
                    left.Vector4Value = vl;
                    break;
                case VariantType.Quaternion:
                    left.QuaternionValue *= Quaternion.Inverse(right.QuaternionValue);
                    break;
                case VariantType.LayerMask:
                    left.LayerMaskValue /= right.LayerMaskValue;
                    break;
                case VariantType.Numeric:
                    left.DoubleValue /= right.DoubleValue;
                    break;
                default:
                    left.DoubleValue = double.NaN;
                    break;
            }
        }



        private static bool TryEquals(VariantReference left, VariantReference right)
        {
            switch(left.ValueType)
            {
                case VariantType.Object:
                    return left.ObjectValue == right.ObjectValue;
                case VariantType.Null:
                    return right.ValueType == VariantType.Null;
                case VariantType.String:
                    return left.StringValue == right.StringValue;
                case VariantType.Boolean:
                    return left.BoolValue == right.BoolValue;
                case VariantType.Integer:
                    if (right.ValueType == VariantType.Integer)
                        return left.IntValue == right.IntValue;
                    else
                        return left.DoubleValue == right.DoubleValue;
                case VariantType.Float:
                    return left.FloatValue == right.FloatValue;
                case VariantType.Double:
                    return left.DoubleValue == right.DoubleValue;
                case VariantType.Vector2:
                    return left.Vector2Value == right.Vector2Value;
                case VariantType.Vector3:
                    return left.Vector3Value == right.Vector3Value;
                case VariantType.Vector4:
                    return left.Vector4Value == right.Vector4Value;
                case VariantType.Quaternion:
                    return left.QuaternionValue == right.QuaternionValue;
                case VariantType.Color:
                    return left.ColorValue == right.ColorValue;
                case VariantType.DateTime:
                    return left.DateValue == right.DateValue;
                case VariantType.GameObject:
                    return left.GameObjectValue == right.GameObjectValue;
                case VariantType.Component:
                    return left.ComponentValue == right.ComponentValue;
                case VariantType.LayerMask:
                    return left.LayerMaskValue == right.LayerMaskValue;
                case VariantType.Rect:
                    return left.RectValue == right.RectValue;
                case VariantType.Numeric:
                    return left.DoubleValue == right.DoubleValue;
                default:
                    return false;
            }
        }



        public static object TrySum(object a, object b)
        {
            if (a == null) return b;
            if (b == null) return a;

            var atp = a.GetType();
            if (ConvertUtil.IsNumericType(atp))
            {
                return ConvertUtil.ToPrim(ConvertUtil.ToDouble(a) + ConvertUtil.ToDouble(b), atp);
            }
            else if (atp == typeof(UnityEngine.Vector2))
            {
                return ConvertUtil.ToVector2(a) + ConvertUtil.ToVector2(b);
            }
            else if (atp == typeof(UnityEngine.Vector3))
            {
                return ConvertUtil.ToVector3(a) + ConvertUtil.ToVector3(b);
            }
            else if (atp == typeof(UnityEngine.Vector4))
            {
                return ConvertUtil.ToVector4(a) + ConvertUtil.ToVector4(b);
            }
            else if (atp == typeof(UnityEngine.Quaternion))
            {
                return ConvertUtil.ToQuaternion(a) * ConvertUtil.ToQuaternion(b);
            }
            else if (atp == typeof(UnityEngine.Color))
            {
                return ConvertUtil.ToColor(a) + ConvertUtil.ToColor(b);
            }
            else if (atp == typeof(UnityEngine.Color32))
            {
                return ConvertUtil.ToColor32(ConvertUtil.ToColor(a) + ConvertUtil.ToColor(b));
            }
            else
            {
                return b;
            }
        }

        public static object TryDifference(object a, object b)
        {
            if (a == null) return b;
            if (b == null) return a;

            var atp = a.GetType();
            if (ConvertUtil.IsNumericType(atp))
            {
                return ConvertUtil.ToPrim(ConvertUtil.ToDouble(a) - ConvertUtil.ToDouble(b), atp);
            }
            else if (atp == typeof(UnityEngine.Vector2))
            {
                return ConvertUtil.ToVector2(a) - ConvertUtil.ToVector2(b);
            }
            else if (atp == typeof(UnityEngine.Vector3))
            {
                return ConvertUtil.ToVector3(a) - ConvertUtil.ToVector3(b);
            }
            else if (atp == typeof(UnityEngine.Vector4))
            {
                return ConvertUtil.ToVector4(a) - ConvertUtil.ToVector4(b);
            }
            else if (atp == typeof(UnityEngine.Quaternion))
            {
                return ConvertUtil.ToQuaternion(a) * UnityEngine.Quaternion.Inverse(ConvertUtil.ToQuaternion(b));
            }
            else if (atp == typeof(UnityEngine.Color))
            {
                return ConvertUtil.ToColor(a) - ConvertUtil.ToColor(b);
            }
            else if (atp == typeof(UnityEngine.Color32))
            {
                return ConvertUtil.ToColor32(ConvertUtil.ToColor(a) - ConvertUtil.ToColor(b));
            }
            else
            {
                return b;
            }
        }

        public static object TryToggle(object value)
        {
            if (value == null) return null;

            var tp = value.GetType();
            if (ConvertUtil.IsNumericType(tp))
            {
                return ConvertUtil.ToPrim(ConvertUtil.ToDouble(value) * -1.0, tp);
            }
            else if (tp == typeof(UnityEngine.Vector2))
            {
                return ConvertUtil.ToVector2(value) * -1f;
            }
            else if (tp == typeof(UnityEngine.Vector3))
            {
                return ConvertUtil.ToVector3(value) * -1f;
            }
            else if (tp == typeof(UnityEngine.Vector4))
            {
                return ConvertUtil.ToVector4(value) * -1f;
            }
            else if (tp == typeof(UnityEngine.Quaternion))
            {
                return UnityEngine.Quaternion.Inverse(ConvertUtil.ToQuaternion(value));
            }
            else
            {
                return value;
            }
        }

        public static object TryLerp(object a, object b, float t)
        {
            if (a == null) return b;
            if (b == null) return a;

            var atp = a.GetType();
            if (ConvertUtil.IsNumericType(atp))
            {
                var x = ConvertUtil.ToDouble(a);
                var y = ConvertUtil.ToDouble(b);
                return ConvertUtil.ToPrim(t * (y - x) + x, atp);
            }
            else if (atp == typeof(UnityEngine.Vector2))
            {
                return UnityEngine.Vector2.LerpUnclamped(ConvertUtil.ToVector2(a), ConvertUtil.ToVector2(b), t);
            }
            else if (atp == typeof(UnityEngine.Vector3))
            {
                return UnityEngine.Vector3.LerpUnclamped(ConvertUtil.ToVector3(a), ConvertUtil.ToVector3(b), t);
            }
            else if (atp == typeof(UnityEngine.Vector4))
            {
                return UnityEngine.Vector4.LerpUnclamped(ConvertUtil.ToVector4(a), ConvertUtil.ToVector4(b), t);
            }
            else if (atp == typeof(UnityEngine.Quaternion))
            {
                return UnityEngine.Quaternion.LerpUnclamped(ConvertUtil.ToQuaternion(a), ConvertUtil.ToQuaternion(b), t);
            }
            else if (atp == typeof(UnityEngine.Color))
            {
                return UnityEngine.Color.LerpUnclamped(ConvertUtil.ToColor(a), ConvertUtil.ToColor(b), t);
            }
            else if (atp == typeof(UnityEngine.Color32))
            {
                return UnityEngine.Color32.LerpUnclamped(ConvertUtil.ToColor32(a), ConvertUtil.ToColor32(b), t);
            }
            else
            {
                return (t < 0.5f) ? a : b;
            }
        }

        public static bool WillArithmeticallyCompute(System.Type tp)
        {
            if (ConvertUtil.IsNumericType(tp)) return true;
            if (tp == typeof(UnityEngine.Vector2)) return true;
            if (tp == typeof(UnityEngine.Vector3)) return true;
            if (tp == typeof(UnityEngine.Vector4)) return true;
            if (tp == typeof(UnityEngine.Quaternion)) return true;
            if (tp == typeof(Variant)) return true;

            return false;
        }

        #endregion

    }

}
