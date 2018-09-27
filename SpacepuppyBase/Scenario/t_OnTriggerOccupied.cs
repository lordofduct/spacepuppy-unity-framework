#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class t_OnTriggerOccupied : SPComponent, ICompoundTriggerEnterResponder, ICompoundTriggerExitResponder, IOccupiedTrigger, IUpdateable
    {

        #region Fields

        [SerializeField]
        private EventActivatorMaskRef _mask = new EventActivatorMaskRef();

        [SerializeField]
        [Tooltip("If true, when an object is intersected we'll habitually retest the object in case its state changes for any reason.")]
        private bool _activelyRetestObjects;

        [SerializeField]
        private Trigger _onTriggerOccupied;

        [SerializeField]
        private Trigger _onTriggerLastExited;
        
        [System.NonSerialized]
        private HashSet<GameObject> _activeObjects = new HashSet<GameObject>();

        #endregion

        #region CONSTRUCTOR

        protected override void OnDisable()
        {
            base.OnDisable();

            GameLoopEntry.UpdatePump.Remove(this);
        }

        #endregion

        #region Properties

        public Trigger OnTriggerOccupied
        {
            get { return _onTriggerOccupied; }
        }

        public Trigger OnTriggerLastExited
        {
            get { return _onTriggerLastExited; }
        }
        
        public IEventActivatorMask Mask
        {
            get { return _mask.Value; }
            set { _mask.Value = value; }
        }

        public bool IsOccupied
        {
            get { return _activeObjects.Count > 0; }
        }

        #endregion

        #region Methods

        private void AddObject(GameObject obj)
        {
            if (_mask.Value != null && !_mask.Value.Intersects(obj)) return;

            if(_activeObjects.Count == 0)
            {
                _activeObjects.Add(obj);
                if (_activelyRetestObjects) GameLoopEntry.UpdatePump.Add(this);
                _onTriggerOccupied.ActivateTrigger(this, obj);
            }
            else
            {
                _activeObjects.Add(obj);
            }
        }

        private void RemoveObject(GameObject obj)
        {
            if (_activeObjects.Count == 0) return;
            
            _activeObjects.Remove(obj);
            if(_activeObjects.Count == 0)
            {
                GameLoopEntry.UpdatePump.Remove(this);
                _onTriggerLastExited.ActivateTrigger(this, obj);
            }
        }

        #endregion

        #region Messages

        void OnTriggerEnter(Collider other)
        {
            if (this.HasComponent<CompoundTrigger>() || other == null) return;

            this.AddObject(other.gameObject);
        }

        void OnTriggerExit(Collider other)
        {
            if (this.HasComponent<CompoundTrigger>() || other == null) return;

            this.RemoveObject(other.gameObject);
        }

        void ICompoundTriggerEnterResponder.OnCompoundTriggerEnter(Collider other)
        {
            if (other == null) return;
            this.AddObject(other.gameObject);
        }

        void ICompoundTriggerExitResponder.OnCompoundTriggerExit(Collider other)
        {
            if (other == null) return;
            this.RemoveObject(other.gameObject);
        }

        #endregion

        #region IUpdateable Interface

        void IUpdateable.Update()
        {
            if(!_activelyRetestObjects || !this.isActiveAndEnabled || _activeObjects.Count == 0)
            {
                GameLoopEntry.UpdatePump.Remove(this);
                return;
            }

            if (_mask.Value == null) return;

            using (var toRemove = TempCollection.GetSet<GameObject>())
            {
                var e = _activeObjects.GetEnumerator();
                while (e.MoveNext())
                {
                    if(!ObjUtil.IsObjectAlive(e.Current) || !e.Current.activeInHierarchy || !_mask.Value.Intersects(e.Current))
                    {
                        toRemove.Add(e.Current);
                    }
                }

                if(toRemove.Count > 0)
                {
                    e = toRemove.GetEnumerator();
                    while(e.MoveNext())
                    {
                        this.RemoveObject(e.Current);
                    }

                    if(_activeObjects.Count == 0)
                    {
                        GameLoopEntry.UpdatePump.Remove(this);
                        return;
                    }
                }
            }
        }

        #endregion

        #region IOccupiedObservableTrigger Interface

        Trigger[] IObservableTrigger.GetTriggers()
        {
            return new Trigger[] { _onTriggerOccupied, _onTriggerLastExited };
        }

        Trigger IOccupiedTrigger.EnterTrigger
        {
            get { return _onTriggerOccupied; }
        }

        Trigger IOccupiedTrigger.ExitTrigger
        {
            get { return _onTriggerLastExited; }
        }
        
        #endregion

    }

}
