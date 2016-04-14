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

        #endregion

        #region CONSTRUCTOR

        public ResourceLink(string path)
        {
            _path = path;
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
            return Resources.Load(_path);
        }

        public T GetResource<T>() where T : UnityEngine.Object
        {
            return Resources.Load<T>(_path);
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
