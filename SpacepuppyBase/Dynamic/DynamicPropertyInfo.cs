using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Dynamic
{

    public class DynamicPropertyInfo : System.Reflection.PropertyInfo
    {

        #region Fields

        private string _name;

        #endregion

        #region CONSTRUCTOR

        public DynamicPropertyInfo(string name)
        {
            _name = name;
        }

        #endregion

        #region PropertyInfo Interface

        public override string Name
        {
            get { return _name; }
        }

        public override System.Reflection.PropertyAttributes Attributes
        {
            get { return System.Reflection.PropertyAttributes.None; }
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
            get { return typeof(object); }
        }

        public override Type DeclaringType
        {
            get { return typeof(StateToken); }
        }

        public override Type ReflectedType
        {
            get { return typeof(StateToken); }
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

        public override System.Reflection.MethodInfo[] GetAccessors(bool nonPublic)
        {
            return new System.Reflection.MethodInfo[] { };
        }

        public override System.Reflection.MethodInfo GetGetMethod(bool nonPublic)
        {
            return null;
        }

        public override System.Reflection.ParameterInfo[] GetIndexParameters()
        {
            return new System.Reflection.ParameterInfo[] { };
        }

        public override System.Reflection.MethodInfo GetSetMethod(bool nonPublic)
        {
            return null;
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return new object[] { };
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return new object[] { };
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return false;
        }

        #endregion

    }

}
