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

        bool OmniPresent { get; }

    }

    public abstract class AbstractAspect : SPEntityComponent, IAspect
    {
        
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

        public abstract bool OmniPresent
        {
            get;
        }
        
        #endregion

    }

}
