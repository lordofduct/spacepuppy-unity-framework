using System;

namespace com.spacepuppy
{

    /// <summary>
    /// A serializable semi-unique id. It is not universely unique, but will be system unique at the least, and should be team unique confidently.
    /// The id is based on the exact moment in time it was generated, and the idea that 2 team members both generate them simultaneously is absurd.
    /// </summary>
    [System.Serializable]
    public struct ShortUid
    {

        public static ShortUid Zero { get { return new ShortUid(); } }

        #region Fields

        //has to be stored with uint's
        //unity has a bug at the time of writing this where long doesn't serialize in prefabs correctly
        //there is a fix in unity beta 2017.1, but we are unsure as to when the full release will be out
        //so stuck with this hack fix
        [UnityEngine.SerializeField]
        private uint _low;
        [UnityEngine.SerializeField]
        private uint _high;

        #endregion

        #region CONSTRUCTOR

        public ShortUid(long value)
        {
            _low = (uint)(value & uint.MaxValue);
            _high = (uint)(value >> 32);
        }

        public static ShortUid NewId()
        {
            return new ShortUid(System.DateTime.UtcNow.Ticks);
        }

        #endregion

        #region Properties

        public long Value
        {
            get
            {
                return ((long)_high << 32) | (long)_low;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the base64 encoded guid as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.ToString("X16");
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a
        /// specified Object represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is ShortUid)
            {
                var uid = (ShortUid)obj;
                return uid._high == _high && uid._low == _low;
            }
            return false;
        }

        /// <summary>
        /// Returns the HashCode for underlying Guid.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (int)(_high ^ _low);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Determines if both ShortGuids have the same underlying
        /// Guid value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(ShortUid x, ShortUid y)
        {
            return x._high == y._high && x._low == y._low;
        }

        /// <summary>
        /// Determines if both ShortGuids do not have the
        /// same underlying Guid value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(ShortUid x, ShortUid y)
        {
            return x._high != y._high || x._low != y._low;
        }

        /// <summary>
        /// Implicitly converts the ShortGuid to it's string equivilent
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static implicit operator string(ShortUid uid)
        {
            return uid.ToString();
        }

        public static implicit operator long(ShortUid uid)
        {
            return uid.Value;
        }

        #endregion

        #region Special Types

        public class ConfigAttribute : Attribute
        {
            public bool ReadOnly;
            public bool AllowZero;
        }

        #endregion

    }

    /// <summary>
    /// A small id that is unique to this Application Domain for the duration of the application. 
    /// It has a limit of 2^32 unique values.
    /// </summary>
    public struct TinyUid
    {

        #region Fields

        [UnityEngine.SerializeField]
        private int _id;

        #endregion

        #region CONSTRUCTOR

        internal TinyUid(int id)
        {
            _id = id;
        }
        
        #endregion

        #region ToString

        /// <summary>
        /// Returns the base64 encoded guid as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _id.ToString("X8");
        }

        #endregion

        #region Equals

        /// <summary>
        /// Returns a value indicating whether this instance and a
        /// specified Object represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is TinyUid)
                return _id == ((TinyUid)obj)._id;
            return false;
        }

        #endregion

        #region GetHashCode

        /// <summary>
        /// Returns the HashCode for underlying Guid.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _id;
        }

        #endregion

        #region NewGuid

        private static int _nextValue;

        /// <summary>
        /// Initialises a new instance of the ShortGuid class
        /// </summary>
        /// <returns></returns>
        public static TinyUid Next()
        {
            return new TinyUid(_nextValue++);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Determines if both ShortGuids have the same underlying
        /// Guid value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(TinyUid x, TinyUid y)
        {
            return x._id == y._id;
        }

        /// <summary>
        /// Determines if both ShortGuids do not have the
        /// same underlying Guid value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(TinyUid x, TinyUid y)
        {
            return x._id != y._id;
        }

        /// <summary>
        /// Implicitly converts the ShortGuid to it's string equivilent
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static implicit operator string(TinyUid uid)
        {
            return uid.ToString();
        }

        #endregion
        
    }

    /// <summary>
    /// Allows you to generate TinyUids that are unique to a specific context. The ids created will not be unique to values returned by TinyUid.Next(). 
    /// This is useful if small ids are needed within a scoped grouping.
    /// </summary>
    public class TinyUidGenerator
    {

        private int _nextValue;

        public TinyUid Next()
        {
            return new TinyUid(_nextValue++);
        }

        public void Reset()
        {
            _nextValue = 0;
        }

    }







    /*
    [System.Serializable]
    public struct ShortUid
    {

        public ShortUid Zero { get { return new ShortUid(); } }

        #region Fields

        [UnityEngine.SerializeField]
        private long _id;
        //private int _low;
        //private int _high;

        #endregion

        #region CONSTRUCTOR / FACTORY

        private static Random _rand = new Random();
        public static ShortUid NewId()
        {
            ShortUid result = new ShortUid();
            result._id = DateTime.UtcNow.Ticks;//store ticks relative to near date
            return result;
        }

        #endregion

        #region Properties

        public long Value
        {
            get { return _id; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the base64 encoded guid as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _id.ToString("X16");
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a
        /// specified Object represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is ShortUid)
                return _id == ((ShortUid)obj)._id;
            return false;
        }

        /// <summary>
        /// Returns the HashCode for underlying Guid.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        #endregion

        #region Operators

        /// <summary>
        /// Determines if both ShortGuids have the same underlying
        /// Guid value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(ShortUid x, ShortUid y)
        {
            return x._id == y._id;
        }

        /// <summary>
        /// Determines if both ShortGuids do not have the
        /// same underlying Guid value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(ShortUid x, ShortUid y)
        {
            return x._id != y._id;
        }

        /// <summary>
        /// Implicitly converts the ShortGuid to it's string equivilent
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static implicit operator string(ShortUid uid)
        {
            return uid.ToString();
        }

        public static implicit operator long(ShortUid uid)
        {
            return uid._id;
        }

        #endregion

        #region Special Types

        public class ConfigAttribute : Attribute
        {
            public readonly bool AllowZero;
            public ConfigAttribute(bool allowZero)
            {
                this.AllowZero = allowZero;
            }
        }

        #endregion

    }
    */

}
