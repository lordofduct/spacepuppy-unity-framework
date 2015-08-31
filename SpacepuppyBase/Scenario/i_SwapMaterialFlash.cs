using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Render;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_SwapMaterialFlash : TriggerableMechanism
    {

        #region Fields

        //[ModifierChain()]
        //[DefaultFromSelf()]
        [InsertButton("Search for Renderers", "FindRenderersInTarget")]
        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(GameObject))]
        private TriggerableTargetObject _target = new TriggerableTargetObject(TriggerableTargetObject.TargetSource.Root);

        [SerializeField()]
        [Tooltip("Leave empty to have it auto-sync on load.")]
        [UnityEngine.Serialization.FormerlySerializedAs("TargetRenderers")]
        private Renderer[] _targetRenderers;

        [SerializeField()]
        [Tooltip("The material to swap out with.")]
        [UnityEngine.Serialization.FormerlySerializedAs("Material")]
        private Material _material;
        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Duration")]
        private float _duration = 0.1f;

        #endregion

        #region CONSTRUCTOR

        protected override void OnStartOrEnable()
        {
            base.OnStartOrEnable();

            if(_target.Source != TriggerableTargetObject.TargetSource.TriggerArg &&
               (_targetRenderers == null || _targetRenderers.Length == 0))
            {
                this.FindRenderersInTarget();
            }
        }

        #endregion

        #region Properties

        public Material Material
        {
            get { return _material; }
            set { _material = value; }
        }

        public float Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        #endregion

        #region Methods

        public void FindRenderersInTarget()
        {
            if (_target == null || _target.Source == TriggerableTargetObject.TargetSource.TriggerArg)
            {
                _targetRenderers = ArrayUtil.Empty<Renderer>();
                return;
            }

            var src = _target.GetTarget<GameObject>(null);
            _targetRenderers = src.GetComponentsInChildren<Renderer>();
        }

        #endregion

        #region ITriggerableMechanism Interface

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            Renderer[] arr;
            if(_target.Source == TriggerableTargetObject.TargetSource.TriggerArg)
            {
                var targ = _target.GetTarget<GameObject>(arg);
                if (targ == null) return false;
                arr = targ.GetComponentsInChildren<Renderer>();
            }
            else
            {
                arr = _targetRenderers;
            }

            if (arr == null || arr.Length == 0) return false;

            for (int i = 0; i < arr.Length; i++)
            {
                if(arr[i] != null) RendererMaterialSwap.Swap(arr[i], _material, _duration);
            }
            return true;
        }

        #endregion

    }

}