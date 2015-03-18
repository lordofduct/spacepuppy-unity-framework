using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy
{
    [System.Serializable()]
    public struct DiscreteFloat
    {
        [SerializeField()]
        private float _value;

        #region Properties

        #endregion

        #region Static/Constants

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

        #endregion

        #region Operators

        #region Addition

        public static DiscreteFloat operator ++(DiscreteFloat df)
        {
            df._value++;
            return df;
        }

        public static DiscreteFloat operator +(DiscreteFloat a, DiscreteFloat b)
        {
            a._value = Mathf.Floor(a._value + b._value);
            return a;
        }
        
        public static DiscreteFloat operator +(DiscreteFloat a, float b)
        {
            a._value = Mathf.Floor(a._value + b);
            return a;
        }

        public static DiscreteFloat operator +(DiscreteFloat a, int b)
        {
            a._value = Mathf.Floor(a._value + b);
            return a;
        }

        public static float operator +(float a, DiscreteFloat b)
        {
            return a + b._value;
        }

        public static double operator +(double a, DiscreteFloat b)
        {
            return a + b._value;
        }

        #endregion

        #region Subtraction

        public static DiscreteFloat operator --(DiscreteFloat df)
        {
            df._value--;
            return df;
        }

        public static DiscreteFloat operator -(DiscreteFloat a, DiscreteFloat b)
        {
            a._value = Mathf.Floor(a._value - b._value);
            return a;
        }

        public static DiscreteFloat operator -(DiscreteFloat a, float b)
        {
            a._value = Mathf.Floor(a._value - b);
            return a;
        }

        public static DiscreteFloat operator -(DiscreteFloat a, int b)
        {
            a._value = Mathf.Floor(a._value - b);
            return a;
        }

        public static float operator -(float a, DiscreteFloat b)
        {
            return a - b._value;
        }

        public static double operator -(double a, DiscreteFloat b)
        {
            return a - b._value;
        }

        #endregion

        #region Multiplication

        public static DiscreteFloat operator *(DiscreteFloat a, DiscreteFloat b)
        {
            a._value = Mathf.Floor(a._value * b._value);
            return a;
        }

        public static DiscreteFloat operator *(DiscreteFloat a, float b)
        {
            a._value = Mathf.Floor(a._value * b);
            return a;
        }

        public static DiscreteFloat operator *(DiscreteFloat a, int b)
        {
            a._value = Mathf.Floor(a._value * b);
            return a;
        }

        public static float operator *(float a, DiscreteFloat b)
        {
            return a * b._value;
        }

        public static double operator *(double a, DiscreteFloat b)
        {
            return a * b._value;
        }

        #endregion

        #region Division

        public static DiscreteFloat operator /(DiscreteFloat a, DiscreteFloat b)
        {
            a._value = Mathf.Floor(a._value / b._value);
            return a;
        }

        public static DiscreteFloat operator /(DiscreteFloat a, float b)
        {
            a._value = Mathf.Floor(a._value / b);
            return a;
        }

        public static DiscreteFloat operator /(DiscreteFloat a, int b)
        {
            a._value = Mathf.Floor(a._value / b);
            return a;
        }

        public static float operator /(float a, DiscreteFloat b)
        {
            return a / b._value;
        }

        public static double operator /(double a, DiscreteFloat b)
        {
            return a / b._value;
        }

        #endregion

        #region Comparison

        public static bool operator >(DiscreteFloat a, DiscreteFloat b)
        {
            return a._value > b._value;
        }
        public static bool operator >(DiscreteFloat a, float b)
        {
            return a._value > b;
        }
        public static bool operator >(float a, DiscreteFloat b)
        {
            return a > b._value;
        }
        public static bool operator >(DiscreteFloat a, int b)
        {
            return a._value > (float)b;
        }
        public static bool operator >(int a, DiscreteFloat b)
        {
            return (float)a > b._value;
        }


        public static bool operator >=(DiscreteFloat a, DiscreteFloat b)
        {
            return a._value >= b._value;
        }
        public static bool operator >=(DiscreteFloat a, float b)
        {
            return a._value >= b;
        }
        public static bool operator >=(float a, DiscreteFloat b)
        {
            return a >= b._value;
        }
        public static bool operator >=(DiscreteFloat a, int b)
        {
            return a._value >= (float)b;
        }
        public static bool operator >=(int a, DiscreteFloat b)
        {
            return (float)a >= b._value;
        }


        public static bool operator <(DiscreteFloat a, DiscreteFloat b)
        {
            return a._value < b._value;
        }
        public static bool operator <(DiscreteFloat a, float b)
        {
            return a._value < b;
        }
        public static bool operator <(float a, DiscreteFloat b)
        {
            return a < b._value;
        }
        public static bool operator <(DiscreteFloat a, int b)
        {
            return a._value < (float)b;
        }
        public static bool operator <(int a, DiscreteFloat b)
        {
            return (float)a < b._value;
        }


        public static bool operator <=(DiscreteFloat a, DiscreteFloat b)
        {
            return a._value <= b._value;
        }
        public static bool operator <=(DiscreteFloat a, float b)
        {
            return a._value <= b;
        }
        public static bool operator <=(float a, DiscreteFloat b)
        {
            return a <= b._value;
        }
        public static bool operator <=(DiscreteFloat a, int b)
        {
            return a._value <= (float)b;
        }
        public static bool operator <=(int a, DiscreteFloat b)
        {
            return (float)a <= b._value;
        }


        public static bool operator ==(DiscreteFloat a, DiscreteFloat b)
        {
            return a._value == b._value;
        }
        public static bool operator ==(DiscreteFloat a, float b)
        {
            return a._value == b;
        }
        public static bool operator ==(float a, DiscreteFloat b)
        {
            return a == b._value;
        }
        public static bool operator ==(DiscreteFloat a, int b)
        {
            return a._value == (float)b;
        }
        public static bool operator ==(int a, DiscreteFloat b)
        {
            return (float)a == b._value;
        }


        public static bool operator !=(DiscreteFloat a, DiscreteFloat b)
        {
            return a._value != b._value;
        }
        public static bool operator !=(DiscreteFloat a, float b)
        {
            return a._value != b;
        }
        public static bool operator !=(float a, DiscreteFloat b)
        {
            return a == b._value;
        }
        public static bool operator !=(DiscreteFloat a, int b)
        {
            return a._value != (float)b;
        }
        public static bool operator !=(int a, DiscreteFloat b)
        {
            return (float)a != b._value;
        }

        #endregion

        #endregion

        #region Conversions

        public static implicit operator DiscreteFloat(int f)
        {
            var df = new DiscreteFloat();
            df._value = (float)f;
            return df;
        }

        public static implicit operator int(DiscreteFloat df)
        {
            return (int)df._value;
        }

        public static implicit operator DiscreteFloat(float f)
        {
            var df = new DiscreteFloat();
            df._value = f;
            return df;
        }

        public static implicit operator float(DiscreteFloat df)
        {
            return df._value;
        }

        public static implicit operator DiscreteFloat(double f)
        {
            var df = new DiscreteFloat();
            df._value = (float)f;
            return df;
        }

        public static implicit operator double(DiscreteFloat df)
        {
            return (double)df._value;
        }

        #endregion

    }
}
