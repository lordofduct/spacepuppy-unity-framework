using System;
using System.Reflection;

namespace com.spacepuppy.Dynamic
{
    public interface IDynamicMemberInfo
    {

        string Name { get; }
        Type DeclaringType { get; }
        Type ReturnType { get; }
        MemberTypes MemberType { get; }

    }
}
