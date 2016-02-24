using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy
{

    [DisallowMultipleComponent()]
    public sealed class RadicalCoroutineManager : MonoBehaviour
    {

        #region Fields

        private HashSet<RadicalCoroutine> _routines = new HashSet<RadicalCoroutine>();
        private Dictionary<MonoBehaviour, bool> _naiveTrackerTable;

        private System.EventHandler _onDisableHandler;
        private System.EventHandler _onEnabledHandler;
        private System.EventHandler _onDestroyHandler;

        #endregion

        #region CONSTRUCTOR
        
        private void Awake()
        {
            _onDisableHandler = this.OnComponentDisabled;
            _onEnabledHandler = this.OnComponentEnabled;
            _onDestroyHandler = this.OnComponentDestroyed;

            this.enabled = false;
        }

        #endregion

        #region Update Messages

        private void OnDisable()
        {
            if (_naiveTrackerTable == null || _naiveTrackerTable.Count == 0) return;

            this.TestNaive();
        }

        private void Update()
        {
            if(_naiveTrackerTable == null || _naiveTrackerTable.Count == 0)
            {
                this.enabled = false;
                return;
            }

            this.TestNaive();
        }

        private void TestNaive()
        {
            //yes, this method of tracking may seem convoluted with the weird temp lists
            //this is to keep GC to a minimum, if not zero
            TempList<MonoBehaviour> stateChanged = null;
            var e = _naiveTrackerTable.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Key == null)
                {
                    if (stateChanged == null) stateChanged = TempCollection.GetList<MonoBehaviour>();
                    stateChanged.Add(e.Current.Key);
                }
                else if (e.Current.Value != e.Current.Key.isActiveAndEnabled)
                {
                    if (stateChanged == null) stateChanged = TempCollection.GetList<MonoBehaviour>();
                    stateChanged.Add(e.Current.Key);
                }
            }

            if (stateChanged != null)
            {
                for (int i = 0; i < stateChanged.Count; i++)
                {
                    var c = stateChanged[i];
                    if (c == null)
                    {
                        this.PurgeCoroutines(c);
                    }
                    else if (c.isActiveAndEnabled)
                    {
                        _naiveTrackerTable[c] = true;
                        this.DealWithEnable(c);
                    }
                    else
                    {
                        _naiveTrackerTable[c] = false;
                        this.DealWithDisable(c);
                    }
                }
                stateChanged.Dispose();
            }
        }

        #endregion

        #region Methods

        public IEnumerable<RadicalCoroutine> GetAllCoroutines()
        {
            var e = _routines.GetEnumerator();
            while(e.MoveNext())
            {
                yield return e.Current;
            }
        }

        public IEnumerable<RadicalCoroutine> GetCoroutines(MonoBehaviour behaviour)
        {
            if (behaviour == null) yield break;

            var e = _routines.GetEnumerator();
            while (e.MoveNext())
            {
                if(e.Current.Operator == behaviour) yield return e.Current;
            }
        }

        internal void PurgeCoroutines(MonoBehaviour component)
        {
            using (var lst = TempCollection.GetList<RadicalCoroutine>())
            {
                var e = _routines.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.Operator == component) lst.Add(e.Current);
                }

                if(lst.Count > 0)
                {
                    for(int i = 0; i < lst.Count; i++)
                    {
                        lst[i].Cancel(true);
                        _routines.Remove(lst[i]);
                    }
                }
                if(component is SPComponent)
                {
                    var spc = component as SPComponent;
                    spc.OnDisabled -= _onDisableHandler;
                    spc.OnEnabled -= _onEnabledHandler;
                    spc.ComponentDestroyed -= _onDestroyHandler;
                }
                else if(_naiveTrackerTable != null)
                {
                    _naiveTrackerTable.Remove(component);
                }
            }
        }

        /// <summary>
        /// Must be only called by RadicalCoroutine itself.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="routine"></param>
        internal void RegisterCoroutine(RadicalCoroutine routine)
        {
            if (routine == null) throw new System.ArgumentNullException("routine");

            //if (_routines.Contains(routine)) throw new System.InvalidOperationException("Attempted to register a routine that is already operating.");
            if (_routines.Contains(routine)) return;

            var component = routine.Operator;
            if (component == null) throw new System.InvalidOperationException("Attempted to register a routine with a null component.");

            if (component is SPComponent)
            {
                var spc = component as SPComponent;
                spc.OnDisabled -= _onDisableHandler;
                spc.OnEnabled -= _onEnabledHandler;
                spc.ComponentDestroyed -= _onDestroyHandler;
                spc.OnDisabled += _onDisableHandler;
                spc.OnEnabled += _onEnabledHandler;
                spc.ComponentDestroyed += _onDestroyHandler;

                _routines.Add(routine);
            }
            else
            {
                if (_naiveTrackerTable == null) _naiveTrackerTable = new Dictionary<MonoBehaviour, bool>();
                _naiveTrackerTable[component] = component.isActiveAndEnabled;
                _routines.Add(routine);
                if (!this.enabled) this.enabled = true;
            }

        }

        /// <summary>
        /// Must be only called by RadicalCoroutine itself.
        /// </summary>
        /// <param name="routine"></param>
        internal void UnregisterCoroutine(RadicalCoroutine routine)
        {
            _routines.Remove(routine);

            if(_naiveTrackerTable != null)
            {
                var comp = routine.Operator;
                if(_naiveTrackerTable.ContainsKey(comp) && !this.GetComponentIsCurrentlyManaged(comp))
                {
                    _naiveTrackerTable.Remove(comp);
                }
            }
        }
        
        private void OnComponentEnabled(object sender, System.EventArgs e)
        {
            var component = sender as MonoBehaviour;
            if (object.ReferenceEquals(component, null)) return;
            this.DealWithEnable(component);
        }

        private void OnComponentDisabled(object sender, System.EventArgs e)
        {
            var component = sender as MonoBehaviour;
            if (object.ReferenceEquals(component, null)) return;
            this.DealWithDisable(component);
        }

        private void OnComponentDestroyed(object sender, System.EventArgs e)
        {
            var component = sender as MonoBehaviour;
            if (object.ReferenceEquals(component, null)) return;
            this.PurgeCoroutines(component);
        }


        private void DealWithEnable(MonoBehaviour component)
        {
            using (var lst = TempCollection.GetList<RadicalCoroutine>())
            {
                var e = _routines.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.Operator == component)
                    {
                        switch (e.Current.OperatingState)
                        {
                            case RadicalCoroutineOperatingState.Active:
                                //if the routine is currently active, that means the routine was already running. This could be because it 
                                //was started in Awake, in an override of OnEnable, or the routine is in a mode that does not pause it OnDisable.
                                continue;
                            case RadicalCoroutineOperatingState.Inactive:
                            case RadicalCoroutineOperatingState.Paused:
                                if (e.Current.DisableMode.HasFlag(RadicalCoroutineDisableMode.Resumes))
                                {
                                    e.Current.Resume(component);
                                }
                                else
                                {
                                    lst.Add(e.Current);
                                    Debug.LogWarning("A leaked RadicalCoroutine was found and cleaned up.", component);
                                }
                                break;
                            default:
                                //somehow a finished routine made its way into the collection... remove it
                                lst.Add(e.Current);
                                Debug.LogWarning("A leaked RadicalCoroutine was found and cleaned up.", component);
                                break;
                        }
                    }
                }

                if(lst.Count > 0)
                {
                    for(int i = 0; i < lst.Count; i++)
                    {
                        _routines.Remove(lst[i]);
                    }
                }
            }
        }

        private void DealWithDisable(MonoBehaviour component)
        {
            using (var lst = TempCollection.GetList<RadicalCoroutine>())
            {
                var e = _routines.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.Operator == component) lst.Add(e.Current);
                }

                if (lst.Count > 0)
                {
                    RadicalCoroutine routine;
                    if (this.gameObject.activeInHierarchy)
                    {
                        for (int i = 0; i < lst.Count; i++)
                        {
                            routine = lst[i];
                            if (routine.DisableMode.HasFlag(RadicalCoroutineDisableMode.CancelOnDisable))
                            {
                                routine.Cancel(true);
                                _routines.Remove(lst[i]);
                                lst.RemoveAt(i);
                                i--;
                            }
                            else if(routine.DisableMode.HasFlag(RadicalCoroutineDisableMode.StopOnDisable))
                            {
                                routine.Stop(true);
                                if (routine.Finished)
                                {
                                    _routines.Remove(lst[i]);
                                    lst.RemoveAt(i);
                                    i--;
                                }
                                else if (!routine.DisableMode.HasFlag(RadicalCoroutineDisableMode.Resumes))
                                {
                                    _routines.Remove(lst[i]);
                                    lst.RemoveAt(i);
                                    i--;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < lst.Count; i++)
                        {
                            routine = lst[i];
                            if (routine.DisableMode.HasFlag(RadicalCoroutineDisableMode.StopOnDeactivate))
                            {
                                routine.Stop(true);
                                if (routine.Finished)
                                {
                                    _routines.Remove(lst[i]);
                                    lst.RemoveAt(i);
                                    i--;
                                }
                                else if (!routine.DisableMode.HasFlag(RadicalCoroutineDisableMode.Resumes))
                                {
                                    _routines.Remove(lst[i]);
                                    lst.RemoveAt(i);
                                    i--;
                                }
                            }
                            else
                            {
                                routine.Cancel(true);
                                _routines.Remove(lst[i]);
                                lst.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }
        }
        
        private bool GetComponentIsCurrentlyManaged(MonoBehaviour component)
        {
            var e = _routines.GetEnumerator();
            while(e.MoveNext())
            {
                if (e.Current.Operator == component) return true;
            }
            return false;
        }

        #endregion

        #region Special Types

        //private class NaiveComponentTracker
        //{

        //    public MonoBehaviour Component;
        //    public bool Active;

        //}

        #endregion

    }
    
}
