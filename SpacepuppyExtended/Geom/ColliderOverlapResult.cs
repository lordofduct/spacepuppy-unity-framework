using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Geom
{

    public class ColliderOverlapResult : RadicalYieldInstruction
    {

        #region Fields

        private List<Collider> _colliders = new List<Collider>();

        #endregion

        #region CONSTRUCTOR

        public ColliderOverlapResult()
        {

        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public Collider[] GetColliders()
        {
            for (int i = 0; i < _colliders.Count; i++)
            {
                if(_colliders[i] == null)
                {
                    _colliders.RemoveAt(i);
                    i--;
                }
            }
            return _colliders.ToArray();
        }

        internal void Internal_AddCollider(Collider c)
        {
            if (_colliders.Contains(c)) return;

            _colliders.Add(c);
        }

        internal void Internal_RemoveCollider(Collider c)
        {
            _colliders.Remove(c);
        }

        internal void Internal_Set()
        {
            this.SetSignal();
        }

        #endregion

    }

}
