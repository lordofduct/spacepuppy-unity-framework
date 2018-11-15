using UnityEngine;

namespace com.spacepuppy.Utils
{
    public static class CoroutineUtil
    {

        public static System.Collections.IEnumerator Wait(object instruction, System.Action<object> callback)
        {
            if (callback == null) throw new System.ArgumentNullException("callback");
            yield return instruction;
            callback(instruction);
        }
        
        #region StartCoroutine

        public static Coroutine StartCoroutine(this MonoBehaviour behaviour, System.Collections.IEnumerable enumerable)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            return behaviour.StartCoroutine(enumerable.GetEnumerator());
        }

        public static Coroutine StartCoroutine(this MonoBehaviour behaviour, System.Func<System.Collections.IEnumerator> method)
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
            if (com.spacepuppy.Utils.TypeUtil.IsType(method.Method.ReturnType, typeof(System.Collections.IEnumerable)))
            {
                e = (method.DynamicInvoke(args) as System.Collections.IEnumerable).GetEnumerator();
            }
            else if (com.spacepuppy.Utils.TypeUtil.IsType(method.Method.ReturnType, typeof(System.Collections.IEnumerator)))
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

        public static RadicalCoroutine StartRadicalCoroutine(this MonoBehaviour behaviour, System.Collections.IEnumerator routine, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine);
            co.Start(behaviour, disableMode);
            return co;
        }

        public static RadicalCoroutine StartRadicalCoroutine(this MonoBehaviour behaviour, System.Collections.IEnumerable routine, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine.GetEnumerator());
            co.Start(behaviour, disableMode);
            return co;
        }

        public static RadicalCoroutine StartRadicalCoroutine(this MonoBehaviour behaviour, System.Func<System.Collections.IEnumerator> method, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (method == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(method());
            co.Start(behaviour, disableMode);
            return co;
        }

        public static RadicalCoroutine StartRadicalCoroutine(this MonoBehaviour behaviour, System.Delegate method, object[] args = null, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (method == null) throw new System.ArgumentNullException("method");

            System.Collections.IEnumerator e;
            if (com.spacepuppy.Utils.TypeUtil.IsType(method.Method.ReturnType, typeof(System.Collections.IEnumerable)))
            {
                e = (method.DynamicInvoke(args) as System.Collections.IEnumerable).GetEnumerator();
            }
            else if (com.spacepuppy.Utils.TypeUtil.IsType(method.Method.ReturnType, typeof(System.Collections.IEnumerator)))
            {
                e = (method.DynamicInvoke(args) as System.Collections.IEnumerator);
            }
            else
            {
                throw new System.ArgumentException("Delegate must have a return type of IEnumerable or IEnumerator.", "method");
            }

            var co = new RadicalCoroutine(e);
            co.Start(behaviour, disableMode);
            return co;
        }
        



        public static RadicalCoroutine StartRadicalCoroutineAsync(this MonoBehaviour behaviour, System.Collections.IEnumerator routine, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine);
            co.StartAsync(behaviour, disableMode);
            return co;
        }

        public static RadicalCoroutine StartRadicalCoroutineAsync(this MonoBehaviour behaviour, System.Collections.IEnumerable routine, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine.GetEnumerator());
            co.StartAsync(behaviour, disableMode);
            return co;
        }

        public static RadicalCoroutine StartRadicalCoroutineAsync(this MonoBehaviour behaviour, System.Func<System.Collections.IEnumerator> method, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (method == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(method());
            co.StartAsync(behaviour, disableMode);
            return co;
        }

        public static RadicalCoroutine StartRadicalCoroutineAsync(this MonoBehaviour behaviour, System.Delegate method, object[] args = null, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (method == null) throw new System.ArgumentNullException("method");

            System.Collections.IEnumerator e;
            if (com.spacepuppy.Utils.TypeUtil.IsType(method.Method.ReturnType, typeof(System.Collections.IEnumerable)))
            {
                e = (method.DynamicInvoke(args) as System.Collections.IEnumerable).GetEnumerator();
            }
            else if (com.spacepuppy.Utils.TypeUtil.IsType(method.Method.ReturnType, typeof(System.Collections.IEnumerator)))
            {
                e = (method.DynamicInvoke(args) as System.Collections.IEnumerator);
            }
            else
            {
                throw new System.ArgumentException("Delegate must have a return type of IEnumerable or IEnumerator.", "method");
            }

            var co = new RadicalCoroutine(e);
            co.StartAsync(behaviour, disableMode);
            return co;
        }



        public static RadicalCoroutine StartAutoKillRadicalCoroutine(this MonoBehaviour behaviour, System.Collections.IEnumerator routine, object autoKillToken, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine);
            co.StartAutoKill(behaviour, autoKillToken, disableMode);
            return co;
        }

        public static RadicalCoroutine StartAutoKillRadicalCoroutine(this MonoBehaviour behaviour, System.Collections.IEnumerable routine, object autoKillToken, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(routine.GetEnumerator());
            co.StartAutoKill(behaviour, autoKillToken, disableMode);
            return co;
        }

        public static RadicalCoroutine StartAutoKillRadicalCoroutine(this MonoBehaviour behaviour, System.Func<System.Collections.IEnumerator> method, object autoKillToken, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (method == null) throw new System.ArgumentNullException("routine");

            var co = new RadicalCoroutine(method());
            co.StartAutoKill(behaviour, autoKillToken, disableMode);
            return co;
        }
        


        public static RadicalCoroutine StartValidatedRadicalCoroutine(this MonoBehaviour behaviour, System.Collections.IEnumerator routine, System.Func<bool> validator, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.Default)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (routine == null) throw new System.ArgumentNullException("routine");
            if (validator == null) throw new System.ArgumentNullException("validator");

            var co = new RadicalCoroutine(ValidatedRoutine(routine, validator));
            co.Start(behaviour, disableMode);
            return co;
        }

        public static System.Collections.IEnumerator ValidatedRoutine(System.Collections.IEnumerator routine, System.Func<bool> validator)
        {
            if (routine == null) throw new System.ArgumentNullException("routine");
            if (validator == null) throw new System.ArgumentNullException("validator");

            while(validator() && routine.MoveNext())
            {
                yield return routine.Current;
            }
        }

        #endregion

        #region Invoke

#if SP_LIB

        public static Coroutine InvokeLegacy(this MonoBehaviour behaviour, System.Action method, float delay)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (method == null) throw new System.ArgumentNullException("method");

            return behaviour.StartCoroutine(InvokeRedirect(method, delay));
        }

        public static RadicalCoroutine Invoke(this MonoBehaviour behaviour, System.Action method, float delay, ITimeSupplier time = null, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.CancelOnDisable)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (method == null) throw new System.ArgumentNullException("method");

            return StartRadicalCoroutine(behaviour, RadicalInvokeRedirect(method, delay, -1f, time), disableMode);
        }

        public static RadicalCoroutine InvokeRepeating(this MonoBehaviour behaviour, System.Action method, float delay, float repeatRate, ITimeSupplier time = null, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.CancelOnDisable)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (method == null) throw new System.ArgumentNullException("method");

            return StartRadicalCoroutine(behaviour, RadicalInvokeRedirect(method, delay, repeatRate, time), disableMode);
        }

        public static InvokeHandle InvokeGuaranteed(this MonoBehaviour behaviour, System.Action method, float delay, ITimeSupplier time = null)
        {
            if (method == null) throw new System.ArgumentNullException("method");
            //return StartRadicalCoroutine(GameLoopEntry.Hook, RadicalInvokeRedirect(method, delay, -1f, time));
            
            return InvokeHandle.Begin(GameLoopEntry.UpdatePump, method, delay, time);
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

        internal static System.Collections.IEnumerator RadicalInvokeRedirect(System.Action method, float delay, float repeatRate = -1f, ITimeSupplier time = null)
        {
            //if (delay < SPConstants.MIN_FRAME_DELTA)
            //    yield return null;
            //else if (delay > 0f)
            //    yield return WaitForDuration.Seconds(delay, time);

            if (time == null) time = SPTime.Normal;
            float t = time.Total;
            while (time.Total - t < delay)
            {
                yield return null;
            }

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
                while (true)
                {
                    method();
                    yield return WaitForDuration.Seconds(repeatRate, time);
                }
            }
        }

#endif


        public static RadicalCoroutine InvokeAfterYield(this MonoBehaviour behaviour, System.Action method, object yieldInstruction, RadicalCoroutineDisableMode disableMode = RadicalCoroutineDisableMode.CancelOnDisable)
        {
            if (behaviour == null) throw new System.ArgumentNullException("behaviour");
            if (method == null) throw new System.ArgumentNullException("method");

            return StartRadicalCoroutine(behaviour, InvokeAfterYieldRedirect(method, yieldInstruction));
        }

        internal static System.Collections.IEnumerator InvokeAfterYieldRedirect(System.Action method, object yieldInstruction)
        {
            yield return yieldInstruction;
            method();
        }

#endregion
        
    }
}
