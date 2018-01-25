using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;
using com.spacepuppy.SPInput;

namespace com.spacepuppy
{

    public interface IGameInputManager : IService, IEnumerable<IInputDevice>
    {
        int Count { get; }
        IInputDevice this[string id] { get; set; }
        IInputDevice Main { get; }

        IInputDevice GetDevice(string id);
        T GetDevice<T>(string id) where T : IInputDevice;
        
    }
    
    public class GameInputManager : ServiceComponent<IGameInputManager>, IGameInputManager
    {

        #region Fields

        private Dictionary<string, IInputDevice> _dict = new Dictionary<string, IInputDevice>();
        private IInputDevice _default_main;
        private IInputDevice _override_main;

        #endregion

        #region CONSTRUCTOR

        public GameInputManager()
        {
            
        }

        #endregion
        
        #region Messages

        private void FixedUpdate()
        {
            var e = _dict.GetEnumerator();
            while(e.MoveNext())
            {
                if (e.Current.Value.Active) e.Current.Value.FixedUpdate();
            }
        }

        /// <summary>
        /// Call once per frame
        /// </summary>
        private void Update()
        {
            var e = _dict.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Value.Active) e.Current.Value.Update();
            }
        }

        #endregion

        #region IGameInputManager Interface

        public int Count { get { return _dict.Count; } }

        public IInputDevice this[string id]
        {
            get
            {
                return this.GetDevice(id);
            }
            set
            {
                this.Add(id, value);
            }
        }

        public IInputDevice Main
        {
            get
            {
                if (_override_main != null) return _override_main;
                if (_default_main == null)
                {
                    var e = _dict.GetEnumerator();
                    while (e.MoveNext())
                    {
                        _default_main = e.Current.Value;
                    }
                }
                return _default_main;
            }
            set
            {
                _override_main = value;
            }
        }

        public IInputDevice GetDevice(string id)
        {
            if (!_dict.ContainsKey(id)) throw new System.Collections.Generic.KeyNotFoundException();
            return _dict[id];
        }

        public T GetDevice<T>(string id) where T : IInputDevice
        {
            if (!_dict.ContainsKey(id)) throw new System.Collections.Generic.KeyNotFoundException();
            return (T)_dict[id];
        }

        #endregion

        #region Collection Interface

        public void Add(string id, IInputDevice dev)
        {
            if (this.Contains(dev) && !(_dict.ContainsKey(id) && _dict[id] == dev)) throw new System.ArgumentException("Manager already contains input device for other player.");

            if (_dict.Count == 0) _default_main = dev;
            _dict[id] = dev;
        }

        public bool Remove(string id)
        {
            if(_default_main != null)
            {
                IInputDevice device;
                if(_dict.TryGetValue(id, out device) && device == _default_main)
                {
                    if(_dict.Remove(id))
                    {
                        _default_main = null;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return _dict.Remove(id);
                }
            }
            else
            {
                return _dict.Remove(id);
            }
        }

        public bool Contains(string id)
        {
            return _dict.ContainsKey(id);
        }

        public bool Contains(IInputDevice dev)
        {
            return (_dict.Values as ICollection<IInputDevice>).Contains(dev);
        }

        public string GetId(IInputDevice dev)
        {
            foreach (var pair in _dict)
            {
                if (pair.Value == dev) return pair.Key;
            }

            throw new System.ArgumentException("Unknown input device.");
        }
        
        #endregion

        #region IEnumerable Interface

        //TODO - implement propert Enumerator, remember dict.Values allocates mem in mono... ugh

        public IEnumerator<IInputDevice> GetEnumerator()
        {
            return _dict.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dict.Values.GetEnumerator();
        }

        #endregion

    }

}
