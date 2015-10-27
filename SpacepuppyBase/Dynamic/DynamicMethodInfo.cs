using System;
using System.Reflection;

using com.spacepuppy.Dynamic.Accessors;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Dynamic
{
    public class DynamicMethodInfo : MethodInfo, IDynamicMemberInfo
    {

        #region Fields

        private string _name;
        private Type _declaringType;
        private Type _returnType;

        #endregion

        #region CONSTRUCTOR

        public DynamicMethodInfo(string name, Type declaringType)
        {
            _name = name;
            _declaringType = declaringType;
            _returnType = null;
        }

        public DynamicMethodInfo(string name, Type declaringType, Type returnType)
        {
            _name = name;
            _declaringType = declaringType;
            _returnType = returnType;
        }

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

        public override Type ReturnType
        {
            get
            {
                return (_returnType != null) ? _returnType : base.ReturnType;
            }
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
            //return DynamicUtil.InvokeMethod(obj, _name);
            if (obj is IDynamic)
                return (obj as IDynamic).InvokeMethod(_name, parameters);
            else
                return null;
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
