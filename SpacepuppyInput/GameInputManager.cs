using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;
using com.spacepuppy.UserInput;

namespace com.spacepuppy
{

    public class GameInputManager : Singleton, IEnumerable<IPlayerInputDevice>
    {

        #region Fields

        private Dictionary<string, IPlayerInputDevice> _dict = new Dictionary<string, IPlayerInputDevice>();
        private HashSet<ISequence> _sequences = new HashSet<ISequence>();
        private HashSet<ISequence> _fixedSequences = new HashSet<ISequence>();

        #endregion

        #region CONSTRUCTOR

        public GameInputManager()
        {
            
        }

        #endregion

        #region Properties

        public int Count { get { return _dict.Count; } }

        public IPlayerInputDevice this[string id]
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

        #endregion

        #region Messages

        private void FixedUpdate()
        {
            var e = _dict.GetEnumerator();
            while(e.MoveNext())
            {
                if (e.Current.Value.Active) e.Current.Value.FixedUpdate();
            }

            if (_fixedSequences.Count > 0)
            {
                using (var set = TempCollection.GetSet<ISequence>())
                {
                    var e2 = _fixedSequences.GetEnumerator();
                    while(e2.MoveNext())
                    {
                        if (e2.Current.Update()) set.Add(e2.Current);
                    }

                    if(set.Count > 0)
                    {
                        e2 = set.GetEnumerator();
                        while(e2.MoveNext())
                        {
                            _fixedSequences.Remove(e2.Current);
                        }
                    }
                }
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

            if (_sequences.Count > 0)
            {
                using (var set = TempCollection.GetSet<ISequence>())
                {
                    var e2 = _sequences.GetEnumerator();
                    while (e2.MoveNext())
                    {
                        if (e2.Current.Update()) set.Add(e2.Current);
                    }

                    if (set.Count > 0)
                    {
                        e2 = set.GetEnumerator();
                        while (e2.MoveNext())
                        {
                            _sequences.Remove(e2.Current);
                        }
                    }
                }
            }
        }

        #endregion

        #region Methods

        public void Add(string id, IPlayerInputDevice dev)
        {
            if (this.ContainsDevice(dev) && !(_dict.ContainsKey(id) && _dict[id] == dev)) throw new System.ArgumentException("Manager already contains input device for other player.");

            _dict[id] = dev;
        }

        public bool Remove(string id)
        {
            return _dict.Remove(id);
        }

        public bool ContainsId(string id)
        {
            return _dict.ContainsKey(id);
        }

        public bool ContainsDevice(IPlayerInputDevice dev)
        {
            return (_dict.Values as ICollection<IPlayerInputDevice>).Contains(dev);
        }

        public string GetIdForDevice(IPlayerInputDevice dev)
        {
            foreach (var pair in _dict)
            {
                if (pair.Value == dev) return pair.Key;
            }

            throw new System.ArgumentException("Unknown input device.");
        }

        public IPlayerInputDevice GetDevice(string id)
        {
            if (!_dict.ContainsKey(id)) throw new System.Collections.Generic.KeyNotFoundException();
            return _dict[id];
        }

        public T GetDevice<T>(string id) where T : IPlayerInputDevice
        {
            if (!_dict.ContainsKey(id)) throw new System.Collections.Generic.KeyNotFoundException();
            return (T)_dict[id];
        }





        public void RegisterSequence(ISequence sequence, bool useFixedUpdate = false)
        {
            bool b = (useFixedUpdate) ? _fixedSequences.Add(sequence) : _sequences.Add(sequence);
            if (b) sequence.OnStart();
        }

        #endregion

        #region IEnumerable Interface

        //TODO - implement propert Enumerator, remember dict.Values allocates mem in mono... ugh

        public IEnumerator<IPlayerInputDevice> GetEnumerator()
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
