#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;

namespace com.spacepuppy.Scenario
{
    public class i_DebugLog : TriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        private VariantReference _message = new VariantReference(string.Empty);

        [SerializeField]
        private bool _logStackTrace;
        [SerializeField]
        private bool _logEvenInBuild;

        #endregion


        #region TriggerableMechanism Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;
            if (!_logEvenInBuild && !Application.isEditor) return false;

            if(_logStackTrace)
            {
                var str = _message.StringValue;
                if(string.IsNullOrEmpty(str))
                    Debug.Log(UnityEngine.StackTraceUtility.ExtractStackTrace(), this);
                else
                    Debug.Log(str + "\n" + UnityEngine.StackTraceUtility.ExtractStackTrace(), this);
            }
            else
            {
                Debug.Log(_message.StringValue, this);
            }
            return true;
        }

        #endregion

    }
}
