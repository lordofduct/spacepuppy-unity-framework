using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Utils
{
    public static class PrimitiveUtil
    {

        #region CreatePrimitive

        public static GameObject CreatePrimitive(PrimitiveType type)
        {
            return GameObject.CreatePrimitive(type);
        }

        public static GameObject CreatePrimitive(PrimitiveType type, bool bLeaveOutMesh)
        {
            return CreatePrimitive(type, bLeaveOutMesh, false);
        }

        public static GameObject CreatePrimitive(PrimitiveType type, bool bLeaveOutMesh, bool bTrigger)
        {
            GameObject go = null;
            if (bLeaveOutMesh)
            {
                switch (type)
                {
                    case PrimitiveType.Cube:
                        go = new GameObject(type.ToString());
                        go.AddComponent<BoxCollider>();
                        break;
                    case PrimitiveType.Capsule:
                        go = new GameObject(type.ToString());
                        var cap = go.AddComponent<CapsuleCollider>();
                        cap.radius = 0.5f;
                        cap.height = 2.0f;
                        break;
                    case PrimitiveType.Sphere:
                        go = new GameObject(type.ToString());
                        go.AddComponent<SphereCollider>();
                        break;
                    case PrimitiveType.Cylinder:
                        go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        ObjUtil.SmartDestroy(go.GetComponent<MeshCollider>());
                        var coll = go.AddComponent<MeshCollider>();
                        coll.sharedMesh = go.GetComponent<MeshFilter>().sharedMesh;
                        ObjUtil.SmartDestroy(go.GetComponent<MeshFilter>());
                        ObjUtil.SmartDestroy(go.GetComponent<MeshRenderer>());
                        break;
                    case PrimitiveType.Plane:
                        go = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        ObjUtil.SmartDestroy(go.GetComponent<MeshFilter>());
                        ObjUtil.SmartDestroy(go.GetComponent<MeshRenderer>());
                        break;
                    case PrimitiveType.Quad:
                        go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                        ObjUtil.SmartDestroy(go.GetComponent<MeshFilter>());
                        ObjUtil.SmartDestroy(go.GetComponent<MeshRenderer>());
                        break;
                }
            }
            else
            {
                go = GameObject.CreatePrimitive(type);
            }

            if (go != null)
            {
                go.GetComponent<Collider>().isTrigger = bTrigger;
            }
            return go;
        }

        #endregion

        #region AddPrimitiveCollider

        public static Collider AddPrimitiveCollider(this Component comp, PrimitiveType type, bool bTrigger)
        {
            if (comp == null) throw new System.ArgumentNullException("comp");
            return AddPrimitiveCollider(comp.gameObject, type, bTrigger);
        }

        public static Collider AddPrimitiveCollider(this GameObject go, PrimitiveType type, bool bTrigger)
        {
            switch (type)
            {
                case PrimitiveType.Cube:
                    return go.AddComponent<BoxCollider>();
                case PrimitiveType.Capsule:
                case PrimitiveType.Cylinder:
                    var cap = go.AddComponent<CapsuleCollider>();
                    cap.radius = 0.5f;
                    cap.height = 2.0f;
                    return cap;
                case PrimitiveType.Sphere:
                    return go.AddComponent<SphereCollider>();
                case PrimitiveType.Plane:
                case PrimitiveType.Quad:
                    var box = go.AddComponent<BoxCollider>();
                    box.size = new Vector3(1f, 0.01f, 1f);
                    return box;
            }

            return null;
        }

        #endregion

        #region CreateMesh

        private static Mesh _sphereMesh;
        private static Mesh _capsuleMesh;
        private static Mesh _cylinderMesh;
        private static Mesh _cubeMesh;
        private static Mesh _planeMesh;
        private static Mesh _quadMesh;
        private static Mesh _ringMesh;
        private static Mesh _lineMesh;

        public static Mesh SphereMesh
        {
            get
            {
                if (_sphereMesh == null) _sphereMesh = CreateMesh(PrimitiveType.Sphere);
                return _sphereMesh;
            }
        }
        public static Mesh CapsuleMesh
        {
            get
            {
                if (_capsuleMesh == null) _capsuleMesh = CreateMesh(PrimitiveType.Capsule);
                return _capsuleMesh;
            }
        }
        public static Mesh CylinderMesh
        {
            get
            {
                if (_cylinderMesh == null) _cylinderMesh = CreateMesh(PrimitiveType.Cylinder);
                return _cylinderMesh;
            }
        }
        public static Mesh CubeMesh
        {
            get
            {
                if (_cubeMesh == null) _cubeMesh = CreateMesh(PrimitiveType.Cube);
                return _cubeMesh;
            }
        }
        public static Mesh PlaneMesh
        {
            get
            {
                if (_planeMesh == null) _planeMesh = CreateMesh(PrimitiveType.Plane);
                return _planeMesh;
            }
        }
        public static Mesh QuadMesh
        {
            get
            {
                if (_quadMesh == null) _quadMesh = CreateMesh(PrimitiveType.Quad);
                return _quadMesh;
            }
        }
        public static Mesh RingMesh
        {
            get
            {
                if (_ringMesh == null) _ringMesh = CreateRingMesh();
                return _ringMesh;
            }
        }
        public static Mesh LineMesh
        {
            get
            {
                if (_lineMesh == null) _lineMesh = CreateLineMesh();
                return _lineMesh;
            }
        }

        public static Mesh GetMesh(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.Sphere:
                    return SphereMesh;
                case PrimitiveType.Capsule:
                    return CapsuleMesh;
                case PrimitiveType.Cylinder:
                    return CylinderMesh;
                case PrimitiveType.Cube:
                    return CubeMesh;
                case PrimitiveType.Plane:
                    return PlaneMesh;
                case PrimitiveType.Quad:
                    return QuadMesh;
            }
            return null;
        }

        public static Mesh CreateMesh(PrimitiveType type)
        {
            var go = CreatePrimitive(type);
            var mesh = go.GetComponent<MeshFilter>().sharedMesh;
            ObjUtil.SmartDestroy(go);
            return mesh;
        }
        public static Mesh CreateRingMesh()
        {
            const float OUT_RADIUS = 0.5f;
            const float IN_RADIUS = 0.45f;
            const int SEGMENT_CNT = 64;

            List<Vector3> verts = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            Vector3 outv = new Vector3(0f, 0f, OUT_RADIUS);
            Vector3 inv = new Vector3(0f, 0f, IN_RADIUS);
            for (int i = 0; i < SEGMENT_CNT; i++)
            {
                float a1 = (float)i * 360f / (float)SEGMENT_CNT;
                float a2 = (float)(i + 1) * 360f / (float)SEGMENT_CNT;
                Quaternion r1 = Quaternion.Euler(0f, 180f + a1, 0f);
                Quaternion r2 = Quaternion.Euler(0f, 180f + a2, 0f);

                int cnt = verts.Count;
                verts.Add(r1 * inv);
                verts.Add(r1 * outv);
                verts.Add(r2 * outv);
                verts.Add(r2 * inv);

                uvs.Add(new Vector2(0f, a1 / 360f));
                uvs.Add(new Vector2(1f, a1 / 360f));
                uvs.Add(new Vector2(1f, a2 / 360f));
                uvs.Add(new Vector2(0f, a2 / 360f));

                tris.Add(cnt);
                tris.Add(cnt + 1);
                tris.Add(cnt + 2);
                tris.Add(cnt);
                tris.Add(cnt + 2);
                tris.Add(cnt + 3);
                tris.Add(cnt + 2);
                tris.Add(cnt + 1);
                tris.Add(cnt);
                tris.Add(cnt + 3);
                tris.Add(cnt + 2);
                tris.Add(cnt);
            }

            var mesh = new Mesh();
            mesh.vertices = verts.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
        public static Mesh CreateLineMesh()
        {
            var mesh = new Mesh();
            mesh.vertices = new Vector3[] {
                new Vector3(0f, 0.05f, 0f),
                new Vector3(0f, 0.05f, 0.5f),
                new Vector3(0f, -0.05f, 0.5f),
                new Vector3(0f, -0.05f, 0f)
            };
            mesh.uv = new Vector2[4];
            mesh.triangles = new int[]
            {
                0,1,2,0,2,3,3,2,1,3,1,0
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        #endregion

    }
}
