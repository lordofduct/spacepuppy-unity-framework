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

        private HashSet<RadicalCoroutine> _routines = new HashSet<RadicalCoroutine>();
        private Dictionary<MonoBehaviour, bool> _naiveTrackerTable;
        private Dictionary<object, RadicalCoroutine> _autoKillTable;

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
                        lst[i].ManagerCancel();
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

        internal void RegisterCoroutine(RadicalCoroutine routine, object autoKillToken)
        {
            if (autoKillToken == null) throw new System.ArgumentNullException("autoKillToken");

            if (_autoKillTable == null)
            {
                _autoKillTable = new Dictionary<object, RadicalCoroutine>();
            }
            else
            {
                RadicalCoroutine old;
                if (_autoKillTable.TryGetValue(autoKillToken, out old))
                {
                    old.ManagerCancel();
                }
            }
            _autoKillTable[autoKillToken] = routine;
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

            if(_autoKillTable != null && routine.AutoKillToken != null)
            {
                RadicalCoroutine other;
                if(_autoKillTable.TryGetValue(routine.AutoKillToken, out other))
                {
                    if (object.ReferenceEquals(other, routine)) _autoKillTable.Remove(routine.AutoKillToken);
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
            using (var todestroy = TempCollection.GetList<RadicalCoroutine>())
            using(var toresume = TempCollection.GetList<RadicalCoroutine>())
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
                                if ((e.Current.DisableMode & RadicalCoroutineDisableMode.Resumes) != 0)
                                {
                                    toresume.Add(e.Current);
                                }
                                else
                                {
                                    todestroy.Add(e.Current);
                                    Debug.LogWarning("A leaked RadicalCoroutine was found and cleaned up.", component);
                                }
                                break;
                            default:
                                //somehow a finished routine made its way into the collection... remove it
                                todestroy.Add(e.Current);
                                Debug.LogWarning("A leaked RadicalCoroutine was found and cleaned up.", component);
                                break;
                        }
                    }
                }

                if (todestroy.Count > 0)
                {
                    for (int i = 0; i < todestroy.Count; i++)
                    {
                        _routines.Remove(todestroy[i]);
                    }
                }

                if (toresume.Count > 0)
                {
                    for(int i = 0; i < toresume.Count; i++)
                    {
                        toresume[i].Resume();
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
                            if ((routine.DisableMode & RadicalCoroutineDisableMode.CancelOnDisable) != 0)
                            {
                                routine.ManagerCancel();
                                _routines.Remove(routine);
                            }
                            else if ((routine.DisableMode & RadicalCoroutineDisableMode.StopOnDisable) != 0)
                            {
                                if (!routine.Finished && (routine.DisableMode & RadicalCoroutineDisableMode.Resumes) != 0)
                                {
                                    routine.Pause();
                                }
                                else
                                {
                                    routine.Stop();
                                    _routines.Remove(routine);
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < lst.Count; i++)
                        {
                            routine = lst[i];
                            if ((routine.DisableMode & RadicalCoroutineDisableMode.StopOnDeactivate) != 0)
                            {
                                if (!routine.Finished && (routine.DisableMode & RadicalCoroutineDisableMode.Resumes) != 0)
                                {
                                    routine.Pause();
                                }
                                else
                                {
                                    routine.Stop();
                                    _routines.Remove(routine);
                                }
                            }
                            else
                            {
                                routine.ManagerCancel();
                                _routines.Remove(routine);
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



        public void AutoKill(object autoKillToken)
        {
            if (autoKillToken == null) throw new System.ArgumentNullException("autoKillToken");

            if(_autoKillTable != null)
            {
                RadicalCoroutine old;
                if (_autoKillTable.TryGetValue(autoKillToken, out old))
                {
                    old.ManagerCancel();
                    _autoKillTable.Remove(autoKillToken);
                }
            }
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
