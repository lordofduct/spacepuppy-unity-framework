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
        
        private HashSet<ManagedCoroutineInfo> _routines = new HashSet<ManagedCoroutineInfo>(ManagedCoroutineInfoEqualityComparer.Default);

        #endregion

        #region Methods

        public IEnumerable<ManagedCoroutineInfo> GetCoroutineInfo()
        {
            return _routines;
        }

        public IEnumerable<ManagedCoroutineInfo> GetCoroutineInfo(MonoBehaviour behaviour)
        {
            return (from i in _routines where i.Component == behaviour select i);
        }

        internal void PurgeCoroutines(SPComponent component)
        {
            using (var lst = TempCollection.GetList<ManagedCoroutineInfo>())
            {
                var e = _routines.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.Component == component) lst.Add(e.Current);
                }

                if(lst.Count > 0)
                {
                    for(int i = 0; i < lst.Count; i++)
                    {
                        lst[i].Routine.Cancel(true);
                        _routines.Remove(lst[i]);
                    }
                }
                component.OnEnabled -= this.OnComponentEnabled;
                component.OnDisabled -= this.OnComponentDisabled;
                component.ComponentDestroyed -= this.OnComponentDestroyed;
            }
        }

        /// <summary>
        /// Must be only called by RadicalCoroutine itself.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="routine"></param>
        internal void RegisterCoroutine(SPComponent component, RadicalCoroutine routine)
        {
            if (routine == null) throw new System.ArgumentNullException("routine");

            var info = new ManagedCoroutineInfo(component, routine);
            if (_routines.Contains(info)) return;

            if(!this.GetComponentIsCurrentlyManaged(component))
            {
                //component.OnEnabled -= this.OnComponentEnabled;
                //component.OnDisabled -= this.OnComponentDisabled;
                //component.ComponentDestroyed -= this.OnComponentDestroyed;
                component.OnEnabled += this.OnComponentEnabled;
                component.OnDisabled += this.OnComponentDisabled;
                component.ComponentDestroyed += this.OnComponentDestroyed;
            }

            _routines.Add(info);
        }

        /// <summary>
        /// Must be only called by RadicalCoroutine itself.
        /// </summary>
        /// <param name="routine"></param>
        internal void UnregisterCoroutine(RadicalCoroutine routine)
        {
            var info = new ManagedCoroutineInfo(routine.Operator, routine);
            _routines.Remove(info);
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
            this.DealWithDestroy(component);
        }


        private void DealWithEnable(MonoBehaviour component)
        {
            using (var lst = TempCollection.GetList<ManagedCoroutineInfo>())
            {
                var e = _routines.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.Component == component)
                    {
                        switch (e.Current.Routine.OperatingState)
                        {
                            case RadicalCoroutineOperatingState.Active:
                                //if the routine is currently active, that means the routine was already running. This could be because it 
                                //was started in Awake, in an override of OnEnable, or the routine is in a mode that does not pause it OnDisable.
                                continue;
                            case RadicalCoroutineOperatingState.Inactive:
                                if (e.Current.Routine.DisableMode.HasFlag(RadicalCoroutineDisableMode.ResumeOnEnable))
                                {
                                    e.Current.Routine.Resume(component);
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
            using (var lst = TempCollection.GetList<ManagedCoroutineInfo>())
            {
                var e = _routines.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.Component == component) lst.Add(e.Current);
                }

                var stoppableMode = (this.gameObject.activeInHierarchy) ? RadicalCoroutineDisableMode.StopOnDisable : RadicalCoroutineDisableMode.StopOnDeactivate;
                RadicalCoroutine routine;
                
                for(int i = 0; i < lst.Count; i++)
                {
                    routine = lst[i].Routine;
                    if (routine.DisableMode == RadicalCoroutineDisableMode.CancelOnDeactivate || routine.DisableMode.HasFlag(RadicalCoroutineDisableMode.CancelOnDisable))
                    {
                        routine.Cancel(true);
                        _routines.Remove(lst[i]);
                        lst.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        if (routine.DisableMode.HasFlag(stoppableMode))
                        {
                            routine.Stop(true);
                            if (routine.Finished)
                            {
                                _routines.Remove(lst[i]);
                                lst.RemoveAt(i);
                                i--;
                            }
                        }
                        if (!routine.DisableMode.HasFlag(RadicalCoroutineDisableMode.ResumeOnEnable))
                        {
                            _routines.Remove(lst[i]);
                            lst.RemoveAt(i);
                            i--;
                        }
                    }
                }

                if (lst.Count == 0 && component is SPComponent)
                {
                    var c = component as SPComponent;
                    c.OnEnabled -= this.OnComponentEnabled;
                    c.OnDisabled -= this.OnComponentDisabled;
                    c.ComponentDestroyed -= this.OnComponentDestroyed;
                }
            }
        }

        private void DealWithDestroy(MonoBehaviour component)
        {
            using (var lst = TempCollection.GetList<ManagedCoroutineInfo>())
            {
                var e = _routines.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.Component == component) lst.Add(e.Current);
                }


                if(lst.Count > 0)
                {
                    for(int i = 0; i < lst.Count; i++)
                    {
                        lst[i].Routine.Cancel(true);
                        _routines.Remove(lst[i]);
                    }
                }
                if (component is SPComponent)
                {
                    var c = component as SPComponent;
                    c.OnEnabled -= this.OnComponentEnabled;
                    c.OnDisabled -= this.OnComponentDisabled;
                    c.ComponentDestroyed -= this.OnComponentDestroyed;
                }
            }
        }




        private bool GetComponentIsCurrentlyManaged(MonoBehaviour component)
        {
            var e = _routines.GetEnumerator();
            while(e.MoveNext())
            {
                if (e.Current.Component == component) return true;
            }
            return false;
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

        private class ManagedCoroutineInfoEqualityComparer : IEqualityComparer<ManagedCoroutineInfo>
        {

            private static ManagedCoroutineInfoEqualityComparer _default = new ManagedCoroutineInfoEqualityComparer();
            public static ManagedCoroutineInfoEqualityComparer Default { get { return _default; } }

            public bool Equals(ManagedCoroutineInfo x, ManagedCoroutineInfo y)
            {
                return x.Routine == y.Routine;
            }

            public int GetHashCode(ManagedCoroutineInfo obj)
            {
                if (obj.Routine == null) return 0;
                return obj.Routine.GetHashCode();
            }
        }

        #endregion

    }
    
}
