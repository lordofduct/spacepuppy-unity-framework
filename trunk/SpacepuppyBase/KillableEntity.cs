using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    public abstract class KillableEntity : SPNotifyingComponent
    {

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            var proxy = this.entityRoot.AddOrGetComponent<KillableEntityProxy>();
            proxy.RegisterKillableEntityController(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            var proxy = this.entityRoot.GetComponent<KillableEntityProxy>();
            if(proxy != null)
            {
                proxy.UnRegisterKillableEntityController(this);
            }
        }

        #endregion


        #region Abstract Interface

        public abstract bool IsDead { get; }

        public abstract void Kill();

        #endregion

    }

    public class KillableEntityProxy : SPComponent
    {

        #region Fields

        private List<KillableEntity> _lst = new List<KillableEntity>();

        #endregion

        #region Properties

        public bool IsDead
        {
            get
            {
                for (int i = 0; i < _lst.Count; i++ )
                {
                    if (_lst[i].IsDead) return true;
                }
                return false;
            }
        }

        #endregion

        #region Methods

        internal void Kill()
        {
            //ToArray because Kill may end up calling 'UnRegisterKillableEntityController'
            foreach(var c in _lst.ToArray())
            {
                c.Kill();
            }
        }

        internal void RegisterKillableEntityController(KillableEntity controller)
        {
            if (_lst.Contains(controller)) return;

            _lst.Add(controller);
        }

        internal void UnRegisterKillableEntityController(KillableEntity controller)
        {
            _lst.Remove(controller);
        }

        #endregion

    }

}
