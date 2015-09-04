using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    /// $secsInMin
    /// $secsInHour
    /// $secsInDay
    /// $secsInWeek
    /// $secsInYear
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

        private StringBuilder _strBuilder = new StringBuilder();
        private int _parenCount;

        #endregion

        #region Methods

        public double EvalStatement(string command, object x)
        {
            _parenCount = 0;
            var result = this.EvalStatement(command.GetEnumerator(), x);
            return result;
        }

        private double EvalStatement(CharEnumerator e, object x)
        {
            var result = EvalNextValue(e, x);
            if (_parenCount > 0 && e.Current == ')')
            {
                _parenCount--;
                return result;
            }

            while(e.MoveNext())
            {
                if (char.IsWhiteSpace(e.Current)) continue;

                switch(e.Current)
                {
                    case '+':
                        result += EvalNextValue(e, x);
                        break;
                    case '*':
                        result *= EvalNextValue(e, x);
                        break;
                    case '-':
                        result -= EvalNextValue(e, x);
                        break;
                    case '/':
                        result /= EvalNextValue(e, x);
                        break;
                    case '^':
                        result = Math.Pow(result, EvalNextValue(e, x));
                        break;
                    case '%':
                        result %= EvalNextValue(e, x);
                        break;
                    case ')':
                        //reached the end of the statement
                        return result;
                }
            }

            //ran out of statement with no errors, must be the end
            return result;
        }

        private double EvalNextValue(CharEnumerator e, object x)
        {
            while (e.MoveNext())
            {
                if (char.IsWhiteSpace(e.Current)) continue;
                if (char.IsDigit(e.Current))
                {
                    return EvalNumber(e);
                }
                if (char.IsLetter(e.Current))
                {
                    return EvalFunc(e, x);
                }

                switch (e.Current)
                {
                    case '$':
                        return EvalVariable(e, x);
                    case '(':
                        _parenCount++;
                        return EvalStatement(e, x);
                    case '-':
                        return -EvalNextValue(e, x);
                    case ')':
                        return 0d;
                    default:
                        throw new System.InvalidOperationException("Failed to parse the command.");
                }

            }

            //ran out of statement... this shouldn't happen
            throw new System.InvalidOperationException("Failed to parse the command.");
        }

        private double EvalNumber(CharEnumerator e)
        {
            _strBuilder.Length = 0;
            _strBuilder.Append(e.Current);

            while(e.MoveNext())
            {
                if (char.IsDigit(e.Current) || e.Current == '.')
                    _strBuilder.Append(e.Current);
                else if (e.Current == ',')
                    continue;
                else if (char.IsWhiteSpace(e.Current))
                    break;
                else if (e.Current == ')')
                    break;
                else if (IsArithmeticSymbol(e.Current))
                    break;
                else
                    throw new System.InvalidOperationException("Failed to parse the command.");

            }

            var str = _strBuilder.ToString();
            _strBuilder.Length = 0;
            return ConvertUtil.ToDouble(str);
        }

        private double EvalVariable(CharEnumerator e, object x)
        {
            _strBuilder.Length = 0;
            if (!e.MoveNext()) return ConvertUtil.ToDouble(x);

            if(e.Current == '.')
            {
                //access x
                while (e.MoveNext())
                {
                    if (char.IsLetterOrDigit(e.Current) || e.Current == '_')
                        _strBuilder.Append(e.Current);
                    else if (char.IsWhiteSpace(e.Current))
                        break;
                    else
                        throw new System.InvalidOperationException("Failed to parse the command.");
                }

                var str = _strBuilder.ToString();
                _strBuilder.Length = 0;
                return ConvertUtil.ToDouble(DynamicUtil.GetValue(x, str));
            }
            else if (char.IsLetterOrDigit(e.Current) || e.Current == '_')
            {
                //global
                _strBuilder.Append(char.ToLower(e.Current));

                while (e.MoveNext())
                {
                    if (char.IsLetterOrDigit(e.Current) || e.Current == '_')
                        _strBuilder.Append(char.ToLower(e.Current));
                    else if (char.IsWhiteSpace(e.Current))
                        break;
                    else
                        throw new System.InvalidOperationException("Failed to parse the command.");
                }


                var str = _strBuilder.ToString();
                _strBuilder.Length = 0;

                switch(str)
                {
                    case "secsinmin":
                        return 60d;
                    case "secsinhour":
                        return 3600d;
                    case "secsinday":
                        return 86400;
                    case "secsinweek":
                        return 604800;
                    case "secsinyear":
                        return 31536000;
                    default:
                        return 0d;
                }
            }
            else if(char.IsWhiteSpace(e.Current) || IsArithmeticSymbol(e.Current) || e.Current == ')')
            {
                return ConvertUtil.ToDouble(x);
            }
            else
            {
                throw new System.InvalidOperationException("Failed to parse the command.");
            }

        }


        private double EvalFunc(CharEnumerator e, object x)
        {
            _strBuilder.Length = 0;
            _strBuilder.Append(char.ToLower(e.Current));

            while(e.MoveNext())
            {
                if (char.IsLetterOrDigit(e.Current))
                    _strBuilder.Append(char.ToLower(e.Current));
                else if (e.Current == '(')
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
                    return Math.Abs(EvalStatement(e, x));
                case "sqrt":
                    return Math.Sqrt(EvalStatement(e, x));
                case "cos":
                    return Math.Cos(EvalStatement(e, x));
                case "sin":
                    return Math.Sin(EvalStatement(e, x));
                case "tan":
                    return Math.Tan(EvalStatement(e, x));
                case "acos":
                    return Math.Acos(EvalStatement(e, x));
                case "asin":
                    return Math.Asin(EvalStatement(e, x));
                case "atan":
                    return Math.Atan(EvalStatement(e, x));
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

        #endregion

    }

}
