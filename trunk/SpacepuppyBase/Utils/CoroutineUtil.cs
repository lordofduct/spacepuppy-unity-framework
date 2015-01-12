using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Utils
{
    public static class CoroutineUtil
    {

        #region StartCoroutine

        public static Coroutine StartCoroutine(this MonoBehaviour behaviour, System.Collections.IEnumerable enumerable)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            return behaviour.StartCoroutine(enumerable.GetEnumerator());
        }

        public static Coroutine StartCoroutine(this MonoBehaviour behaviour, CoroutineMethod method)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (method == null) throw new System.ArgumentNullException("method");

            return behaviour.StartCoroutine(method());
        }

        public static Coroutine StartCoroutine(this MonoBehaviour behaviour, System.Delegate method, params object[] args)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (method == null) throw new System.ArgumentNullException("method");

            System.Collections.IEnumerator e;
            if (com.spacepuppy.Utils.ObjUtil.IsType(method.Method.ReturnType, typeof(System.Collections.IEnumerable)))
            {
                e = (method.DynamicInvoke(args) as System.Collections.IEnumerable).GetEnumerator();
            }
            else if (com.spacepuppy.Utils.ObjUtil.IsType(method.Method.ReturnType, typeof(System.Collections.IEnumerator)))
            {
                e = (method.DynamicInvoke(args) as System.Collections.IEnumerator);
            }
            else
            {
                throw new System.ArgumentException("Delegate must have a return type of IEnumerable or IEnumerator.", "method");
            }

            return behaviour.StartCoroutine(e);
        }

        #endregion

        #region RadicalCoroutine

        public static RadicalCoroutine StartRadicalCoroutine(this MonoBehaviour behaviour, System.Collections.IEnumerator routine)
        {
            return RadicalCoroutine.StartRadicalCoroutine(behaviour, routine);
        }

        public static RadicalCoroutine StartRadicalCoroutine(this MonoBehaviour behaviour, System.Collections.IEnumerable routine)
        {
            return RadicalCoroutine.StartRadicalCoroutine(behaviour, routine);
        }

        public static RadicalCoroutine StartRadicalCoroutine(this MonoBehaviour behaviour, CoroutineMethod method)
        {
            return RadicalCoroutine.StartRadicalCoroutine(behaviour, method);
        }

        public static RadicalCoroutine StartRadicalCoroutine(this MonoBehaviour behaviour, System.Delegate method, params object[] args)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (method == null) throw new System.ArgumentNullException("method");

            System.Collections.IEnumerator e;
            if (com.spacepuppy.Utils.ObjUtil.IsType(method.Method.ReturnType, typeof(System.Collections.IEnumerable)))
            {
                e = (method.DynamicInvoke(args) as System.Collections.IEnumerable).GetEnumerator();
            }
            else if (com.spacepuppy.Utils.ObjUtil.IsType(method.Method.ReturnType, typeof(System.Collections.IEnumerator)))
            {
                e = (method.DynamicInvoke(args) as System.Collections.IEnumerator);
            }
            else
            {
                throw new System.ArgumentException("Delegate must have a return type of IEnumerable or IEnumerator.", "method");
            }

            return RadicalCoroutine.StartRadicalCoroutine(behaviour, e);
        }

        #endregion

        #region Invoke

        public static Coroutine Invoke(this MonoBehaviour behaviour, System.Action method, float delay)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (method == null) throw new System.ArgumentNullException("method");

            return behaviour.StartCoroutine(InvokeRedirect(method, delay));
        }

        public static RadicalCoroutine InvokeRadical(this MonoBehaviour behaviour, System.Action method, float delay)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (method == null) throw new System.ArgumentNullException("method");

            return StartRadicalCoroutine(behaviour, InvokeRedirect(method, delay));
        }

        public static RadicalCoroutine InvokeRepeatingRadical(this MonoBehaviour behaviour, System.Action method, float delay, float repeatRate)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (method == null) throw new System.ArgumentNullException("method");

            return StartRadicalCoroutine(behaviour, InvokeRedirect(method, delay));
        }

        private static System.Collections.IEnumerator InvokeRedirect(System.Action method, float delay, float repeatRate = -1f)
        {
            yield return new WaitForSeconds(delay);
            if (repeatRate < 0f)
            {
                method();
            }
            else if (repeatRate == 0f)
            {
                while (true)
                {
                    method();
                    yield return null;
                }
            }
            else
            {
                var w = new WaitForSeconds(repeatRate);
                while (true)
                {
                    method();
                    yield return w;
                }
            }
        }

        //public static Coroutine Invoke(this MonoBehaviour behaviour, CoroutineMethod method, float delay)
        //{
        //    if (behaviour == null) throw new System.ArgumentNullException("behaviour");
        //    if (method == null) throw new System.ArgumentNullException("method");

        //    return behaviour.StartCoroutine(InvokeRedirect(method, delay));
        //}

        //public static RadicalCoroutine InvokeRadical(this MonoBehaviour behaviour, CoroutineMethod method, float delay)
        //{
        //    if (behaviour == null) throw new System.ArgumentNullException("behaviour");
        //    if (method == null) throw new System.ArgumentNullException("method");

        //    return StartRadicalCoroutine(behaviour, InvokeRedirect(method, delay));
        //}

        //public static RadicalCoroutine InvokeRepeatingRadical(this MonoBehaviour behaviour, CoroutineMethod method, float delay, float repeatRate)
        //{
        //    if (behaviour == null) throw new System.ArgumentNullException("behaviour");
        //    if (method == null) throw new System.ArgumentNullException("method");

        //    return StartRadicalCoroutine(behaviour, InvokeRedirect(method, delay));
        //}

        //private static System.Collections.IEnumerator InvokeRedirect(CoroutineMethod method, float delay, float repeatRate = -1f)
        //{
        //    yield return new WaitForSeconds(delay);
        //    if (repeatRate < 0f)
        //    {
        //        yield return method();
        //    }
        //    else if (repeatRate == 0f)
        //    {
        //        while (true)
        //        {
        //            yield return method();
        //            yield return null;
        //        }
        //    }
        //    else
        //    {
        //        var w = new WaitForSeconds(repeatRate);
        //        while (true)
        //        {
        //            yield return method();
        //            yield return w;
        //        }
        //    }
        //}

        #endregion

    }
}
