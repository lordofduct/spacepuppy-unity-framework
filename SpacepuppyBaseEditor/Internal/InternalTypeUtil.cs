using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.spacepuppyeditor.Internal
{
    internal static class InternalTypeUtil
    {

        #region Fields

        private static Assembly _unityEditorAssembly;
        public static Assembly UnityEditorAssembly
        {
            get
            {
                if (_unityEditorAssembly == null) _unityEditorAssembly = System.Reflection.Assembly.GetAssembly(typeof(Editor));
                return _unityEditorAssembly;
            }
        }

        #endregion

    }
}
