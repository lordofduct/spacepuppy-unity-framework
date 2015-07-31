using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Geom
{

    [System.Serializable()]
    public struct Trans
    {

        [SerializeField()]
        public Vector3 Position;
        [SerializeField()]
        public Quaternion Rotation;
        [SerializeField()]
        public Vector3 Scale;

        public Matrix4x4 Matrix
        {
            get
            {
                return Matrix4x4.TRS(Position, Rotation, Scale);
            }
            set
            {
                Position = TransformUtil.GetTranslation(value);
                Rotation = TransformUtil.GetRotation(value);
                Scale = TransformUtil.GetScale(value);
            }
        }

        #region CONSTRUCTORS

        public Trans(Vector3 pos, Quaternion rot)
        {
            this.Position = pos;
            this.Rotation = rot;
            this.Scale = Vector3.one;
        }

        public Trans(Vector3 pos, Quaternion rot, Vector3 sc)
        {
            this.Position = pos;
            this.Rotation = rot;
            this.Scale = sc;
        }

        public static Trans Identity
        {
            get
            {
                return new Trans(Vector3.zero, Quaternion.identity, Vector3.one);
            }
        }

        public static Trans Translation(Vector3 pos)
        {
            return new Trans(pos, Quaternion.identity, Vector3.one);
        }

        public static Trans Translation(float x, float y, float z)
        {
            return new Trans(new Vector3(x, y, z), Quaternion.identity, Vector3.one);
        }

        public static Trans Rotated(Quaternion q)
        {
            return new Trans(Vector3.zero, q, Vector3.one);
        }

        public static Trans Rotated(Vector3 eulerAngles)
        {
            return new Trans(Vector3.zero, Quaternion.Euler(eulerAngles), Vector3.one);
        }

        public static Trans Rotated(float x, float y, float z)
        {
            return new Trans(Vector3.zero, Quaternion.Euler(x, y, z), Vector3.one);
        }

        public static Trans Scaled(Vector3 sc)
        {
            return new Trans(Vector3.zero, Quaternion.identity, sc);
        }

        public static Trans Scaled(float x, float y, float z)
        {
            return new Trans(Vector3.zero, Quaternion.identity, new Vector3(x, y, z));
        }

        public static Trans Transform(Matrix4x4 mat)
        {
            var t = new Trans();
            t.Matrix = mat;
            return t;
        }

        public static Trans Transform(Vector3 pos, Quaternion rot)
        {
            return new Trans(pos, rot);
        }

        public static Trans Transform(Vector3 pos, Quaternion rot, Vector3 sc)
        {
            return new Trans(pos, rot, sc);
        }

        #endregion

        #region Properties

        public Vector3 Forward
        {
            get { return this.Rotation * Vector3.forward; }
        }

        public Vector3 Up
        {
            get { return this.Rotation * Vector3.up; }
        }

        public Vector3 Right
        {
            get { return this.Rotation * Vector3.right; }
        }

        #endregion

        #region Methods

        public void Translate(Vector3 v)
        {
            Position += v;
        }

        public void Rotate(Quaternion q)
        {
            Rotation *= q;
        }

        public void Rotate(float x, float y, float z)
        {
            Rotation *= Quaternion.Euler(x, y, z);
        }

        public void Rotate(Vector3 eulerRot)
        {
            Rotation *= Quaternion.Euler(eulerRot);
        }

        public void RotateAround(Vector3 point, float angle, Vector3 axis)
        {
            var v = this.Position - point;
            var q = Quaternion.AngleAxis(angle, axis);
            v = q * v;
            this.Position = point + v;
            this.Rotation *= q;
        }

        public void LookAt(Vector3 point, Vector3 up)
        {
            this.Rotation = Quaternion.LookRotation(point - this.Position, up);
        }

        public Vector3 TransformPoint(Vector3 v)
        {
            return this.Matrix.MultiplyPoint(v);
        }

        public Vector3 TransformDirection(Vector3 v)
        {
            return this.Matrix.MultiplyVector(v);
        }

        public Vector3 InverseTransformPoint(Vector3 v)
        {
            return this.Matrix.inverse.MultiplyPoint(v);
        }

        public Vector3 InverseTransformDirection(Vector3 v)
        {
            return this.Matrix.inverse.MultiplyVector(v);
        }


        #endregion

        #region Operators

        public static Trans operator *(Trans t1, Trans t2)
        {
            t1.Position += t2.Position;
            t1.Rotation *= t2.Rotation;
            var v1 = t1.Scale;
            var v2 = t2.Scale;
            v1.x *= v2.x;
            v1.y *= v2.y;
            v1.z *= v2.z;
            t1.Scale = v1;
            return t1;
        }

        public static Trans operator +(Trans t, Vector3 v)
        {
            t.Position += v;
            return t;
        }

        public static Trans operator *(Trans t, Quaternion q)
        {
            t.Rotation *= q;
            return t;
        }

        public static Vector3 operator *(Trans t, Vector3 v)
        {
            return t.Matrix.MultiplyPoint(v);
        }

        #endregion

        #region Get/Set to Transform

        public void SetToLocal(Transform trans)
        {
            trans.localPosition = Position;
            trans.localRotation = Rotation;
            trans.localScale = Scale;
        }

        public void SetToGlobal(Transform trans, bool bSetScale)
        {
            if (bSetScale)
            {
                trans.position = Vector3.zero;
                trans.rotation = Quaternion.identity;
                trans.localScale = Vector3.one;

                var m = trans.worldToLocalMatrix;
                trans.position = Position;
                trans.rotation = Rotation;
                trans.localScale = m.MultiplyPoint(Scale);
            }
            else
            {
                trans.position = Position;
                trans.rotation = Rotation;
            }
        }

        public static Trans GetLocal(Transform trans)
        {
            var t = new Trans();
            t.Position = trans.localPosition;
            t.Rotation = trans.localRotation;
            t.Scale = trans.localScale;
            return t;
        }

        public static Trans GetGlobal(Transform trans)
        {
            var t = new Trans();
            t.Position = trans.position;
            t.Rotation = trans.rotation;
            t.Scale = trans.lossyScale;
            return t;
        }

        #endregion


    }
}