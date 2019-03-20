using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

using System;

namespace com.spacepuppy.Scenario
{

    [System.Serializable()]
    public abstract class BaseSPEvent : ICollection<TriggerTarget>
    {

        public const string ID_DEFAULT = "Trigger";

        #region Events

        protected System.EventHandler<TempEventArgs> _triggerActivated;
        public event System.EventHandler<TempEventArgs> TriggerActivated
        {
            add
            {
                _triggerActivated += value;
            }
            remove
            {
                _triggerActivated -= value;
            }
        }
        protected virtual void OnTriggerActivated(object sender, object arg)
        {
            if (_triggerActivated != null)
            {
                var e = TempEventArgs.Create(arg);
                var d = _triggerActivated;
                d(sender, e);
                TempEventArgs.Release(e);
            }

            //if(_owner != null)
            //{
            //    _owner.PostNotification<TriggerActivatedNotification>(new TriggerActivatedNotification(_owner, _id), false);
            //}
        }
        
        #endregion

        #region Fields

        [SerializeField()]
        private bool _yield;

        [SerializeField()]
        private List<TriggerTarget> _targets = new List<TriggerTarget>();
        
        [System.NonSerialized()]
        private string _id;


        [System.NonSerialized]
        private HashSet<object> _hijackTokens;

        #endregion

        #region CONSTRUCTOR

        public BaseSPEvent()
        {
            _id = ID_DEFAULT;
        }

        public BaseSPEvent(string id)
        {
            _id = id;
        }
        
        public BaseSPEvent(bool yielding)
        {
            _id = ID_DEFAULT;
            _yield = yielding;
        }

        public BaseSPEvent(string id, bool yielding)
        {
            _id = id;
            _yield = yielding;
        }

        #endregion

        #region Properties

        public bool Yielding
        {
            get { return _yield; }
            set { _yield = value; }
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

        /// <summary>
        /// Count is total count of targets including the TriggerActivated event. 
        /// Check the count of 'Targets' for the direct targets only.
        /// </summary>
        public virtual int Count
        {
            get
            {
                if (_triggerActivated != null)
                    return _targets.Count + 1;
                else
                    return _targets.Count;
            }
        }

        public bool CurrentlyHijacked
        {
            get { return _hijackTokens != null && _hijackTokens.Count > 0; }
        }

        #endregion

        #region Methods
        
        public TriggerTarget AddNew()
        {
            var targ = new TriggerTarget();
            _targets.Add(targ);
            return targ;
        }

        /// <summary>
        /// Begins a hijack, when a trigger is hijacked none of its targets are triggered, but its TriggerActivated event still fires. 
        /// If tokens are passed in it allows compounded hijacking so that just because one caller ends the hijack, another can still continue hijacking.
        /// </summary>
        /// <param name="token"></param>
        public void BeginHijack(object token = null)
        {
            if (token == null) token = "*DEFAULT*";

            if (_hijackTokens == null) _hijackTokens = new HashSet<object>();
            _hijackTokens.Add(token);
        }
        
        /// <summary>
        /// Attempts to stop a hijack, but if more than one token has been used to hijack the event it may continue hijacking.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Returns true if the trigger is no longer hijacked after calling this</returns>
        public bool EndHijack(object token = null)
        {
            if (_hijackTokens == null) return true;
            
            _hijackTokens.Remove(token ?? "*DEFAULT*");
            return _hijackTokens.Count == 0;
        }

        /// <summary>
        /// Forces the end of a hijack.
        /// </summary>
        public void ForceEndHijack()
        {
            if (_hijackTokens != null) _hijackTokens.Clear();
        }



        protected void ActivateTrigger(object sender, object arg)
        {
            if (_targets.Count > 0 && !this.CurrentlyHijacked)
            {
                var e = _targets.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current != null) e.Current.Trigger(sender, arg);
                }
            }

            this.OnTriggerActivated(sender, arg);
        }

        protected void ActivateTriggerAt(int index, object sender, object arg)
        {
            if (index >= 0 && index < _targets.Count && !this.CurrentlyHijacked)
            {
                TriggerTarget trig = _targets[index];
                if(trig != null) trig.Trigger(sender, arg);
            }

            this.OnTriggerActivated(sender, arg);
        }

        protected void ActivateRandomTrigger(object sender, object arg, bool considerWeights, bool selectOnlyIfActive)
        {
            if (_targets.Count > 0 && !this.CurrentlyHijacked)
            {
                TriggerTarget trig;
                if (selectOnlyIfActive)
                {
                    using (var lst = TempCollection.GetList<TriggerTarget>())
                    {
                        for(int i = 0; i < _targets.Count; i++)
                        {
                            var go = GameObjectUtil.GetGameObjectFromSource(_targets[i].CalculateTarget(arg));
                            if (object.ReferenceEquals(go, null) || go.IsAliveAndActive()) lst.Add(_targets[i]);
                        }
                        trig = (considerWeights) ? lst.PickRandom((t) => { return t.Weight; }) : lst.PickRandom();
                    }
                }
                else
                {
                    trig = (considerWeights) ? _targets.PickRandom((t) => { return t.Weight; }) : _targets.PickRandom();
                }
                if (trig != null) trig.Trigger(sender, arg);
            }

            this.OnTriggerActivated(sender, arg);
        }

        protected IRadicalYieldInstruction ActivateTriggerYielding(object sender, object arg)
        {
            if (_yield && _targets.Count > 0 && !this.CurrentlyHijacked)
            {
                var instruction = BlockingTriggerYieldInstruction.Create();

                var e = _targets.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current != null) e.Current.Trigger(sender, arg);
                }

                this.OnTriggerActivated(sender, arg);

                return (instruction.Count > 0) ? instruction : null;
            }
            else
            {
                if (_targets.Count > 0 && !this.CurrentlyHijacked)
                {
                    var e = _targets.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (e.Current != null) e.Current.Trigger(sender, arg);
                    }
                }

                this.OnTriggerActivated(sender, arg);

                return null;
            }
        }


        protected void DaisyChainTriggerYielding(object sender, object arg, BlockingTriggerYieldInstruction instruction)
        {
            if (_targets.Count > 0 && !this.CurrentlyHijacked)
            {
                var e = _targets.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current != null) e.Current.Trigger(sender, arg);
                }

                this.OnTriggerActivated(sender, arg);
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

        int ICollection<TriggerTarget>.Count
        {
            get
            {
                return _targets.Count;
            }
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

        public struct Enumerator : IEnumerator<TriggerTarget>
        {

            private List<TriggerTarget>.Enumerator _e;

            public Enumerator(BaseSPEvent t)
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


    [System.Serializable()]
    public class SPEvent : BaseSPEvent
    {

        #region CONSTRUCTOR

        public SPEvent()
        {
        }

        public SPEvent(string id) : base(id)
        {
        }

        public SPEvent(bool yielding) : base(yielding)
        {
        }

        public SPEvent(string id, bool yielding) : base(id, yielding)
        {
        }

        #endregion

        #region Methods

        public new void ActivateTrigger(object sender, object arg)
        {
            base.ActivateTrigger(sender, arg);
        }

        public new void ActivateTriggerAt(int index, object sender, object arg)
        {
            base.ActivateTriggerAt(index, sender, arg);
        }

        public new void ActivateRandomTrigger(object sender, object arg, bool considerWeights, bool selectOnlyIfActive)
        {
            base.ActivateRandomTrigger(sender, arg, considerWeights, selectOnlyIfActive);
        }

        public new IRadicalYieldInstruction ActivateTriggerYielding(object sender, object arg)
        {
            return base.ActivateTriggerYielding(sender, arg);
        }

        public new void DaisyChainTriggerYielding(object sender, object arg, BlockingTriggerYieldInstruction instruction)
        {
            base.DaisyChainTriggerYielding(sender, arg, instruction);
        }

        #endregion

        #region Special Types

        /*
         * This may be defined here, it is still usable on all types inheriting from BaseSPEvent. It's only here for namespace purposes to be consistent across the framework.
         */
        public class ConfigAttribute : System.Attribute
        {
            public bool Weighted;
            public bool AlwaysExpanded;

            public ConfigAttribute()
            {

            }

        }

        #endregion

    }


    [System.Serializable()]
    public class SPEvent<T> : BaseSPEvent where T : System.EventArgs
    {

        #region Events

        public new event System.EventHandler<T> TriggerActivated;
        protected virtual void OnTriggerActivated(object sender, T e)
        {
            this.TriggerActivated?.Invoke(sender, e);
        }

        #endregion

        #region CONSTRUCTOR

        public SPEvent()
        {
        }

        public SPEvent(string id) : base(id)
        {
        }

        public SPEvent(bool yielding) : base(yielding)
        {
        }

        public SPEvent(string id, bool yielding) : base(id, yielding)
        {
        }

        #endregion

        #region Methods

        public override int Count
        {
            get
            {
                if (this.TriggerActivated != null || _triggerActivated != null)
                    return this.Targets.Count + 1;
                else
                    return this.Targets.Count;
            }
        }

        public void ActivateTrigger(object sender, T arg)
        {
            base.ActivateTrigger(sender, arg);
            this.OnTriggerActivated(sender, arg);
        }

        public void ActivateTriggerAt(int index, object sender, T arg)
        {
            base.ActivateTriggerAt(index, sender, arg);
            this.OnTriggerActivated(sender, arg);
        }

        public void ActivateRandomTrigger(object sender, T arg, bool considerWeights, bool selectOnlyIfActive)
        {
            base.ActivateRandomTrigger(sender, arg, considerWeights, selectOnlyIfActive);
            this.OnTriggerActivated(sender, arg);
        }

        public IRadicalYieldInstruction ActivateTriggerYielding(object sender, T arg)
        {
            return base.ActivateTriggerYielding(sender, arg);
            //this.OnTriggerActivated(sender, arg);
        }

        public void DaisyChainTriggerYielding(object sender, T arg, BlockingTriggerYieldInstruction instruction)
        {
            base.DaisyChainTriggerYielding(sender, arg, instruction);
            this.OnTriggerActivated(sender, arg);
        }

        #endregion

    }

    [System.Serializable()]
    public class SPActionEvent<T> : BaseSPEvent
    {

        #region Events

        private System.Action<T> _callback;
        private System.Action<object, T> _evCallback;
        protected virtual void OnTriggerActivated(object sender, T arg)
        {
            if (_callback != null)
            {
                var c = _callback;
                c(arg);
            }

            if (_evCallback != null)
            {
                var c = _evCallback;
                c(sender, arg);
            }
        }

        #endregion

        #region CONSTRUCTOR

        public SPActionEvent()
        {
        }

        public SPActionEvent(string id) : base(id)
        {
        }

        public SPActionEvent(bool yielding) : base(yielding)
        {
        }

        public SPActionEvent(string id, bool yielding) : base(id, yielding)
        {
        }

        #endregion

        #region Methods

        public override int Count
        {
            get
            {
                if (_callback != null || _evCallback != null || _triggerActivated != null)
                    return this.Targets.Count + 1;
                else
                    return this.Targets.Count;
            }
        }

        public void AddListener(System.Action<T> callback)
        {
            _callback += callback;
        }

        public void AddListener(System.Action<object, T> callback)
        {
            _evCallback += callback;
        }

        public void RemoveListener(System.Action<T> callback)
        {
            _callback -= callback;
        }

        public void RemoveListener(System.Action<object, T> callback)
        {
            _evCallback -= callback;
        }

        public void ActivateTrigger(object sender, T arg)
        {
            base.ActivateTrigger(sender, arg);
            this.OnTriggerActivated(sender, arg);
        }

        public void ActivateTriggerAt(int index, object sender, T arg)
        {
            base.ActivateTriggerAt(index, sender, arg);
            this.OnTriggerActivated(sender, arg);
        }

        public void ActivateRandomTrigger(object sender, T arg, bool considerWeights, bool selectOnlyIfActive)
        {
            base.ActivateRandomTrigger(sender, arg, considerWeights, selectOnlyIfActive);
            this.OnTriggerActivated(sender, arg);
        }

        public IRadicalYieldInstruction ActivateTriggerYielding(object sender, T arg)
        {
            return base.ActivateTriggerYielding(sender, arg);
            //this.OnTriggerActivated(sender, arg);
        }

        public void DaisyChainTriggerYielding(object sender, T arg, BlockingTriggerYieldInstruction instruction)
        {
            base.DaisyChainTriggerYielding(sender, arg, instruction);
            this.OnTriggerActivated(sender, arg);
        }

        #endregion

    }


    /// <summary>
    /// Exists to maintain compatibility with the 'Trigger' name. New events should use SPEvent directly.
    /// </summary>
    [System.Serializable()]
    public class Trigger : SPEvent
    {

        #region CONSTRUCTOR

        public Trigger()
        {
        }

        public Trigger(string id) : base(id)
        {
        }

        public Trigger(bool yielding) : base(yielding)
        {
        }

        public Trigger(string id, bool yielding) : base(id, yielding)
        {
        }

        #endregion

        //#region Methods

        //public new void ActivateTrigger(object sender, object arg)
        //{
        //    base.ActivateTrigger(sender, arg);
        //}

        //public new void ActivateTriggerAt(int index, object sender, object arg)
        //{
        //    base.ActivateTriggerAt(index, sender, arg);
        //}

        //public new void ActivateRandomTrigger(object sender, object arg, bool considerWeights)
        //{
        //    base.ActivateRandomTrigger(sender, arg, considerWeights);
        //}

        //public new IRadicalYieldInstruction ActivateTriggerYielding(object sender, object arg)
        //{
        //    return base.ActivateTriggerYielding(sender, arg);
        //}

        //public new void DaisyChainTriggerYielding(object sender, object arg, BlockingTriggerYieldInstruction instruction)
        //{
        //    base.DaisyChainTriggerYielding(sender, arg, instruction);
        //}

        //#endregion

        #region Special Types

        /*
         * Moved to SPEvent
         */
        //public class ConfigAttribute : System.Attribute
        //{
        //    public bool Weighted;
        //    public bool AlwaysExpanded;

        //    public ConfigAttribute()
        //    {

        //    }

        //}

        #endregion

    }


    

    public class BlockingTriggerYieldInstruction : RadicalYieldInstruction, IPooledYieldInstruction
    {

        #region Fields

        private int _count;

        #endregion

        #region CONSTRUCTOR

        private BlockingTriggerYieldInstruction()
        {

        }

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

        #region IDisposable Interface

        void IDisposable.Dispose()
        {
            _count = 0;
            _pool.Release(this);
        }

        #endregion

        #region Static Factory

        private static ObjectCachePool<BlockingTriggerYieldInstruction> _pool = new ObjectCachePool<BlockingTriggerYieldInstruction>(-1, () => new BlockingTriggerYieldInstruction());

        public static BlockingTriggerYieldInstruction Create()
        {
            var obj = _pool.GetInstance();
            obj._count = 0;
            return obj;
        }

        #endregion

    }

}
