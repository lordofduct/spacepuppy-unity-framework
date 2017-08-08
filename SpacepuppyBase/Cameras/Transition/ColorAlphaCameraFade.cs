using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Cameras.Transition
{
    public class ColorAlphaCameraFade : CameraFade
    {

        #region Fields

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Color")]
        private Color _color;
        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("DrawDepth")]
        private int _drawDepth;

        private float _currentOpacity = 0f;

        #endregion

        #region Properties

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public int DrawDepth
        {
            get { return _drawDepth; }
            set { _drawDepth = value; }
        }

        #endregion

        #region Methods

        protected override void UpdateFade(float percentage)
        {
            _currentOpacity = percentage;
        }

        void OnGUI()
        {
            if (!this.IsActiveFade) return;

            var c = _color;
            c.a = _currentOpacity;
            GUI.color = c;
            GUI.depth = _drawDepth;
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
