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
    /// ##Functions
    /// Abs(...)
    /// Sqrt(...)
    /// Cos(...)
    /// Sin(...)
    /// Tan(...)
    /// Acos(...)
    /// Asin(...)
    /// Atan(...)
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

        private static com.spacepuppy.Collections.ObjectCachePool<Evaluator> _pool = new Collections.ObjectCachePool<Evaluator>(64);

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

        //private SamplingCharEnumerator _e = new SamplingCharEnumerator();
        private com.spacepuppy.Collections.ReusableStringReader _reader = new com.spacepuppy.Collections.ReusableStringReader();
        private object _x;
        private StringBuilder _strBuilder = new StringBuilder();
        private int _parenCount;
        private char _current;

        #endregion

        #region Methods

        public double EvalStatement(string command, object x)
        {
            _reader.Reset(command);
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
            int i = _reader.Read();
            for (; i >= 0 && char.IsWhiteSpace((char)i); i = _reader.Read())
            {
            }
            if (i < 0) return 0d;

            _current = (char)i;
            if(!IsValidWordPrefix(_current)) throw new System.InvalidOperationException("Failed to parse the command.");
            double result = this.EvalNextValue();

            if (_parenCount > 0 && _current == ')')
            {
                _parenCount--;
                return result;
            }

            for (i = _current; i >= 0; i = _reader.Read())
            {
                _current = (char)i;

                if (char.IsWhiteSpace(_current)) continue;

                switch(_current)
                {
                    case '+':
                        result += this.EvalStatement();
                        break;
                    case '*':
                        result *= this.EvalStatement();
                        break;
                    case '-':
                        result -= this.EvalStatement();
                        break;
                    case '/':
                        result /= this.EvalStatement();
                        break;
                    case '^':
                        result = Math.Pow(result, this.EvalStatement());
                        break;
                    case '%':
                        result %= this.EvalStatement();
                        break;
                    case '=':
                        {
                            if(_reader.Peek() == '=')
                            {
                                _reader.Read();
                                result = MathUtil.FuzzyEqual((float)result, (float)this.EvalStatement()) ? 1d : 0d;
                            }
                            else
                            {
                                result = MathUtil.FuzzyEqual((float)result, (float)this.EvalStatement()) ? 1d : 0d;
                            }
                        }
                        break;
                    case '<':
                        {
                            if (_reader.Peek() == '=')
                            {
                                _reader.Read();
                                result = (result <= this.EvalStatement()) ? 1d : 0d;
                            }
                            else
                            {
                                result = (result < this.EvalStatement()) ? 1d : 0d;
                            }
                        }
                        break;
                    case '>':
                        {
                            if (_reader.Peek() == '=')
                            {
                                _reader.Read();
                                result = (result >= this.EvalStatement()) ? 1d : 0d;
                            }
                            else
                            {
                                result = (result > this.EvalStatement()) ? 1d : 0d;
                            }
                        }
                        break;
                    case '|':
                        {
                            if (_reader.Peek() == '|')
                            {
                                _reader.Read();
                                result = (ConvertUtil.ToBool(result) || ConvertUtil.ToBool(this.EvalStatement())) ? 1d : 0d;
                            }
                            else
                            {
                                result = (double)((int)result | (int)this.EvalStatement());
                            }
                        }
                        break;
                    case '&':
                        {
                            if (_reader.Peek() == '&')
                            {
                                _reader.Read();
                                result = (ConvertUtil.ToBool(result) && ConvertUtil.ToBool(this.EvalStatement())) ? 1d : 0d;
                            }
                            else
                            {
                                result = (double)((int)result & (int)this.EvalStatement());
                            }
                        }
                        break;
                    case ')':
                        //reached the end of the statement
                        return result;
                }
            }

            //ran out of statement with no errors, must be the end
            return result;
        }

        private double EvalNextValue()
        {
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
                    return this.EvalStatement();
                case '-':
                    return -this.EvalStatement();
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
                    if(lowLen < 0)
                        high = (high * 10) + ((int)_current - CHAR_0);
                    else
                    {
                        low = (low * 10) + ((int)_current - CHAR_0);
                        lowLen++;
                    }
                }
                else if(_current == '.')
                {
                    if (lowLen < 0)
                        lowLen = 0;
                    else
                        throw new System.InvalidOperationException("Failed to parse the command.");
                }
                else if (_current == ',')
                {
                    continue;
                }
                else if (char.IsWhiteSpace(_current))
                {
                    break;
                }
                else if (_current == ')')
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
                //access x
                for (i = _reader.Read(); i >= 0; i = _reader.Read())
                {
                    _current = (char)i;

                    if (char.IsLetterOrDigit(_current) || _current == '_')
                    {
                        _strBuilder.Append(_current);
                    }
                    else if (char.IsWhiteSpace(_current) || IsArithmeticSymbol(_current) || _current == ')')
                        break;
                    else
                        throw new System.InvalidOperationException("Failed to parse the command.");
                }

                var str = _strBuilder.ToString();
                _strBuilder.Length = 0;
                return ConvertUtil.ToDouble(DynamicUtil.GetValue(_x, str));
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
                    else if (char.IsWhiteSpace(_current) || IsArithmeticSymbol(_current) || _current == ')')
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
                    default:
                        return 0d;
                }
            }
            else if(char.IsWhiteSpace(_current) || IsArithmeticSymbol(_current) || _current == ')')
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

            switch(name)
            {
                case "abs":
                    return Math.Abs(EvalStatement());
                case "sqrt":
                    return Math.Sqrt(EvalStatement());
                case "cos":
                    return Math.Cos(EvalStatement());
                case "sin":
                    return Math.Sin(EvalStatement());
                case "tan":
                    return Math.Tan(EvalStatement());
                case "acos":
                    return Math.Acos(EvalStatement());
                case "asin":
                    return Math.Asin(EvalStatement());
                case "atan":
                    return Math.Atan(EvalStatement());
                case "atan2":
                    //TODO - need to resolve having multiple params for a func
                    throw new System.InvalidOperationException("Failed to parse the command: Unknown Function");
                default:
                    throw new System.InvalidOperationException("Failed to parse the command: Unknown Function");
            }
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
            switch(c)
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
