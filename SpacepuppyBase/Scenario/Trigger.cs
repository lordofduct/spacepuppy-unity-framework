using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    [System.Serializable()]
    public class Trigger : ICollection<TriggerTarget>
    {

        #region Fields

        [SerializeField()]
        private List<TriggerTarget> _targets = new List<TriggerTarget>();

        [System.NonSerialized()]
        private IObservableTrigger _owner;
        [System.NonSerialized()]
        private string _id;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        /// <summary>
        /// The INotificationDispatcher associated with this trigger that will post the notification of it being triggered.
        /// </summary>
        public IObservableTrigger ObservableTriggerOwner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public string ObservableTriggerId
        {
            get { return _id; }
            set { _id = value; }
        }

        #endregion

        #region Methods

        public TriggerTarget AddNew()
        {
            var targ = new TriggerTarget();
            _targets.Add(targ);
            return targ;
        }

        public void ActivateTrigger()
        {
            if (_targets.Count == 0) return;

            foreach (var targ in _targets)
            {
                if (targ != null)
                {
                    targ.Trigger();
                }
            }

            if(_owner != null)
            {
                _owner.PostNotification<TriggerActivatedNotification>(new TriggerActivatedNotification(_owner, _id), false);
            }
        }

        public void ActivateTrigger(object arg)
        {
            if (_targets.Count == 0) return;

            foreach (var targ in _targets)
            {
                if (targ != null)
                {
                    targ.Trigger(arg);
                }
            }

            if (_owner != null)
            {
                _owner.PostNotification<TriggerActivatedNotification>(new TriggerActivatedNotification(_owner, _id), false);
            }
        }
        
        public void ActivateRandomTrigger(bool considerWeights)
        {
            TriggerTarget trig = (considerWeights) ? _targets.PickRandom((t) => { return t.Weight; }) : _targets.PickRandom();
            trig.Trigger();

            if (_owner != null)
            {
                _owner.PostNotification<TriggerActivatedNotification>(new TriggerActivatedNotification(_owner, _id), false);
            }
        }

        public void ActivateRandomTrigger(object arg, bool considerWeights)
        {
            TriggerTarget trig = (considerWeights) ? _targets.PickRandom((t) => { return t.Weight; }) : _targets.PickRandom();
            trig.Trigger();

            if (_owner != null)
            {
                _owner.PostNotification<TriggerActivatedNotification>(new TriggerActivatedNotification(_owner, _id), false);
            }
        }

        #endregion

        #region ICollection Interface

        public void Add(TriggerTarget item)
        {
            _targets.Add(item);
        }

        public void Clear()
        {
            _targets.Clear();
        }

        public bool Contains(TriggerTarget item)
        {
            return _targets.Contains(item);
        }

        public void CopyTo(TriggerTarget[] array, int arrayIndex)
        {
            _targets.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _targets.Count; }
        }

        bool ICollection<TriggerTarget>.IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TriggerTarget item)
        {
            return _targets.Remove(item);
        }

        public Enumerator GetEnumerator()
        {
            //return _targets.GetEnumerator();
            return new Enumerator(this);
        }

        IEnumerator<TriggerTarget> IEnumerable<TriggerTarget>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion


        #region Special Types

        public class ConfigAttribute : System.Attribute
        {
            public bool Weighted;

            public ConfigAttribute(bool weighted)
            {
                this.Weighted = weighted;
            }
        }

        public struct Enumerator : IEnumerator<TriggerTarget>
        {

            private List<TriggerTarget>.Enumerator _e;

            public Enumerator(Trigger t)
            {
                _e = t._targets.GetEnumerator();
            }

            public TriggerTarget Current
            {
                get { return _e.Current; }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return _e.Current; }
            }

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            void System.Collections.IEnumerator.Reset()
            {
                (_e as IEnumerator<TriggerTarget>).Reset();
            }

            public void Dispose()
            {
                _e.Dispose();
            }

        }

        #endregion

    }

}
