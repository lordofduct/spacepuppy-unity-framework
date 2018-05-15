using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy
{

    /// <summary>
    /// Defines a contract for something that is nameable.
    /// 
    /// This is usually used in conjunction with NameCache.UnityObjectNameCache to streamline caching of the name field for fast CompareName.
    /// </summary>
    public interface INameable
    {

        string Name { get; set; }

        bool CompareName(string nm);

        void SetDirty();

    }
    
}
