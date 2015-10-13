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
                return new TempList<T>()
                {
                    _global = true
                };
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
                return new TempList<T>(e)
                {
                    _global = true
                };
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
                return new TempList<T>(count)
                {
                    _global = true
                };
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
        
    }
}
