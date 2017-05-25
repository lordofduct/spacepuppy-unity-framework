#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;
using System;

namespace com.spacepuppy.Render
{

    public interface IMaterialSource
    {

        Material Material { get; set; }
        
        bool IsUnique { get; set; }

        Material GetUniqueMaterial();

    }

    public abstract class MaterialSource : SPComponent, IMaterialSource
    {
        public abstract bool IsUnique { get; set; }
        public abstract Material Material { get; set; }

        public abstract Material GetUniqueMaterial();


        #region Static Interface

        public static MaterialSource GetMaterialSource(object src)
        {
            if(src is Renderer)
            {
                return RendererMaterialSource.GetMaterialSource(src as Renderer);
            }
            else if(src is UnityEngine.UI.Graphic)
            {
                return GraphicMaterialSource.GetMaterialSource(src as UnityEngine.UI.Graphic);
            }

            var go = com.spacepuppy.Utils.GameObjectUtil.GetGameObjectFromSource(src);
            var rend = go.GetComponent<Renderer>();
            if (rend != null)
                return RendererMaterialSource.GetMaterialSource(rend);

            var graph = go.GetComponent<UnityEngine.UI.Graphic>();
            if(graph != null)
                return GraphicMaterialSource.GetMaterialSource(graph);

            return null;
        }
        
        #endregion
    }

    /// <summary>
    /// A self-tracking Material source accessor. This allows you to track if a Material is using a shared material, or you had made it unique. 
    /// The default interface for accessing materials on a Renderer in Unity is rather confusing and it's hard to tell if it's shared or not. 
    /// This also leads to accidental memory consumption due to repeated access of the 'Renderer.material' method duplicates the material over 
    /// and over.
    /// 
    /// Instead with this, accessing the field 'Material' gives you the material assigned to the Renderer source. If you want to make that material 
    /// unique, you can call GetUniqueMaterial(). This will flag the MaterialSource as 'Unique'. If you happen to assign this material to another 
    /// Renderer, you can manually flag it back false implying it's shared (though not globally shared).
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class RendererMaterialSource : MaterialSource
    {

        #region Fields

        [SerializeField()]
        [DefaultFromSelf]
        [ForceFromSelf(EntityRelativity.Self)]
        private Renderer _renderer;
        [SerializeField()]
        private bool _makeUniqueOnStart;

        [System.NonSerialized()]
        private bool _unique;

        #endregion

        #region CONSTRUCTOR
        
        protected override void Start()
        {
            if(object.ReferenceEquals(_renderer, null))
            {
                Debug.LogWarning("MaterialSource attached incorrectly to GameObject. Either attach MaterialSource at design time through editor, or call MaterialSource.GetMaterialSource.");
                UnityEngine.Object.Destroy(this);
            }
            else if(_renderer.gameObject != this.gameObject)
            {
                Debug.LogWarning("MaterialSource must be attached to the same GameObject as its Source Renderer.");
                UnityEngine.Object.Destroy(this);
            }

            if(_makeUniqueOnStart && !_unique)
            {
                this.GetUniqueMaterial();
            }

            base.Start();
        }

        #endregion

        #region Properties

        public Renderer Renderer
        {
            get { return _renderer; }
        }

        public override Material Material
        {
            get
            {
                return _renderer.sharedMaterial;
            }
            set
            {
                _renderer.material = value;
            }
        }

        /// <summary>
        /// Does not reflect if the material is truly unique. But suggests the possibly of uniqueness if 
        /// 'GetUniqueMaterial' were called.
        /// </summary>
        public override bool IsUnique
        {
            get { return _unique; }
            set { _unique = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Replaces the material on the Renderer with a copy of itself so it's unique.
        /// </summary>
        /// <returns></returns>
        public override Material GetUniqueMaterial()
        {
            _unique = true;
            return _renderer.material;
        }

        #endregion

        #region Static Interface

        public static RendererMaterialSource GetMaterialSource(Renderer renderer)
        {
            using (var lst = TempCollection.GetList<RendererMaterialSource>())
            {
                renderer.GetComponents<RendererMaterialSource>(lst);
                for (int i = 0; i < lst.Count; i++)
                {
                    if (lst[i]._renderer == renderer) return lst[i];
                }
            }

            var source = renderer.gameObject.AddComponent<RendererMaterialSource>();
            source._renderer = renderer;
            return source;
        }
        
        #endregion

    }

    [RequireComponent(typeof(UnityEngine.UI.Graphic))]
    public class GraphicMaterialSource : MaterialSource
    {

        #region Fields

        [SerializeField()]
        [DefaultFromSelf]
        [ForceFromSelf(EntityRelativity.Self)]
        private UnityEngine.UI.Graphic _graphics;
        [SerializeField()]
        private bool _makeUniqueOnStart;

        [System.NonSerialized()]
        private bool _unique;

        #endregion

        #region CONSTRUCTOR

        protected override void Start()
        {
            if (object.ReferenceEquals(_graphics, null))
            {
                Debug.LogWarning("MaterialSource attached incorrectly to GameObject. Either attach MaterialSource at design time through editor, or call MaterialSource.GetMaterialSource.");
                UnityEngine.Object.Destroy(this);
            }
            else if (_graphics.gameObject != this.gameObject)
            {
                Debug.LogWarning("MaterialSource must be attached to the same GameObject as its Source Renderer.");
                UnityEngine.Object.Destroy(this);
            }

            if (_makeUniqueOnStart && !_unique)
            {
                this.GetUniqueMaterial();
            }

            base.Start();
        }

        #endregion

        #region Properties

        public UnityEngine.UI.Graphic Graphics
        {
            get { return _graphics; }
        }

        public override Material Material
        {
            get
            {
                return _graphics.material ?? _graphics.defaultMaterial;
            }
            set
            {
                _graphics.material = value;
            }
        }

        /// <summary>
        /// Does not reflect if the material is truly unique. But suggests the possibly of uniqueness if 
        /// 'GetUniqueMaterial' were called.
        /// </summary>
        public override bool IsUnique
        {
            get { return _unique; }
            set { _unique = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Replaces the material on the Renderer with a copy of itself so it's unique.
        /// </summary>
        /// <returns></returns>
        public override Material GetUniqueMaterial()
        {
            var mat = _graphics.material ?? _graphics.defaultMaterial;
            if (_unique) return mat;

            _unique = true;
            mat = new Material(mat);
            _graphics.material = mat;
            return mat;
        }

        #endregion

        #region Static Interface

        public static GraphicMaterialSource GetMaterialSource(UnityEngine.UI.Graphic graphic)
        {
            using (var lst = TempCollection.GetList<GraphicMaterialSource>())
            {
                graphic.GetComponents<GraphicMaterialSource>(lst);
                for (int i = 0; i < lst.Count; i++)
                {
                    if (lst[i]._graphics == graphic) return lst[i];
                }
            }

            var source = graphic.gameObject.AddComponent<GraphicMaterialSource>();
            source._graphics = graphic;
            return source;
        }

        #endregion

    }

}
