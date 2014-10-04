﻿using UnityEngine;

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
    }

    public delegate System.Collections.IEnumerable CoroutineMethod();

    public delegate void NotificationHandler(Notification n);
    public delegate void NotificationHandler<T>(T notification) where T : Notification;

    public delegate void CollisionEventHandler(GameObject sender, CollisionEventArgs e);
    public delegate void TriggerEventHandler(GameObject sender, TriggerEventArgs e);
    public delegate void ControllerColliderHitEventHandler(GameObject sender, ControllerColliderHitEventArgs e);

}
