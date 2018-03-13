using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Geom
{

    public class PlanarSurfaceComponent : MonoBehaviour, IPlanarSurface
    {

        #region Fields

        #endregion

        #region Properties

        public Vector3 SurfaceNormal
        {
            get
            {
                return -this.transform.forward;
            }
        }

        public Vector3 SurfaceRight
        {
            get
            {
                return this.transform.right;
            }
        }

        public Vector3 SurfaceUp
        {
            get
            {
                return this.transform.up;
            }
        }

        #endregion

        #region Methods

        public Vector3 MirrorPosition(Vector3 pos)
        {
            var pn = this.SurfaceNormal;
            var pd = Vector3.Dot(this.transform.position, pn);
            //var d = Vector3.Dot(pn, (pos - pn * pd));
            var d = Vector3.Dot(pn, pos) - pd; //this way is just fewer calculations, both work
            return pos - pn * d * 2f;
        }

        public Vector3 MirrorDirection(Vector3 v)
        {
            var pn = this.SurfaceNormal;
            var pd = Vector3.Dot(v, pn);
            return v - pn * pd * 2f;
        }

        public Quaternion GetMirrorLookRotation(Vector3 forw, Vector3 up)
        {
            forw = this.MirrorDirection(forw);
            up = this.MirrorDirection(up);
            return Quaternion.LookRotation(forw, up);
        }

        #endregion

        #region IPlanarSurface Interface

        public Vector3 GetSurfaceNormal(Vector2 location)
        {
            return this.SurfaceNormal;
        }

        public Vector3 GetSurfaceNormal(Vector3 location)
        {
            return this.SurfaceNormal;
        }

        /// <summary>
        /// Converts a 3d vector to closest 2d vector on 2D gameplay surface
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 ProjectVectorTo2D(Vector3 location, Vector3 v)
        {
            return new Vector2(Vector3.Dot(v, this.SurfaceRight), Vector3.Dot(v, this.SurfaceUp));
        }

        /// <summary>
        /// Converts a 2d vector from the gameplay surface to the 3d world
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3 ProjectVectorTo3D(Vector3 location, Vector2 v)
        {
            return this.SurfaceRight * v.x + this.SurfaceUp * v.y;
        }

        /// <summary>
        /// Converts a 3d position to closest 2d position on the 2d gameplay surface
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 ProjectPosition2D(Vector3 v)
        {
            //currently, that is just a plane in the x/y of the world and z = 0
            v -= this.transform.position;
            return new Vector2(Vector3.Dot(v, this.SurfaceRight), Vector3.Dot(v, this.SurfaceUp));
        }

        /// <summary>
        /// Converts a 2d position from the gameplay surface to the 3d world
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3 ProjectPosition3D(Vector2 v)
        {
            //currently, that is just a plane in the x/y of the world and z = 0
            return this.transform.position + this.SurfaceRight * v.x + this.SurfaceUp * v.y;
        }

        public Vector3 ClampToSurface(Vector3 v)
        {
            var p = this.transform.position;
            var r = this.SurfaceRight;
            var u = this.SurfaceUp;
            v -= p;
            return p + (r * Vector3.Dot(v, r)) + (u * Vector3.Dot(v, u));
        }

        #endregion

    }

}
