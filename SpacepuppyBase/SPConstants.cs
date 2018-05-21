using UnityEngine;

namespace com.spacepuppy
{
    public class SPConstants
    {
        /// <summary>
        /// A hard minimum delta time to consider waiting one frame. Used in places like Invoke to determine if a WaitForSeconds/WaitForDuration should be used.
        /// </summary>
        public const float MIN_FRAME_DELTA = 0.01f;
        
        public const int LAYER_DEFAULT = 0;
        public const int LAYER_WATER = 4;


        public const string TAG_UNTAGGED = "Untagged";
        public const string TAG_RESPAWN = "Respawn";
        public const string TAG_FINISH = "Finish";
        public const string TAG_EDITORONLY = "EditorOnly";
        public const string TAG_MAINCAMERA = "MainCamera";
        public const string TAG_GAMECONTROLLER = "GameController";
        public const string TAG_PLAYER = "Player";

        public const string TAG_MULTITAG = "MultiTag";
        public const string TAG_ROOT = "Root";
        
        public static readonly Quaternion ROT_3DSMAX_TO_UNITY = Quaternion.LookRotation(Vector3.down, Vector3.forward);

    }
}
