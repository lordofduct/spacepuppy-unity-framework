using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Tween
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CustomMemberCurveAttribute : System.Attribute
    {

        private System.Type _memberType;
        public int priority;

        public CustomMemberCurveAttribute(System.Type handledMemberType)
        {
            _memberType = handledMemberType;
        }

        public System.Type HandledMemberType { get { return _memberType; } }

    }
}
