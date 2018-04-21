using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Movement
{
    public class XYPlaneConstraint : AbstractMotor2DSurfaceConstraint
    {

        #region IMotorConstraint Interface

        public override void ConstrainSelf()
        {
            if (this.Motor != null)
            {
                var pos = this.Motor.Controller.transform.position;
                pos.z = 0;
                this.Motor.Controller.transform.position = pos;
            }
            else
            {
                var pos = this.Motor.Controller.transform.position;
                pos.z = 0;
                this.Motor.Controller.transform.position = pos;
            }
        }

        #endregion

        #region I2DMotorConstraint Interface

        public override Vector3 SurfaceNormal
        {
            get { return new Vector3(0f, 0f, -1f); }
        }

        /// <summary>
        /// Converts a 3d vector to closest 2d vector on 2D gameplay surface
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public override Vector2 ProjectVectorTo2D(Vector3 v)
        {
            //currently, that is just a plane in the x/y of the world and z = 0
            return new Vector2(v.x, v.y);
        }

        /// <summary>
        /// Converts a 2d vector from the gameplay surface to the 3d world
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public override Vector3 ProjectVectorTo3D(Vector2 v)
        {
            //currently, that is just a plane in the x/y of the world and z = 0
            return new Vector3(v.x, v.y, 0f);
        }

        /// <summary>
        /// Converts a 3d position to closest 2d position on the 2d gameplay surface
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public override Vector2 ProjectPosition2D(Vector3 v)
        {
            //currently, that is just a plane in the x/y of the world and z = 0
            return new Vector2(v.x, v.y);
        }

        /// <summary>
        /// Converts a 2d position from the gameplay surface to the 3d world
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public override Vector3 ProjectPosition3D(Vector2 v)
        {
            //currently, that is just a plane in the x/y of the world and z = 0
            return new Vector3(v.x, v.y, 0);
        }

        #endregion


    }
}
