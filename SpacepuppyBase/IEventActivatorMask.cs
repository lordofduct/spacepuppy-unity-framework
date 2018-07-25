#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using System.Collections.Generic;
using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    public interface IEventActivatorMask
    {
        
        bool Intersects(UnityEngine.Object obj);

    }

    [System.Serializable]
    public sealed class EventActivatorMaskRef : Project.SerializableInterfaceRef<IEventActivatorMask>
    {

    }


    [CreateAssetMenu(fileName = "EventActivatorMask", menuName = "Spacepuppy/EventActivatorMask")]
    public class EventActivatorMask : ScriptableObject, IEventActivatorMask
    {

        #region Fields

        [SerializeField]
        private bool _testRoot;
        [SerializeField]
        private LayerMask _layerMask = -1;

        [SerializeField()]
        [ReorderableArray]
        [TagSelector()]
        private string[] _tags;

        #endregion

        #region Properties

        public LayerMask LayerMask
        {
            get { return _layerMask; }
            set { _layerMask = value; }
        }

        public string[] Tags
        {
            get { return _tags; }
            set { _tags = value; }
        }

        #endregion

        #region Methods

        public bool Intersects(GameObject go)
        {
            if (go == null) return false;

            if (_testRoot) go = go.FindRoot();

            return go.IntersectsLayerMask(_layerMask) && (_tags == null || _tags.Length == 0 || go.HasTag(_tags));
        }

        public bool Intersects(Component comp)
        {
            if (comp == null) return false;
            return Intersects(comp.gameObject);
        }

        #endregion

        #region IEventActivatorMask Interface

        public bool Intersects(Object obj)
        {
            return this.Intersects(GameObjectUtil.GetGameObjectFromSource(obj));
        }

        #endregion

    }

}
