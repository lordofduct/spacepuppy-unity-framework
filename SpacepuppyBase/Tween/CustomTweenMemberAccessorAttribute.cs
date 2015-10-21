namespace com.spacepuppy.Tween
{

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CustomTweenMemberAccessorAttribute : System.Attribute
    {
        
        private System.Type _targetType;
        private string _propName;
        public int priority;

        public CustomTweenMemberAccessorAttribute(System.Type targetType, string propName)
        {
            _targetType = targetType;
            _propName = propName;
        }

        public System.Type HandledTargetType { get { return _targetType; } }

        public string HandledPropName { get { return _propName; } }

    }
}
