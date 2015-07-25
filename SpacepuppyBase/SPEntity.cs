using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    public class SPEntity : SPNotifyingComponent
    {
        
        #region Static Multiton

        private static List<SPEntity> _entities;
        private static System.Collections.ObjectModel.ReadOnlyCollection<SPEntity> _readonlyEntities;

        /// <summary>
        /// A readonly list of all active entities.
        /// </summary>
        public static IList<SPEntity> ActiveEntities { get { return _readonlyEntities; } }

        static SPEntity()
        {
            _entities = new List<SPEntity>();
            _readonlyEntities = new System.Collections.ObjectModel.ReadOnlyCollection<SPEntity>(_entities);
        }

        #endregion

        #region Fields

        [System.NonSerialized()]
        private Transform _trans;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnStartOrEnable()
        {
            base.OnStartOrEnable();

            if (!_entities.Contains(this)) _entities.Add(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _entities.Remove(this);
        }

        #endregion

        #region Properties

        public new Transform transform { get { return _trans ?? base.transform; } }

        #endregion

    }

}
