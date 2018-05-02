using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppyeditor
{
    public static class HandlesHelper
    {

        #region Private Draw Utils

        private static Color RealHandleColor
        {
            get
            {
                return Handles.color * new Color(1f, 1f, 1f, 0.5f) + (!Handles.lighting ? new Color(0.0f, 0.0f, 0.0f, 0.0f) : new Color(0.0f, 0.0f, 0.0f, 0.5f));
            }
        }

        private static Matrix4x4 StartDraw(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Shader.SetGlobalColor("_HandleColor", RealHandleColor);
            Shader.SetGlobalFloat("_HandleSize", 1.0f);
            Matrix4x4 mat = Handles.matrix * Matrix4x4.TRS(position, rotation, scale);
            Shader.SetGlobalMatrix("_ObjectToWorld", mat);
            HandleUtility.handleMaterial.SetPass(0);
            return mat;
        }

        #endregion


        public static void DrawWireCollider(Collider c)
        {
            if (c == null) throw new System.ArgumentNullException("c");

            if (c is BoxCollider)
            {
                var box = c as BoxCollider;
                DrawWireRectoid(box.transform.TransformPoint(box.center), box.transform.rotation, Vector3.Scale(box.size, box.transform.lossyScale));
            }
            else if (c is SphereCollider)
            {
                var sphereGeom = com.spacepuppy.Geom.Sphere.FromCollider(c as SphereCollider);
                DrawWireSphere(sphereGeom.Center, c.transform.rotation, sphereGeom.Radius);
            }
            else if (c is CapsuleCollider)
            {
                var capGeom = com.spacepuppy.Geom.Capsule.FromCollider(c as CapsuleCollider);
                DrawWireCapsule(capGeom.Start, capGeom.End, capGeom.Radius);
            }
            else if (c is CharacterController)
            {
                var capGeom = com.spacepuppy.Geom.Capsule.FromCollider(c as CharacterController);
                DrawWireCapsule(capGeom.Start, capGeom.End, capGeom.Radius);
            }
            else if (c is MeshCollider)
            {
                //not supported
                throw new System.ArgumentException("Unsupported collider type '" + c.GetType().Name + "'.", "c");
            }
            else
            {
                //not supported
                throw new System.ArgumentException("Unsupported collider type '" + c.GetType().Name + "'.", "c");
            }

        }

        public static void DrawWireRectoid(Vector3 center, Quaternion rot, Vector3 size)
        {
            Vector3[] verts = new Vector3[5];
            Vector3 dir;
            Vector3 a;
            Vector3 b;

            //draw top
            dir = Vector3.up * size.y / 2.0f;
            a = Vector3.right * size.x / 2.0f;
            b = Vector3.forward * size.z / 2.0f;

            verts[0] = center + rot * (dir + a + b);
            verts[1] = center + rot * (dir + a - b);
            verts[2] = center + rot * (dir - a - b);
            verts[3] = center + rot * (dir - a + b);
            verts[4] = verts[0];

            Handles.DrawPolyLine(verts);

            //draw bottom
            dir = Vector3.down * size.y / 2.0f;
            a = Vector3.right * size.x / 2.0f;
            b = Vector3.forward * size.z / 2.0f;

            verts[0] = center + rot * (dir + a + b);
            verts[1] = center + rot * (dir + a - b);
            verts[2] = center + rot * (dir - a - b);
            verts[3] = center + rot * (dir - a + b);
            verts[4] = verts[0];

            Handles.DrawPolyLine(verts);

            //draw right
            dir = Vector3.right * size.x / 2.0f;
            a = Vector3.up * size.y / 2.0f;
            b = Vector3.forward * size.z / 2.0f;

            verts[0] = center + rot * (dir + a + b);
            verts[1] = center + rot * (dir + a - b);
            verts[2] = center + rot * (dir - a - b);
            verts[3] = center + rot * (dir - a + b);
            verts[4] = verts[0];

            Handles.DrawPolyLine(verts);

            //draw left
            dir = Vector3.left * size.x / 2.0f;
            a = Vector3.up * size.y / 2.0f;
            b = Vector3.forward * size.z / 2.0f;

            verts[0] = center + rot * (dir + a + b);
            verts[1] = center + rot * (dir + a - b);
            verts[2] = center + rot * (dir - a - b);
            verts[3] = center + rot * (dir - a + b);
            verts[4] = verts[0];

            Handles.DrawPolyLine(verts);

            //draw front
            dir = Vector3.forward * size.z / 2.0f;
            a = Vector3.right * size.x / 2.0f;
            b = Vector3.up * size.y / 2.0f;

            verts[0] = center + rot * (dir + a + b);
            verts[1] = center + rot * (dir + a - b);
            verts[2] = center + rot * (dir - a - b);
            verts[3] = center + rot * (dir - a + b);
            verts[4] = verts[0];

            Handles.DrawPolyLine(verts);

            //draw back
            dir = Vector3.back * size.z / 2.0f;
            a = Vector3.right * size.x / 2.0f;
            b = Vector3.up * size.y / 2.0f;

            verts[0] = center + rot * (dir + a + b);
            verts[1] = center + rot * (dir + a - b);
            verts[2] = center + rot * (dir - a - b);
            verts[3] = center + rot * (dir - a + b);
            verts[4] = verts[0];

            Handles.DrawPolyLine(verts);
        }

        public static void DrawWireSphere(Vector3 center, Quaternion rot, float radius)
        {
            Vector3 norm;
            Vector3 start;

            //draw right
            norm = rot * Vector3.right;
            start = rot * Vector3.up;

            Handles.DrawWireArc(center, norm, start, 360.0f, radius);

            //draw front
            norm = rot * Vector3.forward;
            start = rot * Vector3.up;

            Handles.DrawWireArc(center, norm, start, 360.0f, radius);


            //draw top
            norm = rot * Vector3.up;
            start = rot * Vector3.forward;

            Handles.DrawWireArc(center, norm, start, 360.0f, radius);

        }

        public static void DrawWireCapsule(Vector3 bottom, Vector3 top, float radius)
        {
            DrawWireCylinder(bottom, top, radius);

            var axis = (top - bottom).normalized;
            var rot = Quaternion.FromToRotation(Vector3.up, axis);

            Vector3 norm;
            Vector3 start;

            //draw top circles
            norm = rot * Vector3.right;
            start = rot * Vector3.back;
            Handles.DrawWireArc(top, norm, start, 180.0f, radius);

            norm = rot * Vector3.forward;
            start = rot * Vector3.right;
            Handles.DrawWireArc(top, norm, start, 180.0f, radius);

            //draw bottom circles
            norm = rot * Vector3.right;
            start = rot * Vector3.forward;
            Handles.DrawWireArc(bottom, norm, start, 180.0f, radius);

            norm = rot * Vector3.forward;
            start = rot * Vector3.left;
            Handles.DrawWireArc(bottom, norm, start, 180.0f, radius);
        }

        public static void DrawWireCylinder(Vector3 bottom, Vector3 top, float radius)
        {
            var axis = (top - bottom).normalized;
            var rot = Quaternion.FromToRotation(Vector3.up, axis);

            //circles
            Vector3 norm = rot * Vector3.up;
            Vector3 start = rot * Vector3.right;
            Handles.DrawWireArc(top, norm, start, 360.0f, radius);
            Handles.DrawWireArc(bottom, norm, start, 360.0f, radius);

            //lines
            Vector3 a = rot * Vector3.right;
            Vector3 b = rot * Vector3.forward;

            Handles.DrawPolyLine(bottom + a * radius, top + a * radius);
            Handles.DrawPolyLine(bottom - a * radius, top - a * radius);
            Handles.DrawPolyLine(bottom + b * radius, top + b * radius);
            Handles.DrawPolyLine(bottom - b * radius, top - b * radius);
        }



        public static void DrawCollider(Collider c)
        {
            if (c == null) throw new System.ArgumentNullException("c");

            if (c is BoxCollider)
            {
                var box = c as BoxCollider;
                DrawRectoid(box.transform.TransformPoint(box.center), box.transform.rotation, Vector3.Scale(box.size, box.transform.lossyScale));
            }
            else if (c is SphereCollider)
            {
                var sphereGeom = com.spacepuppy.Geom.Sphere.FromCollider(c as SphereCollider);
                DrawSphere(sphereGeom.Center, c.transform.rotation, sphereGeom.Radius);
            }
            else if (c is CapsuleCollider)
            {
                var capGeom = com.spacepuppy.Geom.Capsule.FromCollider(c as CapsuleCollider);
                DrawCapsule(capGeom.Start, capGeom.End, capGeom.Radius);
            }
            else if (c is CharacterController)
            {
                var capGeom = com.spacepuppy.Geom.Capsule.FromCollider(c as CharacterController);
                DrawCapsule(capGeom.Start, capGeom.End, capGeom.Radius);
            }
            else if (c is MeshCollider)
            {
                var mc = c as MeshCollider;
                if (mc.sharedMesh != null)
                {
                    DrawMesh(mc.sharedMesh, mc.transform.position, mc.transform.rotation, mc.transform.lossyScale);
                }
            }
            else
            {
                //not supported
                throw new System.ArgumentException("Unsupported collider type '" + c.GetType().Name + "'.", "c");
            }
        }

        public static void DrawRectoid(Vector3 center, Quaternion rot, Vector3 size)
        {
            //Vector3[] verts = new Vector3[5];
            //Vector3 dir;
            //Vector3 a;
            //Vector3 b;

            ////draw top
            //dir = Vector3.up * size.y / 2.0f;
            //a = Vector3.right * size.x / 2.0f;
            //b = Vector3.forward * size.z / 2.0f;

            //verts[0] = center + rot * (dir + a + b);
            //verts[1] = center + rot * (dir + a - b);
            //verts[2] = center + rot * (dir - a - b);
            //verts[3] = center + rot * (dir - a + b);
            //verts[4] = verts[0];

            //Handles.DrawSolidRectangleWithOutline(verts, Handles.color, Handles.color);

            ////draw bottom
            //dir = Vector3.down * size.y / 2.0f;
            //a = Vector3.right * size.x / 2.0f;
            //b = Vector3.forward * size.z / 2.0f;

            //verts[0] = center + rot * (dir + a + b);
            //verts[1] = center + rot * (dir + a - b);
            //verts[2] = center + rot * (dir - a - b);
            //verts[3] = center + rot * (dir - a + b);
            //verts[4] = verts[0];

            //Handles.DrawSolidRectangleWithOutline(verts, Handles.color, Handles.color);

            ////draw right
            //dir = Vector3.right * size.x / 2.0f;
            //a = Vector3.up * size.y / 2.0f;
            //b = Vector3.forward * size.z / 2.0f;

            //verts[0] = center + rot * (dir + a + b);
            //verts[1] = center + rot * (dir + a - b);
            //verts[2] = center + rot * (dir - a - b);
            //verts[3] = center + rot * (dir - a + b);
            //verts[4] = verts[0];

            //Handles.DrawSolidRectangleWithOutline(verts, Handles.color, Handles.color);

            ////draw left
            //dir = Vector3.left * size.x / 2.0f;
            //a = Vector3.up * size.y / 2.0f;
            //b = Vector3.forward * size.z / 2.0f;

            //verts[0] = center + rot * (dir + a + b);
            //verts[1] = center + rot * (dir + a - b);
            //verts[2] = center + rot * (dir - a - b);
            //verts[3] = center + rot * (dir - a + b);
            //verts[4] = verts[0];

            //Handles.DrawSolidRectangleWithOutline(verts, Handles.color, Handles.color);

            ////draw front
            //dir = Vector3.forward * size.z / 2.0f;
            //a = Vector3.right * size.x / 2.0f;
            //b = Vector3.up * size.y / 2.0f;

            //verts[0] = center + rot * (dir + a + b);
            //verts[1] = center + rot * (dir + a - b);
            //verts[2] = center + rot * (dir - a - b);
            //verts[3] = center + rot * (dir - a + b);
            //verts[4] = verts[0];

            //Handles.DrawSolidRectangleWithOutline(verts, Handles.color, Handles.color);

            ////draw back
            //dir = Vector3.back * size.z / 2.0f;
            //a = Vector3.right * size.x / 2.0f;
            //b = Vector3.up * size.y / 2.0f;

            //verts[0] = center + rot * (dir + a + b);
            //verts[1] = center + rot * (dir + a - b);
            //verts[2] = center + rot * (dir - a - b);
            //verts[3] = center + rot * (dir - a + b);
            //verts[4] = verts[0];

            //Handles.DrawSolidRectangleWithOutline(verts, Handles.color, Handles.color);



            var mat = StartDraw(center, rot, size);
            Graphics.DrawMeshNow(PrimitiveUtil.CubeMesh, mat);
        }

        public static void DrawSphere(Vector3 center, Quaternion rot, float radius)
        {
            //Handles.SphereCap(0, center, rot, radius * 2.0f);
            Handles.SphereHandleCap(0, center, rot, radius * 2.0f, EventType.Repaint);
        }

        public static void DrawCapsule(Vector3 bottom, Vector3 top, float radius)
        {
            //var axis = (top - bottom).normalized;
            //var rot = Quaternion.FromToRotation(Vector3.up, axis);

            //////spheres
            ////Handles.SphereCap(0, top, rot, radius * 2.0f);
            ////Handles.SphereCap(0, bottom, rot, radius * 2.0f);

            //Vector3 norm;
            //Vector3 start;

            ////draw top circles
            //norm = rot * Vector3.right;
            //start = rot * Vector3.back;
            //Handles.DrawSolidArc(top, norm, start, 180.0f, radius);

            //norm = rot * Vector3.forward;
            //start = rot * Vector3.right;
            //Handles.DrawSolidArc(top, norm, start, 180.0f, radius);

            ////draw bottom circles
            //norm = rot * Vector3.right;
            //start = rot * Vector3.forward;
            //Handles.DrawSolidArc(bottom, norm, start, 180.0f, radius);

            //norm = rot * Vector3.forward;
            //start = rot * Vector3.left;
            //Handles.DrawSolidArc(bottom, norm, start, 180.0f, radius);

            ////cylinder
            //const int DETAIL = 18;
            //const float ANGLE_STEP = 360.0f / DETAIL;
            //for (int i = 0; i < DETAIL; i++)
            //{
            //    Vector3 a = rot * (com.spacepuppy.Utils.VectorUtil.RotateAroundAxis(Vector3.right, i * ANGLE_STEP, Vector3.up, true)) * radius;
            //    Vector3 b = rot * (com.spacepuppy.Utils.VectorUtil.RotateAroundAxis(Vector3.right, (i + 1) * ANGLE_STEP, Vector3.up, true)) * radius;

            //    var verts = new Vector3[] { bottom + a, bottom + b, top + b, top + a, bottom + a };
            //    Handles.DrawSolidRectangleWithOutline(verts, Handles.color, Handles.color);
            //}

            var rod = top - bottom;
            var adjRad = radius / 0.5f;
            var adjHght = rod.magnitude / 2.0f + radius; //(rod.magnitude + radius * 2f);
            var mat = StartDraw(Vector3.Lerp(bottom, top, 0.5f), Quaternion.FromToRotation(Vector3.up, rod.normalized), new Vector3(adjRad, adjHght, adjRad));
            Graphics.DrawMeshNow(PrimitiveUtil.CapsuleMesh, mat);
        }

        public static void DrawCylinder(Vector3 bottom, Vector3 top, float radius)
        {
            //var axis = (top - bottom).normalized;
            //var rot = Quaternion.FromToRotation(Vector3.up, axis);

            ////circles
            //Vector3 norm = rot * Vector3.up;
            //Vector3 start = rot * Vector3.right;
            //Handles.DrawSolidArc(top, norm, start, 360.0f, radius);
            //Handles.DrawSolidArc(bottom, norm, start, 360.0f, radius);

            ////sides
            //const int DETAIL = 18;
            //const float ANGLE_STEP = 360.0f / DETAIL;
            //for (int i = 0; i < DETAIL; i++)
            //{
            //    Vector3 a = rot * (com.spacepuppy.Utils.VectorUtil.RotateAroundAxis(Vector3.right, i * ANGLE_STEP, Vector3.up)) * radius;
            //    Vector3 b = rot * (com.spacepuppy.Utils.VectorUtil.RotateAroundAxis(Vector3.right, (i + 1) * ANGLE_STEP, Vector3.up)) * radius;

            //    var verts = new Vector3[] { bottom + a, bottom + b, top + b, top + a, bottom + a };
            //    Handles.DrawSolidRectangleWithOutline(verts, Handles.color, Handles.color);
            //}


            var rod = top - bottom;
            var adjRad = radius / 0.5f;
            var adjHght = rod.magnitude / 2.0f;
            var mat = StartDraw(Vector3.Lerp(bottom, top, 0.5f), Quaternion.FromToRotation(Vector3.up, rod.normalized), new Vector3(adjRad, adjHght, adjRad));
            Graphics.DrawMeshNow(PrimitiveUtil.CylinderMesh, mat);

        }

        public static void DrawMesh(Mesh mesh, Vector3 pos, Quaternion rot, Vector3 scale)
        {
            if (mesh == null) throw new System.ArgumentNullException("mesh");
            var mat = StartDraw(pos, rot, scale);
            Graphics.DrawMeshNow(mesh, mat);
        }

    }

}
