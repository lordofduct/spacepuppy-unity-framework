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

        [SerializeField]
        [Tooltip("An optional eval string that will be operated as part of the mask, the GameObject of the activator will be passed along as $.")]
        private string _evalStatement;
        
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

        public string EvalStatement
        {
            get { return _evalStatement; }
            set { _evalStatement = value; }
        }

        #endregion

        #region Methods

        public bool Intersects(GameObject go)
        {
            if (go == null) return false;

            if (_testRoot) go = go.FindRoot();
            
            bool result = _layerMask.Intersects(go) && (_tags == null || _tags.Length == 0 || go.HasTag(_tags));
            if (result && !string.IsNullOrEmpty(_evalStatement)) result = com.spacepuppy.Dynamic.Evaluator.EvalBool(_evalStatement, go);
            return result;
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
