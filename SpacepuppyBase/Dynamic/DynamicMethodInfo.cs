using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Dynamic
{
    public class DynamicMethodInfo : MethodInfo
    {

        #region Fields

        private string _name;
        private Type _declaringType;

        #endregion

        #region MethodInfo Interface

        public override string Name
        {
            get { return _name; }
        }

        public override Type DeclaringType
        {
            get { return _declaringType; }
        }

        public override Type ReflectedType
        {
            get { return _declaringType; }
        }

        public override MethodAttributes Attributes
        {
            get { return MethodAttributes.Final | MethodAttributes.Public; }
        }

        public override MethodInfo GetBaseDefinition()
        {
            return this;
        }

        public override ICustomAttributeProvider ReturnTypeCustomAttributes
        {
            get { return DynamicCustomAttributeProvider.Instance; }
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return MethodImplAttributes.ForwardRef;
        }

        public override ParameterInfo[] GetParameters()
        {
            return ArrayUtil.Empty<ParameterInfo>();
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
        {
            return DynamicUtil.InvokeMethod(obj, _name);
        }

        public override RuntimeMethodHandle MethodHandle
        {
            get { return new RuntimeMethodHandle(); }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return ArrayUtil.Empty<object>();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return ArrayUtil.Empty<object>();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return false;
        }

        #endregion


        #region Special Types

        private class DynamicCustomAttributeProvider : ICustomAttributeProvider
        {

            public readonly static DynamicCustomAttributeProvider Instance = new DynamicCustomAttributeProvider();

            public object[] GetCustomAttributes(Type attributeType, bool inherit)
            {
                return ArrayUtil.Empty<object>();
            }

            public object[] GetCustomAttributes(bool inherit)
            {
                return ArrayUtil.Empty<object>();
            }

            public bool IsDefined(Type attributeType, bool inherit)
            {
                return false;
            }
        }

        #endregion
    }
}
