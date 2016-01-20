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
        
        public Material ActiveMaterial { get { return _renderer.sharedMaterial; } }

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
                _renderer.sharedMaterial = mat;
                
                if (duration < float.PositiveInfinity)
                {
                    _flashRoutine = this.InvokeRadical(this.StopSwap, duration);
                }
            }
            else
            {
                //start a new
                _matCache = _renderer.sharedMaterial;
                _renderer.sharedMaterial = mat;
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
                _renderer.sharedMaterial = _matCache;
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

        public static RendererMaterialSwap[] SwapAll(GameObject root, Material material, float dur, bool bIncludeInactiveRenderers = false)
        {
            if (root == null) throw new System.ArgumentNullException("root");
            if (material == null) throw new System.ArgumentNullException("material");

            using (var results = com.spacepuppy.Collections.TempCollection.GetList<RendererMaterialSwap>())
            {
                using (var rendererList = com.spacepuppy.Collections.TempCollection.GetList<Renderer>())
                {
                    root.GetComponentsInChildren<Renderer>(bIncludeInactiveRenderers, rendererList);
                    var e = rendererList.GetEnumerator();
                    while(e.MoveNext())
                    {
                        var swap = e.Current.AddOrGetComponent<RendererMaterialSwap>();
                        swap._renderer = e.Current;
                        swap.StartSwap(material, dur);
                        results.Add(swap);
                    }
                }
                return results.ToArray();
            }
        }

        #endregion

    }
}
