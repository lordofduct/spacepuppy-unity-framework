using System;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Collections
{
    public class LiveSPComponentCollection<T> : ICollection<T> where T : SPComponent
    {

        #region Hook

        private event System.Action _onUpdate;
        public event System.Action OnUpdate
        {
            add
            {
                _onUpdate += value;
                if(_onUpdate != null && _routine == null && !object.ReferenceEquals(_currentMaster, null))
                {
                    _routine = _currentMaster.StartRadicalCoroutine(this.UpdateRoutine(), RadicalCoroutineDisableMode.CancelOnDisable);
                }
            }
            remove
            {
                _onUpdate -= value;
                if(_onUpdate == null && _routine != null)
                {
                    _routine.Cancel();
                    _routine = null;
                }
            }
        }

        #endregion

        #region Methods

        private List<T> _components = new List<T>();
        private List<T> _liveComponents = new List<T>();
        private T _currentMaster;
        private RadicalCoroutine _routine;

        private float _updateInterval;

        #endregion

        #region CONSTRUCTOR

        public LiveSPComponentCollection()
        {

        }

        public LiveSPComponentCollection(float updateInterval)
        {
            _updateInterval = updateInterval;
        }

        #endregion

        #region Properties

        public float UpdateInterval
        {
            get { return _updateInterval; }
            set { _updateInterval = value; }
        }

        public int LiveCount
        {
            get { return _liveComponents.Count; }
        }

        #endregion

        #region Methods

        public bool IsAlive(T item)
        {
            return _liveComponents.Contains(item);
        }

        public T GetLiveObject(int index)
        {
            if (index < 0 || index >= _liveComponents.Count) throw new System.IndexOutOfRangeException();

            return _liveComponents[index];
        }

        public T PopOldest()
        {
            if (_liveComponents.Count == 0) throw new InvalidOperationException("Live objects is empty.");
            var item = _liveComponents[0];
            this.Remove(item);
            return item;
        }

        public T[] PopOldest(int count)
        {
            if (count > _liveComponents.Count) throw new InvalidOperationException("Count is greater than number of live objects.");

            T[] arr = new T[count];
            for(int i = 0; i < count; i++)
            {
                arr[i] = _liveComponents[0];
                _components.Remove(arr[i]);
                _liveComponents.RemoveAt(0);
            }

            if (arr.Contains(_currentMaster))
            {
                if (_routine != null)
                {
                    _routine.Cancel();
                    _routine = null;
                }

                if (_liveComponents.Count > 0)
                {
                    _currentMaster = _liveComponents[0];
                    if (_onUpdate != null)
                    {
                        _routine = _currentMaster.StartRadicalCoroutine(this.UpdateRoutine(), RadicalCoroutineDisableMode.CancelOnDisable);
                    }
                }
                else
                {
                    _currentMaster = null;
                }
            }

            return arr;
        }

        public T PopYoungest()
        {
            if (_liveComponents.Count == 0) throw new InvalidOperationException("Live objects is empty.");
            var item = _liveComponents[_liveComponents.Count - 1];
            this.Remove(item);
            return item;
        }

        public T[] PopYoungest(int count)
        {
            if (count > _liveComponents.Count) throw new InvalidOperationException("Count is greater than number of live objects.");

            T[] arr = new T[count];
            for (int i = 0; i < count; i++)
            {
                arr[i] = _liveComponents[_liveComponents.Count - 1];
                _components.Remove(arr[i]);
                _liveComponents.RemoveAt(_liveComponents.Count - 1);
            }

            if(arr.Contains(_currentMaster))
            {
                if (_routine != null)
                {
                    _routine.Cancel();
                    _routine = null;
                }

                if (_liveComponents.Count > 0)
                {
                    _currentMaster = _liveComponents[0];
                    if (_onUpdate != null)
                    {
                        _routine = _currentMaster.StartRadicalCoroutine(this.UpdateRoutine(), RadicalCoroutineDisableMode.CancelOnDisable);
                    }
                }
                else
                {
                    _currentMaster = null;
                }
            }

            return arr;
        }

        #endregion

        #region Event Handlers

        private void OnComponentEnabled(object sender, System.EventArgs e)
        {
            var item = sender as T;
            if (item == null) return;
            if (_liveComponents.Contains(item)) return;

            _liveComponents.Add(item);
            if (_liveComponents.Count == 1)
            {
                _currentMaster = _liveComponents[0];
                if (_onUpdate != null) _routine = _currentMaster.StartRadicalCoroutine(this.UpdateRoutine(), RadicalCoroutineDisableMode.CancelOnDisable);
            }
        }

        private void OnComponentDisabled(object sender, System.EventArgs e)
        {
            var item = sender as T;
            if (item == null) return;

            _liveComponents.Remove(item);
            if(item == _currentMaster)
            {
                if (_routine != null)
                {
                    _routine.Cancel();
                    _routine = null;
                }

                if (_liveComponents.Count > 0)
                {
                    _currentMaster = _liveComponents[0];
                    if (_onUpdate != null)
                    {
                        _routine = _currentMaster.StartRadicalCoroutine(this.UpdateRoutine(), RadicalCoroutineDisableMode.CancelOnDisable);
                    }
                }
                else
                {
                    _currentMaster = null;
                }
            }
        }

        private void OnComponentDestroyed(object sender, System.EventArgs e)
        {
            var item = sender as T;
            if (item == null) return;

            _components.Remove(item);
            _liveComponents.Remove(item);
            if (item == _currentMaster)
            {
                if (_routine != null)
                {
                    _routine.Cancel();
                    _routine = null;
                }

                if (_liveComponents.Count > 0)
                {
                    _currentMaster = _liveComponents[0];
                    if (_onUpdate != null)
                    {
                        _routine = _currentMaster.StartRadicalCoroutine(this.UpdateRoutine(), RadicalCoroutineDisableMode.CancelOnDisable);
                    }
                }
                else
                {
                    _currentMaster = null;
                }
            }
        }

        private System.Collections.IEnumerator UpdateRoutine()
        {
            yield return null;

            while(true)
            {
                if (_onUpdate != null) _onUpdate();

                if (_updateInterval > 0f)
                {
                    yield return WaitForDuration.Seconds(_updateInterval);
                }
                else
                {
                    yield return null;
                }
            }
        }

        #endregion

        #region ICollection Interface

        public int Count
        {
            get { return _components.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        public void Add(T item)
        {
            if (item == null) throw new System.ArgumentNullException("item");
            if (_components.Contains(item)) return;

            _components.Add(item);
            if(item.isActiveAndEnabled)
            {
                _liveComponents.Add(item);
                if (_liveComponents.Count == 1)
                {
                    _currentMaster = _liveComponents[0];
                    if(_onUpdate != null) _routine = _currentMaster.StartRadicalCoroutine(this.UpdateRoutine(), RadicalCoroutineDisableMode.CancelOnDisable);
                }
            }
        }

        public bool Remove(T item)
        {
            if (object.ReferenceEquals(item, null)) return false;

            if (_components.Remove(item))
            {
                _liveComponents.Remove(item);
                item.OnEnabled -= this.OnComponentEnabled;
                item.OnDisabled -= this.OnComponentDisabled;
                item.ComponentDestroyed -= this.OnComponentDestroyed;

                if (item == _currentMaster)
                {
                    if (_routine != null)
                    {
                        _routine.Cancel();
                        _routine = null;
                    }

                    if(_liveComponents.Count > 0)
                    {
                        _currentMaster = _liveComponents[0];
                        if (_onUpdate != null)
                        {
                            _routine = _currentMaster.StartRadicalCoroutine(this.UpdateRoutine(), RadicalCoroutineDisableMode.CancelOnDisable);
                        }
                    }
                    else
                    {
                        _currentMaster = null;
                    }
                }
                return true;
            }

            return false;
        }

        public void Clear()
        {
            foreach(var item in _liveComponents)
            {
                item.OnEnabled -= this.OnComponentEnabled;
                item.OnDisabled -= this.OnComponentDisabled;
                item.ComponentDestroyed -= this.OnComponentDestroyed;
            }

            if(_routine != null)
            {
                _routine.Cancel();
                _routine = null;
            }

            _components.Clear();
            _liveComponents.Clear();
        }

        public bool Contains(T item)
        {
            return _components.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _components.CopyTo(array, arrayIndex);
        }

        #endregion

        #region IEnumerable Interface

        public System.Collections.IEnumerator GetEnumerator()
        {
            return _components.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _components.GetEnumerator();
        }

        #endregion

    }
}
