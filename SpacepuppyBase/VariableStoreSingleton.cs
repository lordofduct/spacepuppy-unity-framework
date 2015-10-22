using UnityEngine;

using com.spacepuppy.Dynamic;
using System.Collections.Generic;
using System.Reflection;

namespace com.spacepuppy
{

    [Singleton.Config(DefaultLifeCycle = SingletonLifeCycleRule.AlwaysReplace)]
    public class VariableStoreSingleton : PsuedoSingleton<VariableStore>
    {



    }

}
