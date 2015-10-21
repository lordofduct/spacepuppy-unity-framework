using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Cameras
{
    public class ColorAlphaCameraFade : CameraFade
    {

        #region Fields

        public Color Color;
        public int DrawDepth;

        private float _currentOpacity = 0f;

        #endregion

        #region Methods

        protected override void UpdateFade(float percentage)
        {
            _currentOpacity = percentage;
        }

        void OnGUI()
        {
            if (!this.IsActiveFade) return;

            var c = this.Color;
            c.a = _currentOpacity;
            GUI.color = c;
            GUI.depth = DrawDepth;
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), SPAssets.WhiteTexture);
        }

        #endregion

        #region Static Factory

        public static ColorAlphaCameraFade Create(Color c, int drawDepth = 0, bool persistBetweenLoads = true)
        {
            var go = GameObjectUtil.CreateRoot("ColorAlphaCameraFade");
            if (persistBetweenLoads) GameObject.DontDestroyOnLoad(go);
            var fade = go.AddComponent<ColorAlphaCameraFade>();
            fade.DestroyOnComplete = true;
            fade.Color = c;
            fade.DrawDepth = drawDepth;
            return fade;
        }

        public static ColorAlphaCameraFade StartFade(float dur, Color c, int drawDepth = 0, bool persistBetweenLoads = true)
        {
            var fade = Create(c, drawDepth, persistBetweenLoads);
            fade.FadeOut(dur);
            return fade;
        }

        #endregion

    }
}
