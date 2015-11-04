using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Dynamic;

namespace com.spacepuppyeditor.Internal
{
    public class GUIViewProxy
    {

        #region Static Interface

        static GUIViewProxy()
        {
            var klass = InternalTypeUtil.UnityEditorAssembly.GetType("UnityEditor.GUIView");
            _staticWrapper = new TypeAccessWrapper(klass, true);
        }

        private static TypeAccessWrapper _staticWrapper;

        public static GUIViewProxy GetCurrent()
        {
            var obj = _staticWrapper.GetStaticProperty("current");
            if (obj == null) return null;

            return new GUIViewProxy(obj);
        }

        #endregion


        #region Fields

        private object _internalGUIView;

        #endregion

        #region CONSTRUCTOR

        private GUIViewProxy(object obj)
        {
            _internalGUIView = obj;
        }

        #endregion

        #region Methods

        public bool SendEvent(Event e)
        {
            return (bool)DynamicUtil.InvokeMethodDirect(_internalGUIView, "SendEvent", e);
        }

        #endregion

    }
}
