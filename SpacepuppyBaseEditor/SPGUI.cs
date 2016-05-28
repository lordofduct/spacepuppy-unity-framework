using UnityEngine;

namespace com.spacepuppyeditor
{
    public static class SPGUI
    {

        public static GUIDisableCache Disable()
        {
            var result = new GUIDisableCache();
            result.Set();
            return result;
        }

        public static GUIDisableCache DisableIfPlaying()
        {
            if (Application.isPlaying)
                return Disable();
            else
                return new GUIDisableCache()
                {
                    Flag = GUI.enabled
                };
        }

        public static GUIDisableCache DisableIf(bool test)
        {
            if (test)
                return Disable();
            else
                return new GUIDisableCache()
                {
                    Flag = GUI.enabled
                };
        }

        #region Special Types

        public struct GUIDisableCache
        {

            public bool Flag;

            public void Set()
            {
                this.Flag = GUI.enabled;
                GUI.enabled = false;
            }

            public void Reset()
            {
                GUI.enabled = this.Flag;
            }
            
        }

        #endregion

    }
}
