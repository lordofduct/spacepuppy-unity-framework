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
        private ListDictionary<SPComponent, RadicalCoroutine> _routines = new ListDictionary<SPComponent, RadicalCoroutine>();

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

        public IEnumerable<ManagedCoroutineInfo> GetCoroutineInfo(SPComponent behaviour)
        {
            IList<RadicalCoroutine> lst;
            if(_routines.Lists.TryGetList(behaviour, out lst))
            {
                var arr = lst.ToArray();
                foreach(var r in arr)
                {
                    yield return new ManagedCoroutineInfo(behaviour, r);
                }
            }
        }

        internal void PurgeCoroutines(SPComponent behaviour)
        {
            IList<RadicalCoroutine> lst;
            if (_routines.Lists.TryGetList(behaviour, out lst))
            {
                var arr = lst.ToArray();
                foreach (var r in arr)
                {
                    r.Cancel();
                }
                _routines.Remove(behaviour);
            }
        }

        internal void RegisterCoroutine(SPComponent component, RadicalCoroutine routine)
        {
            if (_routines.Contains(routine)) return;

            if(!_routines.ContainsKey(component))
            {
                component.OnEnabled -= this.OnComponentEnabled;
                component.OnDisabled -= this.OnComponentDisabled;
                component.ComponentDestroyed -= this.OnComponentDestroyed;
                component.OnEnabled += this.OnComponentEnabled;
                component.OnDisabled += this.OnComponentDisabled;
                component.ComponentDestroyed += this.OnComponentDestroyed;
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
            var owner = routine.Operator as SPComponent;
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

        private void OnComponentDestroyed(object sender, System.EventArgs e)
        {
            var component = sender as SPComponent;
            if (!_routines.ContainsKey(component)) return;

            var lst = _routines.Lists[component];
        }


        private void DealWithEnable(SPComponent component, IList<RadicalCoroutine> lst)
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
                            //if the routine is currently active, that means the routine was already running. This could be because it 
                            //was started in Awake, in an override of OnEnable, or the routine is in a mode that does not pause it OnDisable.
                            continue;
                        case RadicalCoroutineOperatingState.Inactive:
                            if (routine.DisableMode.HasFlag(RadicalCoroutineDisableMode.ResumeOnEnable))
                            {
                                routine.Resume(component);
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

        private void DealWithDisable(SPComponent component, IList<RadicalCoroutine> lst)
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
                component.OnEnabled -= this.OnComponentEnabled;
                component.OnDisabled -= this.OnComponentDisabled;
                component.ComponentDestroyed -= this.OnComponentDestroyed;
            }
        }

        private void DealWithDestroy(SPComponent component, IList<RadicalCoroutine> lst)
        {
            if (lst.Count > 0)
            {
                foreach (var routine in lst)
                {
                    routine.Cancel();
                    routine.OnFinished -= this.OnRoutineFinished;
                }
            }
            lst.Clear();
            _routines.Remove(component);
            component.OnEnabled -= this.OnComponentEnabled;
            component.OnDisabled -= this.OnComponentDisabled;
            component.ComponentDestroyed -= this.OnComponentDestroyed;
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



    /*
     * OLD support MonoBehaviour version. Problem is it can't track if the MonoBehaviour is destroyed, or had StopCoroutine or StopAllCoroutines called on them. So references could remain in memory.
     * So I've removed support for this feature until unity gives me better hooks into Coroutines.

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

            foreach (var pair in _routines)
            {
                if (!(pair.Key is SPComponent))
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
            foreach (var pair in _routines)
            {
                if (pair.Value.Count > 0)
                {
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        yield return new ManagedCoroutineInfo(pair.Key, pair.Value[i]);
                    }
                }
            }
        }

        public IEnumerable<ManagedCoroutineInfo> GetCoroutineInfo(MonoBehaviour behaviour)
        {
            IList<RadicalCoroutine> lst;
            if (_routines.Lists.TryGetList(behaviour, out lst))
            {
                var arr = lst.ToArray();
                foreach (var r in arr)
                {
                    yield return new ManagedCoroutineInfo(behaviour, r);
                }
            }
        }

        internal void PurgeCoroutines(MonoBehaviour behaviour)
        {
            IList<RadicalCoroutine> lst;
            if (_routines.Lists.TryGetList(behaviour, out lst))
            {
                var arr = lst.ToArray();
                foreach (var r in arr)
                {
                    r.Cancel();
                }
                _routines.Remove(behaviour);
            }
        }

        internal void RegisterCoroutine(MonoBehaviour component, RadicalCoroutine routine)
        {
            if (_routines.Contains(routine)) return;

            if (!(component is SPComponent) && routine.DisableMode > RadicalCoroutineDisableMode.Default && routine.DisableMode != RadicalCoroutineDisableMode.ResumeOnEnable) Debug.LogWarning("A RadicalCoroutine started by a component that doesn't inherit from SPComponent can not be treated with a DisableMode other than Default.", component);

            if (component is SPComponent && !_routines.ContainsKey(component))
            {
                var c = component as SPComponent;
                c.OnEnabled -= this.OnComponentEnabled;
                c.OnDisabled -= this.OnComponentDisabled;
                c.ComponentDestroyed -= this.OnComponentDestroyed;
                c.OnEnabled += this.OnComponentEnabled;
                c.OnDisabled += this.OnComponentDisabled;
                c.ComponentDestroyed += this.OnComponentDestroyed;
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

        private void OnComponentDestroyed(object sender, System.EventArgs e)
        {
            var component = sender as SPComponent;
            if (!_routines.ContainsKey(component)) return;

            var lst = _routines.Lists[component];
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
                            //if the routine is currently active, that means the routine was already running. This could be because it 
                            //was started in Awake, in an override of OnEnable, or the routine is in a mode that does not pause it OnDisable.
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

            if (lst.Count == 0)
            {
                _routines.Remove(component);
                if (component is SPComponent)
                {
                    var c = component as SPComponent;
                    c.OnEnabled -= this.OnComponentEnabled;
                    c.OnDisabled -= this.OnComponentDisabled;
                    c.ComponentDestroyed -= this.OnComponentDestroyed;
                }
            }
        }

        private void DealWithDestroy(MonoBehaviour component, IList<RadicalCoroutine> lst)
        {
            if (lst.Count > 0)
            {
                foreach (var routine in lst)
                {
                    routine.Cancel();
                    routine.OnFinished -= this.OnRoutineFinished;
                }
            }
            lst.Clear();
            _routines.Remove(component);
            if (component is SPComponent)
            {
                var c = component as SPComponent;
                c.OnEnabled -= this.OnComponentEnabled;
                c.OnDisabled -= this.OnComponentDisabled;
                c.ComponentDestroyed -= this.OnComponentDestroyed;
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
     */
}
