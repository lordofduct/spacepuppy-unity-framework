using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.AI.Sensors
{
    public class SensorCollection : ICollection<Sensor>
    {

        #region Fields

        private List<Sensor> _lst = new List<Sensor>();

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        public Sensor this[string name]
        {
            get
            {
                return this.GetSensor(name);
            }
        }

        #endregion

        #region Methods

        public Sensor GetSensor(string name)
        {
            return (from s in _lst where s.name == name select s).FirstOrDefault();
        }

        public IAspect Sense()
        {
            IAspect a;
            for(int i = 0; i < _lst.Count; i++)
            {
                a = _lst[i].Sense();
                if (a != null) return a;
            }
            return null;
        }

        public IAspect Sense(string sensorName)
        {
            Sensor s;
            IAspect a;
            for (int i = 0; i < _lst.Count; i++ )
            {
                s = _lst[i];
                if (s.name != sensorName) continue;

                a = s.Sense();
                if (a != null) return a;
            }
            return null;
        }

        public IAspect Sense(params string[] sensorNames)
        {
            Sensor s;
            IAspect a;
            for (int i = 0; i < _lst.Count; i++)
            {
                s = _lst[i];
                if (System.Array.IndexOf(sensorNames, s.name) < 0) continue;

                a = s.Sense();
                if (a != null) return a;
            }
            return null;
        }

        /// <summary>
        /// Gets a read-only list of all the aspects that can currently be seen by all the sensors.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IAspect> SenseAll()
        {
            return (from s in _lst from a in s.SenseAll() select a).Distinct();
        }

        public IEnumerable<IAspect> SenseAll(string sensorName)
        {
            return (from s in _lst where s.name == sensorName from a in s.SenseAll() select a).Distinct();
        }

        public IEnumerable<IAspect> SenseAll(params string[] sensorNames)
        {
            return (from s in _lst where sensorNames.Contains(s.name) from a in s.SenseAll() select a).Distinct();
        }

        #endregion

        #region Event Handlers

        private void _sensor_Destroyed(object sender, System.EventArgs e)
        {
            var s = sender as Sensor;
            if (Object.ReferenceEquals(s, null)) return;

            s.ComponentDestroyed -= _sensor_Destroyed;
            _lst.Remove(s);
        }

        #endregion

        #region ICollection Interface

        public int Count
        {
            get { return _lst.Count; }
        }

        public void Add(Sensor item)
        {
            if (_lst.Contains(item)) return;

            item.ComponentDestroyed += _sensor_Destroyed;
            _lst.Add(item);
        }

        public void Clear()
        {
            foreach (var s in _lst)
            {
                s.ComponentDestroyed -= _sensor_Destroyed;
            }
            _lst.Clear();
        }

        public bool Contains(Sensor item)
        {
            return _lst.Contains(item);
        }

        public void CopyTo(Sensor[] array, int arrayIndex)
        {
            _lst.CopyTo(array, arrayIndex);
        }

        bool ICollection<Sensor>.IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Sensor item)
        {
            if (_lst.Remove(item))
            {
                item.ComponentDestroyed -= _sensor_Destroyed;
                return true;
            }
            else
            {
                return false;
            }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return _lst.GetEnumerator();
        }

        IEnumerator<Sensor> IEnumerable<Sensor>.GetEnumerator()
        {
            return _lst.GetEnumerator();
        }

        #endregion


    }
}
