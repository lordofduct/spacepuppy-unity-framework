using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Project
{

    [System.Serializable()]
    public struct ResourceLink
    {

        #region Fields

        [SerializeField()]
        private string _path;
        [System.NonSerialized]
        private UnityEngine.Object _obj;

        #endregion

        #region CONSTRUCTOR

        public ResourceLink(string path)
        {
            _path = path;
            _obj = null;
        }

        #endregion

        #region Properties

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        #endregion

        #region Methods

        public UnityEngine.Object GetResource()
        {
            if(_obj == null)
                _obj = Resources.Load(_path);
            return _obj;
        }

        public T GetResource<T>() where T : UnityEngine.Object
        {
            if(_obj == null)
                _obj = Resources.Load<T>(_path);
            return _obj as T;
        }

        #endregion

        #region Special Types

        public class ConfigAttribute : System.Attribute
        {

            public System.Type resourceType;

            public ConfigAttribute(System.Type resourceType)
            {
                this.resourceType = resourceType;
            }

        }

        #endregion

    }
}
