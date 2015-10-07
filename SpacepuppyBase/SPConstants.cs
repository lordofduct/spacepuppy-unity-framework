using UnityEngine;

namespace com.spacepuppy
{
    public class SPConstants
    {

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

        //sent by SPComponent when enabled and disabled
        //public const string MSG_ONSPCOMPONENTENABLED = "OnSPComponentEnabled";
        //public const string MSG_ONSPCOMPONENTDISABLED = "OnSPComponentDisabled";

        //sent by Notification to all members of some entity
        public const string MSG_AUTONOTIFICATIONMESSAGEHANDLER = "AutoNotificationMessageHandler";
        
        //Broadcasted to all children when AddChild or RemoveFromParent is called for a GameObject, allows scripts in that hierarchy to react to the add/remove
        //Calling addchild/RemoveFromParent during this message can result in a stack overflow if the hierarchies intersect
        public const string MSG_ONTRANSFORMHIERARCHYCHANGED = "OnTransformHierarchyChanged";


        public static Quaternion ROT_3DSMAX_TO_UNITY = Quaternion.LookRotation(Vector3.down, Vector3.forward);

    }
}
