using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Cameras
{

    /// <summary>
    /// This event is considered temporary, only use for the life of the event handler, and no longer.
    /// </summary>
    public class CameraRegistrationEvent : System.EventArgs
    {

        #region Fields

        private ICamera _camera;

        #endregion

        #region CONSTRUCTOR

        public CameraRegistrationEvent(ICamera camera)
        {
            _camera = camera;
        }

        #endregion

        #region Properties

        public ICamera Camera
        {
            get { return _camera; }
        }

        #endregion

        #region Static Interface

        private static CameraRegistrationEvent _event;

        public static CameraRegistrationEvent GetTemp(ICamera cam)
        {
            if(_event != null)
            {
                var ev = _event;
                _event = null;
                ev._camera = cam;
                return ev;
            }
            else
            {
                return new CameraRegistrationEvent(cam);
            }
        }

        public static void Release(CameraRegistrationEvent ev)
        {
            if (ev == null) return;

            ev._camera = null;
            if(_event == null)
            {
                _event = ev;
            }
        }

        #endregion

    }

}
