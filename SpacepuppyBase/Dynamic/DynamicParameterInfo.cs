using System;
using System.Reflection;

namespace com.spacepuppy.Dynamic
{
    public struct DynamicParameterInfo
    {
        public MemberInfo Member;
        public string ParameterName;
        public Type ParameterType;

        public DynamicParameterInfo(MemberInfo member, string parameterName, Type parameterType)
        {
            this.Member = member;
            this.ParameterName = parameterName;
            this.ParameterType = parameterType;
        }

        public DynamicParameterInfo(ParameterInfo info)
        {
            this.Member = info.Member;
            this.ParameterName = info.Name;
            this.ParameterType = info.ParameterType;
        }
    }
}
