using UnityEngine;
using com.spacepuppy.Render;
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
        private TriggerableTargetObject _target = new TriggerableTargetObject();

        [SerializeField()]
        private bool _searchEntireEntity = true;

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

            if(!_target.TargetsTriggerArg &&
               (_targetRenderers == null || _targetRenderers.Length == 0))
            {
                this.FindRenderersInTarget();
            }
        }

        #endregion

        #region Properties

        public TriggerableTargetObject Target
        {
            get { return _target; }
        }

        public bool SearchEntireEntity
        {
            get { return _searchEntireEntity; }
            set { _searchEntireEntity = value; }
        }

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

        /// <summary>
        /// If 'Target' is modified, this must be called to resync the cahced renderers.
        /// </summary>
        public void FindRenderersInTarget()
        {
            if (_target == null || !_target.TargetsTriggerArg)
            {
                _targetRenderers = ArrayUtil.Empty<Renderer>();
                return;
            }

            var src = _target.GetTarget<GameObject>(null);
            if (src == null) return;
            if (_searchEntireEntity) src = GameObjectUtil.FindRoot(src);

            _targetRenderers = src.GetComponentsInChildren<Renderer>();
        }

        #endregion

        #region ITriggerableMechanism Interface

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            Renderer[] arr;
            if(_target.TargetsTriggerArg)
            {
                var targ = _target.GetTarget<GameObject>(arg);
                if (targ == null) return false;
                if (_searchEntireEntity) targ = GameObjectUtil.FindRoot(targ);
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