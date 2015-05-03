using System;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils.FastDynamicMemberAccessor;

namespace com.spacepuppy.Tween
{

    public interface ITweenMemberAccessor : IMemberAccessor
    {

        /// <summary>
        /// Initialize the member accessor, and return the type this accessor is supposed to handle.
        /// </summary>
        /// <param name="propName"></param>
        /// <returns></returns>
        System.Type Init(string propName, string args);

    }

}
