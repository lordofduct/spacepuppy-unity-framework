using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    public static class SPAssets
    {

        #region Fields

        private static Texture2D _whiteTexture;

        #endregion

        #region Properties

        public static Texture2D WhiteTexture
        {
            get
            {
                if(_whiteTexture == null)
                {
                    _whiteTexture = new Texture2D(1, 1);
                    _whiteTexture.SetPixel(0, 0, Color.white);
                    _whiteTexture.Apply();
                }
                return _whiteTexture;
            }
        }

        #endregion

    }

}
