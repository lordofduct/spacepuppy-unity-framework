using System;
using System.Reflection;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Dynamic
{

    public class DynamicPropertyInfo : PropertyInfo, IDynamicMemberInfo
    {

        #region Fields

        private string _name;
        private Type _declaringType;
        private Type _propertyType;

        #endregion

        #region CONSTRUCTOR

        public DynamicPropertyInfo(string name, Type declaringType)
        {
            _name = name;
            _declaringType = declaringType;
            _propertyType = typeof(object);
        }

        public DynamicPropertyInfo(string name, Type declaringType, Type propertyType)
        {
            _name = name;
            _declaringType = declaringType;
            _propertyType = propertyType;
        }

        #endregion

        #region PropertyInfo Interface

        public override string Name
        {
            get { return _name; }
        }

        public override System.Reflection.PropertyAttributes Attributes
        {
            get { return PropertyAttributes.None; }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override Type PropertyType
        {
            get { return _propertyType; }
        }

        public override Type DeclaringType
        {
            get { return _declaringType; }
        }

        public override Type ReflectedType
        {
            get { return _declaringType; }
        }



        public override object GetValue(object obj, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, object[] index, System.Globalization.CultureInfo culture)
        {
            if (obj is IDynamic)
            {
                return (obj as IDynamic).GetValue(_name);
            }
            else
            {
                return null;
            }
        }

        public override void SetValue(object obj, object value, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, object[] index, System.Globalization.CultureInfo culture)
        {
            if (obj is IDynamic)
            {
                (obj as IDynamic).SetValue(_name, value);
            }
        }

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            return ArrayUtil.Empty<MethodInfo>();
        }

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            return null;
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            return ArrayUtil.Empty<ParameterInfo>();
        }

        public override System.Reflection.MethodInfo GetSetMethod(bool nonPublic)
        {
            return null;
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

        Type IDynamicMemberInfo.ReturnType { get { return this.PropertyType; } }

    }

}
