using UnityEngine;
using UnityEditor;

using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    public static class CreateGameObjectIn
    {

        #region Create Empty Child

        /*
        [MenuItem("GameObject/Create Empty Child", priority = 0)]
        static void CreateGameObjectAsChild()
        {
            var go = new GameObject("GameObject");
            go.transform.parent = Selection.activeTransform;
            go.transform.ZeroOut(false);
        }

        [MenuItem("GameObject/Create Empty Child", true)]
        static bool ValidateCreateGameObjectAsChild()
        {
            return Selection.activeTransform != null;
        }
         */

        [MenuItem("CONTEXT/Transform/Create Empty Child")]
        static void CreateGameObjectAsChild_Transform(MenuCommand cmnd)
        {
            var tr = cmnd.context as Transform;
            var go = new GameObject("GameObject");
            go.transform.parent = tr;
            go.transform.ZeroOut(false);
        }



        [MenuItem("GameObject/Create Other Child/Cube", priority = 0)]
        static void CreateCubeAsChild()
        {
            var go = PrimitiveUtil.CreatePrimitive(PrimitiveType.Cube);
            go.transform.parent = Selection.activeTransform;
            go.transform.ZeroOut(false);
        }

        [MenuItem("GameObject/Create Other Child/Sphere", priority = 0)]
        static void CreateSphereAsChild()
        {
            var go = PrimitiveUtil.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.parent = Selection.activeTransform;
            go.transform.ZeroOut(false);
        }

        [MenuItem("GameObject/Create Other Child/Capsule", priority = 0)]
        static void CreateCapsuleAsChild()
        {
            var go = PrimitiveUtil.CreatePrimitive(PrimitiveType.Capsule);
            go.transform.parent = Selection.activeTransform;
            go.transform.ZeroOut(false);
        }

        [MenuItem("GameObject/Create Other Child/Cylinder", priority = 0)]
        static void CreateCylinderAsChild()
        {
            var go = PrimitiveUtil.CreatePrimitive(PrimitiveType.Cylinder);
            go.transform.parent = Selection.activeTransform;
            go.transform.ZeroOut(false);
        }

        [MenuItem("GameObject/Create Other Child/Plane", priority = 0)]
        static void CreatePlaneAsChild()
        {
            var go = PrimitiveUtil.CreatePrimitive(PrimitiveType.Plane);
            go.transform.parent = Selection.activeTransform;
            go.transform.ZeroOut(false);
        }

        [MenuItem("GameObject/Create Other Child/Quad", priority = 0)]
        static void CreateQuadAsChild()
        {
            var go = PrimitiveUtil.CreatePrimitive(PrimitiveType.Quad);
            go.transform.parent = Selection.activeTransform;
            go.transform.ZeroOut(false);
        }





        [MenuItem("GameObject/Create Other Child/Cube Trigger", priority = 20)]
        static void CreateCubeTriggerAsChild()
        {
            var go = PrimitiveUtil.CreatePrimitive(PrimitiveType.Cube, true, true);
            go.GetComponent<Collider>().isTrigger = true;
            go.transform.parent = Selection.activeTransform;
            go.transform.ZeroOut(false);
        }

        [MenuItem("GameObject/Create Other Child/Sphere Trigger", priority = 20)]
        static void CreateSphereTriggerAsChild()
        {
            var go = PrimitiveUtil.CreatePrimitive(PrimitiveType.Sphere, true, true);
            go.transform.parent = Selection.activeTransform;
            go.transform.ZeroOut(false);
        }

        [MenuItem("GameObject/Create Other Child/Capsule Trigger", priority = 20)]
        static void CreateCapsuleTriggerAsChild()
        {
            var go = PrimitiveUtil.CreatePrimitive(PrimitiveType.Capsule, true, true);
            go.transform.parent = Selection.activeTransform;
            go.transform.ZeroOut(false);
        }

        [MenuItem("GameObject/Create Other Child/Cylinder Trigger", priority = 20)]
        static void CreateCylinderTriggerAsChild()
        {
            var go = PrimitiveUtil.CreatePrimitive(PrimitiveType.Cylinder, true, true);
            go.transform.parent = Selection.activeTransform;
            go.transform.ZeroOut(false);
        }

        [MenuItem("GameObject/Create Other Child/Plane Trigger", priority = 20)]
        static void CreatePlaneTriggerAsChild()
        {
            var go = PrimitiveUtil.CreatePrimitive(PrimitiveType.Plane, true, true);
            go.transform.parent = Selection.activeTransform;
            go.transform.ZeroOut(false);
        }

        [MenuItem("GameObject/Create Other Child/Quad Trigger", priority = 20)]
        static void CreateQuadTriggerAsChild()
        {
            var go = PrimitiveUtil.CreatePrimitive(PrimitiveType.Quad, true, true);
            go.transform.parent = Selection.activeTransform;
            go.transform.ZeroOut(false);
        }

        #endregion

        #region Create Empty As Parent

        [MenuItem("GameObject/Create Empty As Parent", priority = 0)]
        static void CreateGameObjectAsParent()
        {
            var go = new GameObject("GameObject");
            go.transform.parent = Selection.activeTransform.parent;
            go.transform.ZeroOut(false);

            go.transform.position = Selection.activeTransform.position;
            Selection.activeTransform.parent = go.transform;
            //Selection.activeTransform.ZeroOut(false);
        }

        [MenuItem("GameObject/Create Empty As Parent", true)]
        static bool ValidateCreateGameObjectAsParent()
        {
            return Selection.activeTransform != null;
        }

        [MenuItem("CONTEXT/Transform/Create GameObject As Parent")]
        static void CreateGameObjectAsParent_Transform(MenuCommand cmnd)
        {
            var tr = cmnd.context as Transform;

            var go = new GameObject("GameObject");

            go.transform.position = tr.position;
            go.transform.parent = Selection.activeTransform.parent;
            go.transform.ZeroOut(false);

            go.transform.position = tr.position;
            tr.parent = go.transform;
            //tr.ZeroOut(false);
        }

        #endregion

        #region Create Empty At

        [MenuItem("GameObject/Create Empty At", priority = 0)]
        static void CreateGameObjectAt()
        {
            var go = new GameObject("GameObject");
            go.transform.parent = Selection.activeTransform.parent;
            go.transform.ZeroOut(false);
            go.transform.position = Selection.activeTransform.position;
        }

        [MenuItem("GameObject/Create Empty At", true)]
        static bool ValidateCreateGameObjectAt()
        {
            return Selection.activeTransform != null;
        }

        [MenuItem("CONTEXT/Transform/Create GameObject At")]
        static void CreateGameObjectAt_Transform(MenuCommand cmnd)
        {
            var tr = cmnd.context as Transform;
            var go = new GameObject("GameObject");
            go.transform.parent = tr.parent;
            go.transform.ZeroOut(false);
            go.transform.position = tr.position;
        }

        #endregion

    }

}