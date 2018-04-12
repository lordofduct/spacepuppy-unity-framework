using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.Scenario
{

    public class t_OnTriggerOccupied : SPComponent, ICompoundTriggerEnterResponder, ICompoundTriggerExitResponder, IObservableTrigger
    {

        #region Fields

        [SerializeField]
        private Trigger _onTriggerOccupied;

        [SerializeField]
        private Trigger _onTriggerLastExited;

        [SerializeField]
        private bool _useEntity;

        [SerializeField]
        private ScenarioActivatorMask _mask = new ScenarioActivatorMask(-1);

        [SerializeField]
        private HashSet<GameObject> _activeObjects = new HashSet<GameObject>();

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

        public bool UseEntity
        {
            get { return _useEntity; }
            set { _useEntity = value; }
        }

        public ScenarioActivatorMask Mask
        {
            get { return _mask; }
        }

        #endregion

        #region Methods

        private void AddObject(GameObject obj)
        {
            if(_useEntity)
            {
                var entity = SPEntity.Pool.GetFromSource(obj);
                if (entity == null) return;

                obj = entity.gameObject;
            }

            if (_mask == null || !_mask.Intersects(obj)) return;

            if(_activeObjects.Count == 0)
            {
                _activeObjects.Add(obj);
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

            if (_useEntity)
            {
                var entity = SPEntity.Pool.GetFromSource(obj);
                if (entity == null) return;

                obj = entity.gameObject;
            }
            
            _activeObjects.Remove(obj);
            if(_activeObjects.Count == 0)
            {
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

        #region IObservableTrigger Interface

        Trigger[] IObservableTrigger.GetTriggers()
        {
            return new Trigger[] { _onTriggerOccupied, _onTriggerLastExited };
        }

        #endregion

    }

}
