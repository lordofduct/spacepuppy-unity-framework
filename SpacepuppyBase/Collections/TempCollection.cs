using System;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Collections
{

    public interface ITempCollection<T> : ICollection<T>, IDisposable
    {

    }

    /// <summary>
    /// This is intended for a short lived collection that needs to be memory efficient and fast. 
    /// Call the static 'GetCollection' method to get a cached collection for use. 
    /// When you're done with the collection you call Release to make it available for reuse again later. 
    /// Do NOT use it again after calling Release.
    /// 
    /// Due to the design of this, it should only ever be used in a single threaded manner. Primarily intended 
    /// for the main Unity thread. 
    /// 
    /// If you're in a separate thread, it's best to cache your own list local to there, and don't even bother with 
    /// this.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class TempCollection
    {

        private const int MAX_SIZE_INBYTES = 1024;

        #region Static Interface
        
        /// <summary>
        /// Returns the any available collection for use generically. 
        /// The collection could be a HashSet, List, or any temp implementation. 
        /// This is intended to reduce the need for creating a new collection 
        /// unnecessarily.
        /// </summary>
        /// <returns></returns>
        public static ITempCollection<T> GetCollection<T>()
        {
            return GetList<T>();
        }

        /// <summary>
        /// Returns the any available collection for use generically. 
        /// The collection could be a HashSet, List, or any temp implementation. 
        /// This is intended to reduce the need for creating a new collection 
        /// unnecessarily.
        /// </summary>
        /// <returns></returns>
        public static ITempCollection<T> GetCollection<T>(IEnumerable<T> e)
        {
            return GetList<T>(e);
        }






        public static TempList<T> GetList<T>()
        {
            if(TempList<T>._instance == null)
            {
                return new TempList<T>();
            }
            else
            {
                var coll = TempList<T>._instance;
                TempList<T>._instance = null;
                return coll;
            }
        }

        public static TempList<T> GetList<T>(IEnumerable<T> e)
        {
            if (TempList<T>._instance == null)
            {
                return new TempList<T>(e);
            }
            else
            {
                var coll = TempList<T>._instance;
                TempList<T>._instance = null;
                coll.AddRange(e);
                return coll;
            }
        }

        public static TempList<T> GetList<T>(int count)
        {
            if (TempList<T>._instance == null)
            {
                return new TempList<T>(count);
            }
            else
            {
                var coll = TempList<T>._instance;
                TempList<T>._instance = null;
                if (coll.Capacity < count) coll.Capacity = count;
                return coll;
            }
        }

        #endregion

        #region Special Types

        public class TempList<T> : List<T>, ITempCollection<T>
        {

            #region Fields

            internal static TempList<T> _instance;

            private int _maxCapacityOnRelease;
            private int _version;

            #endregion

            #region CONSTRUCTOR

            internal TempList()
                : base()
            {
                var tp = typeof(T);
                int sz = (tp.IsValueType) ? System.Runtime.InteropServices.Marshal.SizeOf(tp) : 4;
                _maxCapacityOnRelease = MAX_SIZE_INBYTES / sz;
                _version = 1;
            }

            internal TempList(IEnumerable<T> e)
                : base(e)
            {
                var tp = typeof(T);
                int sz = (tp.IsValueType) ? System.Runtime.InteropServices.Marshal.SizeOf(tp) : 4;
                _maxCapacityOnRelease = MAX_SIZE_INBYTES / sz;
                _version = 1;
            }

            internal TempList(int count)
                : base(count)
            {
                var tp = typeof(T);
                int sz = (tp.IsValueType) ? System.Runtime.InteropServices.Marshal.SizeOf(tp) : 4;
                _maxCapacityOnRelease = MAX_SIZE_INBYTES / sz;
                _version = 1;
            }

            #endregion

            #region IDisposable Interface

            public void Dispose()
            {
                this.Clear();
                if (_instance != null) return;

                _instance = this;
                if (this.Capacity > _maxCapacityOnRelease / Math.Min(_version, 4))
                {
                    this.Capacity = _maxCapacityOnRelease / Math.Min(_version, 4);
                    _version = 0;
                }

                _version++;
            }

            #endregion

        }

        #endregion

    }
}
