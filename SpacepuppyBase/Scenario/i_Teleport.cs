using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public class i_Teleport : TriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Target")]
        [TriggerableTargetObject.Config(typeof(Transform))]
        private TriggerableTargetObject _target = new TriggerableTargetObject();

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("Location")]
        [TriggerableTargetObject.Config(typeof(Transform))]
        private TriggerableTargetObject _location = new TriggerableTargetObject();

        [SerializeField()]
        [UnityEngine.Serialization.FormerlySerializedAs("TeleportEntireEntity")]
        private bool _teleportEntireEntity = true;

        #endregion

        #region Properties

        public TriggerableTargetObject Target
        {
            get { return _target; }
        }

        public TriggerableTargetObject Location
        {
            get { return _location; }
        }

        public bool TeleportEntireEntity
        {
            get { return _teleportEntireEntity; }
            set { _teleportEntireEntity = value; }
        }

        #endregion

        #region TriggerableMechanism Interface

        public override bool Trigger(object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = this._target.GetTarget<Transform>(arg);
            if (targ == null) return false;
            if (_teleportEntireEntity) targ = GameObjectUtil.FindRoot(targ).transform;

            var loc = _location.GetTarget<Transform>(arg);
            if (targ == null || loc == null) return false;

            targ.position = loc.position;

            return true;
        }

        #endregion

    }

}