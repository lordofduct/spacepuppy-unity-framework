using System;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Tween
{

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CustomImplicitCurveAttribute : System.Attribute
    {

        private System.Type _targetType;
        private string _propName;
        public int priority;

        public CustomImplicitCurveAttribute(System.Type targetType, string propName)
        {
            _targetType = targetType;
            _propName = propName;
        }

        public System.Type HandledTargetType { get { return _targetType; } }

        public string HandledPropName { get { return _propName; } }

    }

}
