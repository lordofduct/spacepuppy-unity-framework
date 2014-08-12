using UnityEngine;

namespace com.spacepuppy
{
    public class SPConstants
    {
        public const string TAG_UNTAGGED = "Untagged";
        public const string TAG_RESPAWN = "Respawn";
        public const string TAG_FINISH = "Finish";
        public const string TAG_EDITORONLY = "EditorOnly";
        public const string TAG_MAINCAMERA = "MainCamera";
        public const string TAG_GAMECONTROLLER = "GameController";

        public const string TAG_MULTITAG = "MultiTag";
        public const string TAG_ROOT = "Root";
        public const string TAG_PLAYER = "Player";

        //sent by SPComponent when enabled and disabled
        public const string MSG_ONSPCOMPONENTENABLED = "OnSPComponentEnabled";
        public const string MSG_ONSPCOMPONENTDISABLED = "OnSPComponentDisabled";

        //sent by SpawnPool on GameObject being spawned
        public const string MSG_ONSPAWN = "OnSpawn";
        public const string MSG_ONDESPAWN = "OnDespawn";

    }
}
