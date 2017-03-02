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
    /// #Variables
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
    /// 
    /// These are global values, they are not case sensitive
    /// 
    /// #Special Case
    /// $.nameOfProperty
    /// 
    /// In the case of the object referenced on a VariantReference, you can access its properties by typing $ followed by a dot (.) 
    /// and the name of the property. The property is case sensitive and must be spelled as it appears in code.
    /// 
    /// $.CurrentTime / $secsInHour % 24
    /// </summary>
    public class Evaluator
    {

        #region Static Interface

        private static com.spacepuppy.Collections.ObjectCachePool<Evaluator> _pool = new ObjectCachePool<Evaluator>(64);
        private static com.spacepuppy.Collections.ObjectCachePool<ReusableStringReader> _readerPool = new ObjectCachePool<ReusableStringReader>(64);

        public static Vector4 Eval(string command, object x)
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
            try
            {
                return obj.EvalStatement(command, x).x;
            }
            finally
            {
                _pool.Release(obj);
            }
        }

        public static UnityEngine.Vector2 EvalVector2(string command, object x)
        {
            var obj = _pool.GetInstance();
            try
            {
                return ConvertUtil.ToVector2(obj.EvalStatement(command, x));
            }
            finally
            {
                _pool.Release(obj);
            }
        }

        public static UnityEngine.Vector3 EvalVector3(string command, object x)
        {
            var obj = _pool.GetInstance();
            try
            {
                return ConvertUtil.ToVector3(obj.EvalStatement(command, x));
            }
            finally
            {
                _pool.Release(obj);
            }
        }

        public static Quaternion EvalQuaternion(string command, object x)
        {
            var obj = _pool.GetInstance();
            try
            {
                return ConvertUtil.ToQuaternion(obj.EvalStatement(command, x));
            }
            finally
            {
                _pool.Release(obj);
            }
        }

        public static UnityEngine.Color EvalColor(string command, object x)
        {
            var obj = _pool.GetInstance();
            try
            {
                return ConvertUtil.ToColor(obj.EvalStatement(command, x));
            }
            finally
            {
                _pool.Release(obj);
            }
        }

        public static bool EvalBool(string command, object x)
        {
            var obj = _pool.GetInstance();
            try
            {
                return ConvertUtil.ToBool(obj.EvalStatement(command, x).x);
            }
            finally
            {
                _pool.Release(obj);
            }
        }

        public static UnityEngine.Rect EvalRect(string command, object x)
        {
            var obj = _pool.GetInstance();
            try
            {
                var v = obj.EvalStatement(command, x);
                return new UnityEngine.Rect(v.x, v.y, v.z, v.w);
            }
            finally
            {
                _pool.Release(obj);
            }
        }

        public static string EvalString(string command, object x)
        {
            var obj = _pool.GetInstance();
            try
            {
                var r = _readerPool.GetInstance();
                r.Reset(command);

                obj._reader = r;
                obj._x = x;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;

                State t1;
                bool t2;
                Vector4 result = obj.EvalStatement(out t1, out t2);

                obj._reader.Dispose();
                _readerPool.Release(obj._reader as ReusableStringReader);
                obj._x = null;
                obj._strBuilder.Length = 0;
                obj._parenCount = 0;
                obj._current = (char)0;
                
                switch(t1)
                {
                    case State.None:
                        return result.ToString();
                    case State.Scalar:
                        return result.x.ToString();
                    case State.Vector:
                        return result.ToDetailedString();
                    case State.Quaternion:
                        return ConvertUtil.ToQuaternion(result).eulerAngles.ToDetailedString();
                    default:
                        return result.ToString();
                }
            }
            finally
            {
                _pool.Release(obj);
            }
        }

        #endregion


        #region Fields

        private System.IO.TextReader _reader;
        private object _x;
        private StringBuilder _strBuilder = new StringBuilder();
        private int _parenCount;
        private char _current;


        private enum State
        {
            None,
            Scalar,
            Vector,
            Quaternion
        }

        #endregion

        #region Methods

        public Vector4 EvalStatement(string command, object x)
        {
            var r = _readerPool.GetInstance();
            r.Reset(command);

            _reader = r;
            _x = x;
            _strBuilder.Length = 0;
            _parenCount = 0;
            _current = (char)0;

            State t1;
            bool t2;
            Vector4 result = this.EvalStatement(out t1, out t2);

            _reader.Dispose();
            _readerPool.Release(_reader as ReusableStringReader);
            _x = null;
            _strBuilder.Length = 0;
            _parenCount = 0;
            _current = (char)0;

            return result;
        }

        public Vector4 EvalStatement(System.IO.TextReader command, object x)
        {
            if (command == null) throw new System.ArgumentNullException("command");

            _reader = command;
            _x = x;
            _strBuilder.Length = 0;
            _parenCount = 0;
            _current = (char)0;

            State t1;
            bool t2;
            Vector4 result = this.EvalStatement(out t1, out t2);

            _reader.Dispose();
            _x = null;
            _strBuilder.Length = 0;
            _parenCount = 0;
            _current = (char)0;

            return result;
        }


        private Vector4 EvalStatement(out State state, out bool reachedEndOfParams, bool requireClosingParen = false)
        {
            Vector4 result = this.EvalNextValue(out state);

            if (_current == ')')
            {
                int c = _reader.Read();
                if (c >= 0) _current = (char)c;
                _parenCount--;
                reachedEndOfParams = true;
                return result;
            }

            State temp;
            Vector4 v;
            for (int i = _current; i >= 0; i = _reader.Read())
            {
                _current = (char)i;

                if (char.IsWhiteSpace(_current)) continue;

                switch (_current)
                {
                    case '+':
                        v = this.EvalNextValue(out temp);
                        result = DoSum(result, v, state, temp, out state);
                        break;
                    case '-':
                        v = this.EvalNextValue(out temp);
                        result = DoMinus(result, v, state, temp, out state);
                        break;
                    case '*':
                        v = this.EvalNextValue(out temp);
                        result = DoProduct(result, v, state, temp, out state);
                        break;
                    case '/':
                        v = this.EvalNextValue(out temp);
                        result = DoDivide(result, v, state, temp, out state);
                        break;
                    case '^':
                        state = State.Scalar;
                        v = this.EvalNextValue(out temp);
                        result = new Vector4((float)Math.Pow(result.x, v.x), 0f);
                        break;
                    case '%':
                        state = State.Scalar;
                        v = this.EvalNextValue(out temp);
                        result = new Vector4(result.x % v.x, 0f);
                        break;
                    case '=':
                        {
                            if (_reader.Peek() == '=')
                            {
                                _reader.Read();
                                v = this.EvalNextValue(out temp);
                                result = DoEquals(result, v, state, temp, out state);
                            }
                            else
                            {
                                v = this.EvalNextValue(out temp);
                                result = DoEquals(result, v, state, temp, out state);
                            }
                        }
                        break;
                    case '!':
                        {
                            if (_reader.Peek() == '=')
                            {
                                _reader.Read();
                                v = this.EvalNextValue(out temp);
                                result = DoNotEquals(result, v, state, temp, out state);
                            }
                            else
                            {
                                throw new System.InvalidOperationException("Failed to parse the command.");
                            }
                        }
                        break;
                    case '<':
                        {
                            state = State.None;
                            if (_reader.Peek() == '=')
                            {
                                _reader.Read();
                                v = this.EvalNextValue(out temp);
                                result = (result.x <= v.x) ? Vector4.one : Vector4.zero;
                            }
                            else
                            {
                                v = this.EvalNextValue(out temp);
                                result = (result.x < v.x) ? Vector4.one : Vector4.zero;
                            }
                        }
                        break;
                    case '>':
                        {
                            state = State.None;
                            if (_reader.Peek() == '=')
                            {
                                _reader.Read();
                                v = this.EvalNextValue(out temp);
                                result = (result.x >= v.x) ? Vector4.one : Vector4.zero;
                            }
                            else
                            {
                                v = this.EvalNextValue(out temp);
                                result = (result.x > v.x) ? Vector4.one : Vector4.zero;
                            }
                        }
                        break;
                    case '|':
                        {
                            v = this.EvalNextValue(out temp);
                            if (_reader.Peek() == '|')
                            {
                                _reader.Read();
                                state = State.None;
                                result = (ConvertUtil.ToBool(result.x) || ConvertUtil.ToBool(v.x)) ? Vector4.one : Vector4.zero;
                            }
                            else
                            {
                                state = State.Scalar;
                                result = new Vector4((float)((int)result.x | (int)v.x), 0f);
                            }
                        }
                        break;
                    case '&':
                        {
                            v = this.EvalNextValue(out temp);
                            if (_reader.Peek() == '&')
                            {
                                _reader.Read();
                                state = State.None;
                                result = (ConvertUtil.ToBool(result.x) && ConvertUtil.ToBool(v.x)) ? Vector4.one : Vector4.zero;
                            }
                            else
                            {
                                state = State.Scalar;
                                result = new Vector4((float)((int)result.x & (int)v.x), 0f);
                            }
                        }
                        break;
                    case ',':
                        //reached the end of the first parameter
                        reachedEndOfParams = false;
                        return result;
                    case ')':
                        //reached the end of the statement
                        int c = _reader.Read();
                        if (c >= 0) _current = (char)c;
                        _parenCount--;
                        reachedEndOfParams = true;
                        return result;
                }

                if (_current == ',')
                {
                    reachedEndOfParams = false;
                    return result;
                }
                if (_current == ')')
                {
                    int c = _reader.Read();
                    if (c >= 0) _current = (char)c;
                    _parenCount--;
                    reachedEndOfParams = true;
                    return result;
                }
            }

            //ran out of statement with no errors, must be the end
            if (requireClosingParen)
                throw new System.InvalidOperationException("Failed to parse the command.");

            reachedEndOfParams = true;
            return result;
        }

        private Vector4 EvalNextValue(out State state)
        {
            int i = _reader.Read();
            for (; i >= 0 && char.IsWhiteSpace((char)i); i = _reader.Read())
            {
            }
            if (i < 0)
            {
                state = State.None;
                return Vector4.zero;
            }

            _current = (char)i;
            if (!IsValidWordPrefix(_current)) throw new System.InvalidOperationException("Failed to parse the command.");

            if (char.IsDigit(_current))
            {
                state = State.Scalar;
                return new Vector4(EvalNumber(), 0f);
            }
            if (char.IsLetter(_current))
            {
                return EvalFunc(out state);
            }

            switch (_current)
            {
                case '$':
                    return this.EvalVariable(out state);
                case '(':
                    _parenCount++;
                    bool temp;
                    return this.EvalStatement(out state, out temp);
                case '+':
                    return this.EvalNextValue(out state);
                case '-':
                    var result = this.EvalNextValue(out state);
                    if (state == State.Quaternion)
                        result = ConvertUtil.ToVector4(Quaternion.Inverse(ConvertUtil.ToQuaternion(result)));
                    else
                        result = -result;
                    return result;
                case ')':
                    state = State.None;
                    return Vector4.zero;
                default:
                    throw new System.InvalidOperationException("Failed to parse the command.");
            }
        }

        private float EvalNumber()
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
                return (float)((double)high + ((double)low / Math.Pow(10, lowLen)));
            else
                return (float)high;
        }

        private Vector4 EvalVariable(out State state)
        {
            _strBuilder.Length = 0;
            int i = _reader.Read();
            if (i < 0)
            {
                return SmartConvertToVector(_x, out state);
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
                    else if (char.IsWhiteSpace(_current) || IsArithmeticSymbol(_current) || _current == ')' || _current == ',' || _current == ']')
                        break;
                    else
                        throw new System.InvalidOperationException("Failed to parse the command.");
                }

                sprop = _strBuilder.ToString();
                _strBuilder.Length = 0;
                return SmartConvertToVector(DynamicUtil.GetValue(target, sprop), out state);
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
                        state = State.None;
                        return Vector4.one;
                    case "false":
                        state = State.None;
                        return Vector4.zero;
                    case "pi":
                        state = State.Scalar;
                        return new Vector4((float)System.Math.PI, 0f);
                    case "2pi":
                        const float TWO_PI = (float)(System.Math.PI * 2d);
                        state = State.Scalar;
                        return new Vector4(TWO_PI, 0f);
                    case "pi_2":
                        const float PI_TWO = (float)(System.Math.PI / 2d);
                        state = State.Scalar;
                        return new Vector4(PI_TWO, 0f);
                    case "rad2deg":
                        const float RAD2DEG = (float)(180d / System.Math.PI);
                        state = State.Scalar;
                        return new Vector4(RAD2DEG, 0f);
                    case "deg2rad":
                        const float DEG2RAD = (float)(System.Math.PI / 180d);
                        state = State.Scalar;
                        return new Vector4(DEG2RAD, 0f);
                    case "secsinmin":
                        state = State.Scalar;
                        return new Vector4(60f, 0f);
                    case "secsinhour":
                        state = State.Scalar;
                        return new Vector4(3600f, 0f);
                    case "secsinday":
                        state = State.Scalar;
                        return new Vector4(86400f, 0f);
                    case "secsinweek":
                        state = State.Scalar;
                        return new Vector4(604800f, 0f);
                    case "secsinyear":
                        state = State.Scalar;
                        return new Vector4(31536000f, 0f);
                    case "infinity":
                    case "inf":
                        state = State.Scalar;
                        return Vector4.one * float.PositiveInfinity;
                    case "-infinity":
                    case "-inf":
                        state = State.Scalar;
                        return Vector4.one * float.NegativeInfinity;
                    default:
                        state = State.None;
                        return Vector4.zero;
                }
            }
            else if (char.IsWhiteSpace(_current) || IsArithmeticSymbol(_current) || _current == ')' || _current == ',' || _current == ']')
            {
                return SmartConvertToVector(_x, out state);
            }
            else
            {
                throw new System.InvalidOperationException("Failed to parse the command.");
            }

        }

        private Vector4 EvalFunc(out State state)
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

            State temp;
            bool reachedEnd;
            switch (name)
            {
                case "abs":
                    {
                        var result = Math.Abs(EvalStatement(out temp, out reachedEnd, true).x);
                        if (!reachedEnd)
                            throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                        state = State.Scalar;
                        return new Vector4((float)result, 0f);
                    }
                case "sqrt":
                    {
                        var result = Math.Sqrt(EvalStatement(out temp, out reachedEnd, true).x);
                        if (!reachedEnd)
                            throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                        state = State.Scalar;
                        return new Vector4((float)result, 0f);
                    }
                case "cos":
                    {
                        var result = Math.Cos(EvalStatement(out temp, out reachedEnd, true).x);
                        if (!reachedEnd)
                            throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                        state = State.Scalar;
                        return new Vector4((float)result, 0f);
                    }
                case "sin":
                    {
                        var result = Math.Sin(EvalStatement(out temp, out reachedEnd, true).x);
                        if (!reachedEnd)
                            throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                        state = State.Scalar;
                        return new Vector4((float)result, 0f);
                    }
                case "tan":
                    {
                        var result = Math.Tan(EvalStatement(out temp, out reachedEnd, true).x);
                        if (!reachedEnd)
                            throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                        state = State.Scalar;
                        return new Vector4((float)result, 0f);
                    }
                case "acos":
                    {
                        var result = Math.Acos(EvalStatement(out temp, out reachedEnd, true).x);
                        if (!reachedEnd)
                            throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                        state = State.Scalar;
                        return new Vector4((float)result, 0f);
                    }
                case "asin":
                    {
                        var result = Math.Asin(EvalStatement(out temp, out reachedEnd, true).x);
                        if (!reachedEnd)
                            throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                        state = State.Scalar;
                        return new Vector4((float)result, 0f);
                    }
                case "atan":
                    {
                        var result = Math.Atan(EvalStatement(out temp, out reachedEnd, true).x);
                        if (!reachedEnd)
                            throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                        state = State.Scalar;
                        return new Vector4((float)result, 0f);
                    }
                case "atan2":
                    {
                        float y = this.EvalStatement(out temp, out reachedEnd, true).x;
                        if (reachedEnd)
                            throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                        float x = this.EvalStatement(out temp, out reachedEnd, true).x;
                        if (!reachedEnd)
                            throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                        state = State.Scalar;
                        return new Vector4((float)Math.Atan2(y, x), 0f);
                    }
                case "rand":
                    {
                        var x = this.EvalStatement(out state, out reachedEnd, true);
                        if (state == State.Quaternion)
                        {
                            state = State.Quaternion;
                            if (reachedEnd)
                                return ConvertUtil.ToVector4(Quaternion.Slerp(Quaternion.identity, ConvertUtil.ToQuaternion(x), RandomUtil.Standard.Next()));
                            var y = this.EvalStatement(out temp, out reachedEnd, true);
                            if (reachedEnd)
                                return ConvertUtil.ToVector4(Quaternion.Slerp(ConvertUtil.ToQuaternion(x), ConvertUtil.ToQuaternion(y), RandomUtil.Standard.Next()));
                            throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                        }
                        else
                        {
                            if (reachedEnd)
                                return x * RandomUtil.Standard.Next();
                            var y = this.EvalStatement(out temp, out reachedEnd, true);
                            if (reachedEnd)
                                return RandomUtil.Standard.Next() * (y - x) + x;
                            throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                        }
                    }
                case "randint":
                    {
                        state = State.Scalar;
                        int x = (int)this.EvalStatement(out temp, out reachedEnd, true).x;
                        if (reachedEnd)
                            return new Vector4(RandomUtil.Standard.Next(x), 0f);
                        int y = (int)this.EvalStatement(out temp, out reachedEnd, true).x;
                        if (!reachedEnd)
                            throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                        return new Vector4(RandomUtil.Standard.Next(x, y), 0f);
                    }
                case "vec":
                    {
                        state = State.Vector;
                        float x = this.EvalStatement(out temp, out reachedEnd, true).x;
                        if (reachedEnd)
                            return new Vector4(x, 0f);
                        float y = this.EvalStatement(out temp, out reachedEnd, true).x;
                        if (reachedEnd)
                            return new Vector4(x, y);
                        float z = this.EvalStatement(out temp, out reachedEnd, true).x;
                        if (reachedEnd)
                            return new Vector4(x, y, z);
                        float w = this.EvalStatement(out temp, out reachedEnd, true).x;
                        if (reachedEnd)
                            return new Vector4(x, y, z, w);
                        throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                    }
                case "rot":
                    {
                        state = State.Quaternion;
                        float x = this.EvalStatement(out temp, out reachedEnd, true).x;
                        if (reachedEnd)
                            return ConvertUtil.ToVector4(Quaternion.Euler(x, 0f, 0f));
                        float y = this.EvalStatement(out temp, out reachedEnd, true).x;
                        if (reachedEnd)
                            return ConvertUtil.ToVector4(Quaternion.Euler(x, y, 0f));
                        float z = this.EvalStatement(out temp, out reachedEnd, true).x;
                        if (reachedEnd)
                            return ConvertUtil.ToVector4(Quaternion.Euler(x, y, z));
                        throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                    }
                default:
                    throw new System.InvalidOperationException("Failed to parse the command: Unknown Function");
            }
        }

        #endregion

        #region Utils

        private static State GetStateTypeOfObject(object obj)
        {
            if (obj == null) return State.None;

            switch (VariantReference.GetVariantType(obj.GetType()))
            {
                case VariantType.Object:
                case VariantType.Null:
                case VariantType.String:
                case VariantType.Boolean:
                    return State.None;
                case VariantType.Integer:
                case VariantType.Float:
                case VariantType.Double:
                    return State.Scalar;
                case VariantType.Vector2:
                case VariantType.Vector3:
                case VariantType.Vector4:
                    return State.Vector;
                case VariantType.Quaternion:
                    return State.Quaternion;
                case VariantType.Color:
                    return State.Vector;
                case VariantType.DateTime:
                case VariantType.GameObject:
                case VariantType.Component:
                    return State.None;
                case VariantType.LayerMask:
                    return State.Scalar;
                case VariantType.Rect:
                    return State.Vector;
                default:
                    return State.None;
            }
        }

        private static Vector4 SmartConvertToVector(object obj, out State state)
        {
            state = GetStateTypeOfObject(obj);
            switch (state)
            {
                case State.None:
                    return ConvertUtil.ToVector4(obj);
                case State.Scalar:
                    return new Vector4(ConvertUtil.ToSingle(obj), 0f);
                case State.Vector:
                case State.Quaternion:
                    return ConvertUtil.ToVector4(obj);
                default:
                    return Vector4.zero;
            }
        }



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
            return char.IsLetterOrDigit(c) || c == '$' || c == '_' || c == '+' || c == '-' || c == '(';
        }



        private static Vector4 DoSum(Vector4 left, Vector4 right, State sleft, State sright, out State state)
        {
            state = State.None;
            Vector4 result = Vector4.zero;

            switch (sleft)
            {
                case State.None:
                    switch (sright)
                    {
                        case State.None:
                        case State.Vector:
                        case State.Scalar:
                            state = sright;
                            result = left + right;
                            break;
                        case State.Quaternion:
                            state = State.None;
                            result = left + right;
                            break;
                    }
                    break;
                case State.Scalar:
                    switch (sright)
                    {
                        case State.None:
                        case State.Scalar:
                        case State.Vector:
                        case State.Quaternion:
                            state = State.Scalar;
                            result.x = left.x + right.x;
                            break;
                    }
                    break;
                case State.Vector:
                    switch (sright)
                    {
                        case State.None:
                            state = State.None;
                            result = left + right;
                            break;
                        case State.Scalar:
                            state = State.Vector;
                            result.x = left.x + right.x;
                            break;
                        case State.Vector:
                            state = State.Vector;
                            result = left + right;
                            break;
                        case State.Quaternion:
                            state = State.None;
                            result = left + right;
                            break;
                    }
                    break;
                case State.Quaternion:
                    switch (sright)
                    {
                        case State.None:
                        case State.Scalar:
                        case State.Vector:
                        case State.Quaternion:
                            state = State.None;
                            result = left + right;
                            break;
                    }
                    break;
            }

            return result;
        }

        private static Vector4 DoMinus(Vector4 left, Vector4 right, State sleft, State sright, out State state)
        {
            state = State.None;
            Vector4 result = Vector4.zero;

            switch (sleft)
            {
                case State.None:
                    switch (sright)
                    {
                        case State.None:
                        case State.Vector:
                        case State.Scalar:
                            state = sright;
                            result = left - right;
                            break;
                        case State.Quaternion:
                            state = State.None;
                            result = left - right;
                            break;
                    }
                    break;
                case State.Scalar:
                    switch (sright)
                    {
                        case State.None:
                        case State.Scalar:
                        case State.Vector:
                        case State.Quaternion:
                            state = State.Scalar;
                            result.x = left.x - right.x;
                            break;
                    }
                    break;
                case State.Vector:
                    switch (sright)
                    {
                        case State.None:
                            state = State.None;
                            result = left - right;
                            break;
                        case State.Scalar:
                            state = State.Vector;
                            result.x = left.x - right.x;
                            break;
                        case State.Vector:
                            state = State.Vector;
                            result = left - right;
                            break;
                        case State.Quaternion:
                            state = State.None;
                            result = left - right;
                            break;
                    }
                    break;
                case State.Quaternion:
                    switch (sright)
                    {
                        case State.None:
                        case State.Scalar:
                        case State.Vector:
                        case State.Quaternion:
                            state = State.None;
                            result = left - right;
                            break;
                    }
                    break;
            }

            return result;
        }

        private static Vector4 DoProduct(Vector4 left, Vector4 right, State sleft, State sright, out State state)
        {
            state = State.None;
            Vector4 result = Vector4.zero;

            switch (sleft)
            {
                case State.None:
                    switch (sright)
                    {
                        case State.None:
                        case State.Vector:
                            state = State.None;
                            result.x = left.x * right.x;
                            result.y = left.y * right.y;
                            result.z = left.z * right.z;
                            result.w = left.w * right.w;
                            break;
                        case State.Scalar:
                            state = State.Scalar;
                            result.x = left.x * right.x;
                            break;
                        case State.Quaternion:
                            state = State.Quaternion;
                            result = ConvertUtil.ToVector4(ConvertUtil.ToQuaternion(left) * ConvertUtil.ToQuaternion(right));
                            break;
                    }
                    break;
                case State.Scalar:
                    switch (sright)
                    {
                        case State.None:
                        case State.Scalar:
                            state = State.Scalar;
                            result.x = left.x * right.x;
                            break;
                        case State.Vector:
                            state = State.Vector;
                            result = right * left.x;
                            break;
                        case State.Quaternion:
                            state = State.None;
                            result = right * left.x;
                            break;
                    }
                    break;
                case State.Vector:
                    switch (sright)
                    {
                        case State.None:
                        case State.Scalar:
                            state = State.Vector;
                            result = left * right.x;
                            break;
                        case State.Vector:
                            state = State.None;
                            result.x = left.x * right.x;
                            result.y = left.y * right.y;
                            result.z = left.z * right.z;
                            result.w = left.w * right.w;
                            break;
                        case State.Quaternion:
                            state = State.Quaternion;
                            result = ConvertUtil.ToVector4(ConvertUtil.ToQuaternion(right) * left);
                            break;
                    }
                    break;
                case State.Quaternion:
                    switch (sright)
                    {
                        case State.None:
                            state = State.None;
                            result.x = left.x * right.x;
                            result.y = left.y * right.y;
                            result.z = left.z * right.z;
                            result.w = left.w * right.w;
                            break;
                        case State.Scalar:
                            state = State.None;
                            result = left * right.x;
                            break;
                        case State.Vector:
                            state = State.Vector;
                            result = ConvertUtil.ToVector4(ConvertUtil.ToQuaternion(left) * right);
                            break;
                        case State.Quaternion:
                            state = State.Quaternion;
                            result = ConvertUtil.ToVector4(ConvertUtil.ToQuaternion(left) * ConvertUtil.ToQuaternion(right));
                            break;
                    }
                    break;
            }

            return result;
        }

        private static Vector4 DoDivide(Vector4 left, Vector4 right, State sleft, State sright, out State state)
        {
            state = State.None;
            Vector4 result = Vector4.zero;

            switch (sleft)
            {
                case State.None:
                    switch (sright)
                    {
                        case State.None:
                        case State.Vector:
                            state = State.None;
                            result.x = left.x / right.x;
                            result.y = left.y / right.y;
                            result.z = left.z / right.z;
                            result.w = left.w / right.w;
                            break;
                        case State.Scalar:
                            state = State.Scalar;
                            result.x = left.x / right.x;
                            break;
                        case State.Quaternion:
                            state = State.Quaternion;
                            result = ConvertUtil.ToVector4(ConvertUtil.ToQuaternion(left) * Quaternion.Inverse(ConvertUtil.ToQuaternion(right)));
                            break;
                    }
                    break;
                case State.Scalar:
                    switch (sright)
                    {
                        case State.None:
                        case State.Scalar:
                            state = State.Scalar;
                            result.x = left.x / right.x;
                            break;
                        case State.Vector:
                            state = State.Vector;
                            result = right / left.x;
                            break;
                        case State.Quaternion:
                            state = State.None;
                            result = ConvertUtil.ToVector4(Quaternion.Inverse(ConvertUtil.ToQuaternion(right))) * left.x;
                            break;
                    }
                    break;
                case State.Vector:
                    switch (sright)
                    {
                        case State.None:
                        case State.Scalar:
                            state = State.Vector;
                            result = left / right.x;
                            break;
                        case State.Vector:
                            state = State.None;
                            result.x = left.x / right.x;
                            result.y = left.y / right.y;
                            result.z = left.z / right.z;
                            result.w = left.w / right.w;
                            break;
                        case State.Quaternion:
                            state = State.Quaternion;
                            result = ConvertUtil.ToVector4(Quaternion.Inverse(ConvertUtil.ToQuaternion(right)) * left);
                            break;
                    }
                    break;
                case State.Quaternion:
                    switch (sright)
                    {
                        case State.None:
                        case State.Vector:
                            state = State.None;
                            result.x = left.x / right.x;
                            result.y = left.y / right.y;
                            result.z = left.z / right.z;
                            result.w = left.w / right.w;
                            break;
                        case State.Scalar:
                            state = State.None;
                            result = left / right.x;
                            break;
                        case State.Quaternion:
                            state = State.Quaternion;
                            result = ConvertUtil.ToVector4(ConvertUtil.ToQuaternion(left) * Quaternion.Inverse(ConvertUtil.ToQuaternion(right)));
                            break;
                    }
                    break;
            }

            return result;
        }

        private static Vector4 DoEquals(Vector4 left, Vector4 right, State sleft, State sright, out State state)
        {
            state = State.None;
            Vector4 result = Vector4.zero;

            switch (sleft)
            {
                case State.Scalar:
                    state = State.None;
                    result = MathUtil.FuzzyEqual(left.x, right.x) ? Vector4.one : Vector4.zero;
                    break;
                default:
                    state = State.None;
                    result = VectorUtil.FuzzyEquals(left, right) ? Vector4.one : Vector4.zero;
                    break;
            }
            return result;
        }

        private static Vector4 DoNotEquals(Vector4 left, Vector4 right, State sleft, State sright, out State state)
        {
            state = State.None;
            Vector4 result = Vector4.zero;

            switch (sleft)
            {
                case State.Scalar:
                    state = State.None;
                    result = MathUtil.FuzzyEqual(left.x, right.x) ? Vector4.zero : Vector4.one;
                    break;
                default:
                    state = State.None;
                    result = VectorUtil.FuzzyEquals(left, right) ? Vector4.zero : Vector4.one;
                    break;
            }
            return result;
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
                return ConvertUtil.ToPrim(MathUtil.Interpolate(ConvertUtil.ToSingle(a), ConvertUtil.ToSingle(b), t), atp);
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

            return false;
        }

        #endregion

    }

}
