using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Project;
using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    [AddComponentMenu("SpacePuppy/Multi Tag")]
    [DisallowMultipleComponent()]
    public class MultiTag : com.spacepuppy.SPComponent
    {

        #region Multiton Interface

        private static Dictionary<GameObject, MultiTag> _pool = new Dictionary<GameObject, MultiTag>(com.spacepuppy.Collections.ObjectInstanceIDEqualityComparer<GameObject>.Default);

        internal static bool TryGetMultiTag(GameObject go, out MultiTag c)
        {
            if( _pool.TryGetValue(go, out c))
            {
                return true;
            }
            else if (go.CompareTag(SPConstants.TAG_MULTITAG))
            {
                c = go.GetComponent<MultiTag>();
                return c != null;
            }
            return false;
        }

        internal static MultiTag[] FindAll(string tag)
        {
            return (from c in _pool.Values where c.HasTag(tag) select c).ToArray();
        }

        internal static MultiTag Find(string tag)
        {
            foreach(var c in _pool.Values)
            {
                if (c.HasTag(tag)) return c;
            }
            return null;
        }

        #endregion

        #region Fields

        [SerializeField]
        private string[] _tags;

        #endregion

        #region CONSTRUCTOR

        // Use this for initialization
        protected override void Awake()
        {
            base.Awake();

            if(!this.gameObject.CompareTag(SPConstants.TAG_MULTITAG))
                this.gameObject.tag = SPConstants.TAG_MULTITAG;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _pool[this.gameObject] = this;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _pool.Remove(this.gameObject);
        }

        #endregion

        #region Properties

        public int Count { get { return (_tags != null) ? _tags.Length : 0; } }

        #endregion

        #region Methods

        public void UpdateTags(IEnumerable<string> tags)
        {
            if (tags == null)
            {
                _tags = null;
            }
            else
            {
                _tags = (from s in tags where IsValidMultiTag(s) select s).ToArray();
            }
        }

        public IEnumerable<string> GetTags()
        {
            if (_tags == null) yield break;

            foreach (var tag in _tags) yield return tag;
        }

        public bool ContainsTag(string tag)
        {
            if (_tags == null || _tags.Length == 0) return MultiTag.IsEmptyTag(tag);
            return System.Array.IndexOf(_tags, tag) >= 0;
        }

        public bool ContainsTag(params string[] tags)
        {
            if (_tags == null || _tags.Length == 0) return tags.Contains(SPConstants.TAG_UNTAGGED);
            return _tags.ContainsAny(tags);
        }

        public bool ContainsTag(IEnumerable<string> tags)
        {
            if (_tags == null || _tags.Length == 0) return tags.Contains(SPConstants.TAG_UNTAGGED);
            return _tags.ContainsAny(tags);
        }

        public void AddTag(string tag)
        {
            if (!IsValidMultiTag(tag)) return;

            if (_tags == null)
            {
                _tags = new string[] { tag };
            }
            else
            {
                if (!_tags.Contains(tag))
                {
                    System.Array.Resize(ref _tags, _tags.Length + 1);
                    _tags[_tags.Length - 1] = tag;
                }
            }
        }

        public bool RemoveTag(string tag)
        {
            if (_tags == null) return false;

            using (var lst = com.spacepuppy.Collections.TempCollection.GetList<string>(_tags))
            {
                if (lst.Remove(tag))
                {
                    _tags = lst.ToArray();
                }
            }

            return false;
        }

        public void ClearTags()
        {
            _tags = null;
        }

        #endregion


        #region Static Methods

        public static bool IsValidTag(string stag)
        {
            //TODO - need a way to find list of known tags
            //return !string.IsNullOrEmpty(stag);
            return TagData.Tags.Contains(stag);
        }

        public static bool IsValidActiveTag(string stag)
        {
            //return !string.IsNullOrEmpty(stag) && stag != SPConstants.TAG_UNTAGGED;
            return TagData.Tags.Contains(stag) && stag != SPConstants.TAG_UNTAGGED;
        }

        public static bool IsValidMultiTag(string stag)
        {
            //return !string.IsNullOrEmpty(stag) && stag != SPConstants.TAG_UNTAGGED && stag != SPConstants.TAG_MULTITAG;
            return TagData.Tags.Contains(stag) && stag != SPConstants.TAG_UNTAGGED && stag != SPConstants.TAG_MULTITAG;
        }

        public static bool IsEmptyTag(string stag)
        {
            return string.IsNullOrEmpty(stag) || stag == SPConstants.TAG_UNTAGGED;
        }

        #endregion

    }

    public static class MultiTagHelper
    {

        /// <summary>
        /// Determines if GameObject has a tag.
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static bool HasTag(this GameObject go)
        {
            if (go == null) return false;

            if (go.CompareTag(SPConstants.TAG_UNTAGGED)) return false;

            //if (go.CompareTag(SPConstants.TAG_MULTITAG))
            //{
            //    var multitag = go.GetComponent<MultiTag>();
            //    return (multitag != null) ? multitag.Count > 0 : false;
            //}
            //else
            //{
            //    return true;
            //}
            MultiTag c;
            if (MultiTag.TryGetMultiTag(go, out c))
                return c.Count > 0;
            else
                return true;
        }

        public static bool HasTag(this Component c)
        {
            if (c == null) throw new System.ArgumentNullException("c");

            return HasTag(c.gameObject);
        }

        /**
         * HasTag
         */

        public static bool HasTag(this GameObject go, string stag)
        {
            if (go == null) return false;
            if (go.CompareTag(stag)) return true;

            //var multitag = go.GetComponent<MultiTag>();
            //if (multitag != null && multitag.ContainsTag(stag)) return true;

            //return false;

            MultiTag c;
            if (MultiTag.TryGetMultiTag(go, out c))
                return c.ContainsTag(stag);
            else
                return false;
        }

        public static bool HasTag(this Component c, string stag)
        {
            if (c == null) return false;
            return HasTag(c.gameObject, stag);
        }

        public static bool HasTag(this GameObject go, params string[] stags)
        {
            if (stags == null) return false;
            if (go == null) return false;
            //foreach(var stag in stags)
            //{
            //    if (go.CompareTag(stag)) return true;
            //}

            //var multitag = go.GetComponent<MultiTag>();
            //if (multitag != null && multitag.ContainsTag(stags)) return true;

            //return false;

            MultiTag c;
            if (MultiTag.TryGetMultiTag(go, out c))
            {
                return c.ContainsTag(stags);
            }
            else
            {
                foreach (var stag in stags)
                {
                    if (go.CompareTag(stag)) return true;
                }
            }
            return false;
        }

        public static bool HasTag(this Component c, params string[] stags)
        {
            if (c == null) return false;
            return HasTag(c.gameObject, stags);
        }

        public static bool HasTag(this GameObject go, IEnumerable<string> stags)
        {
            if (stags == null) return false;
            if (go == null) return false;
            //if (stags.Contains(go.tag)) return true;

            //var multitag = go.GetComponent<MultiTag>();
            //if (multitag != null && multitag.ContainsTag(stags)) return true;

            //return false;

            MultiTag c;
            if (MultiTag.TryGetMultiTag(go, out c))
            {
                return c.ContainsTag(stags);
            }
            else
            {
                return stags.Contains(go.tag);
            }
        }

        public static bool HasTag(this Component c, IEnumerable<string> stags)
        {
            if (c == null) return false;
            return HasTag(c.gameObject, stags);
        }


        /**
         * AddTag
         */

        public static void AddTag(this GameObject go, string stag)
        {
            if (go == null) throw new System.ArgumentNullException("go");

            //var multitag = go.GetComponent<MultiTag>();
            //if (multitag != null)
            //{
            //    multitag.AddTag(stag);
            //}
            //else
            //{
            //    if (MultiTag.IsEmptyTag(go.tag))
            //    {
            //        go.tag = stag;
            //    }
            //    else
            //    {
            //        multitag = go.AddComponent<MultiTag>();
            //        multitag.AddTag(stag);
            //    }
            //}

            MultiTag multitag;
            if (MultiTag.TryGetMultiTag(go, out multitag))
            {
                multitag.AddTag(stag);
            }
            else
            {
                if (MultiTag.IsEmptyTag(go.tag))
                {
                    go.tag = stag;
                }
                else
                {
                    multitag = go.AddComponent<MultiTag>();
                    multitag.AddTag(stag);
                }
            }
        }

        public static void AddTag(this Component c, string stag)
        {
            if (c == null) throw new System.ArgumentNullException("c");

            AddTag(c.gameObject, stag);
        }

        /**
         * RemoveTag
         */

        public static void RemoveTag(this GameObject go, string stag, bool bDestroyMultiTagComponentOnEmpty = false)
        {
            if (go == null) throw new System.ArgumentNullException("go");

            //var multitag = go.GetComponent<MultiTag>();
            //if (multitag != null)
            //{
            //    multitag.RemoveTag(stag);
            //    if (bDestroyMultiTagComponentOnEmpty && multitag.Count == 0)
            //    {
            //        Object.Destroy(multitag);
            //        go.tag = SPConstants.TAG_UNTAGGED;
            //    }
            //}
            //else
            //{
            //    if (go.CompareTag(stag)) go.tag = SPConstants.TAG_UNTAGGED;
            //}

            MultiTag multitag;
            if (MultiTag.TryGetMultiTag(go, out multitag))
            {
                multitag.RemoveTag(stag);
                if (bDestroyMultiTagComponentOnEmpty && multitag.Count == 0)
                {
                    Object.Destroy(multitag);
                    go.tag = SPConstants.TAG_UNTAGGED;
                }
            }
            else
            {
                if (go.CompareTag(stag)) go.tag = SPConstants.TAG_UNTAGGED;
            }
        }

        public static void RemoveTag(this Component c, string stag, bool bDestroyMultiTagComponentOnEmpty = false)
        {
            if (c == null) throw new System.ArgumentNullException("c");

            RemoveTag(c.gameObject, stag, bDestroyMultiTagComponentOnEmpty);
        }

        /**
         * SetTag
         */

        public static void SetTag(this GameObject go, string stag, bool bDestroyMultiTagComponent = false)
        {
            if (go == null) throw new System.ArgumentNullException("go");

            if (bDestroyMultiTagComponent)
            {
                go.RemoveComponents<MultiTag>();
                if (MultiTag.IsValidMultiTag(stag))
                {
                    go.tag = stag;
                }
                else
                {
                    go.tag = SPConstants.TAG_UNTAGGED;
                }
            }
            else
            {
                //var multitag = go.GetComponent<MultiTag>();
                //if (multitag != null)
                //{
                //    multitag.ClearTags();
                //    if (MultiTag.IsValidMultiTag(stag)) multitag.AddTag(stag);
                //}
                //else if (MultiTag.IsValidMultiTag(stag))
                //{
                //    go.tag = stag;
                //}
                //else
                //{
                //    go.tag = SPConstants.TAG_UNTAGGED;
                //}

                MultiTag multitag;
                if (MultiTag.TryGetMultiTag(go, out multitag))
                {
                    multitag.ClearTags();
                    if (MultiTag.IsValidMultiTag(stag)) multitag.AddTag(stag);
                }
                else if (MultiTag.IsValidMultiTag(stag))
                {
                    go.tag = stag;
                }
                else
                {
                    go.tag = SPConstants.TAG_UNTAGGED;
                }
            }
        }

        public static void SetTag(this Component c, string stag, bool bDestroyMultiTagComponent = false)
        {
            if (c == null) throw new System.ArgumentNullException("c");

            SetTag(c.gameObject, stag, bDestroyMultiTagComponent);
        }

        /**
         * ClearTag
         */

        public static void ClearTags(this GameObject go, bool bDestroyMultiTagComponent = false)
        {
            if (go == null) throw new System.ArgumentNullException("go");

            //var multitag = go.GetComponent<MultiTag>();
            //if (multitag != null)
            //{
            //    if (bDestroyMultiTagComponent)
            //    {
            //        Object.Destroy(multitag);
            //        go.tag = SPConstants.TAG_UNTAGGED;
            //    }
            //    else
            //    {
            //        multitag.ClearTags();
            //    }
            //}
            //else
            //{
            //    go.tag = SPConstants.TAG_UNTAGGED;
            //}

            MultiTag multitag;
            if (MultiTag.TryGetMultiTag(go, out multitag))
            {
                if (bDestroyMultiTagComponent)
                {
                    Object.Destroy(multitag);
                    go.tag = SPConstants.TAG_UNTAGGED;
                }
                else
                {
                    multitag.ClearTags();
                }
            }
            else
            {
                go.tag = SPConstants.TAG_UNTAGGED;
            }
        }

        public static void ClearTags(this Component c, bool bDestroyMultiTagComponent = false)
        {
            if (c == null) throw new System.ArgumentNullException("c");

            ClearTags(c.gameObject, bDestroyMultiTagComponent);
        }

    }

}