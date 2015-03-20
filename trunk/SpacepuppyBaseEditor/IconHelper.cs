using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;
using com.spacepuppy.Utils.Dynamic;

namespace com.spacepuppyeditor
{
    public static class IconHelper
    {

        public enum LabelIcon
        {
            Gray = 0,
            Blue,
            Teal,
            Green,
            Yellow,
            Orange,
            Red,
            Purple
        }

        public enum Icon
        {
            CircleGray = 0,
            CircleBlue,
            CircleTeal,
            CircleGreen,
            CircleYellow,
            CircleOrange,
            CircleRed,
            CirclePurple,
            DiamondGray,
            DiamondBlue,
            DiamondTeal,
            DiamondGreen,
            DiamondYellow,
            DiamondOrange,
            DiamondRed,
            DiamondPurple
        }

        #region Fields

        private static TypeAccessWrapper _accessWrapper;

        private static System.Action<UnityEngine.Object, Texture2D> _imp_setIconForObject;

        private static ReadOnlyCollection<GUIContent> _labelIcons;
        private static ReadOnlyCollection<GUIContent> _largeIcons;

        #endregion

        #region CONSTRUCTOR

        static IconHelper()
        {
            _accessWrapper = new TypeAccessWrapper(typeof(EditorGUIUtility), true);
        }

        #endregion

        #region Properties

        public static IList<GUIContent> LabelIcons
        {
            get
            {
                if (_labelIcons == null) _labelIcons = new ReadOnlyCollection<GUIContent>(GetTextures("sv_label_", string.Empty, 0, 8));
                return _labelIcons;
            }
        }

        public static IList<GUIContent> Icons
        {
            get
            {
                if (_largeIcons == null) _largeIcons = new ReadOnlyCollection<GUIContent>(GetTextures("sv_icon_dot", "_pix16_gizmo", 0, 16));
                return _largeIcons;
            }
        }

        #endregion

        #region Methods

        public static void SetIconForObject(GameObject go, LabelIcon icon)
        {
            SetIconForObject(go, IconHelper.LabelIcons[(int)icon].image as Texture2D);
        }

        public static void SetIconForObject(GameObject go, Icon icon)
        {
            SetIconForObject(go, IconHelper.Icons[(int)icon].image as Texture2D);
        }

        public static void SetIconForObject(GameObject go, Texture2D texture)
        {
            if (_imp_setIconForObject == null) _imp_setIconForObject = _accessWrapper.GetStaticMethod("SetIconForObject", typeof(System.Action<UnityEngine.Object, Texture2D>)) as System.Action<UnityEngine.Object, Texture2D>;
            _imp_setIconForObject(go, texture);
        }

        private static GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count)
        {
            GUIContent[] arr = new GUIContent[count];

            var m = _accessWrapper.GetStaticMethod("IconContent", typeof(System.Func<string, GUIContent>)) as System.Func<string, GUIContent>;
            for (int index = 0; index < count; ++index)
            {
                arr[index] = m(baseName + (startIndex + index).ToString() + postFix);
            }

            return arr;
        }

        #endregion

    }
}
