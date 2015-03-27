using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    [DisallowMultipleComponent()]
    public sealed class RadicalCoroutineManager : MonoBehaviour
    {

        #region Fields

        [System.NonSerialized()]
        private ListDictionary<MonoBehaviour, RadicalCoroutine> _routines = new ListDictionary<MonoBehaviour, RadicalCoroutine>();

        #endregion

        #region Messages

        private void OnEnable()
        {
            if (this.gameObject.activeInHierarchy) return;

            foreach(var pair in _routines)
            {
                if(!(pair.Key is SPComponent))
                {
                    this.DealWithEnable(pair.Key, pair.Value);
                }
            }
        }

        private void OnDisable()
        {
            foreach (var pair in _routines)
            {
                if (!(pair.Key is SPComponent))
                {
                    this.DealWithDisable(pair.Key, pair.Value);
                }
            }
        }

        #endregion

        #region Methods

        public IEnumerable<ManagedCoroutineInfo> GetCoroutineInfo()
        {
            foreach(var pair in _routines)
            {
                if(pair.Value.Count > 0)
                {
                    for(int i = 0; i < pair.Value.Count; i++)
                    {
                        yield return new ManagedCoroutineInfo(pair.Key, pair.Value[i]);
                    }
                }
            }
        }



        internal void RegisterCoroutine(MonoBehaviour component, RadicalCoroutine routine)
        {
            if (_routines.Contains(routine)) return;

            if (!(component is SPComponent) && routine.DisableMode.HasFlag(RadicalCoroutineDisableMode.StopOnDisable)) Debug.LogWarning("A RadicalCoroutine started by a component that doesn't inherit from SPComponent can not pause the coroutine 'OnDisable'.", component);

            if(component is SPComponent && !_routines.ContainsKey(component))
            {
                (component as SPComponent).OnEnabled -= this.OnComponentEnabled;
                (component as SPComponent).OnDisabled -= this.OnComponentDisabled;
                (component as SPComponent).OnEnabled += this.OnComponentEnabled;
                (component as SPComponent).OnDisabled += this.OnComponentDisabled;
            }

            routine.OnFinished -= this.OnRoutineFinished;
            routine.OnFinished += this.OnRoutineFinished;
            _routines.Add(component, routine);
        }

        private void OnRoutineFinished(object sender, System.EventArgs e)
        {
            var routine = sender as RadicalCoroutine;
            if (routine == null) return;

            routine.OnComplete -= this.OnRoutineFinished;
            var owner = routine.Operator as MonoBehaviour;
            if (!object.ReferenceEquals(owner, null) && _routines.ContainsKey(owner)) _routines.Lists[owner].Remove(routine);
        }

        private void OnComponentEnabled(object sender, System.EventArgs e)
        {
            var component = sender as SPComponent;
            if (!_routines.ContainsKey(component)) return;

            var lst = _routines.Lists[component];
            this.DealWithEnable(component, lst);
        }

        private void OnComponentDisabled(object sender, System.EventArgs e)
        {
            var component = sender as SPComponent;
            if (!_routines.ContainsKey(component)) return;

            var lst = _routines.Lists[component];
            this.DealWithDisable(component, lst);
        }


        private void DealWithEnable(MonoBehaviour component, IList<RadicalCoroutine> lst)
        {
            if (lst.Count > 0)
            {
                RadicalCoroutine routine;
                for (int i = 0; i < lst.Count; i++)
                {
                    routine = lst[i];
                    switch (routine.OperatingState)
                    {
                        case RadicalCoroutineOperatingState.Active:
                            //if the routine is currently active, that means that the routine was started before OnEnable was called.
                            //either on Awake, or in an override of OnEnable
                            continue;
                        case RadicalCoroutineOperatingState.Inactive:
                            if (routine.DisableMode.HasFlag(RadicalCoroutineDisableMode.ResumeOnEnable) && component is SPComponent)
                            {
                                routine.Resume(component as SPComponent);
                            }
                            else
                            {
                                routine.OnFinished -= this.OnRoutineFinished;
                                lst.RemoveAt(i);
                                i--;
                                Debug.LogWarning("A leaked RadicalCoroutine was found and cleaned up.", component);
                            }
                            break;
                        default:
                            //somehow a finished routine made its way into the collection... remove it
                            routine.OnFinished -= this.OnRoutineFinished;
                            lst.RemoveAt(i);
                            i--;
                            Debug.LogWarning("A leaked RadicalCoroutine was found and cleaned up.", component);
                            break;
                    }
                }
            }
        }

        private void DealWithDisable(MonoBehaviour component, IList<RadicalCoroutine> lst)
        {
            if (lst.Count > 0)
            {
                var arr = lst.ToArray();
                var stoppableMode = (this.gameObject.activeInHierarchy) ? RadicalCoroutineDisableMode.StopOnDisable : RadicalCoroutineDisableMode.StopOnDeactivate;
                RadicalCoroutine routine;
                for (int i = 0; i < arr.Length; i++)
                {
                    routine = arr[i];
                    if (routine.DisableMode == RadicalCoroutineDisableMode.CancelOnDeactivate || routine.DisableMode.HasFlag(RadicalCoroutineDisableMode.CancelOnDisable))
                    {
                        routine.Cancel();
                        routine.OnFinished -= this.OnRoutineFinished;
                        lst.Remove(routine);
                    }
                    else
                    {
                        if (routine.DisableMode.HasFlag(stoppableMode))
                        {
                            routine.Stop();
                            if (routine.Finished)
                            {
                                routine.OnFinished -= this.OnRoutineFinished;
                                lst.Remove(routine);
                            }
                        }
                        if (!routine.DisableMode.HasFlag(RadicalCoroutineDisableMode.ResumeOnEnable))
                        {
                            routine.OnFinished -= this.OnRoutineFinished;
                            lst.Remove(routine);
                        }
                    }
                }
            }

            if(lst.Count == 0)
            {
                _routines.Remove(component);
                if(component is SPComponent)
                {
                    (component as SPComponent).OnEnabled -= this.OnComponentEnabled;
                    (component as SPComponent).OnDisabled -= this.OnComponentDisabled;
                }
            }
        }

        #endregion


        #region Special Types

        public struct ManagedCoroutineInfo
        {
            public MonoBehaviour Component;
            public RadicalCoroutine Routine;

            public ManagedCoroutineInfo(MonoBehaviour c, RadicalCoroutine r)
            {
                this.Component = c;
                this.Routine = r;
            }
        }

        #endregion

    }
}
