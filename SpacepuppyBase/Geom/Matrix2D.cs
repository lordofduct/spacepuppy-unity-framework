using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Geom
{

    [System.Serializable()]
    public struct Matrix2D
    {

        /*
         | a  c  x |
         | b  d  y |
         | 0  0  1 |
          
         right - <a, b>
         up - <c, d>
         */

        #region Fields

        [SerializeField()]
        public float a;
        [SerializeField()]
        public float b;
        [SerializeField()]
        public float c;
        [SerializeField()]
        public float d;
        [SerializeField()]
        public float tx;
        [SerializeField()]
        public float ty; 

        #endregion

        #region CONSTRUCTOR

        public Matrix2D(float a, float b, float c, float d, float tx, float ty)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.tx = tx;
            this.ty = ty;
        }

        #endregion

        #region Properties

        public Vector2 right
        {
            get { return new Vector2(a, b); }
            set
            {
                a = value.x;
                b = value.y;
            }
        }

        public Vector2 up
        {
            get { return new Vector2(c, d); }
            set
            {
                c = value.x;
                d = value.y;
            }
        }

        public float angle
        {
            get { return Mathf.Atan2(b, a); }
        }

        public Matrix2D inverse
        {
            get { return Matrix2D.Inverse(this); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Rotate matrix by an angle clockwise.
        /// </summary>
        /// <param name="angle">Angle to rotate by in radians.</param>
        public void Rotate(float angle)
        {
            var cos = Mathf.Cos(angle);
            var sin = Mathf.Sin(angle);
            float a = this.a;
            float b = this.b;
            float c = this.c;
            float d = this.d;
            float x = this.tx;
            float y = this.ty;
            this.a = a * cos - b * sin;
            this.b = a * sin + b * cos;
            this.c = c * cos - d * sin;
            this.d = c * sin + d * cos;
            this.tx = x * cos - y * sin;
            this.ty = x * sin + y * cos;
        }

        public void Scale(float sx, float sy)
        {
            if(sx != 1.0f)
            {
                a *= sx;
                c *= sx;
                tx *= sx;
            }
            if(sy != 1.0f)
            {
                b *= sy;
                d *= sy;
                ty *= sy;
            }
        }

        public void Translate(float dx, float dy)
        {
            tx += dx;
            ty += dy;
        }

        public Vector2 TransformPoint(Vector2 p)
        {
            return new Vector2(a * p.x + c * p.y + tx, b * p.x + d * p.y + ty);
        }

        public Vector2 TransformVector(Vector2 v)
        {
            return new Vector2(a * v.x + c * v.y, b * v.x + d * v.y);
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return "("+ string.Join(", ", new string[]{"a="+a,"b="+b,"c="+c,"d="+d,"tx="+tx,"ty="+ty}) +")";
        }

        #endregion

        #region Static Members

        public static Matrix2D Identity
        {
            get
            {
                return new Matrix2D(1f, 0f, 0f, 1f, 0f, 0f);
            }
        }

        public static Matrix2D Translation(float tx, float ty)
        {
            return new Matrix2D(1f, 0f, 0f, 1f, tx, ty);
        }
        
        public static Matrix2D Translation(Vector2 pos)
        {
            return new Matrix2D(1f, 0f, 0f, 1f, pos.x, pos.y);
        }

        public static Matrix2D Rotation(float angle)
        {
            var cos = Mathf.Cos(angle);
            var sin = Mathf.Sin(angle);
            return new Matrix2D(cos, sin, -sin, cos, 0f, 0f);
        }

        public static Matrix2D Scale(float scale)
        {
            return new Matrix2D(scale, 0f, 0f, scale, 0f, 0f);
        }

        public static Matrix2D Scale(Vector2 scale)
        {
            return new Matrix2D(scale.x, 0f, 0f, scale.y, 0f, 0f);
        }

        public static Matrix2D TR(Vector2 pos, float angle)
        {
            var cos = Mathf.Cos(angle);
            var sin = Mathf.Sin(angle);
            return new Matrix2D(cos, sin, -sin, cos, pos.x, pos.y);
        }

        public static Matrix2D TRS(Vector2 pos, float angle, Vector2 scale)
        {
            var cos = Mathf.Cos(angle);
            var sin = Mathf.Sin(angle);
            return new Matrix2D(cos * scale.x, sin * scale.y, -sin * scale.x, cos * scale.y, pos.x, pos.y);
        }

        public static Matrix2D Look(Vector2 dir)
        {
            return Rotation(Mathf.Atan2(dir.y, dir.x));
        }

        public static Matrix2D Look(Vector2 dir, Vector2 up)
        {
            up = com.spacepuppy.Utils.VectorUtil.RotateToward(dir, up, Mathf.PI / 2.0f, true).normalized;
            dir.Normalize();
            return new Matrix2D(dir.x, dir.y, up.x, up.y, 0f, 0f);
        }

        public static Matrix2D Inverse(Matrix2D m)
        {
            float det = m.a * m.d - m.c * m.b;
            return new Matrix2D()
            {
                a = m.d / det,
                b = -m.b / det,
                c = -m.c / det,
                d = m.a / det,
                tx = (m.a * m.tx + m.c * m.ty),
                ty = (m.b * m.tx + m.d * m.ty)
            };
        }

        #endregion

        #region Operators

        public static Matrix2D operator *(Matrix2D m1, Matrix2D m2)
        {
            return new Matrix2D()
            {
                a = m1.a * m2.a + m1.b * m2.c,
                b = m1.a * m2.b + m1.b * m2.d,
                c = m1.c * m2.a + m1.d * m2.c,
                d = m1.c * m2.b + m1.d * m2.d,
                tx = m1.tx * m2.a + m1.ty * m2.c + m2.tx,
                ty = m1.tx * m2.b + m1.ty * m2.d + m2.ty
            };
        }

        #endregion

    }
}
