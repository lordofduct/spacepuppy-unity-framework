using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Collections
{
    public class GameObjectCollection : IEnumerable<GameObject>
    {

        #region Fields

        private string _name;
        private List<GameObject> _roots = new List<GameObject>();

        #endregion

        #region CONSTRUCTOR

        public GameObjectCollection()
        {
        }

        public GameObjectCollection(IEnumerable<GameObject> roots)
        {
            _roots.AddRange(roots);
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public bool IsAlive
        {
            get { return (from r in _roots where r != null select r).Any(); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns all the gameobjects in this collection as well as their children.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GameObject> GetAll()
        {
            return (from r in _roots where r != null from child in r.GetAllChildrenAndSelf() where child.gameObject != null select child.gameObject).Distinct();
        }

        public void Unload()
        {
            foreach (var r in _roots)
            {
                GameObject.Destroy(r);
            }
            _roots.Clear();
        }

        public void Unload(float t)
        {
            foreach (var r in _roots)
            {
                GameObject.Destroy(r, t);
            }
            _roots.Clear();
        }

        public GameObject Find(string spath, bool bIgnoreCase = false)
        {
            if (spath == null) return null;

            if (spath.StartsWith("./")) spath = spath.Substring(2);

            if (spath.Contains("/"))
            {
                int i = spath.IndexOf("/");
                var sParName = spath.Substring(0, i);
                var sChildPath = spath.Substring(i + 1);
                foreach (var r in _roots)
                {
                    if (r != null && StringUtil.Equals(r.name, sParName, bIgnoreCase))
                    {
                        var child = r.Find(sChildPath, bIgnoreCase);
                        if (child != null) return child;
                    }
                }
            }
            else
            {
                foreach (var r in _roots)
                {
                    if (r != null && StringUtil.Equals(r.name, spath, bIgnoreCase))
                    {
                        return r;
                    }
                }
            }

            return null;
        }

        public GameObject Search(string spath, bool bIgnoreCase = false)
        {
            if (spath == null) return null;

            if (spath.StartsWith("./")) spath = spath.Substring(2);

            if (spath.Contains("/"))
            {
                int i = spath.IndexOf("/");
                var sParName = spath.Substring(0, i);
                var sChildPath = spath.Substring(i + 1);
                foreach (var r in _roots)
                {
                    if (r != null && (sParName == "*" || StringUtil.Equals(r.name, sParName, bIgnoreCase)))
                    {
                        var child = r.Search(sChildPath, bIgnoreCase);
                        if (child != null) return child;
                    }
                }
            }
            else
            {
                foreach (var r in _roots)
                {
                    if (r != null && (spath == "*" || StringUtil.Equals(r.name, spath, bIgnoreCase)))
                    {
                        return r;
                    }
                }
            }

            return null;
        }

        public GameObject SearchFor(string sname, bool bIgnoreCase = false)
        {
            if (sname == null) return null;

            if (sname == "*") return (from r in _roots where r != null select r).FirstOrDefault();

            foreach (var r in _roots)
            {
                if (r != null && StringUtil.Equals(r.name, sname, bIgnoreCase)) return r;
            }

            foreach (var r in _roots)
            {
                if (r != null)
                {
                    var child = r.SearchFor(sname, bIgnoreCase);
                    if (child != null) return child;
                }
            }

            return null;
        }

        public IEnumerable<GameObject> FindWithTag(string tag)
        {
            foreach (var r in _roots)
            {
                if (r != null)
                {
                    foreach (var go in r.FindAllWithMultiTag(tag))
                    {
                        if (go != null) yield return go;
                    }
                }
            }
        }

        public IEnumerable<GameObject> FindOnLayer(int layerMask)
        {
            foreach (var r in _roots)
            {
                if (r != null)
                {
                    foreach (var go in r.FindGameObjectOnLayer(layerMask))
                    {
                        if (go != null) yield return go;
                    }
                }
            }
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<GameObject> GetEnumerator()
        {
            return (from r in _roots where r != null select r).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

    }
}
