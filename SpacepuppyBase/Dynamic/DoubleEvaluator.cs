using System;
using System.Text;

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
    /// Abs(...)
    /// Sqrt(...)
    /// Cos(...)
    /// Sin(...)
    /// Tan(...)
    /// Acos(...)
    /// Asin(...)
    /// Atan(...)
    /// Atan2(y, x)
    /// Rand(max)
    /// Rand(min, max)
    /// RandInt(max)
    /// RandInt(min, max)
    /// 
    /// These function names are not case sensitive
    /// 
    /// #Variables
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
    /// $fixeddeltatime
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
    /// </summary>
    public class DoubleEvaluator
    {

        #region Static Interface

        private static com.spacepuppy.Collections.ObjectCachePool<DoubleEvaluator> _pool = new ObjectCachePool<DoubleEvaluator>(64);
        private static com.spacepuppy.Collections.ObjectCachePool<ReusableStringReader> _readerPool = new ObjectCachePool<ReusableStringReader>(64);

        public static double Eval(string command, object x)
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
        
        #endregion


        #region Fields

        private System.IO.TextReader _reader;
        private object _x;
        private StringBuilder _strBuilder = new StringBuilder();
        private int _parenCount;
        private char _current;

        #endregion

        #region Methods

        public double EvalStatement(string command, object x)
        {
            var r = _readerPool.GetInstance();
            r.Reset(command);

            _reader = r;
            _x = x;
            _strBuilder.Length = 0;
            _parenCount = 0;
            _current = (char)0;

            double result = this.EvalStatement();

            _reader.Dispose();
            _readerPool.Release(_reader as ReusableStringReader);
            _x = null;
            _strBuilder.Length = 0;
            _parenCount = 0;
            _current = (char)0;

            return result;
        }

        public double EvalStatement(System.IO.TextReader command, object x)
        {
            if (command == null) throw new System.ArgumentNullException("command");

            _reader = command;
            _x = x;
            _strBuilder.Length = 0;
            _parenCount = 0;
            _current = (char)0;

            double result = this.EvalStatement();

            _reader.Dispose();
            _x = null;
            _strBuilder.Length = 0;
            _parenCount = 0;
            _current = (char)0;

            return result;
        }

        private double EvalStatement()
        {
            double result = this.EvalNextValue();

            if (_current == ')')
            {
                int c = _reader.Read();
                if (c >= 0) _current = (char)c;
                _parenCount--;
                return result;
            }

            for (int i = _current; i >= 0; i = _reader.Read())
            {
                _current = (char)i;

                if (char.IsWhiteSpace(_current)) continue;

                switch (_current)
                {
                    case '+':
                        result += this.EvalNextValue();
                        break;
                    case '*':
                        result *= this.EvalNextValue();
                        break;
                    case '-':
                        result -= this.EvalNextValue();
                        break;
                    case '/':
                        result /= this.EvalNextValue();
                        break;
                    case '^':
                        result = Math.Pow(result, this.EvalNextValue());
                        break;
                    case '%':
                        result %= this.EvalNextValue();
                        break;
                    case '=':
                        {
                            if (_reader.Peek() == '=')
                            {
                                _reader.Read();
                                result = MathUtil.FuzzyEqual((float)result, (float)this.EvalNextValue()) ? 1d : 0d;
                            }
                            else
                            {
                                result = MathUtil.FuzzyEqual((float)result, (float)this.EvalNextValue()) ? 1d : 0d;
                            }
                        }
                        break;
                    case '<':
                        {
                            if (_reader.Peek() == '=')
                            {
                                _reader.Read();
                                result = (result <= this.EvalNextValue()) ? 1d : 0d;
                            }
                            else
                            {
                                result = (result < this.EvalNextValue()) ? 1d : 0d;
                            }
                        }
                        break;
                    case '>':
                        {
                            if (_reader.Peek() == '=')
                            {
                                _reader.Read();
                                result = (result >= this.EvalNextValue()) ? 1d : 0d;
                            }
                            else
                            {
                                result = (result > this.EvalNextValue()) ? 1d : 0d;
                            }
                        }
                        break;
                    case '|':
                        {
                            if (_reader.Peek() == '|')
                            {
                                _reader.Read();
                                result = (ConvertUtil.ToBool(result) || ConvertUtil.ToBool(this.EvalNextValue())) ? 1d : 0d;
                            }
                            else
                            {
                                result = (double)((int)result | (int)this.EvalNextValue());
                            }
                        }
                        break;
                    case '&':
                        {
                            if (_reader.Peek() == '&')
                            {
                                _reader.Read();
                                result = (ConvertUtil.ToBool(result) && ConvertUtil.ToBool(this.EvalNextValue())) ? 1d : 0d;
                            }
                            else
                            {
                                result = (double)((int)result & (int)this.EvalNextValue());
                            }
                        }
                        break;
                    case ')':
                        //reached the end of the statement
                        i = _reader.Read();
                        if (i >= 0) _current = (char)i;
                        return result;
                }

                if (_current == ')')
                {
                    int c = _reader.Read();
                    if (c >= 0) _current = (char)c;
                    _parenCount--;
                    return result;
                }
            }

            //ran out of statement with no errors, must be the end
            return result;
        }

        private double EvalNextValue()
        {
            int i = _reader.Read();
            for (; i >= 0 && char.IsWhiteSpace((char)i); i = _reader.Read())
            {
            }
            if (i < 0) return 0d;

            _current = (char)i;
            if (!IsValidWordPrefix(_current)) throw new System.InvalidOperationException("Failed to parse the command.");

            if (char.IsDigit(_current))
            {
                return EvalNumber();
            }
            if (char.IsLetter(_current))
            {
                return EvalFunc();
            }

            switch (_current)
            {
                case '$':
                    return this.EvalVariable();
                case '(':
                    _parenCount++;
                    return this.EvalStatement();
                case '+':
                    return this.EvalNextValue();
                case '-':
                    return -this.EvalNextValue();
                case ')':
                    return 0d;
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
                return (double)high + ((double)low / Math.Pow(10, lowLen));
            else
                return (double)high;
        }

        private double EvalVariable()
        {
            _strBuilder.Length = 0;
            int i = _reader.Read();
            if (i < 0) return ConvertUtil.ToDouble(_x);

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
                    else if (char.IsWhiteSpace(_current) || IsArithmeticSymbol(_current) || _current == ')' || _current == ',')
                        break;
                    else
                        throw new System.InvalidOperationException("Failed to parse the command.");
                }

                sprop = _strBuilder.ToString();
                _strBuilder.Length = 0;
                return ConvertUtil.ToDouble(DynamicUtil.GetValue(target, sprop));
            }
            else if (_current == '(')
            {
                for (i = _reader.Read(); i >= 0; i = _reader.Read())
                {
                    _current = (char)i;

                    if (char.IsLetterOrDigit(_current) || _current == '_')
                    {
                        _strBuilder.Append(_current);
                    }
                    else if (_current == ')')
                    {
                        break;
                    }
                    else
                        throw new System.InvalidOperationException("Failed to parse the command.");
                }

                i = _reader.Read();
                if (i < 0 || (char)i != '.')
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
                    else if (char.IsWhiteSpace(_current) || IsArithmeticSymbol(_current) || _current == ')' || _current == ',' || _current == ']')
                        break;
                    else
                        throw new System.InvalidOperationException("Failed to parse the command.");
                }

                sprop = _strBuilder.ToString();
                _strBuilder.Length = 0;
                return ConvertUtil.ToDouble(DynamicUtil.GetValue(target, sprop));
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
                    else if (char.IsWhiteSpace(_current) || IsArithmeticSymbol(_current) || _current == ')' || _current == ',')
                        break;
                    else
                        throw new System.InvalidOperationException("Failed to parse the command.");
                }


                var str = _strBuilder.ToString();
                _strBuilder.Length = 0;

                switch (str)
                {
                    case "pi":
                        return System.Math.PI;
                    case "2pi":
                        const double TWO_PI = System.Math.PI * 2d;
                        return TWO_PI;
                    case "pi_2":
                        const double PI_TWO = System.Math.PI / 2d;
                        return PI_TWO;
                    case "rad2deg":
                        const double RAD2DEG = 180d / System.Math.PI;
                        return RAD2DEG;
                    case "deg2rad":
                        const double DEG2RAD = System.Math.PI / 180d;
                        return DEG2RAD;
                    case "secsinmin":
                        return 60d;
                    case "secsinhour":
                        return 3600d;
                    case "secsinday":
                        return 86400d;
                    case "secsinweek":
                        return 604800d;
                    case "secsinyear":
                        return 31536000d;
                    case "infinity":
                    case "inf":
                        return double.PositiveInfinity;
                    case "-infinity":
                    case "-inf":
                        return double.NegativeInfinity;
                    case "time":
                        return UnityEngine.Time.time;
                    case "unscaledtime":
                        return UnityEngine.Time.unscaledTime;
                    case "fixedtime":
                        return UnityEngine.Time.fixedTime;
                    case "deltatime":
                        return UnityEngine.Time.deltaTime;
                    case "fixeddeltatime":
                        return UnityEngine.Time.fixedDeltaTime;
                    default:
                        return 0d;
                }
            }
            else if (char.IsWhiteSpace(_current) || IsArithmeticSymbol(_current) || _current == ')' || _current == ',')
            {
                return ConvertUtil.ToDouble(_x);
            }
            else
            {
                throw new System.InvalidOperationException("Failed to parse the command.");
            }

        }

        private double EvalFunc()
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

            bool reachedEnd;
            double result;
            switch (name)
            {
                case "abs":
                    result = Math.Abs(EvalParams(out reachedEnd));
                    if (!reachedEnd)
                        throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                    break;
                case "sqrt":
                    result = Math.Sqrt(EvalParams(out reachedEnd));
                    if (!reachedEnd)
                        throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                    break;
                case "cos":
                    result = Math.Cos(EvalParams(out reachedEnd));
                    if (!reachedEnd)
                        throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                    break;
                case "sin":
                    result = Math.Sin(EvalParams(out reachedEnd));
                    if (!reachedEnd)
                        throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                    break;
                case "tan":
                    result = Math.Tan(EvalParams(out reachedEnd));
                    if (!reachedEnd)
                        throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                    break;
                case "acos":
                    result = Math.Acos(EvalParams(out reachedEnd));
                    if (!reachedEnd)
                        throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                    break;
                case "asin":
                    result = Math.Asin(EvalParams(out reachedEnd));
                    if (!reachedEnd)
                        throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                    break;
                case "atan":
                    result = Math.Atan(EvalParams(out reachedEnd));
                    if (!reachedEnd)
                        throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                    break;
                case "atan2":
                    {
                        double y = this.EvalParams(out reachedEnd);
                        if (reachedEnd)
                            throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                        double x = this.EvalParams(out reachedEnd);
                        if (!reachedEnd)
                            throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                        result = Math.Atan2(y, x);
                    }
                    break;
                case "rand":
                    {
                        double x = this.EvalParams(out reachedEnd);
                        if (reachedEnd)
                            return RandomUtil.Standard.Range((float)x);
                        double y = this.EvalParams(out reachedEnd);
                        if (!reachedEnd)
                            throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                        result = RandomUtil.Standard.Range((float)y, (float)x);
                    }
                    break;
                case "randint":
                    {
                        double x = this.EvalParams(out reachedEnd);
                        if (reachedEnd)
                            return RandomUtil.Standard.Next((int)x);
                        double y = this.EvalParams(out reachedEnd);
                        if (!reachedEnd)
                            throw new System.InvalidOperationException("Failed to parse the command: Parameter count mismatch.");
                        result = RandomUtil.Standard.Next((int)x, (int)y);
                    }
                    break;
                default:
                    throw new System.InvalidOperationException("Failed to parse the command: Unknown Function");
            }
            return result;
        }

        private double EvalParams(out bool reachedEndOfParams, bool ignoreNecessityOfClosingParen = false)
        {
            double result = this.EvalNextValue();

            if (_current == ')')
            {
                int c = _reader.Read();
                if (c >= 0) _current = (char)c;
                _parenCount--;
                reachedEndOfParams = true;
                return result;
            }

            for (int i = _current; i >= 0; i = _reader.Read())
            {
                _current = (char)i;

                if (char.IsWhiteSpace(_current)) continue;

                switch (_current)
                {
                    case '+':
                        result += this.EvalNextValue();
                        break;
                    case '*':
                        result *= this.EvalNextValue();
                        break;
                    case '-':
                        result -= this.EvalNextValue();
                        break;
                    case '/':
                        result /= this.EvalNextValue();
                        break;
                    case '^':
                        result = Math.Pow(result, this.EvalNextValue());
                        break;
                    case '%':
                        result %= this.EvalNextValue();
                        break;
                    case '=':
                        {
                            if (_reader.Peek() == '=')
                            {
                                _reader.Read();
                                result = MathUtil.FuzzyEqual((float)result, (float)this.EvalNextValue()) ? 1d : 0d;
                            }
                            else
                            {
                                result = MathUtil.FuzzyEqual((float)result, (float)this.EvalNextValue()) ? 1d : 0d;
                            }
                        }
                        break;
                    case '<':
                        {
                            if (_reader.Peek() == '=')
                            {
                                _reader.Read();
                                result = (result <= this.EvalNextValue()) ? 1d : 0d;
                            }
                            else
                            {
                                result = (result < this.EvalNextValue()) ? 1d : 0d;
                            }
                        }
                        break;
                    case '>':
                        {
                            if (_reader.Peek() == '=')
                            {
                                _reader.Read();
                                result = (result >= this.EvalNextValue()) ? 1d : 0d;
                            }
                            else
                            {
                                result = (result > this.EvalNextValue()) ? 1d : 0d;
                            }
                        }
                        break;
                    case '|':
                        {
                            if (_reader.Peek() == '|')
                            {
                                _reader.Read();
                                result = (ConvertUtil.ToBool(result) || ConvertUtil.ToBool(this.EvalNextValue())) ? 1d : 0d;
                            }
                            else
                            {
                                result = (double)((int)result | (int)this.EvalNextValue());
                            }
                        }
                        break;
                    case '&':
                        {
                            if (_reader.Peek() == '&')
                            {
                                _reader.Read();
                                result = (ConvertUtil.ToBool(result) && ConvertUtil.ToBool(this.EvalNextValue())) ? 1d : 0d;
                            }
                            else
                            {
                                result = (double)((int)result & (int)this.EvalNextValue());
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
            if (ignoreNecessityOfClosingParen)
            {
                reachedEndOfParams = true;
                return result;
            }

            throw new System.InvalidOperationException("Failed to parse the command: malformed function parameters.");
        }

        #endregion

        #region Utils

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

        #endregion

    }
}
