using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Dynamic
{

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

    }

}
