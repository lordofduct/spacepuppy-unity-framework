using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Geom;

namespace com.spacepuppyeditor.Base.Commands
{
    public static class TransformCopyMenu
    {

        #region Fields

        private static Trans? _currentTrans;
        private static bool _local;

        #endregion

        #region Methods

        [MenuItem("GameObject/Transform/Copy Transform Local", priority = 0)]
        static void CopyTransformLocal()
        {
            var tr = Selection.activeTransform;
            if (tr == null) return;

            _currentTrans = Trans.GetLocal(tr);
            _local = true;
        }

        [MenuItem("GameObject/Transform/Copy Transform Local", validate = true)]
        static bool CopyTransformLocal_Validate()
        {
            return Selection.activeTransform != null;
        }

        [MenuItem("CONTEXT/Transform/Copy Transform Local", priority=100)]
        static void CopyTransformLocal_Transform(MenuCommand cmnd)
        {
            var tr = cmnd.context as Transform;
            if (tr == null) return;

            _currentTrans = Trans.GetLocal(tr);
            _local = true;
        }




        [MenuItem("GameObject/Transform/Copy Transform Global", priority = 0)]
        static void CopyTransformGlobal()
        {
            var tr = Selection.activeTransform;
            if (tr == null) return;

            _currentTrans = Trans.GetGlobal(tr);
            _local = false;
        }

        [MenuItem("GameObject/Transform/Copy Transform Global", validate = true)]
        static bool CopyTransformGlobal_Validate()
        {
            return Selection.activeTransform != null;
        }

        [MenuItem("CONTEXT/Transform/Copy Transform Global", priority = 100)]
        static void CopyTransformGlobal_Transform(MenuCommand cmnd)
        {
            var tr = cmnd.context as Transform;
            if (tr == null) return;

            _currentTrans = Trans.GetGlobal(tr);
            _local = false;
        }




        [MenuItem("GameObject/Transform/Paste Transform", priority = 0)]
        static void PasteTransform()
        {
            if (_currentTrans == null) return;

            var tr = Selection.activeTransform;
            if (tr == null) return;

            if (_local)
                _currentTrans.Value.SetToLocal(tr);
            else
                _currentTrans.Value.SetToGlobal(tr, false);
        }

        [MenuItem("GameObject/Transform/Paste Transform", validate = true)]
        static bool PasteTransform_Validate(MenuCommand cmnd)
        {
            if (_currentTrans == null) return false;

            var tr = Selection.activeTransform;
            if (tr == null) return false;

            return true;

        }

        [MenuItem("CONTEXT/Transform/Paste Transform", priority = 100)]
        static void PasteTransform_Transform(MenuCommand cmnd)
        {
            if (_currentTrans == null) return;

            var tr = cmnd.context as Transform;
            if (tr == null) return;

            if (_local)
                _currentTrans.Value.SetToLocal(tr);
            else
                _currentTrans.Value.SetToGlobal(tr, false);
        }

        [MenuItem("CONTEXT/Transform/Paste Transform", validate=true)]
        static bool PasteTransform_Transform_Validate(MenuCommand cmnd)
        {
            if (_currentTrans == null) return false;

            var tr = cmnd.context as Transform;
            if (tr == null) return false;

            return true;

        }





        [MenuItem("GameObject/Transform/Move To Scene Camera")]
        public static void MoveToSceneCamera()
        {
            var go = Selection.activeGameObject;
            if (go == null) return;

            var view = SceneView.lastActiveSceneView;
            if (view == null || view.camera == null) return;

            var t = view.camera.transform;
            go.transform.position = t.position;
            go.transform.rotation = t.rotation;
        }
        [MenuItem("GameObject/Transform/Move To Scene Camera", validate = true)]
        public static bool MoveToSceneCamera_Validate(MenuCommand cmnd)
        {
            var go = Selection.activeGameObject;
            if (go == null) return false;

            return true;
        }

        [MenuItem("CONTEXT/Transform/Move To Scene Camera")]
        public static void MoveToSceneCamera_Transform(MenuCommand cmnd)
        {
            var go = com.spacepuppy.Utils.GameObjectUtil.GetGameObjectFromSource(cmnd.context);
            if (go == null) return;

            var view = SceneView.lastActiveSceneView;
            if (view == null || view.camera == null) return;

            var t = view.camera.transform;
            go.transform.position = t.position;
            go.transform.rotation = t.rotation;
        }
        [MenuItem("CONTEXT/Transform/Move To Scene Camera", validate = true)]
        public static bool MoveToSceneCamera_Transform_Validate(MenuCommand cmnd)
        {
            var go = com.spacepuppy.Utils.GameObjectUtil.GetGameObjectFromSource(cmnd.context);
            if (go == null) return false;

            return true;
        }


        #endregion

    }
}
