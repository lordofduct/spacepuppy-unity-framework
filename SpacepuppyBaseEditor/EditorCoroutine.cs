using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using IEnumerator = System.Collections.IEnumerator;

using com.spacepuppy;

namespace com.spacepuppyeditor
{
    public static class EditorCoroutine
    {

        #region Fields

        private static List<CoroutineHandle> _routines = new List<CoroutineHandle>();

        #endregion

        #region Methods

        public static RadicalCoroutine StartEditorCoroutine(IEnumerator e)
        {
            var routine = new RadicalCoroutine(e);
            var handle = new CoroutineHandle()
            {
                Routine = routine,
                Yield = null
            };

            if(_routines.Count == 0)
            {
                EditorApplication.update -= DoUpdate;
                EditorApplication.update += DoUpdate;
            }

            _routines.Add(handle);
            return routine;
        }

        public static object YieldForDuration(double dur)
        {
            return new WaitForEditorDuration()
            {
                Duration = dur
            };
        }




        private static void DoUpdate()
        {
            for(int i = 0; i < _routines.Count; i++)
            {
                var handle = _routines[i];
                if(handle.Routine.Finished)
                {
                    _routines.RemoveAt(i);
                    i--;
                    continue;
                }

                //Wait on yield instruction
                if(handle.Yield is WaitForEditorDuration)
                {
                    var wait = handle.Yield as WaitForEditorDuration;
                    if((System.DateTime.Now - wait.Start).TotalSeconds >= wait.Duration)
                    {
                        handle.Yield = null;
                    }
                    else
                    {
                        continue;
                    }
                }
                else if(handle.Yield is IEnumerator)
                {
                    var e = handle.Yield as IEnumerator;
                    if(e.MoveNext())
                    {
                        continue;
                    }
                    else
                    {
                        handle.Yield = null;
                    }
                }
                else
                {
                    handle.Yield = null;
                }

                //perform action
                object yieldObj;
                if(!handle.Routine.ManualTick(out yieldObj) || handle.Routine.Finished)
                {
                    _routines.RemoveAt(i);
                    i--;
                    continue;
                }

                //store yield object
                if(yieldObj is WaitForEditorDuration)
                {
                    (yieldObj as WaitForEditorDuration).Start = System.DateTime.Now;
                    handle.Yield = yieldObj;
                }
                else if (yieldObj is IEnumerator && (yieldObj as IEnumerator).MoveNext())
                {
                    handle.Yield = yieldObj;
                }
                else
                {
                    handle.Yield = null;
                }
            }
        }

        #endregion

        private class CoroutineHandle
        {
            public RadicalCoroutine Routine;
            public object Yield;
        }

        private class WaitForEditorDuration
        {
            public System.DateTime Start;
            public double Duration;
        }

    }
}
