using UnityEngine;

namespace com.spacepuppy
{

    /// <summary>
    /// Stores a whole number as a floating point value. You get the range of a float, as well as infinity representations. 
    /// Implicit conversion between float and int is defined.
    /// </summary>
    [System.Serializable()]
    public struct DiscreteFloat
    {
        [SerializeField()]
        private float _value;

        #region CONSTRUCTOR

        public DiscreteFloat(float f)
        {
            _value = Mathf.Round(f);
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            if (obj is DiscreteFloat)
            {
                return _value == ((DiscreteFloat)obj)._value;
            }
            else if (obj is System.IConvertible)
            {
                try
                {
                    var f = System.Convert.ToSingle(obj);
                    return _value == f;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        #endregion

        #region Static/Constants

        public static DiscreteFloat Zero
        {
            get
            {
                return new DiscreteFloat()
                            {
                                _value = 0f
                            };
            }
        }

        public static DiscreteFloat PositiveInfinity { get { return new DiscreteFloat() { _value = float.PositiveInfinity }; } }
        public static DiscreteFloat NegativeInfinity { get { return new DiscreteFloat() { _value = float.NegativeInfinity }; } }

        public static bool IsNaN(DiscreteFloat value)
        {
            return float.IsNaN(value._value);
        }

        public static bool IsInfinity(DiscreteFloat value)
        {
            return float.IsInfinity(value._value);
        }

        public static bool IsPositiveInfinity(DiscreteFloat value)
        {
            return float.IsPositiveInfinity(value._value);
        }

        public static bool IsNegativeInfinity(DiscreteFloat value)
        {
            return float.IsNegativeInfinity(value._value);
        }

        public static bool IsReal(DiscreteFloat value)
        {
            return !(float.IsNaN(value._value) || float.IsInfinity(value._value));
        }

        #endregion

        #region Operators
        
        public static DiscreteFloat operator ++(DiscreteFloat df)
        {
            df._value++;
            return df;
        }

        public static DiscreteFloat operator +(DiscreteFloat df)
        {
            return df;
        }

        public static DiscreteFloat operator +(DiscreteFloat a, DiscreteFloat b)
        {
            a._value = Mathf.Floor(a._value + b._value);
            return a;
        }

        public static DiscreteFloat operator --(DiscreteFloat df)
        {
            df._value--;
            return df;
        }

        public static DiscreteFloat operator -(DiscreteFloat df)
        {
            df._value = -df._value;
            return df;
        }

        public static DiscreteFloat operator -(DiscreteFloat a, DiscreteFloat b)
        {
            a._value = Mathf.Floor(a._value - b._value);
            return a;
        }
        
        public static DiscreteFloat operator *(DiscreteFloat a, DiscreteFloat b)
        {
            a._value = Mathf.Floor(a._value * b._value);
            return a;
        }
        
        public static DiscreteFloat operator /(DiscreteFloat a, DiscreteFloat b)
        {
            a._value = Mathf.Floor(a._value / b._value);
            return a;
        }
        
        public static bool operator >(DiscreteFloat a, DiscreteFloat b)
        {
            return a._value > b._value;
        }

        public static bool operator >=(DiscreteFloat a, DiscreteFloat b)
        {
            return a._value >= b._value;
        }

        public static bool operator <(DiscreteFloat a, DiscreteFloat b)
        {
            return a._value < b._value;
        }

        public static bool operator <=(DiscreteFloat a, DiscreteFloat b)
        {
            return a._value <= b._value;
        }

        public static bool operator ==(DiscreteFloat a, DiscreteFloat b)
        {
            return a._value == b._value;
        }

        public static bool operator !=(DiscreteFloat a, DiscreteFloat b)
        {
            return a._value != b._value;
        }
        
        #endregion

        #region Conversions

        public static implicit operator DiscreteFloat(int f)
        {
            return new DiscreteFloat((float)f);
        }

        public static implicit operator int(DiscreteFloat df)
        {
            return (int)df._value;
        }

        public static implicit operator DiscreteFloat(float f)
        {
            return new DiscreteFloat(f);
        }

        public static implicit operator float(DiscreteFloat df)
        {
            return df._value;
        }

        public static implicit operator DiscreteFloat(double d)
        {
            return new DiscreteFloat((float)d);
        }

        public static implicit operator double(DiscreteFloat df)
        {
            return (double)df._value;
        }

        public static implicit operator DiscreteFloat(decimal d)
        {
            return new DiscreteFloat((float)d);
        }

        public static implicit operator decimal(DiscreteFloat df)
        {
            return (decimal)df._value;
        }

        #endregion



        #region Special Types

        public abstract class ConfigAttribute : System.Attribute
        {

            public abstract float Normalize(float value);

        }

        public class NonNegative : ConfigAttribute
        {

            public override float Normalize(float value)
            {
                if (value < 0f) return 0f;
                else return value;
            }

        }

        #endregion

    }
}
