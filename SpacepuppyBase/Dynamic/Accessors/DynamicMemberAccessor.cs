namespace com.spacepuppy.Dynamic.Accessors
{

    /// <summary>
    /// Acts as a IMemberAccessor that accesses IDynamic objects.
    /// 
    /// NOTE - when updating DynamicUtil later to use IMemberAccessors for speed... we need to reverse this implementation.
    /// </summary>
    public class DynamicMemberAccessor : IMemberAccessor
    {

        #region Fields

        private string _memberName;

        #endregion

        #region CONSTRUCTOR

        public DynamicMemberAccessor(string memberName)
        {
            _memberName = memberName;
        }

        #endregion



        public object Get(object target)
        {
            return DynamicUtil.GetValue(target, _memberName);
        }

        public void Set(object target, object value)
        {
            DynamicUtil.SetValue(target, _memberName, value);
        }
    }
}
