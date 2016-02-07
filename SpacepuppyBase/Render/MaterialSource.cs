using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Render
{

    /// <summary>
    /// A self-tracking Material source accessor. This allows you to track if a Material is using a shared material, or you had made it unique. 
    /// The default interface for accessing materials on a Renderer in Unity is rather confusing and it's hard to tell if it's shared or not. 
    /// This also leads to accidental memory consumption due to repeated access of the 'Renderer.material' method duplicates the material over 
    /// and over.
    /// 
    /// Instead with this, accessing the field 'Materia' gives you the material assigned to the Renderer source. If you want to make that material 
    /// unique, you can call GetUniqueMaterial(). This will flag the MaterialSource as 'Unique'. If you happen to assign this material to another 
    /// Renderer, you can manually flag it back false implying it's shared (though not globally shared).
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class MaterialSource : SPComponent
    {

        #region Fields

        [SerializeField()]
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

        public Material Material
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
        public bool IsUnique
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
        public Material GetUniqueMaterial()
        {
            _unique = true;
            return _renderer.material;
        }

        #endregion

        #region Static Interface

        public static MaterialSource GetMaterialSource(Renderer renderer)
        {
            using (var lst = TempCollection.GetList<MaterialSource>())
            {
                renderer.GetComponents<MaterialSource>(lst);
                for(int i = 0; i < lst.Count; i++)
                {
                    if (lst[i]._renderer == renderer) return lst[i];
                }
            }

            var source = renderer.gameObject.AddComponent<MaterialSource>();
            source._renderer = renderer;
            return source;
        }

        public static MaterialSource GetMaterialSource(UnityEngine.Object obj)
        {
            Renderer renderer = com.spacepuppy.Utils.ComponentUtil.GetComponentFromSource<Renderer>(obj);
            if (renderer == null) return null;

            return GetMaterialSource(renderer);
        }

        #endregion

    }
}
