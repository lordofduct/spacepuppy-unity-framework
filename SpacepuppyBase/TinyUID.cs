namespace com.spacepuppy
{

    /// <summary>
    /// A small id that is unique to this Application Domain for the duration of the application. 
    /// It has a limit of 2^32 unique values.
    /// </summary>
    public struct TinyUid
    {

        #region Fields

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
        /// <param name="shortGuid"></param>
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

}
