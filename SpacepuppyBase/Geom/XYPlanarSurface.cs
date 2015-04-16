using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Geom
{
    public class XYPlanarSurface : IPlanarSurface
    {

        #region Singleton

        private static XYPlanarSurface _surface = new XYPlanarSurface();

        public static XYPlanarSurface Default { get { return _surface; } }

        #endregion

        #region Fields

        private float _zdepth;

        #endregion

        #region CONSTRUCTOR

        public XYPlanarSurface()
        {
            _zdepth = 0f;
        }

        public XYPlanarSurface(float zdepth)
        {
            _zdepth = zdepth;
        }

        #endregion

        #region Properties

        public float ZDepth
        {
            get { return _zdepth; }
            set { _zdepth = value; }
        }

        #endregion

        #region IPlanarSurface Interface

        public Vector3 GetSurfaceNormal(Vector2 location)
        {
            return -Vector3.forward;
        }

        public Vector3 GetSurfaceNormal(Vector3 location)
        {
            return -Vector3.forward;
        }

        /// <summary>
        /// Converts a 3d vector to closest 2d vector on 2D gameplay surface
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 ProjectVectorTo2D(Vector3 location, Vector3 v)
        {
            //currently, that is just a plane in the x/y of the world and z = 0
            return new Vector2(v.x, v.y);
        }

        /// <summary>
        /// Converts a 2d vector from the gameplay surface to the 3d world
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3 ProjectVectorTo3D(Vector3 location, Vector2 v)
        {
            //currently, that is just a plane in the x/y of the world and z = 0
            return new Vector3(v.x, v.y, 0f);
        }

        /// <summary>
        /// Converts a 3d position to closest 2d position on the 2d gameplay surface
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector2 ProjectPosition2D(Vector3 v)
        {
            //currently, that is just a plane in the x/y of the world and z = 0
            return new Vector2(v.x, v.y);
        }

        /// <summary>
        /// Converts a 2d position from the gameplay surface to the 3d world
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3 ProjectPosition3D(Vector2 v)
        {
            //currently, that is just a plane in the x/y of the world and z = 0
            return new Vector3(v.x, v.y, _zdepth);
        }

        public Vector3 ClampToSurface(Vector3 v)
        {
            return new Vector3(v.x, v.y, _zdepth);
        }

        #endregion

    }
}
