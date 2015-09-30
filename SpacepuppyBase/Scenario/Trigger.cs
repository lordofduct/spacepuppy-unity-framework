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
        private bool _yield;

        [SerializeField()]
        private List<TriggerTarget> _targets = new List<TriggerTarget>();

        [System.NonSerialized()]
        private IObservableTrigger _owner;
        [System.NonSerialized()]
        private string _id;
        
        #endregion

        #region CONSTRUCTOR

        public Trigger()
        {

        }

        public Trigger(bool yielding)
        {
            _yield = yielding;
        }

        #endregion

        #region Properties

        public bool Yielding
        {
            get { return _yield; }
            set { _yield = value; }
        }

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

        public List<TriggerTarget> Targets
        {
            get { return _targets; }
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
            
            var e = _targets.GetEnumerator();
            while(e.MoveNext())
            {
                if (e.Current != null) e.Current.Trigger();
            }

            //if(_owner != null)
            //{
            //    _owner.PostNotification<TriggerActivatedNotification>(new TriggerActivatedNotification(_owner, _id), false);
            //}
        }

        public void ActivateTrigger(object arg)
        {
            if (_targets.Count == 0) return;
            
            var e = _targets.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current != null) e.Current.Trigger(arg);
            }
            
            //if (_owner != null)
            //{
            //    _owner.PostNotification<TriggerActivatedNotification>(new TriggerActivatedNotification(_owner, _id), false);
            //}
        }
        
        public void ActivateRandomTrigger(bool considerWeights)
        {
            TriggerTarget trig = (considerWeights) ? _targets.PickRandom((t) => { return t.Weight; }) : _targets.PickRandom();
            trig.Trigger();

            //if (_owner != null)
            //{
            //    _owner.PostNotification<TriggerActivatedNotification>(new TriggerActivatedNotification(_owner, _id), false);
            //}
        }

        public void ActivateRandomTrigger(object arg, bool considerWeights)
        {
            TriggerTarget trig = (considerWeights) ? _targets.PickRandom((t) => { return t.Weight; }) : _targets.PickRandom();
            trig.Trigger();

            //if (_owner != null)
            //{
            //    _owner.PostNotification<TriggerActivatedNotification>(new TriggerActivatedNotification(_owner, _id), false);
            //}
        }

        public IRadicalYieldInstruction ActivateTriggerYielding()
        {
            if (this.Targets.Count == 0) return null;
            if (!_yield)
            {
                this.ActivateTrigger();
                return null;
            }

            var instruction = new BlockingTriggerYieldInstruction();

            var e = this.Targets.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current != null) e.Current.Trigger();
            }

            return (instruction.Count > 0) ? instruction : null;
        }

        public IRadicalYieldInstruction ActivateTriggerYielding(object arg)
        {
            if (this.Targets.Count == 0) return null;
            if (!_yield)
            {
                this.ActivateTrigger(arg);
                return null;
            }

            var instruction = new BlockingTriggerYieldInstruction();

            var e = this.Targets.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current != null) e.Current.Trigger(arg);
            }

            return (instruction.Count > 0) ? instruction : null;
        }


        public void DaisyChainTriggerYielding(object arg, BlockingTriggerYieldInstruction instruction)
        {
            if (this.Targets.Count == 0) return;

            var e = this.Targets.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current != null) e.Current.Trigger(arg);
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
            public bool AlwaysExpanded;

            public ConfigAttribute()
            {

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
    
    public class BlockingTriggerYieldInstruction : RadicalYieldInstruction
    {

        #region Fields

        private int _count;

        #endregion

        #region Properties

        public int Count
        {
            get { return _count; }
        }

        #endregion

        #region Methods

        public void BeginBlock()
        {
            _count++;
        }

        public void EndBlock()
        {
            if (this.IsComplete) return;

            _count--;
            if(_count <= 0)
            {
                this.SetSignal();
            }
        }

        #endregion

    }

}
