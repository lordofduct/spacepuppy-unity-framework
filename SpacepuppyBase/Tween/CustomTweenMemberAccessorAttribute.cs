namespace com.spacepuppy.Tween
{

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CustomTweenMemberAccessorAttribute : System.Attribute
    {
        
        private System.Type _targetType;
        private System.Type _memberType;
        private string _propName;
        public int priority;
        
        public CustomTweenMemberAccessorAttribute(System.Type targetType, System.Type memberType, string propName)
        {
            _targetType = targetType;
            _memberType = memberType;
            _propName = propName;
        }

        public System.Type HandledTargetType { get { return _targetType; } }

        /// <summary>
        /// The type of the member being handled. Same as what would be returned by ITweenMemberAccessor.GetMemberType.
        /// </summary>
        public System.Type MemberType { get { return _memberType; } }

        public string HandledPropName { get { return _propName; } }
        
    }
}
