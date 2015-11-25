using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Render
{
    public class RendererMaterialSwap : MonoBehaviour
    {

        #region Fields

        [System.NonSerialized()]
        private Renderer _renderer;

        [System.NonSerialized()]
        private Material _matCache;
        [System.NonSerialized()]
        private bool _flashing;

        [System.NonSerialized()]
        private RadicalCoroutine _flashRoutine;

        #endregion

        #region CONSTRUCTOR

        private void Awake()
        {
            _renderer = this.GetComponent<Renderer>();
        }

        #endregion

        #region Properties

        public bool Flashing { get { return _flashing; } }

        #endregion

        #region Methods

        public void StartSwap(Material mat, float duration)
        {
            if (duration <= 0f)
            {
                this.StopSwap();
                return;
            }
            if (!this.enabled) this.enabled = true;

            if(_flashRoutine != null)
            {
                _flashRoutine.Cancel();
                _flashRoutine = null;
            }

            if (_flashing)
            {
                //already started, just replace
                _renderer.material = mat;
                
                if (duration < float.PositiveInfinity)
                {
                    _flashRoutine = this.InvokeRadical(this.StopSwap, duration);
                }
            }
            else
            {
                //start a new
                _matCache = _renderer.material;
                _renderer.material = mat;
                _flashing = true;

                if (duration < float.PositiveInfinity)
                {
                    _flashRoutine = this.InvokeRadical(this.StopSwap, duration);
                }
            }
        }

        public void StopSwap()
        {
            if (_flashing)
            {
                _renderer.material = _matCache;
                _matCache = null;
                _flashing = false;
                this.enabled = false;
            }

            if (_flashRoutine != null)
            {
                _flashRoutine.Cancel();
                _flashRoutine = null;
            }
        }
        
        #endregion

        #region Static Factory

        public static RendererMaterialSwap Swap(Renderer renderer, Material material, float dur)
        {
            if (renderer == null) throw new System.ArgumentNullException("renderer");
            if (material == null) throw new System.ArgumentNullException("material");
            var swap = renderer.AddOrGetComponent<RendererMaterialSwap>();
            swap._renderer = renderer;
            swap.StartSwap(material, dur);
            return swap;
        }

        #endregion

    }
}
