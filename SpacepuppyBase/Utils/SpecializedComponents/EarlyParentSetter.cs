using UnityEngine;

namespace com.spacepuppy.Utils.SpecializedComponents
{

    /// <summary>
    /// Used by PrefabUtil, should never be used by anyone else.
    /// 
    /// This is force injected onto a prefab, so that when a clone of it is made, that clone will set its parent before any other script on it oeprates. 
    /// This requires that its execution order be set very low.
    /// </summary>
    [Infobox("A special script used at runtime for advanced 'Instantiate', if you've added this yourself, remove it.")]
    public class EarlyParentSetter : MonoBehaviour
    {

        private static Transform _parent;

        public void Init(Transform par)
        {
            _parent = par;
        }

        private void Awake()
        {
            if (_parent != null)
            {
                this.transform.parent = _parent;
                _parent = null;
            }

            if (Application.isPlaying)
            {
                Destroy(this);
            }
        }

    }
}
