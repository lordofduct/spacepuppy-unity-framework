namespace com.spacepuppy
{

    public static class ActionHelper
    {
        public static System.Action Null()
        {
            return null;
        }
        public static System.Action<T> Null<T>()
        {
            return null;
        }
        public static System.Action<T1, T2> Null<T1, T2>()
        {
            return null;
        }
        public static System.Action<T1, T2, T3> Null<T1, T2, T3>()
        {
            return null;
        }
        public static System.Action<T1, T2, T3, T4> Null<T1, T2, T3, T4>()
        {
            return null;
        }
        public static Action<T1, T2, T3, T4, T5> Null<T1, T2, T3, T4, T5>()
        {
            return null;
        }
    }

    public delegate void Action<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    public delegate void Action<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    public delegate void Action<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

    public delegate System.Collections.IEnumerable CoroutineMethod();

    public delegate void NotificationHandler(object sender, Notification n);
    public delegate void NotificationHandler<T>(object sender, T notification) where T : Notification;

}
