using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.AI.Sensors
{

    public interface IAspect : IGameObjectSource
    {

        bool IsActive { get; }

        float Precedence { get; }

        SPEntity Entity { get; }

    }

    public abstract class AbstractAspect : SPComponent, IAspect
    {

        #region Fields

        [System.NonSerialized]
        private SPEntity _entity;
        [System.NonSerialized]
        private bool _synced;

        #endregion

        #region Methods

        protected virtual void OnTransformParentChanged()
        {
            _synced = false;
            _entity = null;
        }

        #endregion

        #region IAspect Interface

        public virtual bool IsActive
        {
            get { return this.isActiveAndEnabled; }
        }

        public abstract float Precedence
        {
            get;
            set;
        }

        public SPEntity Entity
        {
            get
            {
                if(!_synced)
                {
                    _synced = true;
                    _entity = SPEntity.Pool.GetFromSource(this);
                }
                return _entity;
            }
        }

        #endregion

    }

}
