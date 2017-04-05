using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.Sensors
{

    public class CompositeSensor : Sensor, IEnumerable<Sensor>
    {

        #region Fields

        [SerializeField]
        private bool _mustBeVisibleByAll;

        [System.NonSerialized()]
        private Sensor[] _sensors;

        #endregion

        #region CONSTRUCTOR

        protected override void OnStartOrEnable()
        {
            base.OnStartOrEnable();

            this.SyncChildSensors();
        }

        #endregion

        #region Methods

        public void SyncChildSensors()
        {
            _sensors = (from c in this.GetAllChildrenAndSelf() from s in c.GetComponents<Sensor>() where !object.ReferenceEquals(s, this) select s).ToArray();
            //_sensors = this.GetChildComponents<Sensor>(true)
        }

        public bool Contains(Sensor sensor)
        {
            if (_sensors == null) return false;

            return System.Array.IndexOf(_sensors, sensor) >= 0;
        }

        #endregion

        #region Sensor Interface

        public override bool ConcernedWith(UnityEngine.Object obj)
        {
            if (_sensors == null) this.SyncChildSensors();
            if (_sensors.Length == 0) return false;

            if (_mustBeVisibleByAll)
            {
                for (int i = 0; i < _sensors.Length; i++)
                {
                    if (!_sensors[i].ConcernedWith(obj)) return false;
                }
                return true;
            }
            else
            {
                for(int i = 0; i < _sensors.Length; i++)
                {
                    if (_sensors[i].ConcernedWith(obj)) return true;
                }
                return false;
            }
        }

        public override bool SenseAny(System.Func<IAspect, bool> p = null)
        {
            if (_sensors == null) this.SyncChildSensors();
            if (_sensors.Length == 0) return false;

            if(_mustBeVisibleByAll && _sensors.Length > 1)
            {
                using (var set = com.spacepuppy.Collections.TempCollection.GetSet<IAspect>())
                {
                    _sensors[0].SenseAll(set, p);
                    var e = set.GetEnumerator();
                    while(e.MoveNext())
                    {
                        int cnt = 1;
                        for (int i = 1; i < _sensors.Length; i++)
                        {
                            if (!_sensors[i].Visible(e.Current)) cnt++;
                        }
                        if (cnt == _sensors.Length) return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _sensors.Length; i++)
                {
                    if (_sensors[i].SenseAny(p)) return true;
                }
            }

            return false;
        }

        public override IAspect Sense(System.Func<IAspect, bool> p = null)
        {
            if (_sensors == null) this.SyncChildSensors();
            if (_sensors.Length == 0) return null;

            if (_mustBeVisibleByAll && _sensors.Length > 1)
            {
                using (var set = com.spacepuppy.Collections.TempCollection.GetSet<IAspect>())
                {
                    _sensors[0].SenseAll(set, p);
                    var e = set.GetEnumerator();
                    while (e.MoveNext())
                    {
                        int cnt = 1;
                        for (int i = 1; i < _sensors.Length; i++)
                        {
                            if (!_sensors[i].Visible(e.Current)) cnt++;
                        }
                        if (cnt == _sensors.Length) return e.Current;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _sensors.Length; i++)
                {
                    var a = _sensors[i].Sense(p);
                    if (a != null) return a;
                }
            }
            return null;
        }

        public override IEnumerable<IAspect> SenseAll(System.Func<IAspect, bool> p = null)
        {
            if (_sensors == null) this.SyncChildSensors();

            if (_sensors.Length == 0)
                return Enumerable.Empty<IAspect>();
            else if (_sensors.Length == 1)
                return _sensors[0].SenseAll(p);
            else
            {
                if (_mustBeVisibleByAll && _sensors.Length > 1)
                {
                    using (var set = com.spacepuppy.Collections.TempCollection.GetSet<IAspect>())
                    using(var results = com.spacepuppy.Collections.TempCollection.GetSet<IAspect>())
                    {
                        _sensors[0].SenseAll(set, p);
                        var e = set.GetEnumerator();
                        while (e.MoveNext())
                        {
                            int cnt = 1;
                            for (int i = 1; i < _sensors.Length; i++)
                            {
                                if (!_sensors[i].Visible(e.Current)) cnt++;
                            }
                            if (cnt == _sensors.Length) results.Add(e.Current);
                        }
                        return results.ToArray();
                    }
                }
                else
                {
                    return (from s in _sensors from a in s.SenseAll(p) select a).Distinct();
                }
            }
        }

        public override int SenseAll(ICollection<IAspect> lst, System.Func<IAspect, bool> p = null)
        {
            if (lst == null) throw new System.ArgumentNullException("lst");
            if (lst.IsReadOnly) throw new System.ArgumentException("List to fill can not be read-only.", "lst");
            if (_sensors == null) this.SyncChildSensors();
            if (_sensors.Length == 0) return 0;
            
            if (_mustBeVisibleByAll && _sensors.Length > 1)
            {
                using (var set = com.spacepuppy.Collections.TempCollection.GetSet<IAspect>())
                {
                    int resultCnt = 0;
                    _sensors[0].SenseAll(set, p);
                    var e = set.GetEnumerator();
                    while (e.MoveNext())
                    {
                        int cnt = 1;
                        for (int i = 1; i < _sensors.Length; i++)
                        {
                            if (!_sensors[i].Visible(e.Current)) cnt++;
                        }
                        if (cnt == _sensors.Length)
                        {
                            resultCnt++;
                            lst.Add(e.Current);
                        }
                    }
                    return resultCnt;
                }
            }
            else
            {
                int cnt = 0;
                for (int i = 0; i < _sensors.Length; i++)
                {
                    cnt += _sensors[i].SenseAll(lst, p);
                }
                return cnt;
            }
        }

        public override int SenseAll<T>(ICollection<T> lst, System.Func<T, bool> p = null)
        {
            if (lst == null) throw new System.ArgumentNullException("lst");
            if (lst.IsReadOnly) throw new System.ArgumentException("List to fill can not be read-only.", "lst");
            if (_sensors == null) this.SyncChildSensors();
            if (_sensors.Length == 0) return 0;

            if (_mustBeVisibleByAll && _sensors.Length > 1)
            {
                using (var set = com.spacepuppy.Collections.TempCollection.GetSet<T>())
                {
                    int resultCnt = 0;
                    _sensors[0].SenseAll<T>(set, p);
                    var e = set.GetEnumerator();
                    while (e.MoveNext())
                    {
                        int cnt = 1;
                        for (int i = 1; i < _sensors.Length; i++)
                        {
                            if (!_sensors[i].Visible(e.Current)) cnt++;
                        }
                        if (cnt == _sensors.Length)
                        {
                            resultCnt++;
                            lst.Add(e.Current);
                        }
                    }
                    return resultCnt;
                }
            }
            else
            {
                int cnt = 0;
                for (int i = 0; i < _sensors.Length; i++)
                {
                    cnt += _sensors[i].SenseAll<T>(lst, p);
                }
                return cnt;
            }
        }

        public override bool Visible(IAspect aspect)
        {
            if (_sensors == null) this.SyncChildSensors();
            if (_sensors.Length == 0) return false;

            if (_mustBeVisibleByAll && _sensors.Length > 1)
            {
                for (int i = 0; i < _sensors.Length; i++)
                {
                    if (!_sensors[i].Visible(aspect)) return false;
                }
                return true;
            }
            else
            {
                for (int i = 0; i < _sensors.Length; i++)
                {
                    if (_sensors[i].Visible(aspect)) return true;
                }
            }
                
            return false;
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<Sensor> GetEnumerator()
        {
            return (_sensors != null) ? (_sensors as IEnumerable<Sensor>).GetEnumerator() : System.Linq.Enumerable.Empty<Sensor>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

    }

}
